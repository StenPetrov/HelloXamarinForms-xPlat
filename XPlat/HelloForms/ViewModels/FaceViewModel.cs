using System;
using System.Threading.Tasks;
using XamarinAzureMobileAppTestService.DataObjects;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using System.Linq;
using System.Collections.Generic;
using XLabs.Forms.Mvvm;
using System.Windows.Input;
using System.IO;
using XLabs.Platform.Services.Media;

namespace HelloForms
{
    public partial class FaceViewModel : ViewModel
    {
        // See https://www.microsoft.com/cognitive-services/en-us/subscriptions
        // private string ProjectOxfordFaceApiKey = "<your key here>";
        // private string ProjectOxfordEmotionApiKey = "<your key here>";

        private Stream _currentPhotoStream;
        public Stream CurrentPhotoStream {
            get { return _currentPhotoStream; }
            set {
                // hide the photo in case something fails it will remain hidden
                IsPhotoAvailable = false;

                if (_currentPhotoStream != value) {
                    // try our best to dispose of images
                    if (_currentPhotoStream != null) _currentPhotoStream.Dispose ();

                    IsCurrentPhotoAnalyzed = false;
                    SetProperty (ref _currentPhotoStream, value);
                    if (value != null) {
                        var isStreamCopy = new MemoryStream ((int)value.Length);
                        value.CopyTo (isStreamCopy);
                        isStreamCopy.Seek (0, SeekOrigin.Begin);
                        value.Seek (0, SeekOrigin.Begin);
                        ImageSource = ImageSource.FromStream (() => isStreamCopy);
                        IsPhotoAvailable = true;
                    } else {
                        ImageSource = null;
                    }
                }

                UpdateCommands ();
            }
        }

        public ImageSource ImageSource {
            get { return _ImageSource; }
            private set { SetProperty (ref _ImageSource, value); }
        }
        private ImageSource _ImageSource = default (ImageSource);

        public bool IsPhotoAvailable {
            get { return _IsPhotoAvailable; }
            set { SetProperty (ref _IsPhotoAvailable, value); }
        }
        private bool _IsPhotoAvailable = default (bool);


        public bool IsCurrentPhotoAnalyzed {
            get { return _IsCurrentPhotoAnalyzed; }
            set { SetProperty (ref _IsCurrentPhotoAnalyzed, value); }
        }
        private bool _IsCurrentPhotoAnalyzed = default (bool);

        public int Age {
            get { return _Age; }
            set { SetProperty (ref _Age, value); }
        }
        private int _Age = default (int);

        public string Gender {
            get { return _Gender; }
            set { SetProperty (ref _Gender, value); }
        }
        private string _Gender = default (string);

        public string Emotion {
            get { return _Emotion; }
            set { SetProperty (ref _Emotion, value); }
        }
        private string _Emotion = default (string);

        public string StatusMessage {
            get { return _StatusMessage; }
            set { SetProperty (ref _StatusMessage, value); }
        }
        private string _StatusMessage = default (string);

        public ICommand TakePhotoCommand { get; set; }

        public ICommand PickPhotoCommand { get; set; }

        public ICommand AnalyzePhotoCommand { get; set; }

        private IMediaPicker _mediaPicker;
        private Microsoft.ProjectOxford.Face.FaceServiceClient faceClient;
        private Microsoft.ProjectOxford.Emotion.EmotionServiceClient emotionClient;

        public FaceViewModel () : base ()
        {
            _mediaPicker = XLabs.Ioc.Resolver.Resolve<IMediaPicker> ();

            TakePhotoCommand = new Command (async (_) => await DoTakePhoto (true),
                                     (_) => _mediaPicker.IsCameraAvailable);
            PickPhotoCommand = new Command (async (_) => await DoTakePhoto (false),
                                     (_) => _mediaPicker.IsPhotosSupported);

            faceClient = new Microsoft.ProjectOxford.Face.FaceServiceClient (ProjectOxfordFaceApiKey);
            emotionClient = new Microsoft.ProjectOxford.Emotion.EmotionServiceClient (ProjectOxfordEmotionApiKey);

            AnalyzePhotoCommand = new Command (async (_) => await DoAnalyzePhoto (), (_) => IsPhotoAvailable);
        }

        public void UpdateCommands ()
        {
            ((Command)TakePhotoCommand)?.ChangeCanExecute ();
            ((Command)PickPhotoCommand)?.ChangeCanExecute ();
            ((Command)AnalyzePhotoCommand)?.ChangeCanExecute ();
        }

        public async Task DoAnalyzePhoto ()
        {
            if (IsBusy || IsCurrentPhotoAnalyzed) return; // prevent reentering this method, can happen by quickly tapping the button
            IsBusy = true;

            try {

                UpdateCommands ();

                StatusMessage = "Looking for a face...";
                // reset the stream so it can be re-read
                CurrentPhotoStream.Seek (0, SeekOrigin.Begin);
                var faces = await faceClient.DetectAsync (CurrentPhotoStream, true, false,
                    new []{
                        Microsoft.ProjectOxford.Face.FaceAttributeType.Age,
                        Microsoft.ProjectOxford.Face.FaceAttributeType.Gender,
                    });

                // do our best to avoid exceptions
                if (faces != null && faces.Length == 1 && faces [0].FaceAttributes != null) {

                    Age = (int)faces [0].FaceAttributes.Age;
                    Gender = faces [0].FaceAttributes.Gender;
                    IsCurrentPhotoAnalyzed = true;

                    CurrentPhotoStream.Seek (0, SeekOrigin.Begin);

                    // this is an example of bad API design
                    var faceCRect = new Microsoft.ProjectOxford.Common.Rectangle {
                        Top = faces [0].FaceRectangle.Top,
                        Left = faces [0].FaceRectangle.Left,
                        Width = faces [0].FaceRectangle.Width,
                        Height = faces [0].FaceRectangle.Height,
                    };

                    StatusMessage = "Analyzing emotion...";
                    Emotion = "...";
                    var emotionResults = await emotionClient.RecognizeAsync (CurrentPhotoStream, new [] { faceCRect });
                    if (emotionResults != null && emotionResults.Length > 0 && emotionResults [0] != null) {
                        Emotion = string.Join (", ", emotionResults [0].Scores.ToRankedList ()
                                                    .Where (e => e.Value > 0.75)
                                                    .OrderByDescending (e => e.Value)
                                                    .Take (2)
                                                    .Select (e => e.Key + " (" + e.Value.ToString ("F2") + ")"));
                        if (string.IsNullOrWhiteSpace (Emotion))
                            Emotion = "Not recognized";

                        StatusMessage = "Ready.";
                    }
                }
            } catch (Exception x) {
                StatusMessage = "Error: " + x.Message;
                System.Diagnostics.Debug.WriteLine ("Error while analyzing a photo: " + x);
            } finally {
                IsBusy = false;
                UpdateCommands ();
            }
        }

        public async Task DoTakePhoto (bool isNewPhoto)
        {
            if (IsBusy) return; // prevent reentering this method, can happen by quickly tapping the button
            IsBusy = true;

            var cameraOptions = new CameraMediaStorageOptions {
                MaxPixelDimension = 1024,
                SaveMediaOnCapture = false
            };

            MediaFile mediaFile = null;
            try {

                UpdateCommands ();

                if (isNewPhoto) {
                    StatusMessage = "Taking a photo";
                    mediaFile = await _mediaPicker.TakePhotoAsync (cameraOptions);
                } else {
                    StatusMessage = "Picking a photo";
                    mediaFile = await _mediaPicker.SelectPhotoAsync (cameraOptions);
                }
                if (mediaFile != null && mediaFile.Source != null) {
                    // move the image stream to our own
                    MemoryStream copyImage = new MemoryStream ((int)mediaFile.Source.Length);
                    mediaFile.Source.CopyTo (copyImage);
                    copyImage.Seek (0, SeekOrigin.Begin);
                    CurrentPhotoStream = copyImage;
                    StatusMessage = "Photo " + (isNewPhoto ? "taken" : "selected");
                } else {
                    StatusMessage = "No photo";
                }

            } catch (TaskCanceledException) {
                // ignored, this is thrown when user clicks "Cancel"
                StatusMessage = "Cancelled";
            } catch (Exception x) {
                StatusMessage = "Error: " + x.Message;
                // always output exceptions unless well known
                System.Diagnostics.Debug.WriteLine ($"Error in DoTakePhoto({isNewPhoto}): {x}");
            } finally {
                // dispose the stream we received
                if (mediaFile != null)
                    mediaFile.Dispose ();
                IsBusy = false;

                UpdateCommands ();
            }
        }
    }
}


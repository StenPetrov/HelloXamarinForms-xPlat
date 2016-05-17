using System;
using System.Threading.Tasks;
using XamarinAzureMobileAppTestService.DataObjects;
using Xamarin.Forms;
using System.Collections.ObjectModel;

namespace HelloForms
{
    public class HelloViewModel : XLabs.Forms.Mvvm.ViewModel
    {
        public ObservableCollection<HelloItem> AllItems { get; private set; } = new ObservableCollection<HelloItem> ();

        private string _resultMessage = null;
        public string ResultMessage {
            get { return _resultMessage; }
            set { SetProperty (ref _resultMessage, value); }
        }

        private HelloItem _currentItem = new HelloItem ();

        public HelloItem CurrentItem {
            get { return _currentItem; }
            set { SetProperty (ref _currentItem, value); }
        }

        public Command InsertCommand { get; set; } 

        public Command ReloadCommand { get; set; }

        public HelloViewModel ()
        {
            ReloadCommand = new Command (async (prm) => {
                    await Reload (this);
                }, (viewModel) => !this.IsBusy);

            InsertCommand = new Command (async (viewModel) => {
                    await Insert (this);
                }, (viewModel) => !this.IsBusy);
        }


        public void ChangeIsBusy (bool isBusy)
        {
            IsBusy = isBusy;
            InsertCommand.ChangeCanExecute ();
            ReloadCommand.ChangeCanExecute ();
        }

        public static async Task Reload (HelloViewModel caller)
        {
            if (caller != null && !caller.IsBusy) {
                try {
                    caller.ChangeIsBusy (true);

                    var table = MobileAppClient.Client.GetTable<HelloItem> ();
                    var list = await table.Where (i => i.Id != null).ToListAsync ();

                    caller.AllItems.Clear ();
                    foreach (var item in list) {
                        caller.AllItems.Add (item);
                    }

                    caller.ResultMessage = $"Loaded {list.Count} items.";
                } catch (Exception x) {
                    caller.ResultMessage = "Error: " + x.Message;
                } finally {
                    caller.ChangeIsBusy (false);
                }
            }
        }

        public static async Task Insert (HelloViewModel caller)
        {
            if (caller != null && !caller.IsBusy) {
                caller.ChangeIsBusy (true);

                try {
                    var item = caller.CurrentItem;
                    var table = MobileAppClient.Client.GetTable<HelloItem> ();

                    if (string.IsNullOrEmpty (item.Id)) {
                        item.Id = Guid.NewGuid ().ToString ();
                        await table.InsertAsync (item);
                        caller.ResultMessage = "Created: " + item.Id;

                        caller.AllItems.Add (item);
                    } else {
                        await table.UpdateAsync (item);
                        caller.ResultMessage = "Updated: " + item.Id;
                    }
                } catch (Exception x) {
                    caller.ResultMessage = "Error: " + x.Message;
                } finally {
                    caller.ChangeIsBusy (false);
                }
            }
        }
    }
}


using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;

namespace HelloForms.iOS
{
    [Register ("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        public override bool FinishedLaunching (UIApplication app, NSDictionary options)
        { 
            Microsoft.WindowsAzure.MobileServices.CurrentPlatform.Init ();
            SQLitePCL.CurrentPlatform.Init ();

            global::Xamarin.Forms.Forms.Init ();

            // Code for starting up the Xamarin Test Cloud Agent
#if ENABLE_TEST_CLOUD
            Xamarin.Calabash.Start();
#endif
 
            XLabs.Ioc.Resolver.SetResolver (
                new XLabs.Ioc.SimpleContainer ()
                .Register<XLabs.Platform.Services.Media.IMediaPicker,XLabs.Platform.Services.Media.MediaPicker>()
                .GetResolver ());

            LoadApplication (new App ());

            return base.FinishedLaunching (app, options);
        }
    }
}


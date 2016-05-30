﻿using System;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace HelloForms.Droid
{
    [Activity (Label = "HelloForms.Droid", Icon = "@drawable/icon", Theme = "@style/MyTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate (Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate (bundle);

            Microsoft.WindowsAzure.MobileServices.CurrentPlatform.Init ();

            global::Xamarin.Forms.Forms.Init (this, bundle);

            XLabs.Ioc.Resolver.SetResolver (
                new XLabs.Ioc.SimpleContainer ()
                .Register<XLabs.Platform.Services.Media.IMediaPicker, XLabs.Platform.Services.Media.MediaPicker> ()
                .GetResolver ());

            LoadApplication (new App ());
        }
    }
}


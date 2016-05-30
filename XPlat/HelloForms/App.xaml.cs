using System;
using System.Collections.Generic;

using Xamarin.Forms;
using XLabs.Forms.Mvvm;

namespace HelloForms
{
    public partial class App : Application
    {
        public App ()
        { 
            ViewFactory.Register<FacePage, FaceViewModel> ();
            MainPage = (Page)ViewFactory.CreatePage (typeof (FaceViewModel));
        }

        protected override void OnStart ()
        {

        }

        protected override void OnSleep ()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume ()
        {
            // Handle when your app resumes
        }
    }
}


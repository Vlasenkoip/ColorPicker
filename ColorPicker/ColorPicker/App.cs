﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace ColorPicker
{
    public class App : Application
    {
        public static double ScreenWidth;
        public static double ScreenHeight;
        public static double ScreenDensity;

        public App()
        {
            MainPage = new PagePickerSample();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}

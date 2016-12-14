using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using NControl.Droid;

namespace ColorPicker.Droid
{
    [Activity(Label = "ColorPicker", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            global::Xamarin.Forms.Forms.Init(this, bundle);

            App.ScreenWidth = Resources.DisplayMetrics.WidthPixels;
            App.ScreenHeight = Resources.DisplayMetrics.HeightPixels;
            App.ScreenDensity = Resources.DisplayMetrics.Density;

            NControlViewRenderer.Init();

            LoadApplication(new App());
        }
    }
}


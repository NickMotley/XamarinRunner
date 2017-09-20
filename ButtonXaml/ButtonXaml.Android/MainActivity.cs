using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.App;
using AudioManager;
using Xamarin.Forms.Platform.Android;

namespace ButtonXaml.Android
{
    //[Activity(Label = "MotleyRunner", MainLauncher = true, 
    //    ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    //public class MainActivity : FormsApplicationActivity
    [Activity(Label = "MotleyRunner", 
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : FormsApplicationActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            Xamarin.Forms.Forms.Init(this, bundle);
            Xamarin.FormsMaps.Init(this, bundle);

			LoadApplication (new App ());

            Initializer.Initialize();
        }
    }
}


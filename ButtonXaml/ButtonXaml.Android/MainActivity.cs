using Android.App;
using Android.Content.PM;
using Android.OS;
using AudioManager;
using Xamarin.Forms.Platform.Android;

namespace ButtonXaml.Android
{
    [Activity(Label = "MotleyRunner", MainLauncher = true, ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : FormsApplicationActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            Xamarin.Forms.Forms.Init(this, bundle);
            //Xamarin.FormsMaps.Init(this, bundle);

			LoadApplication (new App ());

            Initializer.Initialize();
        }
    }
}


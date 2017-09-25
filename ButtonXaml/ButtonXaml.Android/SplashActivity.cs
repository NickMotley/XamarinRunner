using Android.App;
using Android.Content.PM;
using Android.OS;

namespace ButtonXaml.Android
{
    [Activity(Label = "MotleyRunner", MainLauncher = true, NoHistory = true, ScreenOrientation = ScreenOrientation.Portrait)]
    public class SplashActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            this.StartActivity(typeof(MainActivity));
        }
    }
}

using Android.App;
using Android.Content.PM;
using Android.OS;

namespace XamFormsWebSockets.Droid
{
    [Activity(Label = "XamFormsWebSockets", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            // Call in your platform (non-pcl) startup            
            // 1) Link in your main activity or AppDelegate or whatever
            Websockets.Droid.WebsocketConnection.Link();

            global::Xamarin.Forms.Forms.Init(this, bundle);
            LoadApplication(new App());
        }
    }
}


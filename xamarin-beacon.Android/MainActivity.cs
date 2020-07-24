using xamarin.beacon.Interface;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Org.Altbeacon.Beacon;
using Plugin.CurrentActivity;
using Plugin.Permissions;
using Xamarin.Forms;
using Xamarin.Forms.Platform;

namespace xamarin.beacon.Droid
{
	[Activity(Label = "xamarin.beacon.Droid",
			  Icon = "@mipmap/icon",
			  Theme = "@style/MainTheme",
			  MainLauncher = true,
			  LaunchMode = Android.Content.PM.LaunchMode.SingleInstance,
			  ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity, IBeaconConsumer
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			//TabLayoutResource = xamarin.beacon.Droid.Resource.Layout
			//ToolbarResource = Resource.Layout.Toolbar;

			base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState); // add this line to your code, it may also be called: bundle
            CrossCurrentActivity.Current.Init(this, savedInstanceState);

            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

			LoadApplication(new App());
		}

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        #region IBeaconConsumer Implementation
        public void OnBeaconServiceConnect()
		{
			//var beaconService = Xamarin.Forms.DependencyService.Get<IAltBeaconService>();

			////		beaconService.StartMonitoring();
			//beaconService.StartRanging();
		}
        #endregion
        protected override void OnDestroy()
        {
            base.OnDestroy();
            DependencyService.Get<IbeaconAndroid>().OnDestroy();
        }
    }
}

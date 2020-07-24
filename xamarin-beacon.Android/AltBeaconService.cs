using System;
using AltBeaconLibrary.Sample.Droid.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using AltBeaconLibrarySample;
using AltBeaconLibrarySample.Interface;
using Org.Altbeacon.Beacon;
using AltBeaconLibrarySample.Model;
using Android.Content;

[assembly: Xamarin.Forms.Dependency(typeof(AltBeaconService))]
namespace AltBeaconLibrary.Sample.Droid.Services
{
	public class AltBeaconService : Java.Lang.Object, IAltBeaconService
	{
		private readonly MonitorNotifier _monitorNotifier;
		private readonly RangeNotifier _rangeNotifier;
		private BeaconManager _beaconManager;

        Org.Altbeacon.Beacon.Region _tagRegion;

        Org.Altbeacon.Beacon.Region _emptyRegion;

		public AltBeaconService()
		{
			_monitorNotifier = new MonitorNotifier();
			_rangeNotifier = new RangeNotifier();
		}

		public BeaconManager BeaconManagerImpl
		{
			get
            {
				if (_beaconManager == null)
					_beaconManager = InitializeBeaconManager();
				return _beaconManager;
			}
		}

		public void InitializeService()
		{
            if(_beaconManager == null)
			    _beaconManager = InitializeBeaconManager();
		}

		private BeaconManager InitializeBeaconManager()
		{
			// Enable the BeaconManager 
			BeaconManager bm = BeaconManager.GetInstanceForApplication(Plugin.CurrentActivity.CrossCurrentActivity.Current.Activity);

			var iBeaconParser = new BeaconParser();
			//	Estimote > 2013
			iBeaconParser.SetBeaconLayout("m:2-3=0215,i:4-19,i:20-21,i:22-23,p:24-24");
			bm.BeaconParsers.Add(iBeaconParser);

			_monitorNotifier.EnterRegionComplete += EnteredRegion;
			_monitorNotifier.ExitRegionComplete += ExitedRegion;
			_monitorNotifier.DetermineStateForRegionComplete += DeterminedStateForRegionComplete;
			_rangeNotifier.DidRangeBeaconsInRegionComplete += RangingBeaconsInRegion;

			_tagRegion = new Org.Altbeacon.Beacon.Region/* AltBeaconOrg.BoundBeacon.Region*/("Beacon 2", Identifier.Parse("8E6DBFBB-489D-418A-9560-1BA1CE6301AA"), null, null);
			_tagRegion = new Org.Altbeacon.Beacon.Region/* AltBeaconOrg.BoundBeacon.Region*/("myUniqueBeaconId", Identifier.Parse("B9407F30-F5F8-466E-AFF9-25556B57FE6D"), null, null);
			_emptyRegion = new Org.Altbeacon.Beacon.Region/* AltBeaconOrg.BoundBeacon.Region*/("myEmptyBeaconId", null, null, null);

            //bm.SetBackgroundMode(false);

            bm.BackgroundMode = false;
            bm.Bind((IBeaconConsumer)Plugin.CurrentActivity.CrossCurrentActivity.Current.Activity);

			return bm;
		}

		public void StartMonitoring()
		{
            BeaconManagerImpl.ForegroundBetweenScanPeriod = 5000;
            BeaconManagerImpl.BackgroundBetweenScanPeriod = 5000;

            BeaconManagerImpl.AddMonitorNotifier(_monitorNotifier);
            BeaconManagerImpl.StartMonitoringBeaconsInRegion(_tagRegion);
            BeaconManagerImpl.StartMonitoringBeaconsInRegion(_emptyRegion);
		}

		public void StartBroadcasting()
		{
			

			Context context = Android.App.Application.Context;

			BeaconTransmitter mBeaconTransmitter = new BeaconTransmitter(context, new BeaconParser().SetBeaconLayout("m:2-3=0215,i:4-19,i:20-21,i:22-23,p:24-24"));
			Beacon beacon = new AltBeacon.Builder()
						.SetId1("FDA50693-A4E2-4FB1-AFCF-C6EB07647825")
						.SetId2("1010") //Major
						.SetId3("2000") //Minor
						.SetManufacturer(0x004C)
						.SetTxPower(-59)
						.Build();
			mBeaconTransmitter.StartAdvertising(beacon);

		}

		public void StartRanging()
        {
			BeaconManagerImpl.ForegroundBetweenScanPeriod = 100;
			BeaconManagerImpl.BackgroundScanPeriod = 500;
			BeaconManagerImpl.BackgroundBetweenScanPeriod = 30000;
			BeaconManagerImpl.ForegroundScanPeriod = 200;

			BeaconManagerImpl.AddRangeNotifier(_rangeNotifier);
			try
			{
			BeaconManagerImpl.StartRangingBeaconsInRegion(_tagRegion);
			BeaconManagerImpl.StartRangingBeaconsInRegion(_emptyRegion);
			}
			catch(Exception ex)
			{

			   System.Diagnostics.Debug.WriteLine("StartRangingException: " + ex.Message);
			}

		}


		public void StopRanging()
        {
            if(_beaconManager != null)
            {
                try
                {
                    BeaconManagerImpl.StopRangingBeaconsInRegion(_tagRegion);
                    BeaconManagerImpl.StopRangingBeaconsInRegion(_emptyRegion);
                    BeaconManagerImpl.RemoveRangeNotifier(_rangeNotifier);
                }
                catch (Exception ex)
                {

                    System.Diagnostics.Debug.WriteLine("StopRangingException: " + ex.Message);
                }
            }
        }

		private void DeterminedStateForRegionComplete(object sender, MonitorEventArgs e)
		{
			Console.WriteLine("DeterminedStateForRegionComplete");
		}

		private void ExitedRegion(object sender, MonitorEventArgs e)
		{
			string region = "???";
			if (e.Region != null) {
				if (e.Region.Id1 == null)
					region = "null region";
				else
					region = e.Region.Id1.ToString ().ToUpper ();
			}

			Xamarin.Forms.MessagingCenter.Send<App, string> ((App)Xamarin.Forms.Application.Current, "ExitedRegion", region);
			Console.WriteLine("ExitedRegion");
		}

		private void EnteredRegion(object sender, MonitorEventArgs e)
		{
			string region = "???";
			if (e.Region != null) {
				if (e.Region.Id1 == null)
					region = "null region";
				else
					region = e.Region.Id1.ToString ().ToUpper ();
			}

			Xamarin.Forms.MessagingCenter.Send<App, string> ((App)Xamarin.Forms.Application.Current, "EnteredRegion", region);

			Console.WriteLine("EnteredRegion");
		}

        List<SharedBeacon> _sharedBeacons = new List<SharedBeacon>();

        object _lock = new object();

		void RangingBeaconsInRegion(object sender, RangeEventArgs e)
		{

            _sharedBeacons = new List<SharedBeacon>();

            lock (_lock)
            {

                // Get all beacons and create the SharedBeacon
                foreach (Beacon beacon in e.Beacons)
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("NAME {0} - IP {1} - {2}dB", beacon.BluetoothName, beacon.BluetoothAddress, beacon.Rssi));
                    _sharedBeacons.Add(new SharedBeacon(beacon.BluetoothName, beacon.BluetoothAddress, beacon.Id1.ToString(), beacon.Id2.ToString(), beacon.Id3.ToString(), beacon.Distance, beacon.Rssi));
                };


                Task.Run(() =>
                {
                    // I send beacons to XF project
                    if (_sharedBeacons.Count > 0)
                    {
                        System.Diagnostics.Debug.WriteLine("I SEND TO XF " + _sharedBeacons.Count + " BEACONS");
                        Xamarin.Forms.MessagingCenter.Send<App, List<SharedBeacon>>((App)Xamarin.Forms.Application.Current, "BeaconsReceived", _sharedBeacons);
                    }
                });

            }

        }

        public void SetBackgroundMode(bool isBackground)
        {
            if (_beaconManager != null)
                BeaconManagerImpl.BackgroundMode = isBackground;

        }

        public void OnDestroy()
        {

            if (_beaconManager != null && BeaconManagerImpl.IsBound((IBeaconConsumer)Plugin.CurrentActivity.CrossCurrentActivity.Current.Activity))
                BeaconManagerImpl.Unbind((IBeaconConsumer)Plugin.CurrentActivity.CrossCurrentActivity.Current.Activity);

        }
    }
}


using System;
using CoreBluetooth;
using CoreFoundation;
using CoreLocation;
using Foundation;
using UIKit;
using xamarin.beacon.Interface;

namespace xamarin.beacon.iOS
{
	class PeripheralManagerDelegate : CBPeripheralManagerDelegate
	{
		public override void StateUpdated(CBPeripheralManager peripheral)
		{
		}
	}

	public class Blebroadcast : iOSTransmit
	{
		CBPeripheralManager peripheralManager;



		public CBPeripheralManager beaconset
		{
			get
			{
				if (peripheralManager == null)
					peripheralManager = beaconconfig();
				return peripheralManager;

			}

		}

		public void InitializeService()
		{
			if (peripheralManager == null)
				peripheralManager = beaconconfig();
		}
		public CBPeripheralManager beaconconfig()
		{
			try
			{
				var peripheralDelegate = new PeripheralManagerDelegate();
				peripheralManager = new CBPeripheralManager(peripheralDelegate, DispatchQueue.DefaultGlobalQueue);
				return peripheralManager;
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine("Error is " + ex.Message);
				return null;
			}

		}

		public void StartBroadcasting(string id1)
		{
			CLBeaconRegion region = Helpers.CreateRegion(new NSUuid(id1), new NSNumber(5050), new NSNumber(1234));

			if (region != null)
			{
				peripheralManager.StartAdvertising(region.GetPeripheralData(new NSNumber(50)));
			}
		}
	}

}
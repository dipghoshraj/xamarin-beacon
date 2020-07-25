using System.Collections.Generic;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System;
using System.ComponentModel;
using xamarin.beacon.Model;
using xamarin.beacon.Interface;
using xamarin.beacon.Page;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System.Threading.Tasks;

namespace xamarin.beacon.ViewModel
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        public bool IsStartedRanging { get; set; }

        public bool IsTransmitting { get; set; }


        public ObservableCollection<SharedBeacon> ReceivedBeacons { get; set; } = new ObservableCollection<SharedBeacon>();

        public event PropertyChangedEventHandler PropertyChanged;

        public MainPageViewModel()
        {

            this.StartRangingCommand = new Command(() => {
                startRangingBeacon();
            });

            this.StartTransmitting = new Command(() =>
            {
                starttransmitbeacon();
            });

            this.OnAppearingCommand = new Command(() =>
            {

                Task.Run(async () =>
                {
                    var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Location);

                    if (status != PermissionStatus.Granted)
                        status = await Util.Permissions.CheckPermissions(Permission.Location);
                });

                MessagingCenter.Subscribe<App>(this, "CleanBeacons", (sender) =>
                {
                    List<SharedBeacon> receivedBeacons = new List<SharedBeacon>(ReceivedBeacons);

                    updateBeaconCurrentDateTime(receivedBeacons, DateTime.Now);
                    deleteOldBeacons(receivedBeacons);

                    ReceivedBeacons = convertToObservableCollection(receivedBeacons);
                });

                MessagingCenter.Subscribe<App, List<SharedBeacon>>(this, "BeaconsReceived", (sender, arg) =>
                {
                    if (arg != null && arg is List<SharedBeacon>)
                    {
                        System.Diagnostics.Debug.WriteLine("Received: " + ((List<SharedBeacon>)arg).Count);
                        List<SharedBeacon> temp = arg;
                        List<SharedBeacon> receivedBeacons = new List<SharedBeacon>(ReceivedBeacons);

                        if (arg != null && arg.Count > 0)
                        {

                            DateTime now = DateTime.Now;

                            updateBeaconCurrentDateTime(receivedBeacons, now);

                            foreach (SharedBeacon sharedBeacon in arg)
                            {

                                // Is the beacon already in list?
                                var ret = receivedBeacons.Where(o => o.BluetoothAddress == sharedBeacon.BluetoothAddress).FirstOrDefault();
                                if (ret != null) // Is present
                                {
                                    var index = receivedBeacons.IndexOf(ret);
                                    receivedBeacons[index].Update(now, sharedBeacon.Distance, sharedBeacon.Rssi); // Update last received date time
                                }
                                else
                                {
                                    receivedBeacons.Insert(0, sharedBeacon);
                                }
                            }

                            deleteOldBeacons(receivedBeacons);

                            ReceivedBeacons = convertToObservableCollection(receivedBeacons);

                        }

                    }
                });

            });

            this.OnDisappearingCommand = new Command(() =>
            {
                MessagingCenter.Unsubscribe<App, List<SharedBeacon>>(this, "BeaconsReceived");
                MessagingCenter.Unsubscribe<App>(this, "CleanBeacons");
            });
        }

        private ObservableCollection<SharedBeacon> convertToObservableCollection(List<SharedBeacon> receivedBeacons)
        {
            if (receivedBeacons != null)
                return new ObservableCollection<SharedBeacon>(receivedBeacons.OrderByDescending(o => o.Rssi));

            return null;
        }

        private void startRangingBeacon()
        {
            var beaconService = Xamarin.Forms.DependencyService.Get<IbeaconAndroid>();
            beaconService.BuletoothEnable();

            if (!IsStartedRanging)
                beaconService.StartRanging();
            else
                beaconService.StopRanging();

            IsStartedRanging = !IsStartedRanging;
        }

        private void starttransmitbeacon()
        {
            var beaconService = Xamarin.Forms.DependencyService.Get<IbeaconAndroid>();
            beaconService.BuletoothEnable();
            beaconService.StartBroadcasting();

            IsTransmitting = !IsTransmitting;
        }

        private void updateBeaconCurrentDateTime(List<SharedBeacon> receivedBeacons, DateTime now)
        {
            // Update current received date time
            foreach (SharedBeacon shared in receivedBeacons)
                shared.CurrentDateTime = now;
        }

        private void deleteOldBeacons(List<SharedBeacon> receivedBeacons)
        {
            if (receivedBeacons != null)
            {
                int count = receivedBeacons.Count;

                // Delete old beacons
                for (int ii = count - 1; ii >= 0; ii--)
                {
                    try
                    {
                        if (receivedBeacons[ii].ForceDelete)
                            receivedBeacons.RemoveAt(ii);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex.Message);
                    }
                }
            }
        }

        public DataTemplate ViewCellBeaconTemplate
        {
            get
            {
                return new DataTemplate(typeof(MainPage.ViewCellBeacon));
            }
        }

        public ICommand StartRangingCommand { get; protected set; }
        public ICommand OnAppearingCommand { get; protected set; }
        public ICommand OnDisappearingCommand { get; protected set; }
        public ICommand StartTransmitting { get; protected set; }

    }
}

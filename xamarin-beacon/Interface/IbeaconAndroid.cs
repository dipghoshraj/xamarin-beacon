using System;
using System.Collections.Generic;
using System.Text;

namespace xamarin.beacon.Interface
{
    public interface IbeaconAndroid {
        void InitializeService();
        void StartMonitoring();
        void StartBroadcasting();
        void StartRanging();
        void StopRanging();
        void SetBackgroundMode(bool isBackground);
        void OnDestroy();
        void BuletoothEnable();
    }

}
                                                                            
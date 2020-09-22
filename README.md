# xamarin-beacon



[![Build Status](https://travis-ci.org/joemccann/dillinger.svg?branch=master)](https://travis-ci.org/joemccann/dillinger)

Xamarin-beacon is a ibeacon simulation application for android and ios device.

  - Scanning the nearby ibeacons
  - Broadcast an iBeacon

This application is build upon the Altbeacon libery for the android device and for ios device we have used iOS builtin libery corelocation for scanning and corebluetooth for broadcasting. 


### Installation

Xamarin-beacon reqires visual studio(2019 or above) and a physical device this  will not work on a emulaor.

- Clone the repository
- Open the solution file (.sln)
- run on the device



### Development

Want to make your changes, Great!

- Ui changes need to do in application folder Page/MainPage.cs
- Ui related functions are on ViewModel/MainPageViewModel.cs
- Beacon functions for android .Android AltBeaconService.cs
- Beacon function for ios .iOS Blescan.cs --> scanner and BleTransmit.cs --> broadcast
- All configuration for the iBeacon function initilized on application class

### For ios add this on info.plist

``` bash
<key>UIBackgroundModes</key>
<array>
    <!--for connecting to devices (client)-->
    <string>bluetooth-central</string>

    <!--for server configurations if needed-->
    <string>bluetooth-peripheral</string>
</array>

<!--Description of the Bluetooth request message (required on iOS 10, deprecated)-->
<key>NSBluetoothPeripheralUsageDescription</key>
<string>YOUR CUSTOM MESSAGE</string>

<!--Description of the Bluetooth request message (required on iOS 13)-->
<key>NSBluetoothAlwaysUsageDescription</key>
<string>YOUR CUSTOM MESSAGE</string>
```


### Todos

 - Need to fix the codeQL work-flow

License
----
MIT

**Free Software, Hell Yeah!**

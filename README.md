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


### Todos

 - Now not all beacon are scannning on the ios device, working on the libery to scann every beacon
 - Over ios 13 iBeacon broadcast is causing app to crash due to permisson issue need to fixed this

License
----
MIT

**Free Software, Hell Yeah!**

using System;
using System.Globalization;
using Behaviors;
using xamarin.beacon.ViewModel;
using Xamarin.Forms;

namespace xamarin.beacon.Page
{
    public class MainPage : ContentPage
    {

        MainPageViewModel _vm = null;

        public MainPage()
        {
            _vm = new MainPageViewModel();
            this.BindingContext = _vm;


            Button buttonStart = new Button { Text = "START SCANNING BEACONS" };
            buttonStart.SetBinding(Button.CommandProperty, "StartRangingCommand");
            buttonStart.SetBinding(Button.TextProperty, new Binding("IsStartedRanging", converter: new IsStartedRangingToTextConverter()));
            Label labelInfo = new Label() { HorizontalOptions = LayoutOptions.Center, Text = "Beacons oredered by RSSI" };

            Button buttonTransmit = new Button { Text = "START BEACON" };
            buttonTransmit.SetBinding(Button.CommandProperty, "StartTransmitting");
            buttonTransmit.SetBinding(Button.TextProperty, new Binding("IsTransmitting", converter: new IsTransmittingToTextConverter()));


            StackLayout slHeader = new StackLayout() { Children = { buttonTransmit, buttonStart, labelInfo, } };

            ListView lv = new ListView { HasUnevenRows = true, SeparatorColor = Color.Black, SeparatorVisibility = SeparatorVisibility.Default, Header = slHeader };
            lv.SetBinding(ListView.ItemsSourceProperty, "ReceivedBeacons");
            lv.SetBinding(ListView.ItemTemplateProperty, "ViewCellBeaconTemplate");

            StackLayout sl = new StackLayout
            {
                Children = { lv }
            };

            CompressedLayout.SetIsHeadless(sl, true);

            Content = sl;

            InvokeCommandAction icaOnAppearing = new InvokeCommandAction();
            icaOnAppearing.SetBinding(InvokeCommandAction.CommandProperty, "OnAppearingCommand");
            EventHandlerBehavior ehbOnAppearing = new EventHandlerBehavior() { EventName = "Appearing" };
            ehbOnAppearing.Actions.Add(icaOnAppearing);

            InvokeCommandAction icaOnDisappearing = new InvokeCommandAction();
            icaOnDisappearing.SetBinding(InvokeCommandAction.CommandProperty, "OnDisappearingCommand");
            EventHandlerBehavior ehbOnDisappearing = new EventHandlerBehavior() { EventName = "Disappearing" };
            ehbOnDisappearing.Actions.Add(icaOnDisappearing);

            this.Behaviors.Add(ehbOnAppearing);
            this.Behaviors.Add(ehbOnDisappearing);
        }

        public class ViewCellBeacon : ViewCell
        {

            public ViewCellBeacon()
            {

                Label labelId1 = new Label();
                labelId1.SetBinding(Label.TextProperty, new Binding("Id1", stringFormat: "Id1: {0}"));

                Label labelBluetoothName = new Label { FontSize = 30 };
                labelBluetoothName.SetBinding(Label.TextProperty, "BluetoothName");

                Label labelBluetoothAddress = new Label();
                labelBluetoothAddress.SetBinding(Label.TextProperty, "BluetoothAddress");

                Label labelId2 = new Label();
                labelId2.SetBinding(Label.TextProperty, new Binding("Id2", stringFormat: "Id2: {0}"));

                Label labelId3 = new Label();
                labelId3.SetBinding(Label.TextProperty, new Binding("Id3", stringFormat: "Id3: {0}"));


                Grid grid = new Grid();
                grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) });
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(2, GridUnitType.Star) }); // IP
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) }); // ID2
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) }); // ID3
                grid.Children.Add(labelBluetoothAddress, 0, 1, 0, 1);
                grid.Children.Add(labelId2, 1, 2, 0, 1);
                grid.Children.Add(labelId3, 2, 3, 0, 1);

                Label labelRssi = new Label();
                labelRssi.SetBinding(Label.TextProperty, new Binding("Rssi", stringFormat: "Rssi: {0}dB"));

                Label labelDistance = new Label();
                labelDistance.SetBinding(Label.TextProperty, new Binding("Distance", stringFormat: "{0:0.00} mt"));

                Label labelTimeStamp = new Label() { HorizontalOptions = LayoutOptions.Center };
                labelTimeStamp.SetBinding(Label.TextProperty, new Binding("LastReceivedDateTime", stringFormat: "{0:HH:mm:ss.fff}"));
                labelTimeStamp.SetBinding(VisualElement.BackgroundColorProperty, "BackgroundColor");

                Grid gridBottom = new Grid();
                gridBottom.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) });
                gridBottom.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) }); // RSSI
                gridBottom.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) }); // Timestamp
                gridBottom.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) }); // Distnace
                gridBottom.Children.Add(labelRssi, 0, 1, 0, 1);
                gridBottom.Children.Add(labelTimeStamp, 1, 2, 0, 1);
                gridBottom.Children.Add(labelDistance, 2, 3, 0, 1);


                StackLayout sl = new StackLayout { Children = { labelBluetoothName, labelId1, grid, gridBottom } };

                View = sl;
            }

        }

        private class IsStartedRangingToTextConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {

                if (value != null && value is bool)
                {
                    if (((bool)value))
                        return "STOP SCANNING BEACON";
                    return "START SCANNING BEACON";
                }
                return "";
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }


        private class IsTransmittingToTextConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {

                if (value != null && value is bool)
                {
                    if (((bool)value))
                        return "STOP BEACON";
                    return "START BEACON";
                }
                return "";
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }
    }
}

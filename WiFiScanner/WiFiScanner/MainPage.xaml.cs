using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.WiFi;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace WiFiScanner
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        public MainPage()
        {
            this.InitializeComponent();
        }

        private static async System.Threading.Tasks.Task DoNetworkScanAsync(ListView populateList)
        {
            IReadOnlyList<WiFiAdapter> adapters = await WiFiAdapter.FindAllAdaptersAsync();

            populateList.Items.Clear();

            if (adapters.Count == 0)
            {
                populateList.Items.Add("No Wi-Fi adapters found.");
            }
            else
            {
                // use the first adapter because I'm too lazy to implement selecting from multiple adapters
                WiFiAdapter a = adapters[0];

                // Scan for available networks
                await a.ScanAsync();

                // Filter and sort the networks, using Linq for Objects syntax.
                List<WiFiAvailableNetwork> displayList = new List<WiFiAvailableNetwork>();
                IEqualityComparer<WiFiAvailableNetwork> ssidCmp = new SSIDComparer();

                // Remove networks we don't want to display, like empty SSIDs, WiFiDirect endpoints, and infrastructure endpoints, and then throw out duplicate SSIDs.
                displayList = a.NetworkReport.AvailableNetworks
                    .Where(thisNetwork => (thisNetwork.Ssid.Length != 0) && (thisNetwork.IsWiFiDirect == false) && (thisNetwork.NetworkKind == WiFiNetworkKind.Infrastructure))
                    .Distinct(ssidCmp)
                    .ToList();

                // Sort the list by signal strength, and populate the control.
                displayList.Sort(CompareBySignalBars);

                // data bind the list to the ListView
                populateList.ItemsSource = displayList;

            }
        }

        // Compare by strength in bars, highest value first
        private static int CompareBySignalBars(WiFiAvailableNetwork left, WiFiAvailableNetwork right)
        {
            return right.SignalBars.CompareTo(left.SignalBars); // higher before lower
        }

        // Compare by SSID, and by signal bars when SSIDs are same. Alpha, a-z, signal bars from high to low
        private static int CompareByName(WiFiAvailableNetwork left, WiFiAvailableNetwork right)
        {
            int ret = left.Ssid.CompareTo(right.Ssid); // a before z
            if (ret == 0)
            {
                ret = right.SignalBars.CompareTo(left.SignalBars); // higher before lower
            }

            return ret;
        }

        private async void ScanButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            await DoNetworkScanAsync(APList);
        }
    }
    public class BarsToCharacterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Byte val = (Byte)value;
            Char retval = '?';


            // return the Segoe MDL2 Assets glyph for the number of bars given
            // requires that the textbox you're displaying be set to use Segoe MDL2 Assets
            switch (val)
            {
                case 0:
                    retval = (Char)57829; // 0xE9040;
                    break;
                case 1:
                    retval = (Char)57830; // 0xE905;
                    break;
                case 2:
                    retval = (Char)57831; // 0xE906;
                    break;
                case 3:
                    retval = (Char)57832; // 0xE907;
                    break;
                case 4:
                    retval = (Char)57833; // 0xE908;
                    break;
                default:
                    {
                        retval = '!';
                        break;
                    }
            }
            return retval;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    public class FrequencyToChannelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            int val = (int)value;
            string retval = "0";

            switch (val)
            {
                case 2412000:
                    retval = "1";
                    break;
                case 2417000:
                    retval = "2";
                    break;
                case 2422000:
                    retval = "3";
                    break;
                case 2427000:
                    retval = "4";
                    break;
                case 2432000:
                    retval = "5";
                    break;
                case 2437000:
                    retval = "6";
                    break;
                case 2442000:
                    retval = "7";
                    break;
                case 2447000:
                    retval = "8";
                    break;
                case 2452000:
                    retval = "9";
                    break;
                case 2457000:
                    retval = "10";
                    break;
                case 2462000:
                    retval = "11";
                    break;
                default:
                    {
                        if ((val >= 5180000) && (val <= 5320000))
                        { retval = (((val - 5180000) / 5000) + 38).ToString(); }
                        else if ((val >= 5500000) && (val <= 5825000))
                        { retval = (((val - 5500000) / 5000) + 100).ToString(); }
                        else
                        { retval = val.ToString(); }
                        break;
                    }
            }
            return $"{retval}";

        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class SSIDComparer : IEqualityComparer<WiFiAvailableNetwork>
    {
        public bool Equals(WiFiAvailableNetwork left, WiFiAvailableNetwork right)
        {
            return left.Ssid == right.Ssid;
        }

        public int GetHashCode(WiFiAvailableNetwork foo)
        {
            return foo.GetHashCode();
        }
    }

}

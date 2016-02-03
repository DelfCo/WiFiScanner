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
                WiFiAdapter a = adapters[0];

                await a.ScanAsync();
                int numNetworksAfter = a.NetworkReport.AvailableNetworks.Count;

                // Populate the ListView with a list of appropriate network APs.
                if (numNetworksAfter > 0)
                {
                    // fill a list of valid network APs
                    List<WiFiAvailableNetwork> displayList = new List<WiFiAvailableNetwork>();
                    foreach (WiFiAvailableNetwork thisNetwork in a.NetworkReport.AvailableNetworks)
                    {
                        // skip hidden (no SSID) networks, skip Wi-Fi Direct devices, skip ad hoc 
                        if ((thisNetwork.Ssid.Length != 0) && (thisNetwork.IsWiFiDirect == false) && (thisNetwork.NetworkKind == WiFiNetworkKind.Infrastructure))
                        {
                            displayList.Add(thisNetwork);
                        }
                    }

                    // data bind that list to the ListView
                    populateList.ItemsSource = displayList;
                }
            }
        }

        private async void ScanButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            await DoNetworkScanAsync(APList);
        }
    }
}

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Tranquility.Security;
using Windows.ApplicationModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Windows.UI.ViewManagement;
using Windows.Foundation;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Diagnostics;

namespace Tranquility.Views
{
    public sealed partial class SettingsPage : Page, INotifyPropertyChanged
    {
        

        public SettingsPage()
        {
            InitializeComponent();
            Settings.BorderBrush = save_rpc.BorderBrush;
            Settings.BorderThickness = new Thickness(1, 1, 1, 1);
            this.Loaded += SettingsPage_Loaded;
        }

        private void SettingsPage_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            await InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            try
            {
                if (Core.Runtime.WalletRPCprovider != null & Core.Runtime.WalletRPCprovider != "https://autumn-tame-field.solana-mainnet.quiknode.pro/af19ce3e33a293839ae3faf65e4989d05d95ebba")
                    rpcfield.Text = Core.Runtime.WalletRPCprovider;
            }
            catch (Exception error)
            {
                Debug.WriteLine(error);
            }
            await Task.CompletedTask;
        }

  

    

        public event PropertyChangedEventHandler PropertyChanged;

        private void Set<T>(ref T storage, T value, [CallerMemberName]string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return;
            }

            storage = value;
            OnPropertyChanged(propertyName);
        }

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private void save_rpc_Click(object sender, RoutedEventArgs e)
        {
            Core.Runtime.SaveRPCProvider(rpcfield.Text);
        }

        private async void unlock_seed_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var hashPW = SHA512.Hash(seedphrase_passphasefield.Password);
                var hashPWReal = await DataProtection.UnprotectData(Core.Runtime.SolanaVault.HashedProtectedKey);
                if (hashPW == hashPWReal)
                {
                    PhraseDisplay.Text = await DataProtection.UnprotectData(Core.Runtime.SolanaVault.Wallet);
                }
            }
            catch (Exception error)
            {
                Debug.WriteLine(error.Message);
            }
        }

        private async void createAcc_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Core.Runtime.SolanaVault.WalletIndexChart.Add(Convert.ToInt32(indexSelector.Text));
                Core.Runtime.SolanaVault.SaveWalletIndex();
                await Wallets.SolanaWallet.GenerateActiveAccounts();
            }
            catch(Exception error)
            {
                Debug.WriteLine(error.Message);
            }
        }


        private void ReceiveNavButton_Click(object sender, RoutedEventArgs e)
        {
            Core.Runtime.SelectedNavItem = 2;
            Frame.Navigate(typeof(Receive));
        }

        private void OverviewNavButton_Click(object sender, RoutedEventArgs e)
        {
            Core.Runtime.SelectedNavItem = 1;
            Frame.Navigate(typeof(Overview));
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            Core.Runtime.SelectedNavItem = 5;
            Frame.Navigate(typeof(SettingsPage));
        }

    }
}

using Windows.Foundation;
using Tranquility.Security;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using Windows.UI.ViewManagement;

namespace Tranquility.Views
{
    public sealed partial class MainPage : Page
    {
        public bool firstRun = false;
        public bool import = false;
        public MainPage()
        {
            this.InitializeComponent();

            if (Core.Runtime.SuccessfullyLoaded == false)
            {
                this.PasswordField.PlaceholderText = "Create a Wallet Password";
                this.AuthButton.Content = "Create Wallet";
           
                firstRun = true;
            }
            else
            {

                this.PasswordField.PlaceholderText = "Enter Wallet Password";
                this.AuthButton.Content = "Login";

            }
            this.SizeChanged += App_SizeChanged;
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
              
        }

        private void App_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            
        }

        private async void Auth_click(object sender, RoutedEventArgs e)
        {
            try
            {
                    if (String.IsNullOrEmpty(PasswordField.Password))
                        throw new FormatException("Solana Vault requires a password!");

                    if (firstRun == true)
                    {
                        if (!String.IsNullOrEmpty(PasswordField.Password))
                            Wallets.SolanaWallet.CreateNewWallet(PasswordField.Password);
                        Core.Runtime.SelectedAccount = 0;
                        await System.Threading.Tasks.Task.Delay(1000);
                       
                        Frame.Navigate(typeof(Overview));
                    }
                    else
                    {
                        var hashPW = SHA512.Hash(PasswordField.Password);
                        var hashPWReal = await DataProtection.UnprotectData(Core.Runtime.SolanaVault.HashedProtectedKey);
                        if (hashPW == hashPWReal)
                        {
                            //password matched
                            Core.Runtime.SelectedAccount = 0;
                            Core.Runtime.SolanaVault.ProtectedSessionKey = await DataProtection.ProtectAsync(PasswordField.Password);
                            await System.Threading.Tasks.Task.Delay(1000);
                            
                            Frame.Navigate(typeof(Overview));
                        }
                        else
                        {
                            LoginMessageBlock.Visibility = Visibility.Visible;
                            LoginMessageBlock.Text = "Incorrect Password!";
                        }
                    }
              
            }catch (FormatException ex)
            {
                LoginMessageBlock.Text = ex.Message;
                LoginMessageBlock.Visibility = Visibility.Visible;
            }
        }


        private void AppTitle_Copy_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }

        private void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
          
        }

        private void PasswordField_PasswordChanged(object sender, RoutedEventArgs e)
        {

        }

        private void Page_Loaded(FrameworkElement sender, object args)
        {
            ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(960, 540));

        }
    }
}

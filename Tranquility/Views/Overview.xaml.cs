using Solnet.Wallet;
using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using System.Globalization;
using Solnet.Rpc;
using Solnet.Programs;
using Windows.Web.Http;
using Newtonsoft.Json;
using System.Linq;
using Solnet.Metaplex.NFT.Library;
using Solnet.Metaplex.Utilities.Json;
using System.Collections.Generic;
using System.Diagnostics;

namespace Tranquility.Views
{
    public sealed partial class Overview : Page
    {
        public const string UserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:47.0) Gecko/20100101 Firefox/47.0";
        public Overview()
        {
            InitializeComponent();
            this.Loading += Overview_Loading;
            OverviewNavButton.BorderBrush = tokenlistPreviewborder.BorderBrush;
            OverviewNavButton.BorderThickness = new Thickness(1, 1, 1, 1);
            this.Loaded += Overview_loaded;
        }

        private void Overview_loaded(object sender, RoutedEventArgs e)
        {
          
        }

        private async void Overview_Loading(FrameworkElement sender, object args)
        {
            try
            {
                if (Core.Runtime.OverviewInitialized == false)
                {
                    await Wallets.SolanaWallet.GenerateActiveAccounts();
                    Core.Runtime.OverviewInitialized = true;
                }
                else
                {
                    await Wallets.SolanaWallet.RetrieveActiveAccountData();
                }

                ActiveAccountsSelector.Items.Clear();
                foreach (var activeAcc in Core.Runtime.SolanaVault.ActiveAccounts)
                {
                    ActiveAccountsSelector.Items.Add(activeAcc.Address);
                }
                ActiveAccountsSelector.PlaceholderText = ActiveAccountsSelector.Items[Core.Runtime.SelectedAccount].ToString();
            }
            catch
            {
                WalletBalance.Text = "Something went wrong.. Check your internet connection!";
            }
            if(Core.Runtime.IsLinkEnabled)
                LinkStreamToggleSwitch.IsOn = true;
            LoadWalletData();
        }

        private async void LoadWalletData()
        {
            try
            {
                decimal wallet_worth = 0;
                var solanaTile = (ImageBrush)((Border)(((ListViewItem)TokenAccountList.Items[0]).Content)).Background;
                var Position = new TranslateTransform();
                Position.X = 20;
                solanaTile.AlignmentX = AlignmentX.Left;
                solanaTile.Transform = Position;
                ((Border)(((ListViewItem)TokenAccountList.Items[0]).Content)).Background = solanaTile;
                ((TextBlock)((Border)((ListViewItem)TokenAccountList.Items[0]).Content).Child).Text = Core.Runtime.TokenWallet.Sol.ToString() + Environment.NewLine + "SOL";
               Core.Runtime.Inventory = new List<MetadataAccount>();
                int tokenAccCount = 0;
                if (TokenAccountList.Items.Count > 1)
                {
                    foreach (var token in TokenAccountList.Items.ToArray())
                    {
                        if (token != TokenAccountList.Items[0])
                        {
                            TokenAccountList.Items.Remove(token);
                        }
                        tokenAccCount++;
                    }
                }
                InventoryGrid.Items.Clear();
                Core.Runtime.Tokens.Clear();
                foreach (var account in Core.Runtime.TokenWallet.TokenAccounts())
                {
                    try {
                        if (account.QuantityDecimal != 0)
                        {
                            ListViewItem balanceTile = new ListViewItem();
                            Border balanceTileBorder = new Border();
                            string symbol = string.Empty;
                            if (account.QuantityDecimal != 0)
                            {
                                if (account.Symbol != "USDC")
                                {
                                    var price = await Utilities.Dexscreener.GetPrice(account.TokenMint);
                                    var _tokenAccountWorth = account.QuantityDecimal * price;
                                    wallet_worth = wallet_worth + _tokenAccountWorth;
                                }
                                if (account.Symbol == String.Empty)
                                {
                                    var token = await Utilities.Dexscreener.GetTokenData(account.TokenMint);
                                    if (token != null && token.pairs != null && token.pairs.Count() > 0)
                                    {
                                        if (token.pairs[0].baseToken.address == account.TokenMint)
                                        {
                                            symbol = token.pairs[0].baseToken.symbol;
                                            Core.Runtime.Tokens.Add(symbol, account.TokenMint);
                                        }
                                        if (token.pairs[0].quoteToken.address == account.TokenMint)
                                        {
                                            symbol = token.pairs[0].quoteToken.symbol;
                                            Core.Runtime.Tokens.Add(symbol, account.TokenMint);
                                        }
                                    }
                                }
                                var rpcClient = ClientFactory.GetClient(Core.Runtime.WalletRPCprovider);
                                MetadataAccount metadataAccount = null;
                                try
                                {
                                    metadataAccount = await MetadataAccount.GetAccount(rpcClient, AssociatedTokenAccountProgram.DeriveAssociatedTokenAccount(new PublicKey(Core.Runtime.SolanaVault.ActiveAccounts[Core.Runtime.SelectedAccount].Address), new PublicKey(account.TokenMint)));
                                    string offsiteTokenRetrieval = String.Empty;

                                    using (var httpClient = new HttpClient())
                                    {
                                        offsiteTokenRetrieval = await httpClient.GetStringAsync(new Uri(metadataAccount.metadata.uri));
                                    }
                                    MetaplexTokenStandard _Metadata = JsonConvert.DeserializeObject<MetaplexTokenStandard>(offsiteTokenRetrieval);
                                    metadataAccount.offchainData = _Metadata;

                                    BitmapImage tokenLogo = ResizedImage(_Metadata.default_image, 40, 40);
                                    ImageBrush tokenItembackground = new ImageBrush { ImageSource = tokenLogo, Stretch = Stretch.None };
                                    tokenItembackground.AlignmentX = AlignmentX.Left;
                                    tokenItembackground.Transform = Position;
                                    balanceTileBorder.Background = tokenItembackground;
                                }
                                catch (Exception error)
                                {
                                    Debug.WriteLine(error);
                                }

                            }
                            else
                            {
                                var rpcClient = ClientFactory.GetClient(Core.Runtime.WalletRPCprovider);
                                MetadataAccount metadataAccount = null;
                                try
                                {
                                    metadataAccount = await MetadataAccount.GetAccount(rpcClient, AssociatedTokenAccountProgram.DeriveAssociatedTokenAccount(new PublicKey(Core.Runtime.SolanaVault.ActiveAccounts[Core.Runtime.SelectedAccount].Address), new PublicKey(account.TokenMint)));
                                }
                                catch (Exception error)
                                {
                                    Debug.WriteLine(error);
                                }
                                if (metadataAccount != null)
                                {
                                    ListViewItem InventoryItem = new ListViewItem();
                                    Canvas IICanvas = new Canvas();
                                    TextBlock ItemTitle = new TextBlock();
                                    ItemTitle.Margin = new Thickness(40, 170, 0, 0);
                                    ItemTitle.HorizontalAlignment = HorizontalAlignment.Center;
                                    ItemTitle.HorizontalTextAlignment = TextAlignment.Center;
                                    if (metadataAccount.metadata != null)
                                    {
                                        ItemTitle.Text = metadataAccount.metadata.name;

                                        using (var httpClient = new HttpClient())
                                        {
                                            httpClient.DefaultRequestHeaders.Add("User-Agent", UserAgent);
                                            string offsiteTokenRetrieval = String.Empty;


                                            offsiteTokenRetrieval = await httpClient.GetStringAsync(new Uri(metadataAccount.metadata.uri));

                                            MetaplexTokenStandard _Metadata = JsonConvert.DeserializeObject<MetaplexTokenStandard>(offsiteTokenRetrieval);
                                            metadataAccount.offchainData = _Metadata;
                                            BitmapImage digitalAsset = new BitmapImage(new Uri(_Metadata.default_image));
                                            IICanvas.Background = new ImageBrush { ImageSource = digitalAsset, Stretch = Stretch.Fill };
                                        }
                                        IICanvas.Width = 195;
                                        IICanvas.Height = 195;
                                        InventoryItem.CornerRadius = new CornerRadius(15, 15, 15, 15);
                                        IICanvas.Margin = new Thickness(2, 2, 2, 2);
                                        InventoryItem.Padding = new Thickness(1, 1, 1, 1);
                                        InventoryItem.MinHeight = 200;
                                        InventoryItem.MinWidth = 198;
                                        IICanvas.Children.Add(ItemTitle);
                                        InventoryItem.Content = IICanvas;
                                        InventoryGrid.Items.Add(InventoryItem);
                                        Core.Runtime.Inventory.Add(metadataAccount);
                                    }
                                    continue;
                                }
                                else
                                {
                                    try
                                    {
                                        metadataAccount = await MetadataAccount.GetAccount(rpcClient, AssociatedTokenAccountProgram.DeriveAssociatedTokenAccount(new PublicKey(Core.Runtime.SolanaVault.ActiveAccounts[Core.Runtime.SelectedAccount].Address), new PublicKey(account.TokenMint)));
                                    }
                                    catch (Exception error)
                                    {
                                        Debug.WriteLine(error);
                                    }
                                    if (metadataAccount != null)
                                    {
                                        ListViewItem InventoryItem = new ListViewItem();
                                        Canvas IICanvas = new Canvas();
                                        TextBlock ItemTitle = new TextBlock();
                                        ItemTitle.Margin = new Thickness(40, 170, 0, 0);
                                        ItemTitle.HorizontalAlignment = HorizontalAlignment.Center;
                                        ItemTitle.HorizontalTextAlignment = TextAlignment.Center;
                                        if (metadataAccount.metadata != null)
                                        {
                                            ItemTitle.Text = metadataAccount.metadata.name;

                                            using (var httpClient = new HttpClient())
                                            {
                                                httpClient.DefaultRequestHeaders.Add("User-Agent", UserAgent);
                                                string offsiteTokenRetrieval = String.Empty;
                                                offsiteTokenRetrieval = await httpClient.GetStringAsync(new Uri(metadataAccount.metadata.uri));

                                                MetaplexTokenStandard _Metadata = JsonConvert.DeserializeObject<MetaplexTokenStandard>(offsiteTokenRetrieval);
                                                metadataAccount.offchainData = _Metadata;
                                                BitmapImage digitalAsset = new BitmapImage(new Uri(_Metadata.default_image));
                                                IICanvas.Background = new ImageBrush { ImageSource = digitalAsset, Stretch = Stretch.Fill };
                                            }
                                            IICanvas.Width = 195;
                                            IICanvas.Height = 195;
                                            InventoryItem.CornerRadius = new CornerRadius(15, 15, 15, 15);
                                            IICanvas.Margin = new Thickness(2, 2, 2, 2);
                                            InventoryItem.Padding = new Thickness(1, 1, 1, 1);
                                            InventoryItem.MinHeight = 200;
                                            InventoryItem.MinWidth = 198;
                                            IICanvas.Children.Add(ItemTitle);
                                            InventoryItem.Content = IICanvas;
                                            InventoryGrid.Items.Add(InventoryItem);
                                            Core.Runtime.Inventory.Add(metadataAccount);
                                        }
                                        continue;
                                    }
                                }
                            }

                            TextBlock currencyBalance = new TextBlock();
                            #region UI-CODE
                            balanceTile.Margin = ((ListViewItem)TokenAccountList.Items[0]).Margin;
                            balanceTile.BorderBrush = ((ListViewItem)TokenAccountList.Items[0]).BorderBrush;
                            balanceTile.BorderThickness = ((ListViewItem)TokenAccountList.Items[0]).BorderThickness;
                            balanceTile.Padding = ((ListViewItem)TokenAccountList.Items[0]).Padding;
                            balanceTile.Width = ((ListViewItem)TokenAccountList.Items[0]).Width;
                            balanceTile.Height = ((ListViewItem)TokenAccountList.Items[0]).Height;
                            balanceTile.Background = ((ListViewItem)TokenAccountList.Items[0]).Background;

                            currencyBalance.Height = ((TextBlock)((Border)((ListViewItem)TokenAccountList.Items[0]).Content).Child).Height;
                            currencyBalance.Margin = ((TextBlock)((Border)((ListViewItem)TokenAccountList.Items[0]).Content).Child).Margin;
                            currencyBalance.Padding = ((TextBlock)((Border)((ListViewItem)TokenAccountList.Items[0]).Content).Child).Padding;
                            currencyBalance.Foreground = ((TextBlock)((Border)((ListViewItem)TokenAccountList.Items[0]).Content).Child).Foreground;
                            currencyBalance.Text = account.QuantityDecimal.ToString() + Environment.NewLine + symbol;

                            balanceTileBorder.BorderBrush = ((Border)((ListViewItem)TokenAccountList.Items[0]).Content).BorderBrush;
                            balanceTileBorder.BorderThickness = ((Border)((ListViewItem)TokenAccountList.Items[0]).Content).BorderThickness;
                            balanceTileBorder.CornerRadius = ((Border)((ListViewItem)TokenAccountList.Items[0]).Content).CornerRadius;
                            balanceTileBorder.Width = ((Border)((ListViewItem)TokenAccountList.Items[0]).Content).Width;
                            balanceTileBorder.Height = ((Border)((ListViewItem)TokenAccountList.Items[0]).Content).Height;
                            balanceTileBorder.RenderTransform = ((Border)((ListViewItem)TokenAccountList.Items[0]).Content).RenderTransform;
                            balanceTileBorder.Child = currencyBalance;
                            #endregion UI-CODE
                            balanceTile.Content = balanceTileBorder;

                            TokenAccountList.Items.Add(balanceTile);
                        }

                    }
                    catch (Exception error)
                    {
                        Debug.WriteLine(error.Message); 
                    }
                }
                TokenAccountList.UpdateLayout();
                WalletBalance.Text = wallet_worth.ToString("C", CultureInfo.CreateSpecificCulture("en-US"));
                
            }
            catch(Exception error)
            {
                Debug.WriteLine(error.Message);
            }
        }
        public static BitmapImage ResizedImage(string URL, int maxWidth, int maxHeight)
        {
            BitmapImage sourceImage = new BitmapImage(new Uri(URL));
            sourceImage.DecodePixelWidth = maxWidth;
            sourceImage.DecodePixelHeight = maxHeight;

            return sourceImage;
        }
        private void TextBlock_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }
        private void App_SizeChanged(object sender, SizeChangedEventArgs e)
        {

            //ApplicationView.GetForCurrentView().TryResizeView(new Size(960, 540));
        }

        private void TextBlock_SelectionChanged_1(object sender, RoutedEventArgs e)
        {

        }

        private void Receive_Navigate_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Receive));
        }


        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            try
            {
                Core.Runtime.SelectedAccount = Core.Runtime.SolanaVault.ActiveAccounts.FindIndex(e => e.Address == ActiveAccountsSelector.SelectedValue.ToString());
                LoadWalletData();
            }
            catch (Exception error)
            {
                Debug.WriteLine(error.Message);
                WalletBalance.Text = "Something went wrong! Restart the app & check your internet connection!";
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

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

        private void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if (LinkStreamToggleSwitch.IsOn == true)
            {

                Core.Runtime.IsLinkEnabled = true;
                Core.Runtime.LinkNetwork.IsOnline = true;
            }
            else
            {
                Core.Runtime.CutLink();
            }
                
        }
    }
}

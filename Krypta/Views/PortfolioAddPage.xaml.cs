using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Threading.Tasks;
using Krypta.Models;
using Newtonsoft.Json;
using Xamarin.Essentials;

namespace Krypta.Views
{
    public partial class PortfolioAddPage
    {
        private ObservableCollection<CryptoItem> _coins = new ObservableCollection<CryptoItem>();
        private List<CryptoItem> _loadedCoins = new List<CryptoItem>();
        
        public PortfolioAddPage()
        {
            InitializeComponent();
            BindingContext = this;
        }

        protected override async void OnAppearing()
        {
            await FetchCoins();
            base.OnAppearing();
        }

        private async void OnSaveButton_Clicked(object sender, EventArgs e)
        {
            var amount = Entry.Text;
            var selectedCoin = (CryptoItem)Picker.SelectedItem;
            
            
            if (selectedCoin == null)
            {
                await DisplayAlert("No coin selected", "Please choose a coin from the list", "OK");
                return;
            }
            
            if (string.IsNullOrEmpty(amount))
            {
                await DisplayAlert("Invalid input", "Please enter an amount", "OK");
                return;
            }

            var parsed = double.TryParse(amount, out var amountDouble);

            if (!parsed || amountDouble <= 0)
            {
                await DisplayAlert("Invalid number", "Please enter a valid amount", "OK");
                return;
            }
            
            var portfolioCrypto = new PortfolioItem
            {
                Amount = amountDouble,
                LatestPrice = selectedCoin.CurrentPrice,
                Name = selectedCoin.Name,
                Symbol = selectedCoin.Symbol,
                CoinGeckoId = selectedCoin.Id,
                ImageUrl = selectedCoin.Image,
                TotalWorth = amountDouble * selectedCoin.CurrentPrice
            };
            await App.PortfolioDb.SaveCryptoAsync(portfolioCrypto);
            var historyRecord = new PortfolioHistoryRecord
            {
                Amount = amountDouble,
                PortfolioCryptoId = portfolioCrypto.Id,
                Date = DateTime.UtcNow,
            };
            await App.HistoryDb.SaveHistoryRecordAsync(historyRecord);
            await Navigation.PopAsync();
        }
        
        /// <summary>
        /// Fetches the coins from CoinGecko API and updates the UI picker with the new list.
        /// </summary>
        private async Task FetchCoins()
        {
            var currentNetworkAccess = Connectivity.NetworkAccess;
            if (currentNetworkAccess == NetworkAccess.Internet)
            {
                const string url = "https://api.coingecko.com/api/v3/coins/markets?vs_currency=eur";
                var client = new HttpClient();
                var response = await client.GetStringAsync(url);
                _loadedCoins = JsonConvert.DeserializeObject<List<CryptoItem>>(response);
                if (_loadedCoins != null) _coins = new ObservableCollection<CryptoItem>(_loadedCoins);
                Picker.ItemsSource = _coins;
            }
        }
    }
}
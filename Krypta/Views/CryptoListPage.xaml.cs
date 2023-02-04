using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Krypta.Models;
using Newtonsoft.Json;
using Xamarin.Forms;
using Xamarin.Essentials;

namespace Krypta.Views
{
    public partial class CryptoListPage
    {
        private bool _sortingAsc = true;
        private readonly HttpClient _client = new HttpClient();
        private ObservableCollection<CryptoItem> _coins = new ObservableCollection<CryptoItem>();
        private List<CryptoItem> _loadedCoins = new List<CryptoItem>();

        public CryptoListPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            await RefreshCoins();
            base.OnAppearing();
        }

        private async Task RefreshCoins()
        {
            var currentNetworkAccess = Connectivity.NetworkAccess;
            if (currentNetworkAccess == NetworkAccess.Internet)
            {
                const string url = "https://api.coingecko.com/api/v3/coins/markets?vs_currency=eur";
                var response = await _client.GetStringAsync(url);
                _loadedCoins = JsonConvert.DeserializeObject<List<CryptoItem>>(response);
                if (_loadedCoins != null) _coins = new ObservableCollection<CryptoItem>(_loadedCoins);
                CoinListView.ItemsSource = _coins;
            }
        }
        
        private List<CryptoItem> FilterCoins(string searchText)
        {
            var coins = _loadedCoins;
            var filteredCoins = coins.Where(coin => coin.Name.ToLower().Contains(searchText.ToLower())).ToList();
            return filteredCoins;
        }

        private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (e.NewTextValue == null)
            {
                _coins = new ObservableCollection<CryptoItem>(_loadedCoins);
                CoinListView.ItemsSource = _coins;
            }
            else
            {
                _coins = new ObservableCollection<CryptoItem>(FilterCoins(e.NewTextValue));
                CoinListView.ItemsSource = _coins;
            }
        }

        private void OnSortByNameButton_Clicked(object sender, EventArgs args)
        {
            if (_sortingAsc)
            {
                _coins = new ObservableCollection<CryptoItem>(_coins.OrderBy(coin => coin.Name));
                CoinListView.ItemsSource = _coins;
                _sortingAsc = false;
            }
            else
            {
                _coins = new ObservableCollection<CryptoItem>(_coins.OrderByDescending(coin => coin.Name));
                CoinListView.ItemsSource = _coins;
                _sortingAsc = true;
            }
        }

        private void OnSortByPriceButton_Clicked(object sender, EventArgs args)
        {
            if (_sortingAsc)
            {
                _coins = new ObservableCollection<CryptoItem>(_coins.OrderBy(coin => coin.CurrentPrice));
                CoinListView.ItemsSource = _coins;
                _sortingAsc = false;
            }
            else
            {
                _coins = new ObservableCollection<CryptoItem>(_coins.OrderByDescending(coin => coin.CurrentPrice));
                CoinListView.ItemsSource = _coins;
                _sortingAsc = true;
            }
        }

        private void OnSortByChangeButton_Clicked(object sender, EventArgs args)
        {
            if (_sortingAsc)
            {
                _coins = new ObservableCollection<CryptoItem>(_coins.OrderBy(coin => coin.CurrentPrice));
                CoinListView.ItemsSource = _coins;
                _sortingAsc = false;
            }
            else
            {
                _coins = new ObservableCollection<CryptoItem>(
                    _coins.OrderByDescending(coin => coin.PriceChangePercentage24H));
                CoinListView.ItemsSource = _coins;
                _sortingAsc = true;
            }
        }

        private async void OnCoin_ItemClicked(object sender, ItemTappedEventArgs e)
        {
            var data = e.Item as CryptoItem;
            await Navigation.PushAsync(new CryptoDetailPage(data));
        }
    }
}
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Krypta.Models;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Krypta.Views
{
    public partial class PortfolioDetailPage
    {
        private readonly PortfolioItem _passedItem;
        
        private ObservableCollection<PortfolioHistoryRecord> _historyRecordsObservable;
        
        public PortfolioDetailPage(PortfolioItem passedItem)
        {
            _passedItem = passedItem;
            InitializeComponent();
            Title = passedItem.Name;
            UpdateLabels();
        }

        protected override async void OnAppearing()
        {
            await UpdateCryptoPrice();
            await LoadHistoryRecords();
            base.OnAppearing();
            CryptoHistoryView.ItemsSource = _historyRecordsObservable;
        }

        private async void OnDeleteButton_Clicked(object sender, EventArgs e)
        {
            await App.PortfolioDb.DeletePortfolioAsync(_passedItem);
            await Navigation.PopAsync();
        }

        private async void OnBuyButton_Clicked(object sender, EventArgs e)
        {

            var result = await DisplayPromptAsync($"Buy more {_passedItem.Symbol.ToUpper()}", "Please write a number.", keyboard: Keyboard.Numeric, initialValue: "1");
            var parsed = double.TryParse(result, out var inputValue);
            if (!parsed)
            {
                await DisplayAlert("Invalid number", "Please enter a valid amount", "OK");
                return;
            }
            _passedItem.Amount += inputValue;
            _passedItem.TotalWorth = _passedItem.LatestPrice * _passedItem.Amount;
            
            var historyRecord = new PortfolioHistoryRecord
            {
                Amount = inputValue,
                PortfolioCryptoId = _passedItem.Id,
                Date = DateTime.UtcNow,
            };
            await App.HistoryDb.SaveHistoryRecordAsync(historyRecord);
            await App.PortfolioDb.SaveCryptoAsync(_passedItem);
            _historyRecordsObservable.Add(historyRecord);
            UpdateLabels();
        }

        private async void OnSellButton_Clicked(object sender, EventArgs e)
        {
            var result = await DisplayPromptAsync($"Sell {_passedItem.Symbol.ToUpper()}", "Please write a number.",
                keyboard: Keyboard.Numeric, initialValue: "1");
            var parsed = double.TryParse(result, out var inputValue);
            if (!parsed || inputValue > _passedItem.Amount)
            {
                await DisplayAlert("Invalid number", "Please enter a valid amount", "OK");
                return;
            }
            _passedItem.Amount -= inputValue;
            _passedItem.TotalWorth = _passedItem.LatestPrice * _passedItem.Amount;
            
            var historyRecord = new PortfolioHistoryRecord
            {
                Amount = -inputValue,
                PortfolioCryptoId = _passedItem.Id,
                Date = DateTime.UtcNow,
            };
            await App.HistoryDb.SaveHistoryRecordAsync(historyRecord);
            await App.PortfolioDb.SaveCryptoAsync(_passedItem);
            _historyRecordsObservable.Add(historyRecord);
            UpdateLabels();
        }
        
        private async void OnClearHistory_Clicked(object sender, EventArgs e)
        {
            await App.HistoryDb.DeleteAllHistoryRecordsForItemAsync(_passedItem.Id);
            _historyRecordsObservable.Clear();
        }
        
        private async Task UpdateCryptoPrice()
        {
            var currentNetworkAccess = Connectivity.NetworkAccess;
            if (currentNetworkAccess == NetworkAccess.Internet)
            {
                var url = "https://api.coingecko.com/api/v3/simple/price?ids=" + _passedItem.CoinGeckoId + "&vs_currencies=eur";
                var client = new HttpClient();
                var response = await client.GetStringAsync(url);
                var m = Regex.Match(response, @"[0-9]+(?:\.[0-9]+)?");
                var parsed = double.TryParse(m.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out var price);
                if (parsed)
                {
                    _passedItem.LatestPrice = price;
                    _passedItem.TotalWorth = _passedItem.LatestPrice * _passedItem.Amount;
                    UpdateLabels();
                }

                await App.PortfolioDb.SaveCryptoAsync(_passedItem);
            }
        }

        private void UpdateLabels()
        {
            PortfolioPrice.Text = _passedItem.TotalWorth.ToString("0.## €");
            CryptoAmount.Text = _passedItem.Amount.ToString(format:"0.##") + " " + _passedItem.Symbol.ToUpper();
        }
        
        private async Task LoadHistoryRecords()
        {
            var historyRecords = await App.HistoryDb.GetHistoryRecordsForPortfolioCryptoAsync(_passedItem.Id);
            _historyRecordsObservable = new ObservableCollection<PortfolioHistoryRecord>(historyRecords);
        }
    }
}
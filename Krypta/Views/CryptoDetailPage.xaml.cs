using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Krypta.Models;
using Microcharts;
using Newtonsoft.Json;
using SkiaSharp;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Krypta.Views
{
    public partial class CryptoDetailPage
    {
        private ObservableCollection<CryptoDatePrice> _chartEntries = new ObservableCollection<CryptoDatePrice>();
        private List<CryptoDatePrice> _fetchedEntries;
        private readonly CryptoItem _passedCryptoItem;

        public CryptoDetailPage(CryptoItem cryptoItem)
        {
            _passedCryptoItem = cryptoItem;
            InitializeComponent();
            CoinImage.Source = cryptoItem.Image;
            BuildNewChart();
            Title = cryptoItem.Name.ToUpper();
            MarketCapRank.Text = cryptoItem.MarketCapRank + ".";
            if (cryptoItem.MarketCap != null) MarketCap.Text = LargeNumberToReadableString((decimal)cryptoItem.MarketCap) + " €";
            if (cryptoItem.CirculatingSupply != null)
                CirculatingSupply.Text = LargeNumberToReadableString((decimal)cryptoItem.CirculatingSupply) + " €";
            MaxSupply.Text = cryptoItem.MaxSupply != null ? LargeNumberToReadableString((decimal)cryptoItem.MaxSupply) : "-";
            TotalSupply.Text = cryptoItem.TotalSupply != null ? LargeNumberToReadableString((decimal)cryptoItem.TotalSupply) : "-";
            AllTimeHigh.Text = cryptoItem.Ath != null ? cryptoItem.Ath.Value.ToString("0.##") + " €" : "-";
        }
        
        private async Task FetchChartData()
        {
            var currentNetworkAccess = Connectivity.NetworkAccess;
            if (currentNetworkAccess == NetworkAccess.Internet)
            {
                var url = "https://api.coingecko.com/api/v3/coins/" + _passedCryptoItem.Id +
                          "/market_chart?vs_currency=eur&days=30";
                var client = new HttpClient();
                var response = await client.GetStringAsync(url);
                _fetchedEntries = JsonConvert.DeserializeObject<CryptoPricesHistoryData>(response)?.GetChartData();
                if (_fetchedEntries != null) _chartEntries = new ObservableCollection<CryptoDatePrice>(_fetchedEntries);
            }
        }

        private async void BuildNewChart()
        {
            await FetchChartData();
            BuildChart();
        }

        private void BuildChart()
        {
            var minValue = _chartEntries.Min(x => x.Price);
            var maxValue = _chartEntries.Max(x => x.Price);

            var entries = new List<ChartEntry>();

            var divider = _chartEntries.Count / 5;
            for (var i = 0; i < _chartEntries.Count; i++)
            {
                if (i % divider == 0 && i != 0 && i < _chartEntries.Count - 5)
                    entries.Add(new ChartEntry((float)_chartEntries[i].Price)
                    {
                        Label = _chartEntries[i].Date.ToShortDateString(),
                        ValueLabel = _chartEntries[i].Price.ToString("0.##") + " €",
                        Color = SKColor.Parse("#e85d04"),
                        ValueLabelColor = SKColor.Parse("#e85d04")
                    });
                else
                    entries.Add(new ChartEntry((float)_chartEntries[i].Price)
                    {
                        Color = SKColor.Parse("#e85d04")
                    });
            }

            Chart1.Chart = new LineChart
            {
                Entries = entries, LineMode = LineMode.Straight, LineSize = 8,
                PointMode = PointMode.Circle, PointSize = 10,
                MaxValue = (float)maxValue, MinValue = (float)minValue,
                LabelTextSize = 35,
                BackgroundColor = SKColor.Parse("#14213d"),
                LabelColor = SKColor.Parse("#e85d04")
            };
        }

        private void OnChartFilterButton_Clicked(object sender, EventArgs e)
        {
            var button = (Button)sender;
            var buttonText = button.Text;
            switch (buttonText)
            {
                case "30d":
                    _chartEntries = new ObservableCollection<CryptoDatePrice>(_fetchedEntries);
                    BuildChart();
                    break;
                case "14d":
                    _chartEntries = new ObservableCollection<CryptoDatePrice>(_fetchedEntries.Where( x=> x.Date >= DateTime.Now.AddDays(-14)));
                    BuildChart();
                    break;
                case "7d":
                    _chartEntries = new ObservableCollection<CryptoDatePrice>(_fetchedEntries.Where(x => x.Date >= DateTime.Now.AddDays(-7)));
                    BuildChart();
                    break;
                case "1d":
                    _chartEntries = new ObservableCollection<CryptoDatePrice>(_fetchedEntries.Where(x => x.Date >= DateTime.Now.AddDays(-1)));
                    BuildChart();
                    break;
            }
            
            BuildChart();
        }

        private static string LargeNumberToReadableString(decimal num)
        {
            if (num > 999999999 || num < -999999999)
                return num.ToString("0,,,.###B", CultureInfo.InvariantCulture);
            if (num > 999999 || num < -999999)
                return num.ToString("0,,.##M", CultureInfo.InvariantCulture);
            if (num > 999 || num < -999)
                return num.ToString("0,.#K", CultureInfo.InvariantCulture);
            return num.ToString(CultureInfo.InvariantCulture);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Krypta.Models;
using Xamarin.Forms;

namespace Krypta.Views
{
    public partial class PortfolioListPage
    {
        private List<PortfolioItem> _portfolioCryptos;

        public PortfolioListPage()
        {
            InitializeComponent();
            
        }

        protected override async void OnAppearing()
        {
            await LoadPortfolioCoinsFromDatabase();
            base.OnAppearing();
            UserCoinsView.ItemsSource = _portfolioCryptos;
            await UpdatePortfolioLabels();
        }

        private async void OnAddButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new PortfolioAddPage());
        }
        
        private async void Portfolio_ItemClicked(object sender, ItemTappedEventArgs e)
        {
            var data = e.Item as PortfolioItem;
            await Navigation.PushAsync(new PortfolioDetailPage(data));
        }

        private async Task LoadPortfolioCoinsFromDatabase()
        {
            _portfolioCryptos = await App.PortfolioDb.GetPortfolioAsync();
        }

        private async Task UpdatePortfolioLabels()
        {
            PortfolioPrice.Text = (await App.PortfolioDb.SumOfAllPortfolio()).ToString("0.## €");
            var numberOfCoins = await App.PortfolioDb.CountOfAllPortfolio();
            NumberOfCoins.Text = "in " + numberOfCoins + $" {(numberOfCoins == 1 ? "coin" : "coins")}";
        }
    }
}
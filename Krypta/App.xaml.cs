using System;
using System.IO;
using Krypta.Models.database;
using Krypta.Views;
using Xamarin.Forms;

namespace Krypta
{
    public partial class App
    {
        private static PortfolioDatabase _portfolioDatabase;
        
        private static HistoryDatabase _historyDatabase;

        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new MainPage())
            {
                BarBackgroundColor = Color.FromHex("#042940"),
                BarTextColor = Color.FromHex("#e85d04")
            };
        }
        
        public static PortfolioDatabase PortfolioDb =>
            _portfolioDatabase ?? (_portfolioDatabase = new PortfolioDatabase(Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "PortfolioCryptos.db3")));
        
        public static HistoryDatabase HistoryDb =>
            _historyDatabase ?? (_historyDatabase = new HistoryDatabase(Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "HistoryRecords.db3")));
        
        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
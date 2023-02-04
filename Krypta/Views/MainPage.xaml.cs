using System;

namespace Krypta.Views
{
    public partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();
            Title = CurrentPage.Title;
            CurrentPageChanged += CurrentPageHasChanged;
        }

        private void CurrentPageHasChanged(object sender, EventArgs e)
        {
            Title = CurrentPage.Title;
        }
    }
}
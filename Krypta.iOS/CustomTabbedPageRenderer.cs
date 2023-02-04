using Krypta.iOS;
using Krypta.Views;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(MainPage), typeof(CustomTabbedPageRenderer))]
namespace Krypta.iOS
{
    public class CustomTabbedPageRenderer : TabbedRenderer
    {
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            this.TabBar.TintColor = UIColor.FromRGB(232,93,4);
            this.TabBar.BarTintColor = UIColor.White;
            this.TabBar.UnselectedItemTintColor = UIColor.FromRGB(0, 122, 255);
            
            if (TabBar?.Items == null) return;

            //Setting the Icons
            TabBar.Items[0].Image = GetTabIcon(UITabBarSystemItem.Favorites);
            TabBar.Items[1].Image = GetTabIcon(UITabBarSystemItem.Search);
        }
        
        private static UIImage GetTabIcon(UITabBarSystemItem systemItem)
        {
            //Convert UITabBarItem to UIImage
            var item = new UITabBarItem(systemItem, 0);

            return UIImage.FromImage(item.SelectedImage.CGImage, item.SelectedImage.CurrentScale, item.SelectedImage.Orientation);
        }
    }
}
using System.Windows;
using TacoBell.ViewModels;

namespace TacoBell.Views
{
    public partial class CartWindow : Window
    {
        public CartWindow()
        {
            InitializeComponent();
            DataContext = new CartViewModel();
        }
    }
}

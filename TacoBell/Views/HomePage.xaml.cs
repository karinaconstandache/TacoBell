using System.Windows;
using TacoBell.Services;
using TacoBell.ViewModels;

namespace TacoBell.Views
{
    public partial class HomePage : Window
    {
        public HomePage()
        {
            InitializeComponent();
            DataContext = new HomePageVM(new NavigationService());
        }
    }
}

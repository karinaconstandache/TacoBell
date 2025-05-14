using System.Windows;
using TacoBell.Services;
using TacoBell.ViewModels;

namespace TacoBell.Views
{
    public partial class MenuPage : Window
    {
        public MenuPage()
        {
            InitializeComponent();
            DataContext = new MenuPageVM(new NavigationService(), new UserSessionService());
        }
    }
}

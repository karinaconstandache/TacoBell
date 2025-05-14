using System.Windows;
using TacoBell.Services;
using TacoBell.ViewModels;

namespace TacoBell.Views
{
    public partial class AccountPage : Window
    {
        public AccountPage()
        {
            InitializeComponent();
            DataContext = new MenuPageVM(new NavigationService(), new UserSessionService());
        }
    }
}

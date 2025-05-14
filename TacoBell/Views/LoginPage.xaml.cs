using System.Windows;
using TacoBell.Services;
using TacoBell.ViewModels;

namespace TacoBell.Views
{
    public partial class LoginPage : Window
    {
        public LoginPage()
        {
            InitializeComponent();
            DataContext = new MenuPageVM(new NavigationService(), new UserSessionService());
        }
    }
}

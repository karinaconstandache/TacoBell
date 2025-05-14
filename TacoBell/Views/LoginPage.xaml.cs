using System.Windows;
using System.Windows.Controls;
using TacoBell.Services;
using TacoBell.ViewModels;

namespace TacoBell.Views
{
    public partial class LoginPage : Window
    {
        private readonly LoginPageVM _viewModel;

        public LoginPage()
        {
            InitializeComponent();
            _viewModel = new LoginPageVM(new NavigationService());
            DataContext = _viewModel;
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox pb)
                _viewModel.Password = pb.Password;
        }
    }
}

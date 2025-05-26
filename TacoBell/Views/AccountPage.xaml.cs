using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TacoBell.Services;
using TacoBell.ViewModels;

namespace TacoBell.Views
{
    public partial class AccountPage : Window
    {
        private AccountPageVM _viewModel;

        public AccountPage()
        {
            InitializeComponent();
            _viewModel = new AccountPageVM(new NavigationService());
            DataContext = _viewModel;
        }

        private void CloseOrderDetails(object sender, MouseButtonEventArgs e)
        {
            // Close modal when clicking on background
            if (sender is Border)
            {
                _viewModel.IsOrderDetailsVisible = false;
            }
        }

        private void CloseOrderDetails(object sender, RoutedEventArgs e)
        {
            // Close modal when clicking close button
            _viewModel.IsOrderDetailsVisible = false;
        }
    }
}
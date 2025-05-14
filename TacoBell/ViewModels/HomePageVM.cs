using System.Windows.Input;
using TacoBell.Helpers;
using TacoBell.Services;

namespace TacoBell.ViewModels
{
    public class HomePageVM : BaseViewModel
    {
        private readonly NavigationService _navigationService;

        public HomePageVM(NavigationService navigationService)
        {
            _navigationService = navigationService;

            NavigateToLoginCommand = new RelayCommand(_ => _navigationService.NavigateTo("LoginPage"));
            OpenAppMenuCommand = new RelayCommand(_ => IsAppMenuVisible = !IsAppMenuVisible);
            NavigateToHomeCommand = new RelayCommand(_ => _navigationService.NavigateTo("HomePage"));
            NavigateToMenuCommand = new RelayCommand(_ => _navigationService.NavigateTo("MenuPage"));
            NavigateToAccountCommand = new RelayCommand(_ => NavigateToAccount());
            LogoutCommand = new RelayCommand(_ => Logout());
        }

        private bool _isAppMenuVisible;
        public bool IsAppMenuVisible
        {
            get => _isAppMenuVisible;
            set { _isAppMenuVisible = value; OnPropertyChanged(); }
        }

        public string CurrentUserName => UserSessionService.CurrentUser?.FirstName ?? "";

        public bool IsUserLoggedIn => UserSessionService.IsUserLoggedIn;

        public ICommand NavigateToLoginCommand { get; }
        public ICommand OpenAppMenuCommand { get; }
        public ICommand NavigateToHomeCommand { get; }
        public ICommand NavigateToMenuCommand { get; }
        public ICommand NavigateToAccountCommand { get; }
        public ICommand LogoutCommand { get; }

        private void NavigateToAccount()
        {
            if (UserSessionService.IsUserLoggedIn)
                _navigationService.NavigateTo("AccountPage");
            else
                _navigationService.NavigateTo("LoginPage");
        }

        private void Logout()
        {
            UserSessionService.Logout();
            _navigationService.NavigateTo("LoginPage");
            OnPropertyChanged(nameof(CurrentUserName));
            OnPropertyChanged(nameof(IsUserLoggedIn));
        }
    }
}

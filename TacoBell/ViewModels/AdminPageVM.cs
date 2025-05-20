using System.Windows.Input;
using TacoBell.Helpers;
using TacoBell.Services;
using TacoBell.Models.Enums;

namespace TacoBell.ViewModels
{
    public class AdminPageVM : BaseViewModel
    {
        private readonly NavigationService _navigationService;

        public AdminPageVM(NavigationService navigationService)
        {
            _navigationService = navigationService;

            OpenAppMenuCommand = new RelayCommand(_ => IsAppMenuVisible = !IsAppMenuVisible);
            LogoutCommand = new RelayCommand(_ => Logout());
            ShowManageMenuCommand = new RelayCommand(_ => ToggleSection(true));
            ShowManageOrdersCommand = new RelayCommand(_ => ToggleSection(false));
        }

        public string CurrentUserName => UserSessionService.CurrentUser?.FirstName ?? "";
        public ICommand OpenAppMenuCommand { get; }
        public ICommand LogoutCommand { get; }
        public ICommand ShowManageMenuCommand { get; }
        public ICommand ShowManageOrdersCommand { get; }

        private bool _isAppMenuVisible;
        public bool IsAppMenuVisible
        {
            get => _isAppMenuVisible;
            set { _isAppMenuVisible = value; OnPropertyChanged(); }
        }

        private bool _isManageMenuVisible = true;
        public bool IsManageMenuVisible
        {
            get => _isManageMenuVisible;
            set { _isManageMenuVisible = value; OnPropertyChanged(); }
        }

        private bool _isManageOrdersVisible;
        public bool IsManageOrdersVisible
        {
            get => _isManageOrdersVisible;
            set { _isManageOrdersVisible = value; OnPropertyChanged(); }
        }

        private void ToggleSection(bool showMenu)
        {
            IsManageMenuVisible = showMenu;
            IsManageOrdersVisible = !showMenu;
        }

        private void Logout()
        {
            UserSessionService.Logout();
            _navigationService.NavigateTo("LoginPage");
        }
    }
}

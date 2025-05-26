using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TacoBell.Helpers;
using TacoBell.Models.BusinessLogicLayer;
using TacoBell.Models.DTOs;
using TacoBell.Models.Entities;
using TacoBell.Services;

namespace TacoBell.ViewModels
{
    public class AccountPageVM : BaseViewModel
    {
        private readonly NavigationService _navigationService;
        private readonly AccountBLL _accountBLL = new();

        public AccountPageVM(NavigationService navigationService)
        {
            _navigationService = navigationService;

            NavigateToLoginCommand = new RelayCommand(_ => _navigationService.NavigateTo("LoginPage"));
            OpenAppMenuCommand = new RelayCommand(_ => IsAppMenuVisible = !IsAppMenuVisible);
            NavigateToHomeCommand = new RelayCommand(_ => _navigationService.NavigateTo("HomePage"));
            NavigateToMenuCommand = new RelayCommand(_ => _navigationService.NavigateTo("MenuPage"));
            NavigateToAccountCommand = new RelayCommand(_ => NavigateToAccount());
            LogoutCommand = new RelayCommand(_ => Logout());

            ShowPersonalInfoCommand = new RelayCommand(_ => CurrentSection = AccountSection.PersonalInfo);
            ShowActiveOrdersCommand = new RelayCommand(_ => { CurrentSection = AccountSection.ActiveOrders; LoadActiveOrders(); });
            ShowOrderHistoryCommand = new RelayCommand(_ => { CurrentSection = AccountSection.OrderHistory; LoadOrderHistory(); });

            UpdatePersonalInfoCommand = new RelayCommand(_ => UpdatePersonalInfo());
            ViewOrderDetailsCommand = new RelayCommand(async order => await ViewOrderDetails(order));
            CancelOrderCommand = new RelayCommand(async order => await CancelOrder(order));

            if (UserSessionService.IsUserLoggedIn)
            {
                LoadUserDetails();
                CurrentSection = AccountSection.PersonalInfo;
            }
            else
            {
                // If not logged in, redirect to login page
                _navigationService.NavigateTo("LoginPage");
            }
        }

        #region Properties

        private bool _isAppMenuVisible;
        public bool IsAppMenuVisible
        {
            get => _isAppMenuVisible;
            set { _isAppMenuVisible = value; OnPropertyChanged(); }
        }

        private AccountSection _currentSection = AccountSection.PersonalInfo;
        public AccountSection CurrentSection
        {
            get => _currentSection;
            set
            {
                _currentSection = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsPersonalInfoVisible));
                OnPropertyChanged(nameof(IsActiveOrdersVisible));
                OnPropertyChanged(nameof(IsOrderHistoryVisible));
            }
        }

        public bool IsPersonalInfoVisible => CurrentSection == AccountSection.PersonalInfo;
        public bool IsActiveOrdersVisible => CurrentSection == AccountSection.ActiveOrders;
        public bool IsOrderHistoryVisible => CurrentSection == AccountSection.OrderHistory;

        // User details for editing
        private string _firstName;
        public string FirstName
        {
            get => _firstName;
            set { _firstName = value; OnPropertyChanged(); }
        }

        private string _lastName;
        public string LastName
        {
            get => _lastName;
            set { _lastName = value; OnPropertyChanged(); }
        }

        private string _phoneNumber;
        public string PhoneNumber
        {
            get => _phoneNumber;
            set { _phoneNumber = value; OnPropertyChanged(); }
        }

        private string _deliveryAddress;
        public string DeliveryAddress
        {
            get => _deliveryAddress;
            set { _deliveryAddress = value; OnPropertyChanged(); }
        }

        private string _email;
        public string Email
        {
            get => _email;
            set { _email = value; OnPropertyChanged(); }
        }

        // Orders collections
        public ObservableCollection<OrderDisplayDTO> ActiveOrders { get; set; } = new();
        public ObservableCollection<OrderDisplayDTO> OrderHistory { get; set; } = new();

        private OrderDisplayDTO _selectedOrder;
        public OrderDisplayDTO SelectedOrder
        {
            get => _selectedOrder;
            set { _selectedOrder = value; OnPropertyChanged(); }
        }

        private bool _isOrderDetailsVisible;
        public bool IsOrderDetailsVisible
        {
            get => _isOrderDetailsVisible;
            set { _isOrderDetailsVisible = value; OnPropertyChanged(); }
        }

        public string CurrentUserName => UserSessionService.CurrentUser?.FirstName ?? "";
        public bool IsUserLoggedIn => UserSessionService.IsUserLoggedIn;

        #endregion

        #region Commands

        public ICommand NavigateToLoginCommand { get; }
        public ICommand OpenAppMenuCommand { get; }
        public ICommand NavigateToHomeCommand { get; }
        public ICommand NavigateToMenuCommand { get; }
        public ICommand NavigateToAccountCommand { get; }
        public ICommand LogoutCommand { get; }

        public ICommand ShowPersonalInfoCommand { get; }
        public ICommand ShowActiveOrdersCommand { get; }
        public ICommand ShowOrderHistoryCommand { get; }

        public ICommand UpdatePersonalInfoCommand { get; }
        public ICommand ViewOrderDetailsCommand { get; }
        public ICommand CancelOrderCommand { get; }

        #endregion

        #region Methods

        private void LoadUserDetails()
        {
            if (UserSessionService.CurrentUser != null)
            {
                var user = _accountBLL.GetUserDetails(UserSessionService.CurrentUser.UserId);
                if (user != null)
                {
                    FirstName = user.FirstName;
                    LastName = user.LastName;
                    PhoneNumber = user.PhoneNumber;
                    DeliveryAddress = user.DeliveryAddress;
                    Email = user.Email;
                }
            }
        }

        private async void LoadActiveOrders()
        {
            if (UserSessionService.CurrentUser != null)
            {
                try
                {
                    var orders = await _accountBLL.GetUserActiveOrdersAsync(UserSessionService.CurrentUser.UserId);
                    ActiveOrders.Clear();
                    foreach (var order in orders)
                    {
                        ActiveOrders.Add(order);
                    }
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show($"Eroare la încărcarea comenzilor active: {ex.Message}");
                }
            }
        }

        private async void LoadOrderHistory()
        {
            if (UserSessionService.CurrentUser != null)
            {
                try
                {
                    var orders = await _accountBLL.GetUserOrderHistoryAsync(UserSessionService.CurrentUser.UserId);
                    OrderHistory.Clear();
                    foreach (var order in orders)
                    {
                        OrderHistory.Add(order);
                    }
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show($"Eroare la încărcarea istoricului comenzilor: {ex.Message}");
                }
            }
        }

        private async Task ViewOrderDetails(object orderObj)
        {
            if (orderObj is OrderDisplayDTO order)
            {
                try
                {
                    var orderItems = await _accountBLL.GetOrderDetailsAsync(order.OrderId);
                    order.OrderItems.Clear();
                    foreach (var item in orderItems)
                    {
                        order.OrderItems.Add(item);
                    }

                    SelectedOrder = order;
                    IsOrderDetailsVisible = true;
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show($"Eroare la încărcarea detaliilor comenzii: {ex.Message}");
                }
            }
        }

        private async Task CancelOrder(object orderObj)
        {
            if (orderObj is OrderDisplayDTO order && UserSessionService.CurrentUser != null)
            {
                var result = MessageBox.Show(
                    $"Sunteți sigur că doriți să anulați comanda {order.OrderCode}?",
                    "Confirmare anulare",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        bool success = await _accountBLL.CancelOrderAsync(order.OrderId, UserSessionService.CurrentUser.UserId);
                        if (success)
                        {
                            MessageBox.Show("Comanda a fost anulată cu succes!");

                            // Refresh both lists as the order might appear in history now
                            LoadActiveOrders();
                            if (CurrentSection == AccountSection.OrderHistory)
                            {
                                LoadOrderHistory();
                            }
                        }
                        else
                        {
                            MessageBox.Show("Nu s-a putut anula comanda. Verificați dacă comanda poate fi anulată.");
                        }
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show($"Eroare la anularea comenzii: {ex.Message}");
                    }
                }
            }
        }

        private void UpdatePersonalInfo()
        {
            if (UserSessionService.CurrentUser != null &&
                !string.IsNullOrWhiteSpace(FirstName) &&
                !string.IsNullOrWhiteSpace(LastName) &&
                !string.IsNullOrWhiteSpace(PhoneNumber) &&
                !string.IsNullOrWhiteSpace(DeliveryAddress))
            {
                try
                {
                    var updatedUser = new User
                    {
                        UserId = UserSessionService.CurrentUser.UserId,
                        FirstName = FirstName,
                        LastName = LastName,
                        PhoneNumber = PhoneNumber,
                        DeliveryAddress = DeliveryAddress
                    };

                    _accountBLL.UpdateUserDetails(updatedUser);

                    // Update the session user
                    UserSessionService.CurrentUser.FirstName = FirstName;
                    UserSessionService.CurrentUser.LastName = LastName;
                    UserSessionService.CurrentUser.PhoneNumber = PhoneNumber;
                    UserSessionService.CurrentUser.DeliveryAddress = DeliveryAddress;

                    OnPropertyChanged(nameof(CurrentUserName));
                    MessageBox.Show("Informațiile personale au fost actualizate cu succes!");
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show($"Eroare la actualizarea informațiilor: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Toate câmpurile sunt obligatorii!");
            }
        }

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

        #endregion
    }

    public enum AccountSection
    {
        PersonalInfo,
        ActiveOrders,
        OrderHistory
    }
}
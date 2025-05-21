using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using TacoBell.BusinessLogicLayer;
using TacoBell.Helpers;
using TacoBell.Models.DTOs;
using TacoBell.Services;

namespace TacoBell.ViewModels
{
    public class CartViewModel : INotifyPropertyChanged
    {
        private readonly OrderBLL _orderBLL;

        public ObservableCollection<ProductDisplayDTO> Products => CartService.Instance.Products;
        public ObservableCollection<MenuDisplayDTO> Menus => CartService.Instance.Menus;

        public ICommand RemoveProductCommand { get; }
        public ICommand RemoveMenuCommand { get; }
        public ICommand PlaceOrderCommand { get; }

        public string TotalText => $"Total: {CalculateTotal():0.##} lei";

        public CartViewModel()
        {
            _orderBLL = new OrderBLL(Config.ConnectionString);

            RemoveProductCommand = new RelayCommand(RemoveProduct);
            RemoveMenuCommand = new RelayCommand(RemoveMenu);
            PlaceOrderCommand = new RelayCommand(_ => PlaceOrder());

            // Asigură reactivitate UI la modificări
            Products.CollectionChanged += (_, _) => OnPropertyChanged(nameof(TotalText));
            Menus.CollectionChanged += (_, _) => OnPropertyChanged(nameof(TotalText));
        }

        private decimal CalculateTotal()
        {
            return Products.Sum(p => p.Price) + Menus.Sum(m => m.Price);
        }

        public void AddProduct(ProductDisplayDTO product)
        {
            CartService.Instance.AddProduct(product);
            OnPropertyChanged(nameof(TotalText));
        }

        public void AddMenu(MenuDisplayDTO menu)
        {
            CartService.Instance.AddMenu(menu);
            OnPropertyChanged(nameof(TotalText));
        }


        private void RemoveProduct(object obj)
        {
            if (obj is ProductDisplayDTO p)
                CartService.Instance.Products.Remove(p);
        }

        private void RemoveMenu(object obj)
        {
            if (obj is MenuDisplayDTO m)
                CartService.Instance.Menus.Remove(m);
        }

        private void PlaceOrder()
        {
            if (!UserSessionService.IsUserLoggedIn)
            {
                MessageBox.Show("Trebuie să fii logat pentru a plasa o comandă.", "Eroare", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (CartService.Instance.IsEmpty)
            {
                MessageBox.Show("Coșul este gol.", "Avertisment", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                int userId = UserSessionService.CurrentUser.UserId;
                decimal shippingFee = 10;
                bool discountApplied = false;

                int orderId = _orderBLL.CreateOrder(userId, shippingFee, discountApplied);

                // TODO: Pasul următor — salvarea produselor și meniurilor (alta procedură stocată)

                MessageBox.Show("Comanda a fost plasată cu succes!", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
                CartService.Instance.Clear();
                OnPropertyChanged(nameof(TotalText));
            }
            catch
            {
                MessageBox.Show("A apărut o eroare la plasarea comenzii.", "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}

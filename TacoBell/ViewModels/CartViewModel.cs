using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using TacoBell.Helpers;
using TacoBell.Models.DTOs;
using TacoBell.Services;

namespace TacoBell.ViewModels
{
    public class CartViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<ProductDisplayDTO> Products => CartService.Instance.Products;
        public ObservableCollection<MenuDisplayDTO> Menus => CartService.Instance.Menus;

        public ICommand RemoveProductCommand { get; }
        public ICommand RemoveMenuCommand { get; }
        public ICommand PlaceOrderCommand { get; }

        public string TotalText => $"Total: {CalculateTotal():0.##} lei";

        public CartViewModel()
        {
            RemoveProductCommand = new RelayCommand(RemoveProduct);
            RemoveMenuCommand = new RelayCommand(RemoveMenu);
            PlaceOrderCommand = new RelayCommand(_ => PlaceOrder());
        }

        private decimal CalculateTotal()
        {
            return Products.Sum(p => p.Price) + Menus.Sum(m => m.Price);
        }

        private void RemoveProduct(object obj)
        {
            if (obj is ProductDisplayDTO p)
                CartService.Instance.Products.Remove(p);
            OnPropertyChanged(nameof(TotalText));
        }

        private void RemoveMenu(object obj)
        {
            if (obj is MenuDisplayDTO m)
                CartService.Instance.Menus.Remove(m);
            OnPropertyChanged(nameof(TotalText));
        }

        private void PlaceOrder()
        {
            MessageBox.Show("Comanda a fost plasată (simulare pentru moment).");
            CartService.Instance.Clear();
            OnPropertyChanged(nameof(TotalText));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}

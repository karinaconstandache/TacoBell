using System.Collections.ObjectModel;
using TacoBell.Models.DTOs;

namespace TacoBell.Services
{
    public class CartService
    {
        // Singleton
        private static CartService _instance;
        public static CartService Instance => _instance ??= new CartService();

        public ObservableCollection<ProductDisplayDTO> Products { get; private set; } = new();
        public ObservableCollection<MenuDisplayDTO> Menus { get; private set; } = new();

        public void AddProduct(ProductDisplayDTO product)
        {
            Products.Add(product);
        }

        public void AddMenu(MenuDisplayDTO menu)
        {
            Menus.Add(menu);
        }

        public void Clear()
        {
            Products.Clear();
            Menus.Clear();
        }

        public bool IsEmpty => Products.Count == 0 && Menus.Count == 0;
    }
}

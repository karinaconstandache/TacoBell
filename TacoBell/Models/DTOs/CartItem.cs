using System.ComponentModel;

namespace TacoBell.Models.DTOs
{
    public class CartItem : INotifyPropertyChanged
    {
        private int _quantity;

        public string Name { get; set; }
        public int? DishId { get; set; }
        public int? MenuId { get; set; }
        public decimal Price { get; set; }

        public int Quantity
        {
            get => _quantity;
            set
            {
                if (_quantity != value)
                {
                    _quantity = value;
                    OnPropertyChanged(nameof(Quantity));
                    OnPropertyChanged(nameof(TotalPrice));
                }
            }
        }

        public decimal TotalPrice => Price * Quantity;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

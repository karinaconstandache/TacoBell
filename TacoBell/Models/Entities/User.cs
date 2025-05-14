using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TacoBell.Models.Enums;

namespace TacoBell.Models.Entities
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string DeliveryAddress { get; set; }
        public string Password { get; set; }
        public UserRole Role { get; set; } = UserRole.USER;
        public ICollection<Order> Orders { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TacoBell.Models.Entities
{
    public class OrderMenu
    {
        public int OrderId { get; set; }
        public Order Order { get; set; }

        public int MenuId { get; set; }
        public Menu Menu { get; set; }

        public int Quantity { get; set; }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TacoBell.Models.Entities
{
    public class MenuDish
    {
        public int MenuId { get; set; }
        public Menu Menu { get; set; }

        public int DishId { get; set; }
        public Dish Dish { get; set; }

        public decimal DishQuantityInMenu { get; set; }
    }
}

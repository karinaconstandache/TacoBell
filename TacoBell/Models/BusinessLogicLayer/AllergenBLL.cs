using System;
using System.Collections.Generic;
using System.Linq;
using TacoBell.Models.Entities;

namespace TacoBell.Models.BusinessLogicLayer
{
    public class AllergenBLL
    {
        private readonly TacoBellDbContext _db = new();

        public List<Allergen> GetAllAllergens()
        {
            return _db.Allergens.ToList();
        }

        public void AddAllergen(Allergen allergen)
        {
            _db.Allergens.Add(allergen);
            _db.SaveChanges();
        }

        public void UpdateAllergen(Allergen allergen)
        {
            var existing = _db.Allergens.Find(allergen.AllergenId);
            if (existing != null)
            {
                existing.Name = allergen.Name;
                _db.SaveChanges();
            }
        }

        public void DeleteAllergen(int allergenId)
        {
            var allergen = _db.Allergens.Find(allergenId);
            if (allergen != null)
            {
                bool isUsed = _db.DishAllergens.Any(da => da.AllergenId == allergenId);
                if (isUsed)
                    throw new InvalidOperationException("Acest alergen este utilizat într-un preparat și nu poate fi șters.");

                _db.Allergens.Remove(allergen);
                _db.SaveChanges();
            }
        }



    }
}

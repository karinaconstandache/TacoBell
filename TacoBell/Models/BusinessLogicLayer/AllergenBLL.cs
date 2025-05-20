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

        public void DeleteAllergen(int id)
        {
            var allergen = _db.Allergens.Find(id);
            if (allergen != null)
            {
                _db.Allergens.Remove(allergen);
                _db.SaveChanges();
            }
        }
    }
}

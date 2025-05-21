using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TacoBell.Models;
using TacoBell.Models.Entities;

namespace TacoBell.Services
{
    public class CategoryService
    {
        public async Task<List<Category>> GetAllAsync()
        {
            using var db = new TacoBellDbContext();
            return await db.Categories.OrderBy(c => c.Name).ToListAsync();
        }
    }
}

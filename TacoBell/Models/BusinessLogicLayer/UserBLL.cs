using System.Linq;
using TacoBell.Models.Entities;
using TacoBell.Models.Enums;

namespace TacoBell.Models.BusinessLogicLayer
{
    public class UserBLL
    {
        private readonly TacoBellDbContext _db;

        public UserBLL()
        {
            _db = new TacoBellDbContext();
        }

        public User Login(string email, string password)
        {
            return _db.Users.FirstOrDefault(u => u.Email == email && u.Password == password);
        }

        public User Register(User newUser)
        {
            _db.Users.Add(newUser);
            _db.SaveChanges();
            return newUser;
        }

        public bool IsEmailTaken(string email)
        {
            return _db.Users.Any(u => u.Email == email);
        }
    }
}

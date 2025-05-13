using TacoBell.Models.Entities;

namespace TacoBell.Services
{
    public class UserSessionService
    {
        public bool IsUserLoggedIn => CurrentUser != null;
        public User CurrentUser { get; set; }
    }
}


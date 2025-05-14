using TacoBell.Models.Entities;

namespace TacoBell.Services
{
    public class UserSessionService
    {
        public User CurrentUser { get; private set; }

        public bool IsUserLoggedIn => CurrentUser != null;

        public void SetUser(User user)
        {
            CurrentUser = user;
        }

        public void Logout()
        {
            CurrentUser = null;
        }
    }
}

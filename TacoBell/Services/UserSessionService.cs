using TacoBell.Models.Entities;

namespace TacoBell.Services
{
    public static class UserSessionService
    {
        public static User? CurrentUser { get; private set; }

        public static bool IsUserLoggedIn => CurrentUser != null;

        public static void SetUser(User user)
        {
            CurrentUser = user;
        }

        public static void Logout()
        {
            CurrentUser = null;
        }
    }
}

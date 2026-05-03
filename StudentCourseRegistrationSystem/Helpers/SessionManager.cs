using StudentCourseRegistrationSystem.Models;

namespace StudentCourseRegistrationSystem.Helpers
{
    public static class SessionManager
    {
        public static User CurrentUser { get; private set; }
        public static bool IsLoggedIn => CurrentUser != null;

        public static void Login(User user)
        {
            CurrentUser = user;
        }

        public static void Logout()
        {
            CurrentUser = null;
        }
        
        public static bool IsStudent => CurrentUser?.Role?.Name == "Student";
        public static bool IsInstructor => CurrentUser?.Role?.Name == "Instructor";
        public static bool IsAdmin => CurrentUser?.Role?.Name == "Admin";
    }
}

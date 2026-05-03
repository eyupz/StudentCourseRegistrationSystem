using System;
using System.Windows.Forms;
using StudentCourseRegistrationSystem.Forms;

namespace StudentCourseRegistrationSystem
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new LoginForm());
        }
    }
}

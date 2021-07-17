using System;
using System.Windows.Forms;

namespace KeyReassigner
{
    static class Program
    {

        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            using (AppContext appContext = new AppContext())
            {
                Application.Run(appContext);
            }
        }
    }
}

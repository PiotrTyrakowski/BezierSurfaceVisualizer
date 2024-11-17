using System;
using System.Windows.Forms;

namespace BezierSurfaceVisualizer
{
    static class Program
    {
        // Główna metoda uruchamiająca aplikację
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new UI.MainForm());
        }
    }
}

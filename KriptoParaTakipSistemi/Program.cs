using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KriptoParaTakipSistemi
{
    internal static class Program
    {
        /// <summary>
        /// Uygulamanın ana girdi noktası.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new GirişPaneli());
            
        }
        private static void Form1_FormÇıkıs(object sender, FormClosedEventArgs e)
        {
            // Form1 kapanınca uygulamayı tamamen sonlandır
            Application.Exit();
        }
    }
}

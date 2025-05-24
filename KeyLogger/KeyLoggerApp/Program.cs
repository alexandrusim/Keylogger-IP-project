using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using KeyloggerApp;

namespace KeyLoggerApp
{
    static class Program
    {
        /// <summary>
        /// Punctul de intrare principal pentru aplicatie
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}

using Eto.Forms;
using System;

namespace DomainScanner
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            new Application(Eto.Platform.Detect).Run(new MainForm());
        }
    }
}

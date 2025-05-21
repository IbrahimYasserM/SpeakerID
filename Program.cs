using Recorder.MFCC;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace Recorder
{
    static class Program
    {
       
        [STAThread]
        static void Main()
        {

            if (Environment.OSVersion.Version.Major >= 6)
                SetProcessDPIAware();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());


            //Pruning Test Cases In Sample Folder.
            //TestCasesRunner.runSample(1, true, 333);


            //Case 1 
            //TestCasesRunner.runTestCase1(23);



        }


        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();
    }
}

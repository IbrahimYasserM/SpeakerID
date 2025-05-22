using Recorder.MFCC;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace Recorder
{
    static class Program
    {
       
        static void TestCasesProgram()
        {
            // min , WithSilence, Pruning 
            //TestCasesRunner.PruningTest(1, true, 333);
            //TestCasesRunner.PruningTest(4, true, 1111);
            //var input = AudioOperations.OpenAudioFile("SAMPLE\\Input sample\\ItIsPlausible_Rich_US_English.wav");
            //var seq = AudioOperations.ExtractFeatures(input);
            //TestCasesRunner.testTrain(seq);
            //Case , Pruning Width
            TestCasesRunner.completeCases(1,23);
        }
        static void MainProgramProgram()
        {
            if (Environment.OSVersion.Version.Major >= 6)
                SetProcessDPIAware();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
        [STAThread]
        static void Main()
        {
            while (true)
            {
                Console.WriteLine("Do you want to run the Program(P) or run the Tests(T)");
                char choice = Console.ReadKey().KeyChar;
                if (choice == 'P' || choice == 'p')
                {
                    MainProgramProgram();
                    break;
                }
                else if (choice == 'T' || choice == 't')
                {
                    TestCasesProgram();
                    break;
                }
                else
                    Console.WriteLine("Invalid choice, please enter P or T");
            }
            

        }


        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();
    }
}

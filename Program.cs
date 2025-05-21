using Recorder.MFCC;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace Recorder
{
    static class Program
    {
       
        [STAThread]
        private static void TestCasesProgram()
        {
            TestCasesRunner.runSample(1, true, 333);
            TestCasesRunner.runTestCase1(23);
        }
        private static AudioSignal inputAdioSignal()
        {
            if (Environment.OSVersion.Version.Major >= 6)
                SetProcessDPIAware();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
            return new AudioSignal();
        }
        private static void MainProgramProgram()
        {
            while (true)
            {
                Console.WriteLine("Do you want to add a new audio into the database(A) or identify an audio(I) or any other key to exit");
                char choice = Console.ReadKey().KeyChar;
                if (choice == 'A' || choice == 'a')
                {
                    Console.WriteLine("Enter your name");
                    string name = Console.ReadLine();
                    Algorithms.enroll(name, inputAdioSignal());
                }
                else if (choice == 'I' || choice == 'i')
                {
                    int W = -1;
                    while (true)
                    {
                        Console.WriteLine("Wich algorithm do you prune search in the mating algorith? (Y/N)");
                        char choice2 = Console.ReadKey().KeyChar;
                        if(choice2 == 'Y' || choice2 == 'y')
                        {
                            Console.WriteLine("Enter the value for your prune");
                            W = Convert.ToInt32(Console.ReadLine());
                            break;
                        }
                        else if (choice2 == 'N' || choice2 == 'n')
                            break;
                    }
                    Algorithms.BestSequence best = Algorithms.identify(inputAdioSignal(), W);
                    Console.WriteLine("You are matched with " + best.name + " with cost " + best.distance);
                }
                else
                    break;
            }
        }
        static void Main()
        {
            while(true)
            {
                Console.WriteLine("Do you want to run the Program(P) or run the Tests(T) or any other key to exit");
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
                    break;
            }
        }


        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();
    }
}

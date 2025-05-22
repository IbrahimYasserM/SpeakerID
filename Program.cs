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
        public static string curName = null;
        private static MainForm mainForm = null;
        private static void TestCasesProgram()
        {
            //TestCasesRunner.runSample(1, true, 333);
            //TestCasesRunner.runTestCase1(23);
        }
        private static AudioSignal inputAdioSignal()
        {
            mainForm = new MainForm();
            Application.Run(mainForm);
            AudioSignal audioSignal = mainForm.getSignal();
            if (audioSignal == null)
                throw new Exception("No audio signal was recorded");
            return audioSignal;
        }
        private static void MainProgramProgram()
        {

            if (Environment.OSVersion.Version.Major >= 6)
                SetProcessDPIAware();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            mainForm = new MainForm();
            mainForm.loadFromDatabase();
            while (true)
            {
                Console.WriteLine("\nDo you want to add a new audio into the database(A) or identify an audio(I) or any other key to exit");
                char choice = Console.ReadKey().KeyChar;
                if (choice == 'A' || choice == 'a')
                {
                    Console.WriteLine("\nEnter your name");
                    string name = Console.ReadLine();
                    curName = name;

                    Algorithms.enroll(name, inputAdioSignal());
                }
                else if (choice == 'I' || choice == 'i')
                {
                    int W = -1;
                    bool removeSilence = false;
                    while (true)
                    {
                        Console.WriteLine("\nDo you want to remove silence from your audio file ?  (Y/N) \n");
                        char choice3 = Console.ReadKey().KeyChar;
                        if (choice3 == 'Y' || choice3 == 'y')
                            removeSilence = true;
                        else if (choice3 == 'N' || choice3 == 'n')
                            removeSilence = false;


                        Console.WriteLine("\nDo you want to prune search in the matching algorithm? (Y/N)");
                        char choice2 = Console.ReadKey().KeyChar;
                        if (choice2 == 'Y' || choice2 == 'y')
                        {
                            Console.WriteLine("\nEnter the value for your prune");
                            W = Convert.ToInt32(Console.ReadLine());
                            break;

                        }
                        else if (choice2 == 'N' || choice2 == 'n')
                            break;



                    }
                    Algorithms.BestSequence best = Algorithms.identify(inputAdioSignal(),removeSilence, W);
                    Console.WriteLine("\nYou are matched with " + best.name + " with cost " + best.distance);
                }
                else
                    break;
            }
        }
        [STAThread]
        static void Main()
        {
            while (true)
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
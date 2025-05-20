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
            //if (Environment.OSVersion.Version.Major >= 6)
            //    SetProcessDPIAware();

            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new MainForm());

            //var input = AudioOperations.OpenAudioFile("E:/university/Algo/project/OneDrive_2025-05-17/[2] SPEAKER IDENTIFICATION/TEST CASES/[1] SAMPLE/Pruning Test/4 min/[Input] Why Study Algorithms - (4 min).wav");
            //input = AudioOperations.RemoveSilence(input);
            //var inputseq = AudioOperations.ExtractFeatures(input);
            //var temp = AudioOperations.OpenAudioFile("E:/university/Algo/project/OneDrive_2025-05-17/[2] SPEAKER IDENTIFICATION/TEST CASES/[1] SAMPLE/Pruning Test/4 min/[Template] Big-Oh Notation (4 min).wav");
            //temp = AudioOperations.RemoveSilence(temp);
            //var tempseq = AudioOperations.ExtractFeatures(temp);
            //Stopwatch stopwatch = new Stopwatch();
            //stopwatch.Start();
            //double dist = Algorithms.dynamicTimeWarping(inputseq, tempseq);
            //stopwatch.Stop();
            //Console.WriteLine(dist);
            //Console.WriteLine(stopwatch.ElapsedMilliseconds);
            Stopwatch trainTime = new Stopwatch();
            Stopwatch testTime = new Stopwatch();

            //load & extract features of the train set.
            trainTime.Start();
            var trainSet = TestcaseLoader.LoadTestcase1Training("TrainingList.txt");
            var trainExtracted = TestController.extractFeatures(trainSet);
            trainTime.Stop();

            testTime.Start();
            var testDataset = TestcaseLoader.LoadTestcase1Testing("TestingList5Samples.txt");
            var testExtracted = TestController.extractFeatures(testDataset);
            var result = TestController.matching(trainExtracted, testExtracted);
            Console.WriteLine(result);
            var T1 = (trainTime.ElapsedMilliseconds / 1000.0f) / 60.0f;
            var T2 = (testTime.ElapsedMilliseconds / 1000.0f) / 60.0f;
            Console.WriteLine("Train Time: " + T1);
            Console.WriteLine("Test Time: " + T2);

            //var input = AudioOperations.OpenAudioFile("E:/university/Algo/project/OneDrive_2025-05-17/[2] SPEAKER IDENTIFICATION/TEST CASES/[1] SAMPLE/Pruning Test/4 min/[Input] Why Study Algorithms - (4 min).wav");
            //input = AudioOperations.RemoveSilence(input);
            //var inputseq = AudioOperations.ExtractFeatures(input);
            //var temp = AudioOperations.OpenAudioFile("E:/university/Algo/project/OneDrive_2025-05-17/[2] SPEAKER IDENTIFICATION/TEST CASES/[1] SAMPLE/Pruning Test/4 min/[Template] Big-Oh Notation (4 min).wav");
            //temp = AudioOperations.RemoveSilence(temp);
            //var tempseq = AudioOperations.ExtractFeatures(temp);
            //Stopwatch stopwatch = new Stopwatch();
            //stopwatch.Start();
            //double dist = Algorithms.dynamicTimeWarpingWithPruning(inputseq, tempseq, 1111);
            //stopwatch.Stop();
            //Console.WriteLine(dist);
            //Console.WriteLine(stopwatch.ElapsedMilliseconds);
        }


        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();
    }
}

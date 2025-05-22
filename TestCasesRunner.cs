using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace Recorder
{
    static class TestCasesRunner
    {
        // minutes  , WithSilence, W
        public static void runSample(int min, bool silence, int W)
        {
            string pathInput = "SAMPLE/Pruning Test/" + min.ToString() + " min/" + "[Input] Why Study Algorithms - (" + min.ToString() + " min).wav";
            string pathTemp = "SAMPLE/Pruning Test/" + min.ToString() + " min/" + "[Template] Big-Oh Notation (" + min.ToString() + " min).wav";

            var input = AudioOperations.OpenAudioFile(pathInput);
            var temp = AudioOperations.OpenAudioFile(pathTemp);

            if (silence)
            {
                input = AudioOperations.RemoveSilence(input);
                temp = AudioOperations.RemoveSilence(temp);
            }

            var inputseq = AudioOperations.ExtractFeatures(input);
            var tempseq = AudioOperations.ExtractFeatures(temp);

            Stopwatch dtwTime = new Stopwatch();
            dtwTime.Start();
            double dtwDis = Algorithms.dynamicTimeWarping(inputseq, tempseq);
            dtwTime.Stop();

            Stopwatch dtwPtime = new Stopwatch();
            dtwPtime.Start();
            double dtwPDis = Algorithms.dynamicTimeWarpingWithPruning(inputseq, tempseq, 1111);
            dtwPtime.Stop();

            Console.WriteLine("Distance With DTW: " + dtwDis);
            Console.WriteLine("Time For DTW Without Pruning: " + dtwTime.ElapsedMilliseconds);
            Console.WriteLine("--------------------------------");
            Console.WriteLine("Distance With DTW + Pruning: " + dtwPDis);
            Console.WriteLine("Time For DTW With Pruning: " + dtwPtime.ElapsedMilliseconds);

        }

        public static void runTestCase1(int W)
        {
            //load & extract features of the train set.
            Stopwatch trainTime = new Stopwatch();
            // Train time 
            trainTime.Start();
            var trainSet = TestcaseLoader.LoadTestcase1Training("TrainingList.txt");
            var trainExtracted = TestController.extractFeatures(trainSet);
            trainTime.Stop();

            // load & extract features of the test set.
            Stopwatch loadTestTime = new Stopwatch();
            loadTestTime.Start();
            var testDataset = TestcaseLoader.LoadTestcase1Testing("TestingList5Samples.txt");
            var testExtracted = TestController.extractFeatures(testDataset);
            loadTestTime.Stop();

            Stopwatch matchTime = new Stopwatch();
            matchTime.Start();
            var result = TestController.matching(trainExtracted, testExtracted);
            matchTime.Stop();

            Stopwatch matchTimeWithP = new Stopwatch();
            matchTimeWithP.Start();
            var resultWithP = TestController.matchingWithPruning(trainExtracted, testExtracted, W);
            matchTimeWithP.Stop();

            Console.WriteLine("Load & Extract TrainingSet: " + ((trainTime.ElapsedMilliseconds) / 1000.0) / 60.0);
            Console.WriteLine("DTW Accuracy: " + result);
            Console.WriteLine("DTW Time: " + ((matchTime.ElapsedMilliseconds + loadTestTime.ElapsedMilliseconds) / 1000.0) / 60.0);
            Console.WriteLine("------------------------------");
            Console.WriteLine("DTW Accuracy With Pruning: " + resultWithP);
            Console.WriteLine("DTW with Pruning Time: " + ((matchTimeWithP.ElapsedMilliseconds + loadTestTime.ElapsedMilliseconds) / 1000.0) / 60.0);
        }
    }
}
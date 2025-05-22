using Recorder.MFCC;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace Recorder
{
    static class TestCasesRunner
    {
        struct per
        {
            public string Name;
            public Sequence seq;
            public per(string n ,  Sequence s)
            {
                Name = n;
                seq = s;
            }
        }
        // minutes  , WithSilence, W
        public static void PruningTest(int min,bool silence, int W) {
            Console.WriteLine("\nRunning The "+ min + " - min Test: ");
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
            
            Stopwatch dtwPtime = new Stopwatch();
            dtwPtime.Start();
            double dtwPDis = Algorithms.dynamicTimeWarpingWithPruning(inputseq, tempseq, W);
            dtwPtime.Stop();

            Stopwatch dtwTime = new Stopwatch();
            dtwTime.Start();
            double dtwDis = Algorithms.dynamicTimeWarping(inputseq, tempseq); 
            dtwTime.Stop();


            Console.WriteLine("Distance With DTW: " + Math.Round(dtwDis,2));
            Console.WriteLine("Time For DTW Without Pruning: " + dtwTime.ElapsedMilliseconds);
            Console.WriteLine("--------------------------------");
            Console.WriteLine("Distance With DTW + Pruning: " + Math.Round(dtwPDis, 2));
            Console.WriteLine("Time For DTW With Pruning: " + dtwPtime.ElapsedMilliseconds);
            Console.WriteLine("--------------------------------");

        }

        public static void completeCases(int test , int W) {
            Console.WriteLine("\nRunning The Test Case " + test + ": ");
            //load & extract features of the train set.
            Stopwatch trainTime = new Stopwatch();
            // Train time 
            trainTime.Start();
            List<User> trainSet = new List<User>();
            if(test == 1)
                trainSet = TestcaseLoader.LoadTestcase1Training("case" + test.ToString() + "\\TrainingList.txt");
            else if (test == 2)
                trainSet = TestcaseLoader.LoadTestcase1Training("case" + test.ToString() + "\\TrainingList5Samples.txt");
            else if (test == 3)
                trainSet = TestcaseLoader.LoadTestcase1Training("case" + test.ToString() + "\\TrainingList1Sample.txt");

            var trainExtracted = TestController.extractFeatures(trainSet);
            trainTime.Stop();

            // load & extract features of the test set.
            Stopwatch loadTestTime = new Stopwatch();
            loadTestTime.Start();
            List<User> testDataset = new List<User>();
            if (test == 1)
                testDataset = TestcaseLoader.LoadTestcase1Testing("case" + test.ToString() + "\\TestingList5Samples.txt");
            else if (test == 2)
                testDataset = TestcaseLoader.LoadTestcase1Testing("case" + test.ToString() + "\\TestingList1Sample.txt");
            else if (test == 3)
                testDataset = TestcaseLoader.LoadTestcase1Testing("case" + test.ToString() + "\\TestingList.txt");

            var testExtracted = TestController.extractFeatures(testDataset);
            loadTestTime.Stop();

            Console.WriteLine("Load & Extract TrainingSet: " + ((trainTime.ElapsedMilliseconds) / 1000.0) / 60.0);
            Console.WriteLine("Test Info: ");

            //Console.WriteLine("--------------------------------");
            //Stopwatch enhancedDTW = new Stopwatch();
            //enhancedDTW.Start();
            //var enhancedR = TestController.syncMatching(trainExtracted,testExtracted);
            //enhancedDTW.Stop();
            //Console.WriteLine("Enhanced DTW Accuracy: " + enhancedR);
            //Console.WriteLine("Enhanced DTW Time: " + (enhancedDTW.ElapsedMilliseconds / 1000.0f) / 60.0);
            
            Stopwatch matchTime = new Stopwatch();
            matchTime.Start();
            var result = TestController.matching(trainExtracted, testExtracted);
            matchTime.Stop();
            Console.WriteLine("------------------------------");
            Console.WriteLine("DTW Accuracy: " + Math.Round(result, 2) + '%');
            Console.WriteLine("DTW Time: " + ((matchTime.ElapsedMilliseconds + loadTestTime.ElapsedMilliseconds) / 1000.0) / 60.0);
            Console.WriteLine("------------------------------");
            

            Stopwatch matchTimeWithP = new Stopwatch();
            matchTimeWithP.Start();
            var resultWithP = TestController.matching(trainExtracted, testExtracted,W);
            matchTimeWithP.Stop();


            Console.WriteLine("DTW Accuracy With Pruning: " + Math.Round(resultWithP,2) + '%');
            Console.WriteLine("DTW with Pruning Time: " + ((matchTimeWithP.ElapsedMilliseconds + loadTestTime.ElapsedMilliseconds) / 1000.0) / 60.0);
            Console.WriteLine("------------------------------");
        }

        private static List<per> loadTrainTEst()
        {
            List<per> result = new List<per>();
            string path = "SAMPLE\\Training set\\";
           result.Add(new per("conspiracy_Crystal_US_English" , AudioOperations.ExtractFeatures(AudioOperations.OpenAudioFile(path + "conspiracy_Crystal_US_English.wav"))));
           result.Add(new per("conspiracy_Mike_US_English", AudioOperations.ExtractFeatures(AudioOperations.OpenAudioFile(path + "conspiracy_Mike_US_English.wav"))));
           result.Add(new per("conspiracy_Rich_US_English", AudioOperations.ExtractFeatures(AudioOperations.OpenAudioFile(path + "conspiracy_Rich_US_English.wav"))));
           result.Add(new per("plausible_Crystal_US_English", AudioOperations.ExtractFeatures(AudioOperations.OpenAudioFile(path + "plausible_Crystal_US_English.wav"))));
           result.Add(new per("plausible_Mike_US_English", AudioOperations.ExtractFeatures(AudioOperations.OpenAudioFile(path + "plausible_Mike_US_English.wav"))));
           result.Add(new per("plausible_Rich_US_English", AudioOperations.ExtractFeatures(AudioOperations.OpenAudioFile(path + "plausible_Rich_US_English.wav"))));
            return result;
        } 

        public static void testTrain(Sequence q,int W = -1)
        {
            var train = loadTrainTEst(); 
            double dis = double.PositiveInfinity;
            string name = "";
            for (int i = 0; i < train.Count; i++) {
                double cost;
                if (W == -1)
                    cost = Algorithms.dynamicTimeWarping(q, train[i].seq);
                else
                    cost = Algorithms.dynamicTimeWarpingWithPruning(q, train[i].seq, W);
                if(cost< dis)
                {
                    name = train[i].Name;
                    dis = cost;
                }
            }
            //Console.WriteLine("------------------");
            //Console.WriteLine("Semi Optmized: ");
            List<Sequence> sequences = new List<Sequence>();
            for (int i = 0; i < train.Count; i++) sequences.Add(train[i].seq);
            Pair p = bonus.syncDTW(q, sequences);



            Console.WriteLine("------------------------");
            Console.WriteLine("Matched With: " + train[p.ind].Name);
            Console.WriteLine("With Distance: " + p.dist);
            Console.WriteLine("------------------------");
        }


    }
}

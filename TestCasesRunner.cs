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
        public static void runSample(int min,bool silence, int W) {
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


    }
}

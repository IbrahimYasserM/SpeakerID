using Accord.Audio.Windows;
using Recorder.MFCC;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace Recorder
{
    static class Algorithms
    {
        // get the elidean distance between two frames
        // each frame has 13 coefficients (features).
        private static double getuEuclideanDistance(double[] frameA, double[] frameB)
        {
            double res = 0;
            for (int i = 0; i < 13; i++)
            {
                res += (frameA[i] - frameB[i]) * (frameA[i] - frameB[i]);
            }
            res = Math.Sqrt(res);
            return res;
        }

        public static double dynamicTimeWarping(Sequence A, Sequence B)
        {
            int n = A.Frames.Length;
            int m = B.Frames.Length;

            double[] prevRow = new double[m + 1];
            double[] currRow = new double[m + 1];

            for (int j = 0; j <= m; j++)
                prevRow[j] = double.PositiveInfinity;
            prevRow[0] = 0;

            for (int i = 1; i <= n; i++)
            {
                for (int j = 0; j <= m; j++)
                    currRow[j] = double.PositiveInfinity;
                var aFeatures = A.Frames[i - 1].Features;
                for (int j = 1; j <= m; j++)
                {
                    double cost = getuEuclideanDistance(aFeatures, B.Frames[j - 1].Features);
                    double minPrev = Math.Min(
                        prevRow[j - 1], // No warping
                        Math.Min(
                            prevRow[j], // Stretching
                            (j >= 2) ? prevRow[j - 2] : double.PositiveInfinity // Shrinking
                        )
                    );
                    currRow[j] = cost + minPrev;
                }
                double[] temp = prevRow;
                prevRow = currRow;
                currRow = temp;
            }
            return prevRow[m];
        }

        public static double dynamicTimeWarpingWithPruning(Sequence A, Sequence B, int W) // Ebrahim & Adham
        {

            // Each sequence is a list of frames.
            // we want to compute min distance between the two sequences USING DP.
            //Transtions:
            // 1. Next input frame aligns to same template frame (stretching).
            // 2. Next input frame aligns to next template frame (no warping).
            // 3. Next input frame skips one template frame (shrinking, at most 1/2).
            int n = A.Frames.Length;
            int m = B.Frames.Length;

            // Calculate maximum allowed difference between i and j
            double[,] dtw = new double[n + 1, m + 1];

            // Initialize all values to infinity
            for (int i = 0; i <= n; i++)
                for (int j = 0; j <= m; j++)
                    dtw[i, j] = double.PositiveInfinity;

            // Base case
            dtw[0, 0] = 0;
            int window = Math.Max(W, 2 * Math.Abs(n-m));
            for (int i = 1; i <= n; i++)
            {
                int start = Math.Max(1, i - window);
                int end = Math.Min(m, i + window);
                double[] aFeatures = A.Frames[i - 1].Features;
                for (int j = start; j <= end; j++)
                {
                    double cost = getuEuclideanDistance(aFeatures, B.Frames[j - 1].Features);
                    double prev = dtw[i - 1, j - 1]; // No warping
                    prev = Math.Min(prev, dtw[i - 1, j]); // Stretching
                    if(j>= 2)
                        prev = Math.Min(prev, dtw[i - 1, j - 2]); // Shrinking

                    if (Math.Abs(i - j) > window)
                        continue;

                    dtw[i, j] = cost + prev;
                }
            }

            return dtw[n, m];
        }
        
        private static SortedDictionary<string, List<Sequence>> dataset = new SortedDictionary<string, List<Sequence>>();
        public static void enroll(string name, AudioSignal record) // Ebrahim & Adham
        {
            Sequence sequence = AudioOperations.ExtractFeatures(AudioOperations.RemoveSilence(record));
            dataset[name].Add(sequence);
        }

        public struct BestSequence
        {
            public string name;
            public double distance;
            public BestSequence(string name, double distance)
            {
                this.name = name;
                this.distance = distance;
            }
        }
        public static BestSequence identify(AudioSignal signal, int W = -1) // Ibrahim & Zamel
        {
            Sequence A = AudioOperations.ExtractFeatures(AudioOperations.RemoveSilence(signal));
            String name = null;
            double mn = double.MaxValue;
            // loop over dataset and minimize the distance
            foreach(var user in dataset)
                foreach(var sequence in user.Value)
                {
                    double distance = W == -1 ? dynamicTimeWarping(A, sequence) : dynamicTimeWarpingWithPruning(A, sequence, W);
                    if (distance < mn)
                    {
                        mn = distance;
                        name = user.Key;
                    }
                }
            return new BestSequence(name, mn);
        }

    }
}

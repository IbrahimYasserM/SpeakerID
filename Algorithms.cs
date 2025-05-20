using Accord.Audio.Windows;
using Recorder.MFCC;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace Recorder
{
    static class Algorithms
    {
        // get the elidean distance between two frames
        // each frame has 13 coefficients (features).
        public static double getuEuclideanDistance(double[] frameA, double[] frameB)
        {
            double res = 0;
            for (int i = 0; i < frameA.Length; i++)
            {
                res += (frameA[i] - frameB[i]) * (frameA[i] - frameB[i]);
            }
            res = Math.Sqrt(res);
            return res;
        }

        public static  double dynamicTimeWarping(Sequence A, Sequence B) // Ibrahim & Zamel
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
            

            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    double cost = getuEuclideanDistance(A.Frames[i - 1].Features, B.Frames[j - 1].Features);
                    double prev = dtw[i - 1, j - 1]; // No warping
                    prev = Math.Min(prev, dtw[i - 1, j]); // Stretching
                    if(j>= 2)
                        prev = Math.Min(prev, dtw[i - 1, j - 2]); // Shrinking


                    dtw[i, j] = cost + prev;
                }
            }

            return dtw[n, m];
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
            

            for (int i = 1; i <= n; i++)
            {
                int window = Math.Min(W, Math.Max(n, m));
                int start = Math.Max(1, i - window);
                int end = Math.Min(m, i + window);

                for (int j = start; j <= end; j++)
                {
                    double cost = getuEuclideanDistance(A.Frames[i - 1].Features, B.Frames[j - 1].Features);
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


        public static void enroll() // Ebrahim & Adham
        {

        }

        public static string identify(Sequence A) // Ibrahim & Zamel
        {
            String ans = null;
            double mn = double.MaxValue;
            // loop over dataset and minimize the distance
            return ans;
        }

    }
}

using Accord.Audio.Windows;
using Recorder.MFCC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace Recorder
{
    class Algorithms
    {
        // get the elidean distance between two frames
        // each frame has 13 coefficients (features).
        private double getuEuclideanDistance(double[] frameA, double[] frameB)
        {
            double res = 0;
            for (int i = 0; i < 13; i++)
            {
                res += (frameA[i] - frameB[i]) * (frameA[i] - frameB[i]);
            }
            res = Math.Sqrt(res);
            return res;
        }

        public double dynamicTimeWarping(Sequence A, Sequence B) // Ibrahim & Zamel
        {
            // Each sequence is a list of frames.
            // we want to compute min distance between the two sequences USING DP.
            //Transtions:
            // 1. Next input frame aligns to same template frame (stretching).
            // 2. Next input frame aligns to next template frame (no warping).
            // 3. Next input frame skips one template frame (shrinking, at most 1/2).
            int n = A.Frames.Length;
            int m = B.Frames.Length;
            double[,] dtw = new double[n + 1, m + 1];
            // initialize the first row and column to infinity.
            for (int i = 0; i <= n; i++)
                for (int j = 0; j <= m; j++)
                    dtw[i, j] = double.PositiveInfinity;
            dtw[0, 0] = 0;

            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    double cost = getuEuclideanDistance(A.Frames[i - 1].Features, B.Frames[j - 1].Features);
                    dtw[i, j] = cost + Math.Min(dtw[i - 1, j - 1], dtw[i - 1, j - 1]);
                    if(j > 1)
                        dtw[i, j] = Math.Min(dtw[i, j], dtw[i - 1, j - 2] + cost);
                }
            }

            return dtw[n, m];
        }

        public double dynamicTimeWarpingWithPruning(Sequence A, Sequence B, int W) // Ebrahim & Adham
        {

            // This is the basic DTW algorithm , based on the formula sent on discord, this still doesn't include pruning

            int n = A.Frames.Length;
            int m = B.Frames.Length;

            double[,] dtw = new double[n + 1, m + 1];

            for (int i = 0; i <= n; i++)
                for (int j = 0; j <= m; j++)
                    dtw[i, j] = double.PositiveInfinity;

            dtw[0, 0] = 0;

            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    if (Math.Abs(i - j) > W) break;
                    double cost = getuEuclideanDistance(A.Frames[i - 1].Features, B.Frames[j - 1].Features);
                    dtw[i, j] = cost + Math.Min(dtw[i - 1, j], Math.Min(dtw[i, j - 1], dtw[i - 1, j - 1]));
                }
            }

            return dtw[n, m];
        }


        public void enroll() // Ebrahim & Adham
        {

        }

        public string identify(Sequence A) // Ibrahim & Zamel
        {
            String ans = null;
            double mn = double.MaxValue;
            // loop over dataset and minimize the distance
            return ans;
        }

    }
}

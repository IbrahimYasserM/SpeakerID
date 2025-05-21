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

            // Early termination for identical sequences
            if (n == m)
            {
                bool identical = true;
                for (int i = 0; i < n && identical; i++)
                {
                    if (getuEuclideanDistance(A.Frames[i].Features, B.Frames[i].Features) > 0.001)
                        identical = false;
                }
                if (identical)
                    return 0;
            }

            // Use only two rows instead of the full matrix to save memory
            double[] prevRow = new double[m + 1];
            double[] currRow = new double[m + 1];

            // Initialize the first row
            for (int j = 0; j <= m; j++)
                prevRow[j] = double.PositiveInfinity;
            prevRow[0] = 0;

            for (int i = 1; i <= n; i++)
            {
                currRow[0] = double.PositiveInfinity;
                var aFeatures = A.Frames[i - 1].Features;

                // Unroll the loop for first iteration where j=1
                if (m >= 1)
                {
                    double cost = getuEuclideanDistance(aFeatures, B.Frames[0].Features);
                    currRow[1] = cost + prevRow[0]; // Only one previous value possible here
                }

                // Main loop with optimized comparisons
                for (int j = 2; j <= m; j++)
                {
                    double cost = getuEuclideanDistance(aFeatures, B.Frames[j - 1].Features);

                    // Find minimum of three values without redundant calculations
                    double min1 = prevRow[j - 1]; // No warping
                    double min2 = prevRow[j];     // Stretching
                    double min3 = prevRow[j - 2]; // Shrinking

                    double minValue;
                    if (min1 <= min2 && min1 <= min3)
                        minValue = min1;
                    else if (min2 <= min3)
                        minValue = min2;
                    else
                        minValue = min3;

                    currRow[j] = cost + minValue;
                }

                // Swap rows
                double[] temp = prevRow;
                prevRow = currRow;
                currRow = temp;
            }
            return prevRow[m];
        }

        public static double dynamicTimeWarpingWithPruning(Sequence A, Sequence B, int W)
        {
            int n = A.Frames.Length;
            int m = B.Frames.Length;

            // Early termination for trivial cases
            if (n == 0 || m == 0)
                return n == 0 && m == 0 ? 0 : double.PositiveInfinity;

            // Simple case for single-frame sequences
            if (n == 1 && m == 1)
                return getuEuclideanDistance(A.Frames[0].Features, B.Frames[0].Features);

            // Ensure n <= m for optimized processing (if n > m, swap A and B)
            if (n > m)
            {
                // Swap sequences
                Sequence temp = A;
                A = B;
                B = temp;
                int tempLen = n;
                n = m;
                m = tempLen;
            }

            // Adjust window based on sequence length difference
            int window = Math.Max(W, Math.Abs(n - m));

            // Reuse DTW arrays to avoid memory allocation each time
            double[] prevRow = new double[m + 1];
            double[] currRow = new double[m + 1];

            // ----------------- CORE OPTIMIZATIONS -----------------

            // 1. Initialize only the elements we'll use (within window)
            for (int j = 0; j <= m; j++)
                prevRow[j] = double.PositiveInfinity;
            prevRow[0] = 0;

            // 2. Pre-compute distances that might be reused
            double[][] distanceCache = null;
            bool useCache = n * m < 10000; // Only cache distances for smaller sequences

            if (useCache)
            {
                distanceCache = new double[n][];
                for (int i = 0; i < n; i++)
                {
                    distanceCache[i] = new double[m];
                    for (int j = 0; j < m; j++)
                        distanceCache[i][j] = -1; // -1 indicates not computed yet
                }
            }

            // 3. Process rows with pruning
            for (int i = 1; i <= n; i++)
            {
                // Reset current row efficiently (only reset elements we'll use)
                int startJ = Math.Max(1, i - window);
                int endJ = Math.Min(m, i + window);

                for (int j = 0; j < startJ; j++)
                    currRow[j] = double.PositiveInfinity;

                currRow[0] = double.PositiveInfinity;

                // 4. Extract current frame features once (avoid repetitive access)
                double[] aFeatures = A.Frames[i - 1].Features;

                // 5. Main loop with early pruning and optimization
                for (int j = startJ; j <= endJ; j++)
                {
                    // Get distance from cache if available
                    double cost;
                    if (useCache && distanceCache[i - 1][j - 1] >= 0)
                    {
                        cost = distanceCache[i - 1][j - 1];
                    }
                    else
                    {
                        cost = getuEuclideanDistance(aFeatures, B.Frames[j - 1].Features);
                        if (useCache)
                            distanceCache[i - 1][j - 1] = cost;
                    }

                    // Fast path for common minimum calculation
                    double minPrev;
                    if (j == 1)
                    {
                        minPrev = prevRow[0]; // Only one option when j=1
                    }
                    else if (j >= 2)
                    {
                        // Inline min calculation (faster than calling Math.Min multiple times)
                        double a = prevRow[j - 1]; // No warping
                        double b = prevRow[j];   // Stretching 
                        double c = prevRow[j - 2]; // Shrinking

                        minPrev = a <= b ? (a <= c ? a : c) : (b <= c ? b : c);
                    }
                    else
                    {
                        minPrev = prevRow[j - 1]; // Fallback
                    }

                    currRow[j] = cost + minPrev;
                }

                // Set remaining elements to infinity
                for (int j = endJ + 1; j <= m; j++)
                    currRow[j] = double.PositiveInfinity;

                // 6. Efficiently swap rows (avoid creating new arrays)
                double[] temp = prevRow;
                prevRow = currRow;
                currRow = temp;
            }

            // 7. Result is in the last cell
            return prevRow[m];
        }
        private static Dictionary<string, List<Sequence>> dataset = new Dictionary<string, List<Sequence>>();
        public static void enroll(string name, AudioSignal record) // Ebrahim & Adham
        {
            Sequence sequence = AudioOperations.ExtractFeatures(AudioOperations.RemoveSilence(record));
            dataset[name].Add(sequence);
        }

        public static string identify(Sequence A) // Ibrahim & Zamel
        {
            String ans = null;
            double mn = double.MaxValue;
            // loop over dataset and minimize the distance
            foreach(var user in dataset)
                foreach(var sequence in user.Value)
                {
                    double distance = dynamicTimeWarping(A, sequence);
                    if (distance < mn)
                    {
                        mn = distance;
                        ans = user.Key;
                    }
                }
            return ans;
        }

    }
}

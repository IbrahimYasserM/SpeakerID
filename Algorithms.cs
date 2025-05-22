using Accord.Audio.Windows;
using Recorder.MFCC;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace Recorder
{

    static class Algorithms
    {
        static double[] prev = new double[100000 + 1];
        static double[] curr = new double[100000 + 1];
        static double temp = 0;
        static double cost = 0;
        static int n, m;
        // get the elidean distance between two frames
        // each frame has 13 coefficients (features).
        public static double getuEuclideanDistance(double[] frameA, double[] frameB)
        {
            double d0 = frameA[0] - frameB[0];
            double d1 = frameA[1] - frameB[1];
            double d2 = frameA[2] - frameB[2];
            double d3 = frameA[3] - frameB[3];
            double d4 = frameA[4] - frameB[4];
            double d5 = frameA[5] - frameB[5];
            double d6 = frameA[6] - frameB[6];
            double d7 = frameA[7] - frameB[7];
            double d8 = frameA[8] - frameB[8];
            double d9 = frameA[9] - frameB[9];
            double d10 = frameA[10] - frameB[10];
            double d11 = frameA[11] - frameB[11];
            double d12 = frameA[12] - frameB[12];

            return Math.Sqrt(d0 * d0 + d1 * d1 + d2 * d2 + d3 * d3 + d4 * d4 + d5 * d5 + d6 * d6 +
                            d7 * d7 + d8 * d8 + d9 * d9 + d10 * d10 + d11 * d11 + d12 * d12);
        }

        
        public static double dynamicTimeWarping(Sequence A, Sequence B)
        {
            n = A.Frames.Length;
            m = B.Frames.Length;

            if (n == 0 || m == 0) return double.PositiveInfinity;
            if (n == 1 && m == 1) return getuEuclideanDistance(A.Frames[0].Features, B.Frames[0].Features);

            


            for (int j = 0; j <= m; j++) prev[j] = double.PositiveInfinity;
            prev[0] = 0;

            for (int i = 1; i <= n; i++)
            {
                curr[0] = double.PositiveInfinity;
                double[] aFeatures = A.Frames[i - 1].Features;
                
                for (int j = 1; j <= m; j++)
                {
                    cost = getuEuclideanDistance(aFeatures, B.Frames[j - 1].Features);

                    double min = prev[j - 1]; 
                    if (prev[j] < min) min = prev[j]; 
                    if (j >= 2 && prev[j - 2] < min) min = prev[j - 2]; 

                    curr[j] = cost + min;
                }
                double[] temp = prev;
                prev = curr;
                curr = temp;
            }

            return prev[m];
        }
        public static double dynamicTimeWarpingWithPruning(Sequence A, Sequence B, int W)
        {
            int n = A.Frames.Length;
            int m = B.Frames.Length;

            if (n == 0 || m == 0) return double.PositiveInfinity;
            if (n == 1 && m == 1) return getuEuclideanDistance(A.Frames[0].Features, B.Frames[0].Features);

            for (int j = 0; j <= m; j++) prev[j] = double.PositiveInfinity;
            prev[0] = 0;

            int window = Math.Max(W, Math.Abs(n - m));  

            for (int i = 1; i <= n; i++)
            {
                for (int j = 0; j <= m; j++) curr[j] = double.PositiveInfinity;

                double[] aFeatures = A.Frames[i - 1].Features;

                int start = Math.Max(1, i - window);
                int end = Math.Min(m, i + window);

                for (int j = start; j <= end; j++)
                {
                    double cost = getuEuclideanDistance(aFeatures, B.Frames[j - 1].Features);

                    double min = prev[j - 1]; 
                    if (prev[j] < min) min = prev[j]; 
                    if (j >= 2 && prev[j - 2] < min) min = prev[j - 2];

                    curr[j] = cost + min;
                }

                double[] temp = prev;
                prev = curr;
                curr = temp;
            }

            return prev[m];
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
            foreach (var user in dataset)
                foreach (var sequence in user.Value)
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
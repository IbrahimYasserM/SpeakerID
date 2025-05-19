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
        public double getuEuclideanDistance(double[] frameA, double[] frameB)
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

            return -1.0;
        }

        public double dynamicTimeWarpingWithPruning(Sequence A, Sequence B) // Ebrahim & Adham
        {

            return -1.0;
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

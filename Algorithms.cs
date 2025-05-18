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
        public double dynamicTimeWarping(Sequence A, Sequence B) // Ibrahim & Zamel
        {
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

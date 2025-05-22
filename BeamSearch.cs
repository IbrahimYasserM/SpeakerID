using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Recorder.MFCC;
namespace Recorder
{

    internal class BeamSearch
    { 
        private struct State
        {
            public int i;
            public double cost;
            public State(int i, double cost)
            {
                this.i = i;
                this.cost = cost;
            }
        }
        public static double BeamSearchDTW(Sequence A, Sequence B, double T)
        {
            int n = A.Frames.Length;
            int m = B.Frames.Length;

            List<State> prevBeam = new List<State> { new State(0, 0.0) };

            for (int i = 1; i <= n; i++)
            {
                List<State> candidates = new List<State>();
                double[] aFeatures = A.Frames[i - 1].Features;

                foreach (var state in prevBeam)
                {
                    // Try steps to j+1 and j+2
                    for (int dj = 0; dj <= 2; dj++)
                    {
                        int nextJ = state.i + dj;
                        if (nextJ > 0 && nextJ <= m)
                        {
                            double cost = Algorithms.getuEuclideanDistance(aFeatures, B.Frames[nextJ - 1].Features);
                            double totalCost = state.cost + cost;
                            candidates.Add(new State(nextJ, totalCost));
                        }
                    }
                }

                // Find minimum cost in this beam
                double localMin = candidates.Min(t => t.cost);

                // Prune paths with cost > globalMin + T
                prevBeam = candidates
                    .Where(t => t.cost <= localMin + T)
                    .GroupBy(t => t.i)
                    .Select(g => g.OrderBy(t => t.cost).First())
                    .ToList();
            }

            // Return the best cost that reaches the end
            if(prevBeam.Count(x => x.i == m) == 0)
                return double.PositiveInfinity;
            return prevBeam
                .Where(t => t.i == m)
                .Select(t => t.cost)
                .Min();
        }
    }
}

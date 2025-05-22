using Recorder.MFCC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;


namespace Recorder
{
    struct Frame
    {
        //public double[,] dp;
        //public double[] curRow;
        //public double[] prevRow;
        public Dictionary<int, Dictionary<int, double>> dp;

        public Frame(int n, int m)
        {
            //this.dp = new double[n + 1, m + 1];
            //for (int i = 0; i <= n; i++)
            //    for (int j = 0; j <= m; j++)
            //        dp[i, j] = double.PositiveInfinity;
            //dp[0, 0] = 0;
            dp = new Dictionary<int, Dictionary<int, double>>();
            if (!dp.ContainsKey(0))
                dp[0] = new Dictionary<int, double>();

            dp[0][0] = 0.0;
            for (int i = 1; i <= n + 2; i++)
            {
                dp[i] = new Dictionary<int, double>();
            }


        }

    }
    struct point
    {
        public double dist;
        public int i, j, k;
        public point(int i, int j, int k, double dist)
        {
            this.i = i;
            this.j = j;
            this.k = k;
            this.dist = dist;
        }
    }
    class PointComparer : IComparer<point>
    {
        public int Compare(point a, point b)
        {
            if (a.dist != b.dist) return a.dist.CompareTo(b.dist);
            if (a.i != b.i) return a.i.CompareTo(b.i);
            if (a.j != b.j) return a.j.CompareTo(b.j);
            return a.k.CompareTo(b.k);
        }
    }
    struct Pair
    {
        public double dist;
        public int ind;
        public Pair(double dist, int ind)
        {
            this.dist = dist;
            this.ind = ind;
        }
    }
    static class bonus
    {
        public static Pair syncDTW(Sequence A, List<Sequence> B)
        {
            List<Frame> frames = new List<Frame>();
            SortedSet<point> points = new SortedSet<point>(new PointComparer());
            for (int i = 0; i < B.Count; i++)
            {
                frames.Add(new Frame(A.Frames.Length, B[i].Frames.Length));
                //frames.Add(new Frame());
                points.Add(new point(0, 0, i, 0));
            }
            while (points.Count > 0)
            {
                point p = points.Min;
                points.Remove(p);
                // available transitions
                // 1. Next input frame skips one template frame (shrinking, at most 1/2). {i-1, j-2} => {i, j}
                // 2. Next input frame aligns to next template frame (no warping). {i-1, j-1} => {i, j}
                // 3. Next input frame aligns to same template frame (stretching). {i-1, j} => {i, j}

                if (p.i == A.Frames.Length && p.j == B[p.k].Frames.Length)
                {
                    // we reached the end of the sequence
                    return new Pair(p.dist, p.k);
                }
                // 1. Next input frame skips one template frame (shrinking, at most 1/2). {i-1, j-2} => {i, j}
                if (p.i < A.Frames.Length && p.j + 1 < B[p.k].Frames.Length)
                {
                    double dist = p.dist + Algorithms.getuEuclideanDistance(A.Frames[p.i].Features, B[p.k].Frames[p.j + 1].Features);
                    if (!frames[p.k].dp.ContainsKey(p.i + 1))
                    {
                        frames[p.k].dp[p.i + 1] = new Dictionary<int, double>();
                    }
                    if (!frames[p.k].dp[p.i + 1].ContainsKey(p.j + 2))
                    {
                        frames[p.k].dp[p.i + 1][p.j + 2] = new double();
                        frames[p.k].dp[p.i + 1][p.j + 2] = double.MaxValue;
                    }
                    if (dist < frames[p.k].dp[p.i + 1][p.j + 2])
                    {
                        frames[p.k].dp[p.i + 1][p.j + 2] = dist;
                        points.Add(new point(p.i + 1, p.j + 2, p.k, dist));
                    }
                }
                // 2. Next input frame aligns to next template frame (no warping). {i-1, j-1} => {i, j}
                if (p.i < A.Frames.Length && p.j < B[p.k].Frames.Length)
                {
                    double dist = p.dist + Algorithms.getuEuclideanDistance(A.Frames[p.i].Features, B[p.k].Frames[p.j].Features);
                    if (!frames[p.k].dp.ContainsKey(p.i + 1))
                    {
                        frames[p.k].dp[p.i + 1] = new Dictionary<int, double>();
                    }
                    if (!frames[p.k].dp[p.i + 1].ContainsKey(p.j + 1))
                    {
                        frames[p.k].dp[p.i + 1][p.j + 1] = new double();
                        frames[p.k].dp[p.i + 1][p.j + 1] = double.MaxValue;

                    }
                    if (dist < frames[p.k].dp[p.i + 1][p.j + 1])
                    {
                        frames[p.k].dp[p.i + 1][p.j + 1] = dist;
                        points.Add(new point(p.i + 1, p.j + 1, p.k, dist));
                    }
                }
                // 3. Next input frame aligns to same template frame (stretching). {i-1, j} => {i, j}
                if (p.i < A.Frames.Length && p.j <= B[p.k].Frames.Length && p.j - 1 >= 0)
                {
                    double dist = p.dist + Algorithms.getuEuclideanDistance(A.Frames[p.i].Features, B[p.k].Frames[p.j - 1].Features);
                    if (!frames[p.k].dp.ContainsKey(p.i + 1))
                    {
                        frames[p.k].dp[p.i + 1] = new Dictionary<int, double>();
                    }
                    if (!frames[p.k].dp[p.i + 1].ContainsKey(p.j))
                    {
                        frames[p.k].dp[p.i + 1][p.j] = new double();
                        frames[p.k].dp[p.i + 1][p.j] = double.MaxValue;

                    }

                    if (dist < frames[p.k].dp[p.i + 1][p.j])
                    {
                        frames[p.k].dp[p.i + 1][p.j] = dist;
                        points.Add(new point(p.i + 1, p.j, p.k, dist));
                    }
                }
            }
            return new Pair(-1, -1);

        }

        public static string syncDTWCaller(List<user> trainingset, Sequence seq)
        {
            List<Sequence> sequences = new List<Sequence>();
            for (int i = 0; i < trainingset.Count; i++)
            {
                foreach (var x in trainingset[i].userTemplates)
                    sequences.Add(x);
            }
            Pair ret = syncDTW(seq, sequences);
            int cur = 0, curin = 0;
            while (cur <= ret.ind)
            {
                cur += trainingset[curin].userTemplates.Count;
                curin++;
            }
            return trainingset[curin - 1].userName;
        }


    }
}
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
            Pair ret = syncDTWFaster(seq, sequences);
            int cur = 0, curin = 0;
            while (cur <= ret.ind)
            {
                cur += trainingset[curin].userTemplates.Count;
                curin++;
            }
            return trainingset[curin - 1].userName;
        }

        // Optimized version maintaining synchronous logic
        public static Pair syncDTWOptimized(Sequence A, List<Sequence> B)
        {
            int K = B.Count;
            int N = A.Frames.Length;

            // Use arrays instead of dictionaries for better performance
            var dpTables = new Dictionary<int, double>[K][];
            var visited = new HashSet<int>[K][];

            // Initialize DP tables more efficiently
            for (int k = 0; k < K; k++)
            {
                int M = B[k].Frames.Length;
                dpTables[k] = new Dictionary<int, double>[N + 1];
                visited[k] = new HashSet<int>[N + 1];

                for (int i = 0; i <= N; i++)
                {
                    dpTables[k][i] = new Dictionary<int, double>();
                    visited[k][i] = new HashSet<int>();
                }
            }

            // Use a more efficient priority queue implementation
            var priorityQueue = new SortedDictionary<double, List<State>>();

            // Initialize starting states
            for (int k = 0; k < K; k++)
            {
                var state = new State(0, 0, k, 0.0);
                dpTables[k][0][0] = 0.0;

                if (!priorityQueue.ContainsKey(0.0))
                    priorityQueue[0.0] = new List<State>();
                priorityQueue[0.0].Add(state);
            }

            while (priorityQueue.Count > 0)
            {
                // Get minimum distance states
                var minKey = priorityQueue.Keys.First();
                var states = priorityQueue[minKey];
                var currentState = states[0];
                states.RemoveAt(0);

                if (states.Count == 0)
                    priorityQueue.Remove(minKey);

                int i = currentState.i;
                int j = currentState.j;
                int k = currentState.k;
                double dist = currentState.dist;

                // Skip if we've already processed this state with better distance
                if (visited[k][i].Contains(j))
                    continue;
                visited[k][i].Add(j);

                // Check if we reached the end
                if (i == N && j == B[k].Frames.Length)
                {
                    return new Pair(dist, k);
                }

                // Generate next states with the same three transitions
                AddNextStates(A, B, dpTables, priorityQueue, i, j, k, dist, N);
            }

            return new Pair(-1, -1);
        }

        private static void AddNextStates(Sequence A, List<Sequence> B,
            Dictionary<int, double>[][] dpTables,
            SortedDictionary<double, List<State>> priorityQueue,
            int i, int j, int k, double currentDist, int N)
        {
            int M = B[k].Frames.Length;

            // Transition 1: Skip template frame {i, j} -> {i+1, j+2}
            if (i < N && j + 1 < M)
            {
                double cost = Algorithms.getuEuclideanDistance(A.Frames[i].Features, B[k].Frames[j + 1].Features);
                double newDist = currentDist + cost;

                if (!dpTables[k][i + 1].ContainsKey(j + 2) || newDist < dpTables[k][i + 1][j + 2])
                {
                    dpTables[k][i + 1][j + 2] = newDist;
                    var state = new State(i + 1, j + 2, k, newDist);
                    AddToQueue(priorityQueue, newDist, state);
                }
            }

            // Transition 2: Normal alignment {i, j} -> {i+1, j+1}
            if (i < N && j < M)
            {
                double cost = Algorithms.getuEuclideanDistance(A.Frames[i].Features, B[k].Frames[j].Features);
                double newDist = currentDist + cost;

                if (!dpTables[k][i + 1].ContainsKey(j + 1) || newDist < dpTables[k][i + 1][j + 1])
                {
                    dpTables[k][i + 1][j + 1] = newDist;
                    var state = new State(i + 1, j + 1, k, newDist);
                    AddToQueue(priorityQueue, newDist, state);
                }
            }

            // Transition 3: Repeat template frame {i, j} -> {i+1, j}
            if (i < N && j > 0 && j <= M)
            {
                double cost = Algorithms.getuEuclideanDistance(A.Frames[i].Features, B[k].Frames[j - 1].Features);
                double newDist = currentDist + cost;

                if (!dpTables[k][i + 1].ContainsKey(j) || newDist < dpTables[k][i + 1][j])
                {
                    dpTables[k][i + 1][j] = newDist;
                    var state = new State(i + 1, j, k, newDist);
                    AddToQueue(priorityQueue, newDist, state);
                }
            }
        }

        private static void AddToQueue(SortedDictionary<double, List<State>> queue, double dist, State state)
        {
            if (!queue.ContainsKey(dist))
                queue[dist] = new List<State>();
            queue[dist].Add(state);
        }

        // More efficient state representation
        public struct State
        {
            public int i, j, k;
            public double dist;

            public State(int i, int j, int k, double dist)
            {
                this.i = i;
                this.j = j;
                this.k = k;
                this.dist = dist;
            }
        }

        // Alternative: Even more optimized version using custom priority queue
        public static Pair syncDTWFaster(Sequence A, List<Sequence> B)
        {
            int K = B.Count;
            int N = A.Frames.Length;

            // Use 3D array for better cache performance (if memory allows)
            var maxM = B.Max(seq => seq.Frames.Length);
            if (K * N * maxM < 10_000_000) // Only if memory footprint is reasonable
            {
                return syncDTWArray(A, B, K, N, maxM);
            }

            // Fallback to dictionary version for large datasets
            return syncDTWOptimized(A, B);
        }

        private static Pair syncDTWArray(Sequence A, List<Sequence> B, int K, int N, int maxM)
        {
            // Pre-allocate arrays
            double[,,] dp = new double[K, N + 1, maxM + 1];
            bool[,,] visited = new bool[K, N + 1, maxM + 1];

            // Initialize with infinity
            for (int k = 0; k < K; k++)
                for (int i = 0; i <= N; i++)
                    for (int j = 0; j <= maxM; j++)
                        dp[k, i, j] = double.MaxValue;

            // Custom priority queue using binary heap for better performance
            var pq = new MinHeap<StateArray>();

            // Initialize
            for (int k = 0; k < K; k++)
            {
                dp[k, 0, 0] = 0;
                pq.Insert(new StateArray(0, 0, k, 0.0));
            }

            while (!pq.IsEmpty())
            {
                var state = pq.ExtractMin();
                int i = state.i, j = state.j, k = state.k;
                double dist = state.dist;

                if (visited[k, i, j]) continue;
                visited[k, i, j] = true;

                // Check termination
                if (i == N && j == B[k].Frames.Length)
                    return new Pair(dist, k);

                int M = B[k].Frames.Length;

                // Three transitions
                if (i < N && j + 1 < M)
                {
                    double cost = Algorithms.getuEuclideanDistance(A.Frames[i].Features, B[k].Frames[j + 1].Features);
                    double newDist = dist + cost;
                    if (newDist < dp[k, i + 1, j + 2])
                    {
                        dp[k, i + 1, j + 2] = newDist;
                        pq.Insert(new StateArray(i + 1, j + 2, k, newDist));
                    }
                }

                if (i < N && j < M)
                {
                    double cost = Algorithms.getuEuclideanDistance(A.Frames[i].Features, B[k].Frames[j].Features);
                    double newDist = dist + cost;
                    if (newDist < dp[k, i + 1, j + 1])
                    {
                        dp[k, i + 1, j + 1] = newDist;
                        pq.Insert(new StateArray(i + 1, j + 1, k, newDist));
                    }
                }

                if (i < N && j > 0 && j <= M)
                {
                    double cost = Algorithms.getuEuclideanDistance(A.Frames[i].Features, B[k].Frames[j - 1].Features);
                    double newDist = dist + cost;
                    if (newDist < dp[k, i + 1, j])
                    {
                        dp[k, i + 1, j] = newDist;
                        pq.Insert(new StateArray(i + 1, j, k, newDist));
                    }
                }
            }

            return new Pair(-1, -1);
        }

        public struct StateArray : IComparable<StateArray>
        {
            public int i, j, k;
            public double dist;

            public StateArray(int i, int j, int k, double dist)
            {
                this.i = i; this.j = j; this.k = k; this.dist = dist;
            }

            public int CompareTo(StateArray other) => dist.CompareTo(other.dist);
        }

        // Simple binary heap implementation for better priority queue performance
        public class MinHeap<T> where T : IComparable<T>
        {
            private List<T> heap = new List<T>();

            public bool IsEmpty() => heap.Count == 0;

            public void Insert(T item)
            {
                heap.Add(item);
                HeapifyUp(heap.Count - 1);
            }

            public T ExtractMin()
            {
                if (IsEmpty()) throw new InvalidOperationException();

                T min = heap[0];
                heap[0] = heap[heap.Count - 1];
                heap.RemoveAt(heap.Count - 1);

                if (!IsEmpty())
                    HeapifyDown(0);

                return min;
            }

            private void HeapifyUp(int index)
            {
                while (index > 0)
                {
                    int parent = (index - 1) / 2;
                    if (heap[index].CompareTo(heap[parent]) >= 0) break;

                    Swap(index, parent);
                    index = parent;
                }
            }

            private void HeapifyDown(int index)
            {
                while (true)
                {
                    int smallest = index;
                    int left = 2 * index + 1;
                    int right = 2 * index + 2;

                    if (left < heap.Count && heap[left].CompareTo(heap[smallest]) < 0)
                        smallest = left;

                    if (right < heap.Count && heap[right].CompareTo(heap[smallest]) < 0)
                        smallest = right;

                    if (smallest == index) break;

                    Swap(index, smallest);
                    index = smallest;
                }
            }

            private void Swap(int i, int j)
            {
                T temp = heap[i];
                heap[i] = heap[j];
                heap[j] = temp;
            }
        }
    }
}
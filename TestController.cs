using Recorder.MFCC;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Recorder
{
    struct user
    {
        public string userName;
        public List<Sequence> userTemplates;
    }
    static class TestController
    {
        public static List<user> extractFeatures(List<User> q)
        {
            List<user> res = new List<user>();
            foreach (var x in q)
            {
                user temp = new user();
                temp.userName = x.UserName;
                temp.userTemplates = new List<Sequence>();
                foreach (var template in x.UserTemplates)
                {
                    Sequence seq = AudioOperations.ExtractFeatures(template);
                    temp.userTemplates.Add(seq);
                }
                res.Add(temp);
            }
            return res;
        }
        //Training set , Testing set, W

        public static double matching(List<user> trainingSet, List<user> testingSet,int W = -1)
        {
            double sum = 0, cnt = 0;

            // Pre-compute training set size
            int trainingSetCount = trainingSet.Count;

            // Process each user in testing set
            for (int i = 0; i < testingSet.Count; i++)
            {
                var testUser = testingSet[i];
                int testTemplatesCount = testUser.userTemplates.Count;
                sum += testTemplatesCount;

                // Process each sequence in the current user's templates
                for (int k = 0; k < testTemplatesCount; k++)
                {
                    var sequence = testUser.userTemplates[k];
                    double minDistance = double.MaxValue;
                    string minUser = null;

                    // Compare with each user in training set
                    for (int j = 0; j < trainingSetCount; j++)
                    {
                        var trainUser = trainingSet[j];
                        int trainTemplatesCount = trainUser.userTemplates.Count;

                        // Compare with each template of current training user
                        for (int t = 0; t < trainTemplatesCount; t++)
                        {
                            var temp = trainUser.userTemplates[t];
                            double dis = 0;
                            if (W == -1)
                                dis = Algorithms.dynamicTimeWarping(sequence, temp);
                            else
                             dis = Algorithms.dynamicTimeWarpingWithPruning(sequence, temp, W);

                            // Update minimum if needed
                            if (dis < minDistance)
                            {
                                minDistance = dis;
                                minUser = trainUser.userName;
                            }
                        }
                    }

                    // Count correct matches
                    if (minUser == testUser.userName)
                        cnt++;
                }
            }

            // Calculate percentage
            return (cnt / sum) * 100.0;
        }

        
        public static  double syncMatching(List<user>trainingSet , List<user> testingSet)
        {    
            double sum=0, cnt = 0;
            for (int i = 0; i < testingSet.Count; i++)
            {
                sum+= testingSet[i].userTemplates.Count;
                foreach (var sequence in testingSet[i].userTemplates)
                {
                    double minDistance = double.MaxValue;
                    string minUser = null;
                    for (int j = 0; j < trainingSet.Count; j++)
                    {
                        var dis = bonus.syncDTW(sequence, trainingSet[i].userTemplates);
                        if(dis.dist < minDistance)
                        {
                            minDistance = dis.dist;
                            minUser = trainingSet[j].userName;
                        }
                    }
                    if (minUser == testingSet[i].userName)
                        cnt++;
                }
            }
            return (cnt/sum)*100.0f;
        }
    }
}

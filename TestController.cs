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

        public static  double matchingWithPruning(List<user>trainingSet , List<user> testingSet,int W)
        {
            double sum=0 , cnt = 0;
            for (int i = 0; i < testingSet.Count; i++)
            {
                sum += testingSet[i].userTemplates.Count;
                foreach (var sequence in testingSet[i].userTemplates)
                {
                    double minDistance = double.MaxValue;
                    string minUser = null;
                    for (int j = 0; j < trainingSet.Count; j++)
                    {
                        foreach (var temp in trainingSet[j].userTemplates)
                        {
                            double dis = Algorithms.dynamicTimeWarpingWithPruning(sequence, temp, W);
                            if (dis < minDistance)
                            {
                                minDistance = dis;
                                minUser = trainingSet[j].userName;
                            }
                        }
                    }
                    if (minUser == testingSet[i].userName)
                        cnt++;
                }
            }

            return (cnt / sum) * 100.0f;
        }
        //Training set , Testing set
        public static  double matching(List<user>trainingSet , List<user> testingSet)
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
                        foreach (var temp in trainingSet[j].userTemplates)
                        {
                            double dis = Algorithms.dynamicTimeWarping(sequence, temp);
                            if (dis < minDistance)
                            {
                                minDistance = dis;
                                minUser = trainingSet[j].userName;
                            }
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

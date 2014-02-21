using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using AI.Core.BeeColonySim;
using NUnit.Framework;

namespace TestProblems.TravelingSalesman
{
    [TestFixture]
    public class TSPTest
    {
        
        [Test]
        public void TestGeneric()
        {
            Debug.WriteLine("Loading cities data for SBC Traveling Salesman Problem analysis");
            CitiesData citiesData = new CitiesData(20);
            Debug.WriteLine(citiesData.ToString());
            Debug.WriteLine("Number of cities = " + citiesData.cities.Length);
            Debug.WriteLine("Number of possible paths = " +
              citiesData.NumberOfPossiblePaths().ToString("#,###"));
            Debug.WriteLine("Best possible solution (shortest path) length = " +
              citiesData.ShortestPathLength().ToString("F4"));


            var problemDef = new CitiesProblemDef(citiesData);

            int totalNumberBees = 100;
            int numberInactive = 20;
            int numberScout = 30;
            int maxNumberVisits = 100;
            int maxNumberCycles = 8000;

            GenericHive<Char[]>.random = new Random(0);
            CitiesProblemDef._random = GenericHive<Char[]>.random;

            var hive = new GenericHive<Char[]>(totalNumberBees,
                                 numberInactive,
                                 numberScout,
                                 maxNumberVisits,
                                 maxNumberCycles,
                                 problemDef.GenerateRandomMemoryMatrix,
                                 problemDef.GenerateNeighborMemoryMatrix,
                                 problemDef.MeasureOfQuality);

            Debug.WriteLine("\nInitial random hive");
            Debug.WriteLine(hive);
            Debug.WriteLine(new string(hive.BestSolution));

            bool doProgressBar = true;
            hive.Solve(doProgressBar);
            
            Debug.WriteLine("\nFinal hive");
            Debug.WriteLine(new string(hive.BestSolution));
            Debug.WriteLine(hive);
            Debug.WriteLine("End Simulated Bee Colony demo");

            Assert.AreEqual(citiesData.cities,hive.BestSolution);
        }
        
        [Test]
        public void TextTestGeneric()
        {
            var targetString = "THISISATESTSTRINGTHATISFAIRLYLONGTHISISATESTSTRINGTHATISFAIRLYLONG";
            
            Func<string> genRandom = () => RandomString(targetString.Length);
            Func<string, string> genNeighboor = (s) =>
            {
                var pos = random.Next(s.Length);
                var sArr = s.ToCharArray();
                sArr[pos] = RandomString(1)[0];
                return new string(sArr);
            };

            Func<string, double> evalString = s =>
            {
                return ComputeDistance(s, targetString);
            };

            int totalNumberBees = 100;
            int numberInactive = 20;
            int numberScout = 30;
            int maxNumberVisits = 100;
            int maxNumberCycles = 2000;

            var hive = new GenericHive<string>(totalNumberBees,
                numberInactive,
                numberScout,
                maxNumberVisits,
                maxNumberCycles,
                genRandom,
                genNeighboor,
                evalString);

            Debug.WriteLine("\nInitial random hive");
            Debug.WriteLine(hive);

            bool doProgressBar = true;
            hive.Solve(doProgressBar);

            Debug.WriteLine("\nFinal hive");
            Debug.WriteLine(hive);
            Debug.WriteLine("End Simulated Bee Colony demo");

            Assert.AreEqual(targetString,hive.BestSolution);

        }

        private static Random random = new Random((int)DateTime.Now.Ticks);//thanks to McAden
        private string RandomString(int size)
        {
            StringBuilder builder = new StringBuilder();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }

            return builder.ToString();
        }

        public static int ComputeDistance(string s, string t)
        {
            int m = t.Length;
            int score = 0;

            
            // Step 3
            for (int i = 0; i < m; i++)
            {
                if (s[i] != t[i]) score++;
            }
            // Step 7
            return score;
        }
    }
}

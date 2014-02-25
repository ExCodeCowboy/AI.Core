using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AI.Core.ParticleSwarm;
using NUnit.Framework;

namespace TestProblems.SimpleFormula
{
    [TestFixture]
    public class SimpleFormulaTest
    {
        [Test]
        public void ParticleSwarmSolver()
        {
            var swarm = new Swarm(100, 100, 2,
                x => 3.0 + (x[0]*x[0]) + (x[1]*x[1]), 
                new[] {-1000d, -1000d}, 
                new[] {1000d, 1000d});

            var solution = swarm.Solve();
            var bestfitness = swarm.BestFitness;

            Debug.WriteLine("\nProcessing complete");
            Debug.Write("Final best fitness = ");
            Debug.WriteLine(bestfitness.ToString("F4"));
            Debug.WriteLine("Best position/solution:");
            for (int i = 0; i < solution.Length; ++i){
              Debug.Write("x" + i + " = " );
              Debug.WriteLine(solution[i].ToString("F4") + " ");
            }
            Debug.WriteLine("");
            Debug.WriteLine("\nEnd PSO demonstration\n");

            Assert.AreEqual(0,Math.Round(solution[0],4));
            Assert.AreEqual(0,Math.Round(solution[1],4));
  
        }

    }
}

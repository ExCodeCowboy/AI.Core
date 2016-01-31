using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AI.Core.Solvers.SimulatedAnnealing
{
    public class Annealer<TSolutionType>
    {
        public static Random random = new Random(0);
        private int _maxIteration = int.MaxValue;
        
        private readonly Func<TSolutionType> _randomSolutionGenerator;
        private readonly Func<TSolutionType, TSolutionType> _neighborSolutionGenerator;
        private readonly Func<TSolutionType, double> _measureOfQuality;
        private readonly double _startTemp;
        private readonly double _alpha;

        public Annealer(Func<TSolutionType> randomSolutionGenerator, 
                        Func<TSolutionType, TSolutionType> neighborSolutionGenerator, 
                        Func<TSolutionType, double> measureOfQuality, 
                        double startTemp = 10000.0,
                        double alpha = 0.995)
        {
            _randomSolutionGenerator = randomSolutionGenerator;
            _neighborSolutionGenerator = neighborSolutionGenerator;
            _measureOfQuality = measureOfQuality;
            _startTemp = startTemp;
            _alpha = alpha;
        }

        public TSolutionType Solve()
        {
            // Set up problem data.
            // Create random state.
            TSolutionType state = _randomSolutionGenerator();
            double energy = _measureOfQuality(state);
            TSolutionType bestState = state;
            double bestEnergy = energy;
            TSolutionType adjState;
            double adjEnergy;

            // Set up SA variables for temperature and cooling rate.
            int iteration = 0;
            double currTemp = _startTemp;
            
            while (iteration < _maxIteration && currTemp > 0.0001)
            {
                // Create adjacent state.
                // Compute energy of adjacent state.
                // Check if adjacent state is new best.
                // If adjacent state better, accept state with varying probability.
                // Decrease temperature and increase iteration counter.
                adjState = _neighborSolutionGenerator(state);
                adjEnergy = _measureOfQuality(adjState);
                double p = random.NextDouble();
                if (adjEnergy < bestEnergy)
                {
                    bestState = adjState;
                    bestEnergy = adjEnergy;
                    Console.WriteLine("New best state found:");
                    Console.WriteLine(bestState.ToString());
                    Console.WriteLine("Energy = " + bestEnergy.ToString("F2") + "\n");
                }
                if (AcceptanceProb(energy, adjEnergy, currTemp) > p)
                {
                    
                    Console.WriteLine("Changing state change:");
                    Console.WriteLine(bestState.ToString());
                    Console.WriteLine("Energy = " + adjEnergy.ToString("F2") + "\n");
                    
                    state = adjState;
                    energy = adjEnergy;
                }
                currTemp = currTemp * _alpha;
                ++iteration;
            }

            Console.Write("Temperature has cooled to (almost) zero ");
            Console.WriteLine("at iteration " + iteration);
            Console.WriteLine("Simulated Annealing loop complete");
            Console.WriteLine("\nBest state found:");
            Console.WriteLine(bestState.ToString());
            //Display(bestState);
            Console.WriteLine("Best energy = " + bestEnergy.ToString("F2") + "\n");
            //Interpret(bestState, problemData);
            Console.WriteLine("\nEnd Simulated Annealing demo\n");
            return bestState;
        }

        static double AcceptanceProb(double energy, double adjEnergy, double currTemp)
        {
            if (currTemp > 1)
            {
                return Math.Min(Math.Exp((energy - adjEnergy)/currTemp),1.0);
            }
            return Math.Min(Math.Exp(energy - adjEnergy),1.0);
        }
    }
}

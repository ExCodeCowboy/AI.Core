using System;
using System.Diagnostics;

namespace AI.Core.Solvers.BeeColonySim
{
    public class GenericHive<TSolutionType> {

        public static Random random = new Random();

        private readonly int _totalNumberBees;
        private readonly int _numberInactive;
        private readonly int _maxNumberVisits;
        private readonly int _maxNumberCycles;
        private readonly Func<TSolutionType> _randomSolutionGenerator;
        private readonly Func<TSolutionType,TSolutionType> _neighborSolutionGenerator;
        private readonly Func<TSolutionType, double> _measureOfQuality;

        public double ProbPersuasion = 0.90;
        public double ProbMistake = 0.01;

        public GenericBee<TSolutionType>[] Bees;
        public TSolutionType BestSolution;
        public double BestMeasureOfQuality;
        public int[] IndexesOfInactiveBees;

        // Hive data fields here
        public override string ToString()
        {
            string s = "";
            s += "Best path found: ";
            s += BestSolution.ToString();

            s += "Path quality:    ";
            if (BestMeasureOfQuality < 10000.0)
                s += BestMeasureOfQuality.ToString("F4") + "\n";
            else
                s += BestMeasureOfQuality.ToString("#.####e+00");
            s += "\n";
            return s;
        }
    
        public GenericHive(int totalNumberBees, 
                    int numberInactive,
                    int numberScout, 
                    int maxNumberVisits,
                    int maxNumberCycles,
                    Func<TSolutionType> randomSolutionGenerator,
                    Func<TSolutionType,TSolutionType> neighborSolutionGenerator,
                    Func<TSolutionType,Double> measureOfQuality)
        {
            _totalNumberBees = totalNumberBees;
            _numberInactive = numberInactive;
            _maxNumberVisits = maxNumberVisits;
            _maxNumberCycles = maxNumberCycles;
            _randomSolutionGenerator = randomSolutionGenerator;
            _neighborSolutionGenerator = neighborSolutionGenerator;
            _measureOfQuality = measureOfQuality;

            Bees = new GenericBee<TSolutionType>[totalNumberBees];
            BestSolution = randomSolutionGenerator();
            BestMeasureOfQuality = 
            measureOfQuality(BestSolution);
 
            IndexesOfInactiveBees = new int[numberInactive];

            for (int i = 0; i < totalNumberBees; ++i)
            {
                BeeStatus currStatus;
                if (i < numberInactive)
                {
                    currStatus = BeeStatus.Idle; // inactive
                    IndexesOfInactiveBees[i] = i;
                }
                else if (i < numberInactive + numberScout)
                    currStatus = BeeStatus.Scout; // scout
                else
                    currStatus = BeeStatus.Active; // active

                TSolutionType randomMemoryMatrix = randomSolutionGenerator();
                double mq = _measureOfQuality(randomMemoryMatrix);
                int numberOfVisits = 0;

                var newBee = new GenericBee<TSolutionType>(currStatus,
                    randomMemoryMatrix, mq, numberOfVisits);

                if (newBee.MeasureOfQuality < BestMeasureOfQuality)
                {
                    BestSolution = newBee.CurrentSolution;
                    BestMeasureOfQuality = newBee.MeasureOfQuality;
                }
                Bees[i] = newBee;
            }
        }

        public void Solve(bool doProgressBar)
        {
            bool pb = doProgressBar;
            int numberOfSymbolsToPrint = 10;
            int increment = _maxNumberCycles / numberOfSymbolsToPrint;
            if (pb) Debug.WriteLine("\nEntering SBC Traveling Salesman Problem algorithm main processing loop\n");
            if (pb) Debug.WriteLine("Progress: |==========|");
            if (pb) Debug.Write("           ");
            int cycle = 0;

            while (cycle < _maxNumberCycles)
            {
                for (int i = 0; i < _totalNumberBees; ++i)
                {
                    if (Bees[i].Status == BeeStatus.Active)
                        ProcessActiveBee(i);
                    else if (Bees[i].Status == BeeStatus.Scout)
                        ProcessScoutBee(i);
                    else if (Bees[i].Status == BeeStatus.Idle)
                        ProcessInactiveBee(i);
                }
                ++cycle;

                if (pb && cycle % increment == 0)
                    Debug.Write("^");
            }

            if (pb) Debug.WriteLine("");
        }

        private void ProcessActiveBee(int i)
        {
            var activeBee = Bees[i];

            TSolutionType neighbor = _neighborSolutionGenerator(activeBee.CurrentSolution);
            double neighborQuality = _measureOfQuality(neighbor); 
            double prob = random.NextDouble();
            bool memoryWasUpdated = false;
            bool numberOfVisitsOverLimit = false;

            if (neighborQuality < activeBee.MeasureOfQuality)
            { // better
                if (prob < ProbMistake) { // mistake
                    ++activeBee.NumberOfVisits;
                    if (activeBee.NumberOfVisits > _maxNumberVisits)
                    numberOfVisitsOverLimit = true;
                }
                else { // No mistake
                    activeBee.CurrentSolution = neighbor;
                    activeBee.MeasureOfQuality = neighborQuality;
                    activeBee.NumberOfVisits = 0; 
                    memoryWasUpdated = true; 
                }
            }
            else { // Did not find better neighbor
                if (prob < ProbMistake) { // Mistake
                    activeBee.CurrentSolution = neighbor;
                    activeBee.MeasureOfQuality = neighborQuality;
                    activeBee.NumberOfVisits = 0;
                    memoryWasUpdated = true; 
                }
                else { // No mistake
                    ++activeBee.NumberOfVisits;
                    if (activeBee.NumberOfVisits > _maxNumberVisits)
                    numberOfVisitsOverLimit = true;
                }
            }
 
            if (numberOfVisitsOverLimit) 
            {
                activeBee.Status = BeeStatus.Idle;
                activeBee.NumberOfVisits = 0;
                int x = random.Next(_numberInactive); 
                Bees[IndexesOfInactiveBees[x]].Status = BeeStatus.Active; 
                IndexesOfInactiveBees[x] = i; 
            }
            else if (memoryWasUpdated) 
            {
                if (activeBee.MeasureOfQuality < BestMeasureOfQuality)
                {
                    BestSolution = activeBee.CurrentSolution;
                    BestMeasureOfQuality = activeBee.MeasureOfQuality;
                }
                DoWaggleDance(i);
            }
        }

        private void ProcessScoutBee(int i)
        {
            var scoutBee = Bees[i];
            TSolutionType randomFoodSource = _randomSolutionGenerator();
            double randomFoodSourceQuality = 
                _measureOfQuality(randomFoodSource);
            if (randomFoodSourceQuality < scoutBee.MeasureOfQuality)
            {
                scoutBee.CurrentSolution = randomFoodSource;
                scoutBee.MeasureOfQuality = randomFoodSourceQuality;
                if (scoutBee.MeasureOfQuality < BestMeasureOfQuality)
                {
                    BestSolution = scoutBee.CurrentSolution;
                    BestMeasureOfQuality = scoutBee.MeasureOfQuality;
                } 
                DoWaggleDance(i);
            }
        }

        private void ProcessInactiveBee(int i)
        {
            return;
        }

        private void DoWaggleDance(int i)
        {
            var waggleBee = Bees[i];

            for (int ii = 0; ii < _numberInactive; ++ii)
            {
                var targetBee = Bees[IndexesOfInactiveBees[ii]];
                if (waggleBee.MeasureOfQuality < targetBee.MeasureOfQuality)
                {
                    double p = random.NextDouble();
                    if (ProbPersuasion > p)
                    {
                        targetBee.CurrentSolution = waggleBee.CurrentSolution;
                        targetBee.MeasureOfQuality = waggleBee.MeasureOfQuality;
                    }
                }
            } 
        }
    }
}

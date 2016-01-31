using System;

namespace AI.Core.Solvers.ParticleSwarm
{
    public class Swarm
    {
        private Random _random = new Random(0);

        public int NumberIterations { get; private set; }

        private int _iteration = 0;

        private Particle[] _swarm;
        private double[] _bestGlobalPosition;
        private double _bestGlobalFitness = double.MaxValue;

        public double BestFitness
        {
            get { return _bestGlobalFitness; }
        }
        
        // Initialize all Particle objects
        private double w = 0.729; // inertia weight
        private double c1 = 1.49445; // cognitive weight
        private double c2 = 1.49445; // social weight
        private double r1, r2; // randomizations

        private readonly int _dimensions;
        private Func<double[], double> _fitnessFunction;
        private readonly double[] _min;
        private readonly double[] _max;

        public Swarm(int numberParticles,
            int numberIterations,
            int dimensions,
            Func<double[], double> fitnessFunction,
            double[] min,
            double[] max
            )
        {
            _dimensions = dimensions;
            _fitnessFunction = fitnessFunction;
            _min = min;
            _max = max;
            _swarm = new Particle[numberParticles];
            NumberIterations = numberIterations;
            _bestGlobalPosition = new double[dimensions];
            InitializeSwarm();
        }

        private void InitializeSwarm()
        {
            for (int i = 0; i < _swarm.Length; ++i)
            {
                double[] randomPosition = new double[_dimensions];
                for (int j = 0; j < randomPosition.Length; ++j)
                {
                    var lo = _min[j];
                    var hi = _max[j];
                    randomPosition[j] = (hi - lo)*_random.NextDouble() + lo;
                }

                double fitness = _fitnessFunction(randomPosition);
                double[] randomVelocity = new double[_dimensions];
                for (int j = 0; j < randomVelocity.Length; ++j)
                {
                    var minX = _min[j];
                    var maxX = _max[j];
                    double lo = -1.0*Math.Abs(maxX - minX);
                    double hi = Math.Abs(maxX - minX);
                    randomVelocity[j] = (hi - lo)*_random.NextDouble() + lo;
                }
                _swarm[i] = new Particle(randomPosition,
                    fitness,
                    randomVelocity,
                    randomPosition,
                    fitness);
                if (_swarm[i].fitness < _bestGlobalFitness)
                {
                    _bestGlobalFitness = _swarm[i].fitness;
                    _bestGlobalPosition = _swarm[i].position;
                }
            }
        }

        public double[] Solve()
        {
            for (int n = 0; n < NumberIterations; n++)
            {
                for (int i = 0; i < _swarm.Length; ++i)
                {
                    Particle currP = _swarm[i];
                    var newVelocity = new double[_dimensions];
                    for (int j = 0; j < currP.velocity.Length; ++j)
                    {
                        r1 = _random.NextDouble();
                        r2 = _random.NextDouble();

                        newVelocity[j] = (w*currP.velocity[j]) +
                                         (c1*r1*(currP.bestPosition[j] - currP.position[j])) +
                                         (c2*r2*(_bestGlobalPosition[j] - currP.position[j]));

                        if (newVelocity[j] < _min[j]/10)
                            newVelocity[j] = _min[j]/10;
                        else if (newVelocity[j] > _max[j]/10)
                            newVelocity[j] = _max[j]/10;
                    } // each j
                    currP.velocity = newVelocity;

                    var newPosition = new double[_dimensions];
                    for (int j = 0; j < currP.position.Length; ++j)
                    {
                        newPosition[j] = currP.position[j] + newVelocity[j];
                        if (newPosition[j] < _min[j])
                            newPosition[j] = _min[j];
                        else if (newPosition[j] > _max[j])
                            newPosition[j] = _max[j];
                    }
                    currP.position = newPosition;

                    var newFitness = _fitnessFunction(newPosition);
                    currP.fitness = newFitness;

                    if (newFitness < currP.bestFitness)
                    {
                        currP.bestPosition = currP.position;
                        currP.bestFitness = newFitness;
                    }
                    if (newFitness < _bestGlobalFitness)
                    {
                        _bestGlobalPosition = currP.position;
                        _bestGlobalFitness = newFitness;
                    }
                }
            }
            return _bestGlobalPosition;
        }
    }
}
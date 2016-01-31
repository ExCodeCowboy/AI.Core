namespace AI.Core.Solvers.BeeColonySim
{
    public class GenericBee<TSolutiontype>
    {
        public BeeStatus Status;
        public TSolutiontype CurrentSolution;
        public double MeasureOfQuality;
        public int NumberOfVisits;

        public GenericBee(BeeStatus status, 
                   TSolutiontype solution,
                   double measureOfQuality, 
                   int numberOfVisits)
        {
            Status = status;
            CurrentSolution = solution;
            MeasureOfQuality = measureOfQuality;
            NumberOfVisits = numberOfVisits;
        }

        public override string ToString()
        {
            string s = "";
            s += "Status = " + Status + "\n";
            s += " Solution = " + "\n";
            s += CurrentSolution.ToString();
            s += " Quality = " + MeasureOfQuality.ToString("F4");
            s += " Number visits = " + NumberOfVisits;
            return s;
        }
    }
}
namespace TestProblems.TravelingSalesman
{
    public class CitiesData
    {
        public char[] cities;

        public CitiesData(int numberCities)
        {
            this.cities = new char[numberCities];
            this.cities[0] = 'A';
            for (int i = 1; i < this.cities.Length; ++i)
                this.cities[i] = (char)(this.cities[i - 1] + 1);
        }

        public double Distance(char firstCity, char secondCity)
        {
            if (firstCity < secondCity)
                return 1.0 * ((int)secondCity - (int)firstCity);
            else
                return 1.5 * ((int)firstCity - (int)secondCity);
        }

        public double ShortestPathLength()
        {
            return 1.0 * (this.cities.Length - 1);
        }

        public long NumberOfPossiblePaths()
        {
            long n = this.cities.Length;
            long answer = 1;
            for (int i = 1; i <= n; ++i)
                checked { answer *= i; }
            return answer;
        }

        public override string ToString()
        {
            string s = "";
            s += "Cities: ";
            for (int i = 0; i < this.cities.Length; ++i)
                s += this.cities[i] + " ";
            return s;
        }
    }
}
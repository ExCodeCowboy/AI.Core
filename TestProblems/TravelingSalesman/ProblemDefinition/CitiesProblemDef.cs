using System;

namespace TestProblems.TravelingSalesman
{
    public class CitiesProblemDef
    {
        public static Random _random = new Random();
        private CitiesData _citiesData;

        public CitiesProblemDef(CitiesData citiesData)
        {
            _citiesData = citiesData;
        }

        public char[] GenerateRandomMemoryMatrix()
        {
            char[] result = new char[_citiesData.cities.Length];
            Array.Copy(_citiesData.cities, result,
                _citiesData.cities.Length);
            for (int i = 0; i < result.Length; i++)
            {
                int r = _random.Next(i, result.Length);
                char temp = result[r];
                result[r] = result[i];
                result[i] = temp;
            }
            return result;
        }

        public char[] GenerateNeighborMemoryMatrix(char[] memoryMatrix)
        {
            char[] result = new char[memoryMatrix.Length];
            Array.Copy(memoryMatrix, result, memoryMatrix.Length);

            int ranIndex = _random.Next(0, result.Length);
            int adjIndex = ranIndex==result.Length-1?0:ranIndex+1;

            char tmp = result[ranIndex];
            result[ranIndex] = result[adjIndex];
            result[adjIndex] = tmp;
            return result;
        }

        public double MeasureOfQuality(char[] memoryMatrix)
        {
            double answer = 0.0;
            for (int i = 0; i < memoryMatrix.Length - 1; ++i)
            {
                char c1 = memoryMatrix[i];
                char c2 = memoryMatrix[i + 1];
                double d = _citiesData.Distance(c1, c2);
                answer += d;
            }
            return answer;
        }

        
    }
}
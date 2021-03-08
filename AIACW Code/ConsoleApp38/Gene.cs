using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACW
{
    class Gene
    {
        //This class is a data storage class so is just used to hold the weights and fitness in an easily accessible manner
        private List<double> Weights;
        private double Fitness;

        public void ChangeWeights(List<double> i)
        {
            Weights = i;
        }

        public List<double> GetWeights()
        {
            return Weights;
        }

        public void ChangeFitness(double i)
        {
            Fitness = i;
        }

        public double GetFitness()
        {
            return Fitness;
        }
    }
}

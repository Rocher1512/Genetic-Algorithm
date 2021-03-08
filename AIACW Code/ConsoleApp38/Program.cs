using System;
using System.Collections.Generic;

namespace ACW
{

    class Program
    {
        public static Random _rand = new Random();

        static void Main(string[] args)
        {
            //creates a new population class and runs it
            Population pop = new Population(500);
            pop.Start();
            Console.ReadLine();
        }

        public static double GetResults(List<double> weights)
        {
            Network net = new Network();

            net.SetWeights(weights);

            PendulumMaths p = new PendulumMaths();
            p.initialise(1);

            Network v = new Network();
            v.SetWeights(net.GetWeights());

            double[][] motor_vals = new double[p.getcrabnum()][];

            for (int i = 0; i < motor_vals.Length; i++)
            {
                motor_vals[i] = new double[2];
            }

            do
            {
                double[][] sval = (p.getSensorValues());

                double[] inputs = new double[10];

                for (int i = 0; i < p.getcrabnum(); i++)
                {

                    for (int x = 0; x < sval[0].Length; x++)
                    {
                        inputs[x] = ((sval[i][x]) / (127) * (1 - 0)) + 1;
                    }

                    v.SetInputs(inputs);

                    v.Execute();

                    double[] outputs = v.GetOutputs();

                    motor_vals[i][0] = ((outputs[0])) * 127;
                    motor_vals[i][1] = ((outputs[1])) * 127;

                }

            }
            while (p.performOneStep(motor_vals) == 1);

            return p.getFitness();
        }
    }
    
}
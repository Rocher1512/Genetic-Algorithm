using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace ACW
{
    class Population
    {
        private static Random _rand = new Random();
        private int populationsize = 500;
        private int mutationchance = 5;
        private int crossoverchance = 100;
        private int tournamentsize = 6;
        private List<Gene> populationpool;
        private List<Gene> Proportionalpopulationpool;
        private List<double> textwights;
        private  Gene bestG;
        private bool running = true;
        private int gennumber = 0;
        int programtime = 60; //The time in seconds that the program will run

        string path = @"./Best.txt"; // the file path of where the best weights are saved 

        //The constructor to allow easy changing of the population size
        public Population(int popsize)
        {
            populationsize = popsize;
        }
        public void Start()
        {
            //Create populationsize of new random genes
            populationpool = new List<Gene>();
            for (int i = 0; i < populationsize; i++)
            {
                Gene gene = new Gene();
                List<double> weights = new List<double>();
                for(int j = 0; j < 60; j++)
                {
                    weights.Add((_rand.NextDouble() * 2) - 1);
                }
                populationpool.Add(gene);
                populationpool[i].ChangeWeights(weights);
            }
            //setting up a stopwatch so i can exit the program once it reaches a correct value
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            while (running == true)
            {
                gennumber++;
                List<double> avglist = new List<double>();
                double avg = 0;
                double bestfit = 0;
                // Run through the population calculate and asign the fitness of each gene
                // This section also sets up the calculation for the best and average of the generation
                foreach (Gene G in populationpool)
                {
                    double fitness;
                    List<double> weights = new List<double>();
                    weights = G.GetWeights();
                    fitness = Program.GetResults(weights);
                    G.ChangeFitness(fitness);
                    if(bestfit < fitness)
                    {
                        bestG = G;
                        bestfit = fitness;
                    }
                    avglist.Add(fitness);
                }
                bestfit = 0;
                // changing the list to an int
                foreach (double d in avglist)
                {
                    avg += d;
                }
                TimeSpan ts = stopWatch.Elapsed;
                double fits = bestG.GetFitness();
                int x = (int)ts.TotalSeconds;
                //checks if the exit condition has been satisfied
                if (x > programtime)
                {
                    textwights = bestG.GetWeights();
                    running = false;
                }
                
                //Prints the curent results to console 
                Console.Write("Best: " + fits.ToString());
                Console.Write("     Average: " + avg / avglist.Count);
                Console.Write("     Generation: " + gennumber);
                Console.WriteLine("");

                // Create a matting pool of the genes proportional to there fitness
                Proportionalpopulationpool = new List<Gene>();
                Addingaweightedamountoftimes();

                // Use fitness selection
                populationpool = new List<Gene>();
                for (int i = 0; i < populationsize; i++)
                {

                    Gene parent1 = new Gene();
                    Gene parent2 = new Gene();
                    // Tournament selection FIGHTERSSSSS READY
                    Tournament(ref parent1, ref parent2);

                    // split the 2 parents and add them together
                    List<double> weights1 = new List<double>();
                    List<double> weights2 = new List<double>();
                    List<double> kidsweights = new List<double>();
                    Splitandadd(parent1, parent2, out weights1, out weights2, kidsweights);

                    // mutation chance of each weight to change and become diffrent 
                    int chance = _rand.Next(0, 100);
                    if (chance < mutationchance)
                    {
                        int tochange = _rand.Next(0, 60);
                        kidsweights[tochange] = ((_rand.NextDouble() * 2) - 1);
                    }

                    // add and repeat untill new population = population size
                    Gene gene = new Gene();
                    int crossover = _rand.Next(0,100);
                    if(crossover < crossoverchance)
                    {
                        gene.ChangeWeights(kidsweights);
                    }
                    else
                    {
                        gene = parent1;
                    }

                    populationpool.Add(gene);
                }
            }
            // creating a textfile of the best weights 
            using (StreamWriter sw = File.CreateText(path))
            {
                foreach (double num in textwights)
                {
                    sw.WriteLine(num);
                }
            }
            bestG.ChangeWeights(textwights);
        }

        //This takes the perants and adds them together 50% from one and 50% from the other
        private void Splitandadd(Gene parent1, Gene parent2, out List<double> weights1, out List<double> weights2, List<double> kidsweights)
        {
            int num = _rand.Next(0, 2);
            if (num == 1)
            {
                weights1 = parent1.GetWeights();
                weights2 = parent2.GetWeights();
            }
            else
            {
                weights2 = parent1.GetWeights();
                weights1 = parent2.GetWeights();
            }

            for (int j = 0; j < 30; j++)
            {
                kidsweights.Add(weights1[j]);
            }
            for (int j = 30; j < 60; j++)
            {
                kidsweights.Add(weights2[j]);
            }
        }

        //Tournement selection where the best genes out of a set size are selected 
        private void Tournament(ref Gene parent1, ref Gene parent2)
        {
            double parent1fit = 0;
            double parent2fit = 0;
            List<Gene> tournament = new List<Gene>();
            for (int j = 0; j < tournamentsize; j++)
            {
                int num1 = _rand.Next(0, Proportionalpopulationpool.Count);
                tournament.Add(Proportionalpopulationpool[num1]);
            }
            for (int j = 0; j < tournament.Count; j++)
            {
                double fitness;

                fitness = tournament[j].GetFitness();

                if (fitness > parent1fit)
                {
                    if (parent1fit > parent2fit && parent1 != null)
                    {
                        parent2 = parent1;
                        parent2fit = parent1fit;
                    }
                    parent1 = tournament[j];
                    parent1fit = fitness;

                }
                else if (fitness > parent2fit)
                {
                    parent2 = tournament[j];
                    parent2fit = fitness;
                }
            }
        }
        //adds the genes to the population a proportional amount to there fitness
        private void Addingaweightedamountoftimes()
        {
            foreach (Gene G in populationpool)
            {
                int amountadd;
                double doubleamountadd;
                doubleamountadd = Math.Truncate(G.GetFitness() * 1000);
                amountadd = int.Parse(doubleamountadd.ToString());
                if (amountadd >= 1)
                {
                    Proportionalpopulationpool = Adding(G, Proportionalpopulationpool, amountadd);
                }
            }
        }
        public Gene Bestgene()
        {
            return bestG;
        }
        //allows me to easily add and remove proportional adding 
        public List<Gene> Adding(Gene g, List<Gene> genepool, int amount)
        {
            for(int i = 0; i < amount; i++)
            {
                genepool.Add(g);
            }
            return genepool;
        }
    }
}

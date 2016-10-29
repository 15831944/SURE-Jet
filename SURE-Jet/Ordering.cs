using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GAF;
using GAF.Operators;
using GAF.Extensions;
using System.Windows.Forms;

namespace SURE_Jet
{
    public class Ordering
    {
        Cut[] bestOrder;
        double cost;

        public double getCost(Cut[] cuts)
        {
            cost = 0;
            for (int i = 1; i < cuts.Length; i++)
            {
                cost += cuts[i - 1].p2.getDistance(cuts[i].p1);
            }
            return cost;
        }
        public Cut[] getBestOrder(Cut[] cuts)
        {
            const double crossoverProbability = 0.65;
            const double mutationProbability = 0.08;
            const int elitismPercentage = 5;
            bestOrder = new Cut[cuts.Length];
            var population = new Population();

            //create the chromosomes
            for (var p = 0; p < 100; p++)
            {

                var chromosome = new Chromosome();
                foreach (var cut in cuts)
                {
                    chromosome.Genes.Add(new Gene(cut));
                }

                //chromosome.Genes.ShuffleFast;
                population.Solutions.Add(chromosome);
            }
            //create the elite operator
            var elite = new Elite(5);

            //create the crossover operator
            var crossover = new Crossover(0.8)
            {
                CrossoverType = CrossoverType.DoublePointOrdered
            };

            //create the mutation operator
            var mutate = new SwapMutate(0.1);

            //create the GA
            var ga = new GeneticAlgorithm(population, CalculateFitness);

            //hook up to some useful events
            ga.OnGenerationComplete += ga_OnGenerationComplete;
            ga.OnRunComplete += ga_OnRunComplete;

            //add the operators
            ga.Operators.Add(elite);
            ga.Operators.Add(crossover);
            ga.Operators.Add(mutate);

            //run the GA
            ga.Run(Terminate);
            return bestOrder;
        }
        public void ga_OnRunComplete(object sender, GaEventArgs e)
        {
            var fittest = e.Population.GetTop(1)[0];
            int i = 0;
            foreach (var gene in fittest.Genes)
            {
                i++;
                //Console.WriteLine(((Cut)gene.ObjectValue).Name);
            }
            //bestOrder = new Cut[i + 1];
            int j = 0;
            foreach (var gene in fittest.Genes)
            {
                bestOrder[j] = (Cut)gene.ObjectValue;
                j++;
            }
        }

        private static void ga_OnGenerationComplete(object sender, GaEventArgs e)
        {


        }

        public static double CalculateFitness(Chromosome chromosome)
        {
            var distanceToTravel = CalculateDistance(chromosome);
            return -distanceToTravel;
        }

        private static double CalculateDistance(Chromosome chromosome)
        {
            var distanceToTravel = 0.0;
            Cut previousCut = null;
            //run through each city in the order specified in the chromosome
            foreach (var gene in chromosome.Genes)
            {
                var currentCut = (Cut)gene.ObjectValue;

                if (previousCut != null)
                {
                    var distance = previousCut.p2.getDistance(currentCut.p1);

                    distanceToTravel += distance;
                }

                previousCut = currentCut;
            }

            return distanceToTravel;
        }

        public static bool Terminate(Population population,
            int currentGeneration, long currentEvaluation)
        {
            return currentGeneration > 100;
        }
    }
}

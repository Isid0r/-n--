using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.ComponentModel;

namespace WindowsFormsApp3
{
    static class Rnd
    {
        private static Random rnd = new Random();
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rnd.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }

    public class gene
    {
        public List<int> solution = new List<int>();
        public int fitness;
        public float likelihood;

        public static bool operator ==(gene g1, gene g2)
        {
            for (int i = 0; i < g1.solution.Count; i++)
            {
                if (g1.solution[i] != g2.solution[i])
                    return false;
            }
            return true;
        }

        public static bool operator !=(gene g1, gene g2)
        {
            for (int i = 0; i < g1.solution.Count; i++)
            {
                if (g1.solution[i] != g2.solution[i])
                    return true;
            }
            return false;
        }
    }

    public class GA
    {
        private const int QUANTITY = 75; // Количество особей
        private int size; // Размер поля
        public int countGenerations;
        const int LIMIT = 1000000;

        public List<List<List<int>>> allPop = new List<List<List<int>>>();
        private List<gene> population = new List<gene>();

        private Random rand = new Random();

        public gene GetGene(int i)
        {
            return population[i];
        }

        public GA(int size)
        {
            this.size = size;
        }

        private int Fitness(gene individ) // Вычисляет коэфф. выживаемости для одного гена/особи
        {
            int counter = 0;

            for (int i = 0; i < size - 1; i++)
            {
                for (int j = i + 1; j < size; j++)
                {
                    if (j - i == Math.Abs(individ.solution[j] - individ.solution[i])) counter++;
                }
            }
            return individ.fitness = counter;
        }

        private int CreateFitnesses()   // Заполняет коэфф. выживаемости для всех генов
        {
            int fitness = 0;

            for (int i = 0; i < QUANTITY; i++)
            {
                fitness = Fitness(population[i]);
                population[i].fitness = fitness;
                if (fitness == 0)
                {
                    return i;
                }
            }
            return -1;
        }

        private float MultInv()
        {
            float sum = 0;
            for (int i = 0; i < QUANTITY; i++)
            {
                sum += 1 / ((float)population[i].fitness);
            }
            return sum;
        }

        private void GenerateLikelihoods() //вероятности быть выбранными в качестве родителя
        {
            float multinv = MultInv();
            float last = 0;

            for (int i = 0; i < QUANTITY; i++)
            {
                population[i].likelihood = last
                = last + ((1 / ((float)population[i].fitness) / multinv) * 100);
            }

        }

        private void CreateNewPopulation()
        {
            List<gene> temppop = new List<gene>();
            for (int i = 0; i < QUANTITY; i++)
            {
                int parent1 = 0, parent2 = 0, iterations = 0;
                while ((parent1 == parent2) || (population[parent1] == population[parent2]))
                {
                    parent1 = GetIndex(rand.Next(size));
                    parent2 = GetIndex(rand.Next(size));
                    if (++iterations > (QUANTITY * QUANTITY)) break;
                }

                temppop.Add(new gene());
                temppop[i] = Breed(parent1, parent2);
            }

            for (int i = 0; i < QUANTITY; i++)
            {
                population[i] = temppop[i];
            }
        }

        private void gemmation()
        {
            int itemp1 = 0;
            int itemp2 = 0;
            for (int i = 0; i < QUANTITY; i++)
            {
                if (rand.Next(101) < 50)
                {
                    itemp1 = rand.Next(size);
                    itemp2 = rand.Next(size);
                    if (itemp1 != itemp2)
                    {
                        int tempo = population[i].solution[itemp1];
                        population[i].solution[itemp1] = population[i].solution[itemp2];
                        population[i].solution[itemp2] = tempo;
                    }
                    else
                    {
                        population[i].solution.Shuffle();
                    }
                }
                if (rand.Next(101) < 5) //мутация
                {
                    population[i] = mutation(population[i]);
                }
            }
        }

        private int GetIndex(float val)
        {
            float last = 0;

            for (int i = 0; i < QUANTITY; i++)
            {
                if (last <= val && val <= population[i].likelihood)
                    return i;
                else last = population[i].likelihood;
            }

            return 4;
        }

        private gene Breed(int p1, int p2)           // Создаем новую особь на основе двух существующих
        {
            gene child = new gene();

            for (int i = 0; i < size; i++)
            {
                child.solution.Add(-1);
            }

            for (int i = 0; i < size; i++)
            {
                if (population[p1].solution[i] == population[p2].solution[i])
                    child.solution[i] = population[p1].solution[i];
            }

            int temp = 0;
            for (int i = 0; i < size; i++)
            {
                if (child.solution[i] == -1)
                {
                    do
                        temp = rand.Next(size);
                    while (child.solution.Contains(temp));
                    child.solution[i] = temp;
                }
            }

            if (rand.Next(101) < 5) //мутация
            {
                child = mutation(child);
            }

            return child;
        }

        private gene mutation(gene child)
        {
            int itemp1 = 0;
            int itemp2 = 0;
            while (itemp1 == itemp2)
            {
                itemp1 = rand.Next(size);
                itemp2 = rand.Next(size);
            }
            int tempo = child.solution[itemp1];
            child.solution[itemp1] = child.solution[itemp2];
            child.solution[itemp2] = tempo;
            return child;
        }

        public int Solve()
        {
            int fitness;

            for (int i = 0; i < QUANTITY; i++) // Создание изначальных особей 
            {
                population.Add(new gene());
                for (int j = 0; j < size; j++)
                {
                    population[i].solution.Add(j);
                }
                population[i].solution.Shuffle();
            } // Конец создания 
            GenerateLikelihoods();
            fitness = CreateFitnesses();
            allPop.Add(new List<List<int>>());
            for (int i = 0; i < QUANTITY; i++)
            {
                allPop[0].Add(population[i].solution);
            }
            
            if (fitness != -1)
            {
                countGenerations = 0;
                return fitness;
            }

            countGenerations = 1;
            while (true)
            {
                if (countGenerations == LIMIT) return -1;
                GenerateLikelihoods();
                CreateNewPopulation();
                //gemmation();
                fitness = CreateFitnesses();
                allPop.Add(new List<List<int>>());
                for (int i = 0; i < QUANTITY; i++)
                {
                    allPop[countGenerations].Add(population[i].solution);
                }

                if (fitness != -1)
                {
                    countGenerations++;
                    return fitness;
                }
                countGenerations++;
            }
        }

    }

    public class Solution
    {
        public GA lol;
        private int size;
        int ans;
        
        public Solution(int size)
        {
            lol = new GA(size);
            this.size = size;
        }
        public int[] Solve()
        {
            lol = new GA(size);
            string s = "";
            ans = lol.Solve();
            if (ans == -1)
            {
                int[] l = new int[0];
                return l;
            }
            else
            {

                gene answ = lol.GetGene(ans);
                int[] k = new int[size + 1];
                for (int i = 0; i < size; i++)
                {
                    k[i] = answ.solution[i];
                }
                k[size] = lol.allPop.Count;
                return k;
            }
        }

        public int[] GiveGenPop(int gen, int pop)
        {
            int[] k = new int[size];
            for (int i = 0; i < size; i++)
            {
                k[i] = lol.allPop[gen][pop][i];
            }
            return k;
        }
    }

    static class Program
    {
        [STAThread]

        static void Main()
        {
            Console.WriteLine();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}

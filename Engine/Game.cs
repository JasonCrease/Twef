using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class Game
    {
        private int[,] g;
        public int[,] G { get { return g; } }
        private Random rand = new Random();

        public Game()
        {
            g = new int[4, 4];
            AddRandomNumber();
            AddRandomNumber();
        }

        public override string ToString()
        {
            string retStr = "";
            for (int i = 0; i < 4; i++)
            {
                retStr += "|";
                for (int j = 0; j < 4; j++)
                {
                    if (g[i, j] == 0)
                        retStr += "    ";
                    else retStr += g[i, j];
                    retStr += "|";
                }
                retStr += "\n";
            }

            return retStr;
        }

        private void AddRandomNumber()
        {
            int numToAdd = rand.NextDouble() < 0.9 ? 2 : 4;
            int emptyCells = CountEmptyCells();
            int randomCell = rand.Next(0, emptyCells);
            int soFar = 0;

            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                {
                    if(soFar == randomCell)
                    {
                        g[i, j] = numToAdd;
                        goto done;
                    }
                    if (g[i, j] == 0) soFar++;
                }

            done: ;
        }

        private int CountEmptyCells()
        {
            int count = 0;
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    if (g[i, j] == 0) count++;

            return count;
        }

    }
}

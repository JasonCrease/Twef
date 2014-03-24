using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public enum Direction { Up, Down, Left, Right };

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

        public void Move(Direction direction)
        {
            if (direction == Direction.Down)
            {
                for (int i = 3; i > 0; i--)
                    for (int j = 0; j < 4; j++)
                        if (g[i, j] == g[i + 1, j])
                        {
                            g[i + 1, j] *= 2;
                            g[i, j] = 0;
                        }
            }
            else if (direction == Direction.Up)
            {
                for (int i = 1; i < 4; i++)
                    for (int j = 0; j < 4; j++)
                        if (g[i, j] == g[i - 1, j])
                        {
                            g[i - 1, j] *= 2;
                            g[i, j] = 0;
                        }
            }
        }

        public override string ToString()
        {
            string retStr = "-------------------------\n";
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (g[i, j] == 0)
                        retStr += "|     ";
                    else retStr += String.Format("|{0,4} ", g[i, j]);
                }
                retStr += "|\n-------------------------\n";
            }

            return retStr;
        }

        public void AddRandomNumber()
        {
            int numToAdd = rand.NextDouble() < 0.9 ? 2 : 4;
            int emptyCells = CountEmptyCells();
            int randomCell = rand.Next(0, emptyCells);
            int soFar = 0;

            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                {
                    if (soFar == randomCell && g[i, j] == 0)
                    {
                        g[i, j] = numToAdd;
                        goto done;
                    }
                    else if (g[i, j] == 0) soFar++;
                }

            throw new ApplicationException();

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

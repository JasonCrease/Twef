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

        public bool AreMovesAvailable()
        {
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    if (g[i, j] == 0) return true;

            for (int i = 1; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    if (g[i, j] == g[i - 1, j]) return true;

            for (int i = 0; i < 4; i++)
                for (int j = 1; j < 4; j++)
                    if (g[i, j] == g[i, j - 1]) return true;

            return false;
        }

        public void Move(Direction direction)
        {
            if (direction == Direction.Up)
            {
                for (int col = 0; col < 4; col++)
                {
                    bool previousWasMerged = false;

                    for (int row = 1; row < 4; row++)
                    {
                        if (g[row, col] == g[row - 1, col] && !previousWasMerged)
                        {
                            // Merge
                            g[row, col] = 0;
                            g[row - 1, col] *= 2;
                            m_Score += g[row - 1, col];
                            previousWasMerged = true;
                        }
                        else if (g[row, col] != 0)
                        {
                            // Slide up
                            int i = row - 1;
                            while (i >= 0 && g[i, col] == 0)
                                i--;
                            i++;
                            if (i != row)
                            {
                                g[i, col] = g[row, col];
                                g[row, col] = 0;
                            }

                            if (!previousWasMerged)
                            {
                                if (i> 0 && g[i, col] == g[i - 1, col])
                                {
                                    g[i, col] = 0;
                                    g[i - 1, col] *= 2;
                                    m_Score += g[i - 1, col];
                                    previousWasMerged = true;
                                }
                            }
                            else
                            {
                                previousWasMerged = false;
                            }
                        }
                    }
                }
            }
            else if (direction == Direction.Down)
            {
                for (int col = 0; col < 4; col++)
                {
                    bool previousWasMerged = false;

                    for (int row = 2; row >= 0; row--)
                    {
                        if (g[row, col] == g[row + 1, col] && !previousWasMerged)
                        {
                            // Merge
                            g[row, col] = 0;
                            g[row + 1, col] *= 2;
                            m_Score += g[row + 1, col];
                            previousWasMerged = true;
                        }
                        else if (g[row, col] != 0)
                        {
                            //slide down
                            int i = row + 1;
                            while (i < 4 && g[i, col] == 0)
                                i++;
                            i--;
                            if (i != row)
                            {
                                g[i, col] = g[row, col];
                                g[row, col] = 0;
                            }

                            if (!previousWasMerged)
                            {
                                if (i < 3 && g[i, col] == g[i + 1, col])
                                {
                                    g[i, col] = 0;
                                    g[i + 1, col] *= 2;
                                    m_Score += g[i + 1, col];
                                    previousWasMerged = true;
                                }
                            }
                            else
                            {
                                previousWasMerged = false;
                            }
                        }
                    }
                }
            }
            if (direction == Direction.Left)
            {
                for (int row = 0; row < 4; row++)
                {
                    bool previousWasMerged = false;

                    for (int col = 1; col < 4; col++)
                    {
                        if (g[row, col] == g[row, col - 1] && !previousWasMerged)
                        {
                            // Merge
                            g[row, col] = 0;
                            g[row, col - 1] *= 2;
                            m_Score += g[row, col - 1];
                            previousWasMerged = true;
                        }
                        else if (g[row, col] != 0)
                        {
                            // Slide up
                            int i = col - 1;
                            while (i >= 0 && g[row, i] == 0)
                                i--;
                            i++;
                            if (i != col)
                            {
                                g[row, i] = g[row, col];
                                g[row, col] = 0;
                            }

                            if (!previousWasMerged)
                            {
                                if (i > 0  && g[row, i] == g[row, i - 1])
                                {
                                    g[row, i] = 0;
                                    g[row, i - 1] *= 2;
                                    m_Score += g[row, i - 1];
                                    previousWasMerged = true;
                                }
                            }
                            else
                            {
                                previousWasMerged = false;
                            }
                        }
                    }
                }
            }
            else if (direction == Direction.Right)
            {
                for (int row = 0; row < 4; row++)
                {
                    bool previousWasMerged = false;

                    for (int col = 2; col >= 0; col--)
                    {
                        if (g[row, col] == g[row, col + 1] && !previousWasMerged)
                        {
                            // Merge
                            g[row, col] = 0;
                            g[row, col + 1] *= 2;
                            m_Score += g[row, col + 1];
                            previousWasMerged = true;
                        }
                        else if (g[row, col] != 0)
                        {
                            //slide down
                            int i = col + 1;
                            while (i < 4 && g[row, i] == 0)
                                i++;
                            i--;
                            if (i != col)
                            {
                                g[row, i] = g[row, col];
                                g[row, col] = 0;
                            }

                            if (!previousWasMerged)
                            {
                                if (i < 3 && g[row, i] == g[row, i + 1])
                                {
                                    g[row, i] = 0;
                                    g[row, i + 1] *= 2;
                                    m_Score += g[row, i + 1];
                                    previousWasMerged = true;
                                }
                            }
                            else
                            {
                                previousWasMerged = false;
                            }
                        }
                    }
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


        private int m_Score;

        public int Score
        {
            get { return m_Score; }
        }
    }
}

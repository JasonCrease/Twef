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
        private readonly int[,] x = new int[4, 4];
        private int m_Score;
        private List<Tuple<int[,], int>> m_PreviousStates = new List<Tuple<int[,], int>>();

        public int[,] X { get { return x; } }
        public int Score { get { return m_Score; } }
        private static Random Rand = new Random();


        public Game()
        {
            AddRandomNumber();
            AddRandomNumber();
        }

        public Game(Game otherGame)
        {
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    x[i, j] = otherGame.X[i, j];
            m_PreviousStates = otherGame.m_PreviousStates;
            m_Score = otherGame.m_Score;
        }

        public int HeuristicValue
        {
            get
            {
                int emptyTiles = 0;
                int increasingness = 0;

                for (int i = 0; i < 4; i++)
                    for (int j = 0; j < 4; j++)
                        if (x[i, j] == 0) emptyTiles++;

                for (int j = 0; j < 1; j++)
                    for (int i = 1; i < 4; i++)
                        increasingness += (x[i - 1, j] > x[i, j]) ? 1 : 0;

                for (int i = 0; i < 1; i++)
                    for (int j = 1; j < 4; j++)
                        increasingness += (x[i, j - 1] > x[i, j]) ? 1 : 0;

                //increasingness += x[0, 0];

                return (emptyTiles + 1) * m_Score * increasingness;
            }
        }

        public bool AreMovesAvailable()
        {
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    if (x[i, j] == 0) return true;

            for (int i = 1; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    if (x[i, j] == x[i - 1, j]) return true;

            for (int i = 0; i < 4; i++)
                for (int j = 1; j < 4; j++)
                    if (x[i, j] == x[i, j - 1]) return true;

            return false;
        }

        public bool Move(Direction direction)
        {
            bool moveWorked = false;
            int[,] oldG = new int[4,4];
            
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    oldG[i, j] = x[i, j];
            m_PreviousStates.Add(new Tuple<int[,], int>(oldG, Score));

            if (direction == Direction.Up)
            {
                for (int col = 0; col < 4; col++)
                {
                    bool previousWasMerged = false;

                    for (int row = 1; row < 4; row++)
                    {
                        if (x[row, col] == x[row - 1, col] && !previousWasMerged)
                        {
                            // Merge
                            x[row, col] = 0;
                            x[row - 1, col] *= 2;
                            m_Score += x[row - 1, col];
                            previousWasMerged = true;
                            moveWorked = true;
                        }
                        else if (x[row, col] != 0)
                        {
                            // Slide up
                            int i = row - 1;
                            while (i >= 0 && x[i, col] == 0)
                                i--;
                            i++;
                            if (i != row)
                            {
                                x[i, col] = x[row, col];
                                x[row, col] = 0;
                                moveWorked = true;
                            }

                            if (!previousWasMerged)
                            {
                                if (i> 0 && x[i, col] == x[i - 1, col])
                                {
                                    x[i, col] = 0;
                                    x[i - 1, col] *= 2;
                                    m_Score += x[i - 1, col];
                                    previousWasMerged = true;
                                    moveWorked = true;
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
                        if (x[row, col] == x[row + 1, col] && !previousWasMerged)
                        {
                            // Merge
                            x[row, col] = 0;
                            x[row + 1, col] *= 2;
                            m_Score += x[row + 1, col];
                            previousWasMerged = true;
                            moveWorked = true;
                        }
                        else if (x[row, col] != 0)
                        {
                            //slide down
                            int i = row + 1;
                            while (i < 4 && x[i, col] == 0)
                                i++;
                            i--;
                            if (i != row)
                            {
                                x[i, col] = x[row, col];
                                x[row, col] = 0;
                                moveWorked = true;
                            }

                            if (!previousWasMerged)
                            {
                                if (i < 3 && x[i, col] == x[i + 1, col])
                                {
                                    x[i, col] = 0;
                                    x[i + 1, col] *= 2;
                                    m_Score += x[i + 1, col];
                                    previousWasMerged = true;
                                    moveWorked = true;
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
                        if (x[row, col] == x[row, col - 1] && !previousWasMerged)
                        {
                            // Merge
                            x[row, col] = 0;
                            x[row, col - 1] *= 2;
                            m_Score += x[row, col - 1];
                            previousWasMerged = true;
                            moveWorked = true;
                        }
                        else if (x[row, col] != 0)
                        {
                            // Slide up
                            int i = col - 1;
                            while (i >= 0 && x[row, i] == 0)
                                i--;
                            i++;
                            if (i != col)
                            {
                                x[row, i] = x[row, col];
                                x[row, col] = 0;
                                moveWorked = true;
                            }

                            if (!previousWasMerged)
                            {
                                if (i > 0  && x[row, i] == x[row, i - 1])
                                {
                                    x[row, i] = 0;
                                    x[row, i - 1] *= 2;
                                    m_Score += x[row, i - 1];
                                    previousWasMerged = true;
                                    moveWorked = true;
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
                        if (x[row, col] == x[row, col + 1] && !previousWasMerged)
                        {
                            // Merge
                            x[row, col] = 0;
                            x[row, col + 1] *= 2;
                            m_Score += x[row, col + 1];
                            previousWasMerged = true;
                            moveWorked = true;
                        }
                        else if (x[row, col] != 0)
                        {
                            //slide right
                            int i = col + 1;
                            while (i < 4 && x[row, i] == 0)
                                i++;
                            i--;
                            if (i != col)
                            {
                                x[row, i] = x[row, col];
                                x[row, col] = 0;
                                moveWorked = true;
                            }

                            if (!previousWasMerged)
                            {
                                if (i < 3 && x[row, i] == x[row, i + 1])
                                {
                                    x[row, i] = 0;
                                    x[row, i + 1] *= 2;
                                    m_Score += x[row, i + 1];
                                    previousWasMerged = true;
                                    moveWorked = true;
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

            return moveWorked;
        }

        public override string ToString()
        {
            string retStr = "-------------------------\n";
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (x[i, j] == 0)
                        retStr += "|     ";
                    else retStr += String.Format("|{0,4} ", x[i, j]);
                }
                retStr += "|\n-------------------------\n";
            }

            return retStr;
        }

        public void AddRandomNumber()
        {
            int numToAdd = Rand.NextDouble() < 0.9 ? 2 : 4;
            int emptyCells = CountEmptyCells();
            int randomCell = Rand.Next(0, emptyCells);
            int soFar = 0;

            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                {
                    if (soFar == randomCell && x[i, j] == 0)
                    {
                        x[i, j] = numToAdd;
                        goto done;
                    }
                    else if (x[i, j] == 0) soFar++;
                }

            done: ;
        }

        private int CountEmptyCells()
        {
            int count = 0;
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    if (x[i, j] == 0) count++;

            return count;
        }
    }
}

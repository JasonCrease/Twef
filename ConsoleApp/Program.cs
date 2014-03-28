using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Game g = new Game();
            Random r = new Random();
            int gameCount = 0;
            int totalScore = 0;

            while (true)
            {

                g = new Game();
                while (g.AreMovesAvailable())
                {
                    //Console.Write(g);

                    //char move = 'x';
                    ////ConsoleKeyInfo keyInfo = Console.ReadKey();
                    ////move = keyInfo.KeyChar;

                    //double mov = r.NextDouble();
                    //if (mov < 0.5) move = 'w';
                    //else if (mov < 0.96) move = 'a';
                    //else if (mov < 0.99) move = 'd';
                    //else move = 's';
                    //if (move == 'w')
                    //    g.Move(Direction.Up);
                    //else if (move == 'a')
                    //    g.Move(Direction.Left);
                    //else if (move == 's')
                    //    g.Move(Direction.Down);
                    //else if (move == 'd')
                    //    g.Move(Direction.Right);


                    Game gW = new Game(g);
                    Game gA = new Game(g);
                    Game gS = new Game(g);
                    Game gD = new Game(g);
                    bool wWorked = gW.Move(Direction.Up);
                    bool aWorked = gA.Move(Direction.Left);
                    bool sWorked = gS.Move(Direction.Down);
                    bool dWorked = gD.Move(Direction.Right);
                    int maxValue = Math.Max(gW.HeuristicValue, Math.Max(gA.HeuristicValue, Math.Max(gS.HeuristicValue, gD.HeuristicValue)));

                    bool moveWorked = false;

                    if (gW.HeuristicValue == maxValue && wWorked)
                        moveWorked = g.Move(Direction.Up);
                    else if (gS.HeuristicValue == maxValue && sWorked)
                        moveWorked = g.Move(Direction.Down);
                    else if (gA.HeuristicValue == maxValue && aWorked)
                        moveWorked = g.Move(Direction.Left);
                    else if (gD.HeuristicValue == maxValue && dWorked)
                        moveWorked = g.Move(Direction.Right);

                    if (!moveWorked)
                        moveWorked = g.Move(Direction.Up);
                    if (!moveWorked)
                        moveWorked = g.Move(Direction.Left);
                    if (!moveWorked)
                        moveWorked = g.Move(Direction.Down);
                    if (!moveWorked)
                        moveWorked = g.Move(Direction.Right);

                    g.AddRandomNumber();
                }

                gameCount++;
                totalScore += g.Score;
                if (gameCount % 100 == 0) Console.WriteLine(totalScore / gameCount);

                if (g.Score > 7000) goto outOfHere;
            }

            outOfHere:

            Console.WriteLine();
            Console.Write(g);
            Console.WriteLine("Score: " + g.Score);
            Console.ReadLine();
        }
    }
}

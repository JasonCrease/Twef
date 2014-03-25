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

            while (true)
            {
                gameCount++;
                Console.Write(gameCount + " ");

                g = new Game();
                while (g.AreMovesAvailable())
                {
                    char move = 'x';
                    //ConsoleKeyInfo keyInfo = Console.ReadKey();
                    //move = keyInfo.KeyChar;

                    double mov = r.NextDouble();
                    if (mov < 0.5) move = 'w';
                    else if (mov < 0.96) move = 'a';
                    else if (mov < 0.99) move = 'd';
                    else move = 's';

                    if (move == 'w')
                        g.Move(Direction.Up);
                    else if (move == 'd')
                        g.Move(Direction.Right);
                    else if (move == 'a')
                        g.Move(Direction.Left);
                    else if (move == 's')
                        g.Move(Direction.Down);

                    g.AddRandomNumber();
                }
                if (g.Score > 7000) break;
            }

            Console.WriteLine();
            Console.Write(g);
            Console.WriteLine("Score: " + g.Score);
            Console.ReadLine();
        }
    }
}

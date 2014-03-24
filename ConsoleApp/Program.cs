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
            Console.Write(g);

            for (int i = 0; i < 10; i++ )
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey();

                if (keyInfo.KeyChar == 'w')
                    g.Move(Direction.Up);
                else if (keyInfo.KeyChar == 'd')
                    g.Move(Direction.Right);
                else if (keyInfo.KeyChar == 'a')
                    g.Move(Direction.Left);
                else if (keyInfo.KeyChar == 's')
                    g.Move(Direction.Down);

                g.AddRandomNumber();
                Console.WriteLine();
                Console.Write(g);
            }

            Console.WriteLine("Done");
            Console.ReadLine();
        }
    }
}

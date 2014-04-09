using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class HeuristicAi : IAi
    {
        public Direction SuggestMove(Game g)
        {
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
                return Direction.Up;
            else if (gA.HeuristicValue == maxValue && aWorked)
                return Direction.Left;
            else if (gS.HeuristicValue == maxValue && sWorked)
                return Direction.Down;
            else if (gD.HeuristicValue == maxValue && dWorked)
                return Direction.Right;

            if (wWorked)
                return Direction.Up;
            if (aWorked)
                return Direction.Left;
            if (sWorked)
                return Direction.Down;
            if (dWorked)
                return Direction.Right;
            
            throw new ApplicationException("No move could be found");
        }
    }
}

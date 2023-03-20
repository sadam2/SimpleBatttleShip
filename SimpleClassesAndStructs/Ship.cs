using System.Drawing;

namespace SimpleBatttleShips.SimpleClassesAndStructs
{
    internal class Ship
    {
        internal int Length;
        internal Direction Direction;
        internal Point StartPoint;
        internal int Hitted;
        internal bool Sinken;
        internal List<Point> Cells = new List<Point>();
    }
}

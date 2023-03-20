using System.Drawing;

namespace SimpleBatttleShips.SimpleClassesAndStructs
{
    internal class Cell
    {
        private CellType type;
        internal CellType Type {get => type; set => SetType(value); }
        internal Ship? ship;
        private void SetType(CellType type)
        {
            CellType prevType = this.type;
            this.type = type;
            if (prevType == CellType.Ship && type == CellType.Hitted && ship != null)
            {
                ++ship.Hitted;
                if (ship.Hitted == ship.Length)
                {
                    for (int i = 0; i < ship.Cells.Count; i++)
                    {
                        Point p = ship.Cells[i];
                        Program.board[p.Y, p.X].Type = CellType.Sinken;
                    }
                    ship.Sinken = true;
                }
            }
        }
    }
}

using System;
using System.Drawing;
using SimpleBatttleShips.SimpleClassesAndStructs;

namespace SimpleBatttleShips
{
    internal sealed class GameBoardBuilder
    {
        private static GameBoardBuilder instance;
        private List<Ship> shipsList = new List<Ship>();

        GameBoardBuilder() { }

        internal static GameBoardBuilder Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GameBoardBuilder();
                }
                return instance;
            }
        }

        internal void BuildGameBoard()
        {
            shipsList.Clear();
            InitBoard();
            shipsList.Add(PutBattleShip());
            shipsList.Add(PutDestroyer());
            shipsList.Add(PutDestroyer());
            ClearInaccessiblesInGameBoard();
        }

        internal void ClearGameBoard()
        {
            for (int i = 0; i < Program.board.GetLength(0); i++)
            {
                for (int j = 0; j < Program.board.GetLength(1); j++)
                {
                    Program.board[i,j].Type = CellType.Unknown;
                }
            }
        }

        internal bool CheckCanPlay()
        {
            bool sinken = true;
            for (int i = 0; i < shipsList.Count; i++)
            {
                sinken = sinken && shipsList[i].Sinken;
            }
            return !sinken;
        }

        private void InitBoard()
        {
            for (int i = 0; i < Program.board.GetLength(0); i++)
            {
                for (int j = 0; j < Program.board.GetLength(1); j++)
                {
                    Program.board[i, j] = new Cell();
                }
            }
        }

        private Ship PutBattleShip()
        {
            Ship Ship = new Ship();
            Random random = new Random();
            Ship.Direction = (Direction)random.Next(Enum.GetValues(typeof(Direction)).Length);
            int boardWidth = Program.board.GetLength(1);
            int boardHeight = Program.board.GetLength(0);
            int usedBoardWidth = Ship.Direction == Direction.Horizontal ? boardWidth - Program.BattleShipLength + 1 : boardWidth;
            int usedBoardHeight = Ship.Direction == Direction.Vertical ? boardHeight - Program.BattleShipLength + 1 : boardHeight;
            Ship.StartPoint = new Point(random.Next(usedBoardWidth), random.Next(usedBoardHeight));
            Point currentPoint = new Point((Size)Ship.StartPoint);
            int horizontalIncrement = Ship.Direction == Direction.Vertical ? 0 : 1;
            int vericalIncrement = Ship.Direction == Direction.Horizontal ? 0 : 1;
            for (int i = 0; i < Program.BattleShipLength; i++)
            {
                Program.board[currentPoint.Y, currentPoint.X].Type = CellType.Ship;
                Program.board[currentPoint.Y, currentPoint.X].ship = Ship;
                Ship.Cells.Add(currentPoint);
                currentPoint.X += horizontalIncrement;
                currentPoint.Y += vericalIncrement;
            }
            Ship.Length = Program.BattleShipLength;
            return Ship;
        }

        private Ship PutDestroyer()
        {
            Ship Ship = new Ship();
            Random random = new Random();
            Ship.Direction = (Direction)random.Next(Enum.GetValues(typeof(Direction)).Length);
            BlockEdgeBoard(Ship);
            BlockBoardCloseToShips(Ship);
            PutDestroyer(ref Ship, random);
            Ship.Length = Program.DestroyerLength;
            return Ship;
        }

        private void BlockEdgeBoard(Ship ship)
        {
            int boardWidth = ship.Direction == Direction.Horizontal ? Program.board.GetLength(1) - Program.DestroyerLength + 1 : 0;
            int boardHeight = ship.Direction == Direction.Vertical ? Program.board.GetLength(0) - Program.DestroyerLength + 1 : 0;
            for (int i = boardWidth; i < Program.board.GetLength(1); i++)
            {
                for (int j = boardHeight; j < Program.board.GetLength(0); j++)
                {
                    SetCellInaccesible(j, i);
                }
            }
        }

        private void BlockBoardCloseToShips(Ship ship)
        {
            for (int l = 0; l < shipsList.Count; l++)
            {
                if (shipsList[l].Length == 0) continue;
                int s;
                if (ship.Direction == Direction.Horizontal)
                {
                    if (shipsList[l].StartPoint.X == 0) continue;
                    if (shipsList[l].Direction == Direction.Horizontal)
                    {
                        s = Math.Max(0, shipsList[l].StartPoint.X - Program.DestroyerLength + 1);
                        for (int i = s; i < shipsList[l].StartPoint.X; i++)
                        {
                            SetCellInaccesible(shipsList[l].StartPoint.Y, i);
                        }
                    }
                    else
                    {
                        s = Math.Max(0, shipsList[l].StartPoint.X - Program.DestroyerLength + 1);
                        for (int i = 0; i < shipsList[l].Length; i++)
                        {
                            for (int j = 0; j < Math.Min(shipsList[l].StartPoint.X, Program.DestroyerLength - 1); j++)
                            {
                                SetCellInaccesible(shipsList[l].StartPoint.Y + i, s + j);
                            }
                        }
                    }
                }
                else
                {
                    if (shipsList[l].StartPoint.Y == 0) continue;
                    if (shipsList[l].Direction == Direction.Horizontal)
                    {
                        s = Math.Max(0, shipsList[l].StartPoint.Y - Program.DestroyerLength + 1);
                        for (int i = 0; i < shipsList[l].Length; i++)
                        {
                            for (int j = 0; j < Math.Min(shipsList[l].StartPoint.Y, Program.DestroyerLength - 1); j++)
                            {
                                SetCellInaccesible(s + j, shipsList[l].StartPoint.X + i);
                            }
                        }
                    }
                    else
                    {
                        s = Math.Max(0, shipsList[l].StartPoint.Y - Program.DestroyerLength + 1);
                        for (int i = s; i < shipsList[l].StartPoint.Y; i++)
                        {
                            SetCellInaccesible(i, shipsList[l].StartPoint.X);
                        }
                    }
                }
            }
        }

        private void PutDestroyer(ref Ship ship, Random random)
        {
            int sum = 0, startCellIndex, cellCount = -1;
            ship.StartPoint = new Point();
            foreach (Cell cell in Program.board)
            {
                if (cell.Type == CellType.Unknown) ++sum; 
            }
            startCellIndex = random.Next(sum);
            for (int i = 0; i < Program.board.GetLength(1); i++)
            {
                for (int j = 0; j < Program.board.GetLength(0); j++)
                {
                    if (Program.board[j, i].Type == CellType.Unknown)
                    {
                        ++cellCount;
                        if (startCellIndex == cellCount)
                        {
                            ship.StartPoint.Y = j;
                            break;
                        }
                    }
                }
                if (startCellIndex == cellCount)
                {
                    ship.StartPoint.X = i;
                    break;
                }
            }
            int horizontalIncrement = ship.Direction == Direction.Vertical ? 0 : 1;
            int vericalIncrement = ship.Direction == Direction.Horizontal ? 0 : 1;
            Point currentPoint = new Point((Size)ship.StartPoint);
            for (int i = 0; i < Program.DestroyerLength; i++)
            {
                Program.board[currentPoint.Y, currentPoint.X].Type = CellType.Ship;
                Program.board[currentPoint.Y, currentPoint.X].ship = ship;
                ship.Cells.Add(currentPoint);
                currentPoint.X += horizontalIncrement;
                currentPoint.Y += vericalIncrement;
            }
        }

        private void SetCellInaccesible(int x, int y)
        {
            if (Program.board[x, y].Type == CellType.Unknown) Program.board[x, y].Type = CellType.Inaccessible;
        }

        private void ClearInaccessiblesInGameBoard()
        {
            for (int i = 0; i < Program.board.GetLength(0); i++)
            {
                for (int j = 0; j < Program.board.GetLength(1); j++)
                {
                    if (Program.board[i, j].Type == CellType.Inaccessible)
                    {
                        Program.board[i, j].Type = CellType.Unknown;
                    }
                }
            }
        }
    }
}

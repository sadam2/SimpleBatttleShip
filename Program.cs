using SimpleBatttleShips.SimpleClassesAndStructs;
using System.Drawing;

namespace SimpleBatttleShips
{
    internal class Program
    {
        static internal int BattleShipLength = 5;
        static internal int DestroyerLength = 4;
        static internal Cell[,] board = new Cell[10, 10];

        static void Main(string[] args)
        {
            Point point;
            GameBoardBuilder gameBoardBuilder = GameBoardBuilder.Instance;
            Console.WriteLine("Welcome to Simple BattleShips!");
            gameBoardBuilder.BuildGameBoard();
            ShowBoard();
            AnswerHint();
            bool play;
            do
            {
                point = ReadLine();
                if (point.X == int.MaxValue) break;
                play = DoShot(point, gameBoardBuilder);
                if (play)
                {
                    ShowBoard();
                    AnswerHint();
                }
                else
                {
                    ShowBoard();
                    Console.WriteLine("Play again ? (Y/N)");
                    if (Console.ReadKey().KeyChar == 'y')
                    {
                        gameBoardBuilder.ClearGameBoard();
                        gameBoardBuilder.BuildGameBoard();
                        ShowBoard();
                        AnswerHint();
                        play = true;
                    }
                }
            } while (play);
        }

        static private void AnswerHint()
        {
            Console.WriteLine("Enter X for exit");
            Console.WriteLine("Enter coordinates (for example \"A5\") for shot");
        }

        static private Point ReadLine()
        {
            string? line;
            Point point = new Point();
            do
            {
                line = Console.ReadLine();
                if (string.IsNullOrEmpty(line))
                {
                    AnswerHint();
                    continue;
                }
                line = line.Trim().ToUpper();
                if (!"ABCDEFGHIJX".Contains(line[0]))
                {
                    AnswerHint();
                    continue;
                }
                if ((line[0] == 'X') && line.Length > 1)
                {
                    AnswerHint();
                    continue;
                }
                if ((line[0] == 'X') && line.Length == 1)
                {
                    Console.SetCursorPosition(0, Console.CursorTop - 1);
                    Console.Write(Enumerable.Repeat<char>(' ', Console.BufferWidth).ToArray());
                    Console.SetCursorPosition(0, Console.CursorTop);
                    Console.WriteLine("X");
                    point.X = int.MaxValue;
                    break;
                }
                point.Y = line[0] - 65;
                point.X = -1;
                line = line.Substring(1);
                if (line.Length == 1 && line[0] > 48 && line[0] < 58)
                {
                    point.X = int.Parse(line) - 1;
                }
                else if(line == "10")
                {
                    point.X = 9;
                }
                if (point.X == -1)
                {
                    AnswerHint();
                    continue;
                }
                break;
            } while (true);
            return point;
        }

        static bool DoShot(Point point, GameBoardBuilder gameBoardBuilder)
        {
            switch (board[point.X, point.Y].Type)
            {
                case CellType.Unknown:
                case CellType.Inaccessible:
                    board[point.X, point.Y].Type = CellType.Empty;
                    break;
                case CellType.Ship:
                    board[point.X, point.Y].Type = CellType.Hitted;
                    Ship? ship = board[point.X, point.Y].ship;
                    if (ship != null)
                    {
                        if (ship.Sinken)
                        {
                            return gameBoardBuilder.CheckCanPlay();
                        }
                        else return true;
                    }
                    else return true;
                default:
                    break;
            }
            return true;
        }
        static private void ShowBoard()
        {
            Console.WriteLine("");
            Console.WriteLine("Characters on board:");
            Console.WriteLine("space - empty cell");
            Console.WriteLine("H - hitted ship");
            Console.WriteLine("S - sinked ship");
            Console.WriteLine("? - unknown");
            Console.WriteLine("");
            Console.WriteLine("     A B C D E F G H I J");
            Console.WriteLine("   -----------------------");
            for (int i = 0; i < board.GetLength(0); i++)
            {
                string line = (i < 9 ? " " : "") + (i + 1).ToString() + " | ";
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    switch (board[i, j].Type)
                    {
                        case CellType.Empty:
                            line += " ";
                            break;
                        case CellType.Hitted:
                            line += "H";
                            break;
                        case CellType.Sinken:
                            line += "S";
                            break;
                        default:
                            line += "?";
                            break;
                    }
                    line += " ";
                }
                Console.WriteLine(line + "|");
            }
            Console.WriteLine("   -----------------------");
            Console.WriteLine("");
        }
    }
}
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace YonatanMankovich.Battleships
{
    class Program
    {
        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int cmdShow);//For maximizing the window
        public static Random random = new Random();
        static Board myBoard = new Board();
        static Board opponentBoard = new Board();
        static void Main(string[] args)
        {
            MaximizeWindow();
            Console.Title = "Yonatan's Battleships Game";
            while (!opponentBoard.noMoreShipsFlag && !myBoard.noMoreShipsFlag)
            {
                PrintBoards();
                MakeUserMove();
                MakeComputerMove();
            }
            Console.Clear();
            if (myBoard.noMoreShipsFlag)
                ShowPlayerWon();
            else
                ShowComputerWon();
            Console.WriteLine("Game Over!");
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        private static void ShowComputerWon()
        {
            Console.WriteLine("Your ships:");
            opponentBoard.PrintBoard(true);
            Console.WriteLine("Computer's ships locations:\n");
            myBoard.PrintBoard(true);
            Console.BackgroundColor = ConsoleColor.Yellow;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine(@" __   _____  _   _   _    ___  ___ _____ " + Environment.NewLine +
                              @" \ \ / / _ \| | | | | |  / _ \/ __|_   _|" + Environment.NewLine +
                              @"  \ V / (_) | |_| | | |_| (_) \__ \ | |  " + Environment.NewLine +
                              @"   |_| \___/ \___/  |____\___/|___/ |_|  " + Environment.NewLine);
            Console.ResetColor();
        }

        private static void ShowPlayerWon()
        {
            Console.WriteLine("Your ships:");
            opponentBoard.PrintBoard(true);
            Console.WriteLine("Computer's ships:");
            myBoard.PrintBoard(true);
            Console.BackgroundColor = ConsoleColor.Yellow;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine(@" __   _____  _   _  __      _____  _  _ " + Environment.NewLine +
                              @" \ \ / / _ \| | | | \ \    / / _ \| \| |" + Environment.NewLine +
                              @"  \ V / (_) | |_| |  \ \/\/ / (_) | .` |" + Environment.NewLine +
                              @"   |_| \___/ \___/    \_/\_/ \___/|_|\_|" + Environment.NewLine);
            Console.ResetColor();
        }

        private static void PrintBoards()
        {
            Console.Clear();
            myBoard.PrintShipsInfo();
            Console.WriteLine();
            Console.WriteLine("Your ships:");
            opponentBoard.PrintBoard(true);
            Console.WriteLine("Computer's ships:");
            myBoard.PrintBoard();
            Console.WriteLine();
        }

        private static void MaximizeWindow()
        {
            Process p = Process.GetCurrentProcess();
            ShowWindow(p.MainWindowHandle, 3);
        }

        private static void MakeUserMove()
        {
            int x = GetValidNumber(Board.size[1], 'X'), y = GetValidNumber(Board.size[0], 'Y');
            while (myBoard.isShot[y, x])
            {
                Console.WriteLine("\nYou have already shot to this point...\n");
                x = GetValidNumber(Board.size[1], 'X');
                y = GetValidNumber(Board.size[0], 'Y');
            }
            myBoard.Shoot(x, y);
            if (!myBoard.noMoreShipsFlag && myBoard.wasShotLastShot)
            {
                PrintBoards();
                MakeUserMove();
            }
        }

        static List<Point> nextPossibleComputerMoves = new List<Point>();
        private static void MakeComputerMove()
        {
            int x = -1, y = -1;
            if (nextPossibleComputerMoves.Count == 0)
            {
                do
                {
                    x = random.Next(Board.size[1]);
                    y = random.Next(Board.size[0]);
                } while (opponentBoard.isShot[y, x]);
            }
            else
            {
                int r = random.Next(nextPossibleComputerMoves.Count);
                x = nextPossibleComputerMoves[r].y; //IDK why x and y are switched...
                y = nextPossibleComputerMoves[r].x; //At least it works...
                nextPossibleComputerMoves.RemoveAt(r);
            }
            opponentBoard.Shoot(x, y);
            if (!opponentBoard.noMoreShipsFlag && opponentBoard.wasShotLastShot) //Computer's turn again.
            {
                if (!Ship.IsShipDestroyedAtPoint(opponentBoard.ships, x, y))
                {//Get list of next possible moves
                    if (opponentBoard.ships[Ship.GetShipAtPoint(opponentBoard.ships, x, y)].destroyedParts == 1)
                    {
                        nextPossibleComputerMoves.Clear();
                        if (opponentBoard.IsValidPointForNextMove(x - 1, y))
                            nextPossibleComputerMoves.Add(new Point(x - 1, y));
                        if (opponentBoard.IsValidPointForNextMove(x + 1, y))
                            nextPossibleComputerMoves.Add(new Point(x + 1, y));
                        if (opponentBoard.IsValidPointForNextMove(x, y - 1))
                            nextPossibleComputerMoves.Add(new Point(x, y - 1));
                        if (opponentBoard.IsValidPointForNextMove(x, y + 1))
                            nextPossibleComputerMoves.Add(new Point(x, y + 1));
                    }
                    if (opponentBoard.ships[Ship.GetShipAtPoint(opponentBoard.ships, x, y)].destroyedParts > 1)
                    {
                        nextPossibleComputerMoves.Clear();
                        if (!opponentBoard.ships[Ship.GetShipAtPoint(opponentBoard.ships, x, y)].isVertical)
                        {
                            for (int newX = x + 1; newX < Board.size[1]; newX++) //Check to the right.
                            {
                                if (opponentBoard.IsPointShotShip(newX, y))
                                    continue;
                                if (!opponentBoard.IsValidPointForNextMove(newX, y))
                                    break;
                                if (opponentBoard.IsValidPointForNextMove(newX, y))
                                {
                                    nextPossibleComputerMoves.Add(new Point(newX, y));
                                    break;
                                }
                            }
                            for (int newX = x - 1; newX > 0; newX--) //Check to the left.
                            {
                                if (opponentBoard.IsPointShotShip(newX, y))
                                    continue;
                                if (!opponentBoard.IsValidPointForNextMove(newX, y))
                                    break;
                                if (opponentBoard.IsValidPointForNextMove(newX, y))
                                {
                                    nextPossibleComputerMoves.Add(new Point(newX, y));
                                    break;
                                }
                            }
                        }
                        else
                        {
                            for (int newY = y + 1; newY < Board.size[0]; newY++) //Check to the bottom.
                            {
                                if (opponentBoard.IsPointShotShip(x, newY))
                                    continue;
                                if (!opponentBoard.IsValidPointForNextMove(x, newY))
                                    break;
                                if (opponentBoard.IsValidPointForNextMove(x, newY))
                                {
                                    nextPossibleComputerMoves.Add(new Point(x, newY));
                                    break;
                                }
                            }
                            for (int newY = y - 1; newY > 0; newY--) //Check to the top.
                            {
                                if (opponentBoard.IsPointShotShip(x, newY))
                                    continue;
                                if (!opponentBoard.IsValidPointForNextMove(x, newY))
                                    break;
                                if (opponentBoard.IsValidPointForNextMove(x, newY))
                                {
                                    nextPossibleComputerMoves.Add(new Point(x, newY));
                                    break;
                                }
                            }
                        }
                    }
                }
                MakeComputerMove();
            }
        }

        private static int GetValidNumber(int upTo, char pointName)
        {
            int x = -1;
            while (x < 0 || x > Board.size[1] - 1)
            {
                Console.WriteLine($"Enter your {pointName} point:");
                string input = Console.ReadLine();
                try
                {
                    x = int.Parse(input);
                }
                catch { }
                if (x < 0 || x > Board.size[1] - 1)
                    Console.WriteLine("\nEntered point is out of range...\n");
            }
            return x;
        }
    }

    struct Point
    {
        public int x, y;
        public Point(int y1, int x1)
        {
            x = x1;
            y = y1;
        }
    }

    class Board
    {
        public static int[] size = { 10, 10 };
        public bool[,] isShip = new bool[Board.size[0], Board.size[1]];
        static int[,] shipCountAndSize = { { 1, 4 }, { 2, 3 }, { 3, 2 }, { 4, 1 } };//{ Count, Size }
        //static int[,] shipCountAndSize = { { 1, 5 } }; //For debugging.
        int destroyedShips = 0;
        public bool[,] isShot = new bool[size[0], size[1]];
        public List<Ship> ships = new List<Ship>();
        public bool noMoreShipsFlag = false;
        public bool wasShotLastShot = false;

        public Board()
        {
            for (int i = 0; i < isShot.GetLength(0); i++)
                for (int j = 0; j < isShot.GetLength(1); j++)
                    isShot[i, j] = false;
            for (int i = 0; i < shipCountAndSize.GetLength(0); i++)
                for (int j = 0; j < shipCountAndSize[i, 0]; j++)
                    AddShip(shipCountAndSize[i, 1]);
        }

        public void PrintShipsInfo()
        {
            Console.WriteLine("On the board, there are:");
            for (int i = 0; i < shipCountAndSize.GetLength(0); i++)
                Console.WriteLine($"{shipCountAndSize[i, 0]} ship(s) of {shipCountAndSize[i, 1]} block(s)");
        }

        public void AddShip(int size)
        {
            ships.Add(new Ship(size, isShip));
            UpdateIsShip();
        }

        public void PrintBoard(bool showShip = false)
        {
            Console.Write("  ");
            for (int i = 0; i < isShot.GetLength(1); i++)
                Console.Write(i + " ");
            Console.WriteLine();
            for (int i = 0; i < isShot.GetLength(0); i++)
            {
                Console.Write(i + " ");
                for (int j = 0; j < isShot.GetLength(1); j++)
                {
                    char output = '\0';
                    if (showShip)
                    {
                        if (isShot[i, j] && isShip[i, j])
                            output = '*';
                        else if (!isShot[i, j] && isShip[i, j])
                            output = '+';
                        else if (isShot[i, j] && !isShip[i, j])
                            output = 'X';
                        else
                            output = 'O';
                    }
                    else
                    {
                        if (isShot[i, j] && isShip[i, j])
                            output = '*';
                        else if (isShot[i, j] && !isShip[i, j])
                            output = 'X';
                        else
                            output = 'O';
                    }
                    switch (output)
                    {
                        case '*':
                            {
                                Console.BackgroundColor = ConsoleColor.Red;
                                Console.ForegroundColor = ConsoleColor.White;
                            }
                            break;
                        case '+':
                            {
                                Console.BackgroundColor = ConsoleColor.Magenta;
                                Console.ForegroundColor = ConsoleColor.White;
                            }
                            break;
                        case 'X':
                            {
                                Console.BackgroundColor = ConsoleColor.Green;
                                Console.ForegroundColor = ConsoleColor.Black;
                            }
                            break;
                        case 'O':
                            {
                                Console.BackgroundColor = ConsoleColor.Blue;
                                Console.ForegroundColor = ConsoleColor.White;
                            }
                            break;
                    }
                    Console.Write(output + " ");
                    Console.ResetColor();
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        public void Shoot(int x, int y)
        {
            isShot[y, x] = true;
            if (isShip[y, x])
            {
                int shipIndex = Ship.GetShipAtPoint(ships, x, y);
                ships[shipIndex].destroyedParts++;
                if (ships[shipIndex].destroyedParts == ships[shipIndex].size)
                    DestroyShip(shipIndex);
                wasShotLastShot = true;
                return;
            }
            wasShotLastShot = false;
        }

        public void DestroyShip(int shipIndex)
        {
            int horizontalMultiplier = ships[shipIndex].isVertical ? 1 : 0;
            int verticalMultiplier = ships[shipIndex].isVertical ? 0 : 1;
            for (int i = ships[shipIndex].initialPoint[1] - 1; i <= ships[shipIndex].initialPoint[1] + 1 * verticalMultiplier + ships[shipIndex].size * horizontalMultiplier; i++)
            {
                for (int j = ships[shipIndex].initialPoint[0] - 1; j <= ships[shipIndex].initialPoint[0] + 1 * horizontalMultiplier + ships[shipIndex].size * verticalMultiplier; j++)
                    if (i >= 0 && j >= 0 && i < Board.size[0] && j < Board.size[1])
                        isShot[i, j] = true;
            }
            destroyedShips++;
            noMoreShipsFlag = destroyedShips == ships.Count;
        }

        public void UpdateIsShip()
        {
            for (int i = 0; i < ships.Count; i++)
                for (int j = 0; j < ships[i].size; j++)
                    isShip[ships[i].points[j].y, ships[i].points[j].x] = true;
        }

        public bool IsValidPointForNextMove(int x, int y)
        {
            return (x < size[1] && y < size[0] && x >= 0 && y >= 0 && !isShot[y, x]);
        }

        public bool IsPointShot(int x, int y)
        {
            return (isShot[y, x]);
        }

        public bool IsPointShotShip(int x, int y)
        {
            return (isShot[y, x] && isShip[y, x]);
        }
    }

    class Ship
    {
        public List<Point> points = new List<Point>();
        public int destroyedParts = 0;
        public int[] initialPoint = new int[2];
        public bool isVertical;
        public int size;
        public Ship(int newSize, bool[,] isShip)
        {
            size = newSize;
            int rand = 1, horizontalMultiplier = 1, verticalMultiplier = 0;
            bool isEmpty = false;
            int tryLimiter = 0;
            for (tryLimiter = 0; tryLimiter < 100 && !isEmpty; tryLimiter++)
            {
                rand = Program.random.Next(2);
                horizontalMultiplier = rand;
                verticalMultiplier = 1 - rand;
                isVertical = rand == 1;
                initialPoint[0] = Program.random.Next(Board.size[1] - (size + 1) * verticalMultiplier);
                initialPoint[1] = Program.random.Next(Board.size[0] - (size + 1) * horizontalMultiplier);
                isEmpty = true;
                for (int i = initialPoint[1] - 1; i <= initialPoint[1] + 1 * verticalMultiplier + size * horizontalMultiplier; i++)
                {
                    for (int j = initialPoint[0] - 1; j <= initialPoint[0] + 1 * horizontalMultiplier + size * verticalMultiplier; j++)
                        if (i >= 0 && j >= 0 && i < Board.size[0] && j < Board.size[1])
                            if (isShip[i, j])
                            {
                                isEmpty = false;
                                break;
                            }
                    if (!isEmpty)
                        break;
                }
            }
            if (tryLimiter == 100)
            {
                Console.WriteLine("Too many ships...");
                Console.ReadKey();
                Environment.Exit(10);
            }
            for (int i = 0; i < size; i++)
                points.Add(new Point(initialPoint[1] + i * horizontalMultiplier, initialPoint[0] + i * verticalMultiplier));
        }

        public static int GetShipAtPoint(List<Ship> ships, int x, int y)
        {
            for (int i = 0; i < ships.Count; i++)
                for (int j = 0; j < ships[i].size; j++)
                    if (ships[i].points[j].x == x && ships[i].points[j].y == y)
                        return i;
            return -1;
        }

        public static bool IsShipDestroyedAtPoint(List<Ship> ships, int x, int y)
        {
            return (ships[GetShipAtPoint(ships, x, y)].destroyedParts == ships[GetShipAtPoint(ships, x, y)].size);
        }
    }
}
using System;
using System.Collections.Generic;

namespace Battleships
{
    class Program
    {
        public static Random random = new Random();
        static Board myBoard = new Board();
        static Board oppoBoard = new Board();
        static bool userHit = false, compHit = false;
        static void Main(string[] args)
        {
            Console.Title = "Jonathan's Battleships Game";
            while (!oppoBoard.noMoreShipsFlag && !myBoard.noMoreShipsFlag)
            {
                myBoard.PrintShipsInfo();
                Console.WriteLine();
                oppoBoard.PrintBoard(true);
                myBoard.PrintBoard();
                Console.WriteLine();
                if (!compHit)
                {
                    MakeUserMove();
                    compHit = false;
                }
                if (!userHit)
                {
                    MakeComputerMove();
                    userHit = false;
                }
                Console.Clear();
            }
            if (myBoard.noMoreShipsFlag)
            {
                Console.BackgroundColor = ConsoleColor.Yellow;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.WriteLine("You Won!\n");
                Console.ResetColor();
                myBoard.PrintBoard(true);
            }
            else
            {
                Console.WriteLine("The computer won...\n");
                Console.WriteLine("Computer's ships locations:\n");
                myBoard.PrintBoard(true);
                Console.WriteLine("Game Over!");
            }
            Console.ReadLine();
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
            userHit = myBoard.Shoot(x, y);
        }

        private static void MakeComputerMove()
        {
            int x = -1, y = -1;
            do
            {
                x = random.Next(Board.size[1]);
                y = random.Next(Board.size[0]);
            }
            while (oppoBoard.isShot[y, x]);
            compHit = oppoBoard.Shoot(x, y);
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
        public bool[,] isShip = new bool[Board.size[0], Board.size[1]];
        public static int[] size = { 10, 10 };
        static int[,] shipCountAndSize = { { 1, 4 }, { 2, 3 }, { 3, 2 }, { 4, 1 } };//{ Count, Size }
        //static int[,] shipCountAndSize = { { 3, 2 } };//{ Count, Size }
        int destroyedShips = 0;
        public bool[,] isShot = new bool[size[0], size[1]];
        public List<Ship> ships = new List<Ship>();
        public bool noMoreShipsFlag = false;

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

        public bool Shoot(int x, int y)
        {
            isShot[y, x] = true;
            if (isShip[y, x])
            {
                int shipIndex = Ship.GetShipAtPoint(ships, x, y);
                ships[shipIndex].destroyedParts++;
                if (ships[shipIndex].destroyedParts == ships[shipIndex].size)
                    DestroyShip(shipIndex);
                return true;
            }
            return false;
        }

        public void DestroyShip(int shipIndex)
        {
            int horizontalMultiplier = ships[shipIndex].isHorizontal ? 1 : 0;
            int verticalMultiplier = ships[shipIndex].isHorizontal ? 0 : 1;
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
    }

    class Ship
    {
        public List<Point> points = new List<Point>();
        public int destroyedParts = 0;
        public int[] initialPoint = new int[2];
        public bool isHorizontal;
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
                isHorizontal = rand == 1;
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
    }
}
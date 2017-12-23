using System;
using System.Collections.Generic;

namespace Battleships
{
    class Program
    {
        public static Random random = new Random();
        static Board myBoard = new Board();
        static Board oppoBoard = new Board();
        static void Main(string[] args)
        {
            Console.Title = "Jonathan's Battleships Game";
            while(!oppoBoard.noMoreShipsFlag|| myBoard.noMoreShipsFlag)
            {
                myBoard.PrintShipsInfo();
                Console.WriteLine();
                oppoBoard.PrintBoard(true);
                myBoard.PrintBoard(true);
                Console.WriteLine();
                Move();
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
                Console.WriteLine("Ships locations:\n");
                myBoard.PrintBoard(true);
                Console.WriteLine("Game Over!");
            }
            Console.ReadLine();
        }

        private static bool Move()
        {
            int x = GetValidX(), y = GetValidY();
            while (myBoard.board[y, x])
            {
                Console.WriteLine("\nYou have already shot to this point...\n");
                x = GetValidX();
                y = GetValidY();
            }
            return myBoard.Shoot(x, y);
        }

        private static int GetValidX()
        {
            int x = -1;
            while (x < 0 || x > Board.size[1] - 1)
            {
                Console.WriteLine("Enter your X point:");
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

        private static int GetValidY()
        {
            int y = -1;
            while (y < 0 || y > Board.size[0] - 1)
            {
                Console.WriteLine("Enter your Y point:");
                string input = Console.ReadLine();
                try
                {
                    y = int.Parse(input);
                }
                catch { }
                if (y < 0 || y > Board.size[0] - 1)
                    Console.WriteLine("\nEntered point is out of range...\n");
            }
            return y;
        }
    }

    class Board
    {
        public bool[,] isShip = new bool[Board.size[0], Board.size[1]];
        public static int[] size = { 5, 5 };
        //static int[,] shipCountAndSize = { { 1, 4 }, { 2, 3 }, { 3, 2 }, { 4, 1 } };//{ Count, Size }
        static int[,] shipCountAndSize = { { 3, 2 } };//{ Count, Size }
        int destroyedShips = 0;
        public bool[,] board = new bool[size[0], size[1]];
        public List<Ship> ships = new List<Ship>();
        public bool noMoreShipsFlag = false;

        public Board()
        {
            for (int i = 0; i < board.GetLength(0); i++)
                for (int j = 0; j < board.GetLength(1); j++)
                    board[i, j] = false;
            for (int i = 0; i < shipCountAndSize.GetLength(0); i++)
                for (int j = 0; j < shipCountAndSize[i, 0]; j++)
                {
                    AddShip(shipCountAndSize[i, 1]);
                    isShip=ships[i].CreateShip(isShip);
                }
        }

        public void PrintShipsInfo()
        {
            Console.WriteLine("On the board, there are:");
            for (int i = 0; i < shipCountAndSize.GetLength(0); i++)
                Console.WriteLine($"{shipCountAndSize[i,0]} ship(s) of {shipCountAndSize[i, 1]} block(s)");
        }

        public void AddShip(int size)
        {
            ships.Add(new Ship(size));
        }

        public void PrintBoard(bool showShip = false)
        {
            Console.Write("  ");
            for (int i = 0; i < board.GetLength(1); i++)
                Console.Write(i + " ");
            Console.WriteLine();
            for (int i = 0; i < board.GetLength(0); i++)
            {
                Console.Write(i + " ");
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    char output = '\0';
                    if (showShip)
                    {
                        if (board[i, j] && isShip[i, j])
                            output = '*';
                        else if (!board[i, j] && isShip[i, j])
                            output = '+';
                        else if (board[i, j] && !isShip[i, j])
                            output = 'X';
                        else
                            output = 'O';
                    }
                    else
                    {
                        if (board[i, j] && isShip[i, j])
                            output = '*';
                        else if (board[i, j] && !isShip[i, j])
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
            board[y, x] = true;
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
            for (int i = ships[shipIndex].initPoint[1] - 1; i <= ships[shipIndex].initPoint[1] + 1 * verticalMultiplier + ships[shipIndex].size * horizontalMultiplier; i++)
            {
                for (int j = ships[shipIndex].initPoint[0] - 1; j <= ships[shipIndex].initPoint[0] + 1 * horizontalMultiplier + ships[shipIndex].size * verticalMultiplier; j++)
                    if (i >= 0 && j >= 0 && i < Board.size[0] && j < Board.size[1])
                        board[i, j] = true;
            }
            destroyedShips++;
            noMoreShipsFlag = destroyedShips == ships.Count;
        }
    }

    class Ship
    {
        public int destroyedParts = 0;
        public int[] initPoint = new int[2];
        public bool isHorizontal;
        public int size;
        public Ship(int newSize)
        {
            size = newSize;
        }
        public bool[,] CreateShip(bool[,] isShip)
        {
            int rand = 1, horizontalMultiplier = 1, verticalMultiplier = 0;
            bool isEmpty = false;
            int tryLimiter = 0;
            for (tryLimiter = 0; tryLimiter < 100 && !isEmpty; tryLimiter++)
            {
                rand = Program.random.Next(2);
                horizontalMultiplier = rand;
                verticalMultiplier = 1 - rand;
                isHorizontal = rand == 1;
                initPoint[0] = Program.random.Next(Board.size[1] - (size + 1) * verticalMultiplier);
                initPoint[1] = Program.random.Next(Board.size[0] - (size + 1) * horizontalMultiplier);
                isEmpty = true;
                for (int i = initPoint[1] - 1; i <= initPoint[1] + 1 * verticalMultiplier + size * horizontalMultiplier; i++)
                {
                    for (int j = initPoint[0] - 1; j <= initPoint[0] + 1 * horizontalMultiplier + size * verticalMultiplier; j++)
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
                isShip[initPoint[1] + i * horizontalMultiplier, initPoint[0] + i * verticalMultiplier] = true;
            return isShip;
        }

        public static int GetShipAtPoint(List<Ship> ships, int x, int y)
        {
            for (int i = 0; i < ships.Count; i++)
            {
                if (ships[i].isHorizontal && ships[i].initPoint[0] == x)
                    for (int j = 0; j < ships[i].size; j++)
                        if (ships[i].initPoint[1] + j == y)
                            return i;
                if (!ships[i].isHorizontal && ships[i].initPoint[1] == y)
                    for (int j = 0; j < ships[i].size; j++)
                        if (ships[i].initPoint[0] + j == x)
                            return i;
            }
            return -1;
        }
    }
}
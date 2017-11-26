using System;
using System.Collections.Generic;

namespace Battleships
{
    class Program
    {
        public static Random random = new Random();
        static void Main(string[] args)
        {
            Board myBoard = new Board();
            //myBoard.PrintBoard();
            myBoard.PrintShips();
            Console.ReadLine();
        }
    }

    class Board
    {
        public static int[] size = { 10, 10 };
        int[,] shipCountAndSize = { { 1, 4 }, { 2, 3 }, { 3, 2 }, { 4, 1 } };//{ Count, Size }
        public bool[,] board = new bool[size[0], size[1]];
        public List<Ship> ships = new List<Ship>();
        public Board()
        {
            for (int i = 0; i < board.GetLength(1); i++)
                for (int j = 0; j < board.GetLength(0); j++)
                    board[i, j] = false;
            for (int i = 0; i < shipCountAndSize.GetLength(0); i++)
                for (int j = 0; j < shipCountAndSize[i, 0]; j++)
                    AddShip(shipCountAndSize[i, 1]);
        }

        public void AddShip(int size)
        {
            ships.Add(new Ship(size));
            PrintShips();
            Console.WriteLine();
        }

        public void PrintBoard()
        {
            Console.Write("  ");
            for (int i = 0; i < board.GetLength(0); i++)
                Console.Write(i + " ");
            Console.WriteLine();
            for (int i = 0; i < board.GetLength(1); i++)
            {
                Console.Write(i + " ");
                for (int j = 0; j < board.GetLength(0); j++)
                    Console.Write((board[i, j] ? 'X' : 'O') + " ");
                Console.WriteLine();
            }
        }

        public void PrintShips()
        {
            Console.Write("  ");
            for (int i = 0; i < Ship.isShip.GetLength(0); i++)
                Console.Write(i + " ");
            Console.WriteLine();
            for (int i = 0; i < Ship.isShip.GetLength(1); i++)
            {
                Console.Write(i + " ");
                for (int j = 0; j < Ship.isShip.GetLength(0); j++)
                    Console.Write((Ship.isShip[i, j] ? 'O' : '~') + " ");
                Console.WriteLine();
            }
        }
    }

    class Ship
    {
        static public bool[,] isShip = new bool[Board.size[0], Board.size[1]];
        public int[] initPoint = new int[2];
        public bool isHorizontal;
        public int size;
        public Ship(int newSize)
        {
            size = newSize;
            CreateShip();
        }
        private void CreateShip()
        {
            int rand, horizontalMultiplier, verticalMultiplier;
            bool isEmpty = false;
            do
            {
                rand = Program.random.Next(2);
                horizontalMultiplier = rand;
                verticalMultiplier = 1 - rand;
                isHorizontal = rand == 1;
                initPoint[0] = Program.random.Next(Board.size[1] - (size + 1) * verticalMultiplier);
                initPoint[1] = Program.random.Next(Board.size[0] - (size + 1) * horizontalMultiplier);
                isEmpty = true;
                for (int i = initPoint[1] - 1; i <= initPoint[1] + 1 + size * horizontalMultiplier; i++)
                {
                    for (int j = initPoint[0] - 1; j <= initPoint[0] + 1 + size * verticalMultiplier; j++)
                        if (i >= 0 && j >= 0 && i < isShip.GetLength(0) && j < isShip.GetLength(1))
                            if (isShip[i, j])
                            {
                                isEmpty = false;
                                break;
                            }
                    if (!isEmpty)
                        break;
                }
            } while (!isEmpty);
            for (int i = 0; i < size; i++)
                isShip[initPoint[1] + i * horizontalMultiplier, initPoint[0] + i * verticalMultiplier] = true;
        }
    }
}
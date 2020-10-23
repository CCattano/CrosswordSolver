using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace CrosswordSolver
{
    class Program
    {
        private readonly static char[,] Puzzle = new char[,]
        {
            { 'l', 'n', 'b', 'f', 'o', 'o', 'd', 'h', 'l', 'l', 'e', 'm', 'a', 'n', 'e', 'b', 'c', 'q' },
            { 'o', 'd', 'u', 'o', 'l', 'c', 'm', 'r', 'e', 't', 's', 'a', 'm', 's', 'e', 'o', 'l', 'p' },
            { 'm', 'h', 'b', 'b', 'm', 'e', 'h', 'o', 'g', 's', 'v', 'e', 'g', 'a', 'n', 't', 'e', 'e' },
            { 'o', 'c', 'o', 'r', 'f', 'v', 'x', 'u', 'r', 'e', 'b', 'h', 'r', 'c', 'b', 'a', 'a', 't' },
            { 'q', 'd', 'd', 'k', 'a', 'd', 'm', 'i', 'z', 'i', 'h', 'd', 'i', 'i', 'j', 'g', 'n', 's' },
            { 'g', 'l', 'r', 'a', 'i', 'i', 'o', 'a', 't', 'w', 'g', 'c', 'n', 'h', 'x', 'o', 's', 'y' },
            { 'j', 'o', 'e', 'v', 'e', 'n', 'd', 'l', 'y', 'a', 'a', 'i', 'a', 'c', 'k', 'f', 'e', 'a' },
            { 'w', 'c', 't', 'r', 's', 'r', 'f', 'i', 'l', 'c', 'r', 'i', 'n', 't', 'c', 'f', 'w', 'o' },
            { 'a', 'x', 't', 'e', 'p', 'e', 'b', 'o', 'i', 'a', 's', 'i', 's', 'h', 's', 'a', 'n', 'e' },
            { 'r', 'b', 'i', 'p', 'c', 'l', 'e', 't', 'l', 'm', 'r', 'n', 'a', 't', 'g', 'u', 'a', 'c' },
            { 's', 'l', 'b', 'p', 'o', 'p', 'e', 'r', 'a', 'k', 'g', 'r', 'e', 'n', 'c', 'c', 'm', 'h' },
            { 'y', 'o', 'l', 'o', 'u', 'h', 't', 'l', 'f', 'u', 't', 'n', 'v', 'u', 'l', 'o', 'd', 'o' },
            { 'b', 'g', 'k', 'c', 't', 'p', 'l', 'e', 'm', 'r', 'b', 'h', 'i', 'q', 't', 'e', 'a', 'w' },
            { 'a', 'p', 'y', 's', 'u', 'b', 'h', 'p', 'e', 'n', 'c', 'b', 'n', 'k', 'e', 'r', 'h', 't' },
            { 'g', 'r', 'e', 'r', 'v', 'c', 'a', 'u', 'a', 's', 't', 'm', 'e', 'p', 'n', 'z', 'a', 'r' },
            { 'u', 'a', 's', 'i', 'i', 'r', 's', 't', 't', 'd', 'r', 'n', 'g', 'c', 'r', 'i', 'a', 'd' },
            { 'm', 'i', 'a', 'l', 't', 'e', 'v', 'i', 'c', 'y', 'a', 'h', 'a', 'e', 'u', 'a', 'r', 's' },
            { 'd', 'v', 'c', 'y', 'v', 'm', 'k', 'h', 'l', 'h', 'b', 'j', 'r', 'b', 'l', 'u', 'e', 'd' },
        };

        private readonly static string[] WordBank = new[]
        {
            "aesthetic", "affogato", "air",
            "art", "bag", "batch",
            "beard", "bitter", "blog",
            "blue", "braid", "bread",
            "bun", "chartreuse", "chic",
            "cleanse", "cliche", "cloud",
            "cold", "copper", "deep",
            "disrupt", "dollar", "drinking",
            "echo", "enamel", "etsy",
            "flexitarian", "food", "free",
            "kinfolk", "kitsch", "lomo",
            "man", "master", "meh",
            "mug", "mustache", "neutra",
            "origin", "party", "pop",
            "raw", "small", "vegan",
            "vinegar", "waistcoat", "yolo",
        };

        static readonly int
            YAxisLen = Puzzle.GetLength(0),
            XAxisLen = Puzzle.GetLength(1);

        static void Main(string[] args)
        {
            Stopwatch timer = Stopwatch.StartNew();
            Answer answer;
            foreach (string word in WordBank)
            {
                answer = new Answer();

                //Find all (X,Y) coords that have first letter of word
                List<Coords> firstLetterCoords = new List<Coords>();
                for (int row = 0; row < YAxisLen; row++)
                {
                    for (int col = 0; col < XAxisLen; col++)
                    {
                        if (Puzzle[row, col] == word[0])
                            firstLetterCoords.Add(new Coords() { X = col, Y = row });
                    }
                }

                //Per grid coord-set determine what directions are possible
                bool foundWord = false;
                foreach (Coords coords in firstLetterCoords)
                {
                    List<DirPossibility> possibleDirs = new List<DirPossibility>()
                    {
                        new DirPossibility() { Direction = Direction.Up, IsPossible = CouldBeUp(coords.Y, coords.X, word.Length, word[1]) },
                        new DirPossibility() { Direction = Direction.UpAndRight, IsPossible = CouldBeUpAndRight(coords.Y, coords.X, word.Length, word[1]) },
                        new DirPossibility() { Direction = Direction.Right, IsPossible = CouldBeRight(coords.Y, coords.X, word.Length, word[1]) },
                        new DirPossibility() { Direction = Direction.DownAndRight, IsPossible = CouldBeDownAndRight(coords.Y, coords.X, word.Length, word[1]) },
                        new DirPossibility() { Direction = Direction.Down, IsPossible = CouldBeDown(coords.Y, coords.X, word.Length, word[1]) },
                        new DirPossibility() { Direction = Direction.DownAndLeft, IsPossible = CouldBeDownAndLeft(coords.Y, coords.X, word.Length, word[1]) },
                        new DirPossibility() { Direction = Direction.Left, IsPossible = CouldBeLeft(coords.Y, coords.X, word.Length, word[1]) },
                        new DirPossibility() { Direction = Direction.UpAndLeft, IsPossible = CouldBeUpAndLeft(coords.Y, coords.X, word.Length, word[1]) }
                    };

                    //For each possible direction see if the word is there
                    foreach (DirPossibility dir in possibleDirs)
                    {
                        if (!dir.IsPossible) continue;
                        foundWord = LookForWord(dir.Direction, word, coords.Y, coords.X);
                        if (foundWord) break;
                    }

                    //Don't check any more coord-sets for this word if the word has been found
                    if (foundWord) break;
                }

                //Print per find
                if (!foundWord)
                {
                    Console.WriteLine($"Could not find word: {word}\n");
                }
                else
                {
                    Console.WriteLine($"Word: {word}");
                    for (int row = 0; row < YAxisLen; row++)
                    {
                        for (int col = 0; col < XAxisLen; col++)
                        {
                            if (answer.Coords.Any(coord => coord.Y == row && coord.X == col))
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.Write($"{char.ToUpper(Puzzle[row, col])} ");
                                Console.ResetColor();
                            }
                            else
                            {
                                Console.Write($"{char.ToUpper(Puzzle[row, col])} ");
                            }
                        }
                        Console.WriteLine();
                    }
                    Console.WriteLine();
                }
            }

            timer.Stop();
            Console.WriteLine($"Solved in {timer.Elapsed.Milliseconds}ms");

            static bool CouldBeUp(int row, int col, int wordLen, char nextLetter) =>
                !((row + 1) - wordLen < 0 || Puzzle[row - 1, col] != nextLetter);

            static bool CouldBeUpAndRight(int row, int col, int wordLen, char nextLetter) =>
                !((row + 1) - wordLen < 0 || col + wordLen > XAxisLen || Puzzle[row - 1, col + 1] != nextLetter);

            static bool CouldBeRight(int row, int col, int wordLen, char nextLetter) =>
                !(col + wordLen > XAxisLen || Puzzle[row, col + 1] != nextLetter);

            static bool CouldBeDownAndRight(int row, int col, int wordLen, char nextLetter) =>
                !(row + wordLen > YAxisLen || col + wordLen > XAxisLen || Puzzle[row + 1, col + 1] != nextLetter);

            static bool CouldBeDown(int row, int col, int wordLen, char nextLetter) =>
                !(row + wordLen > YAxisLen || Puzzle[row + 1, col] != nextLetter);

            static bool CouldBeDownAndLeft(int row, int col, int wordLen, char nextLetter) =>
                !(row + wordLen > YAxisLen || (col + 1) - wordLen < 0 || Puzzle[row + 1, col - 1] != nextLetter);

            static bool CouldBeLeft(int row, int col, int wordLen, char nextLetter) =>
                !((col + 1) - wordLen < 0 || Puzzle[row, col - 1] != nextLetter);

            static bool CouldBeUpAndLeft(int row, int col, int wordLen, char nextLetter) =>
                !((row + 1) - wordLen < 0 || (col + 1) - wordLen < 0 || Puzzle[row - 1, col - 1] != nextLetter);

            bool LookForWord(Direction direction, string word, int row, int col)
            {
                bool foundWord = true;
                List<Coords> coords = new List<Coords> { new Coords() { X = col, Y = row } };
                for (int i = 1; i < word.Length; i++)
                {
                    (int rowToCheck, int colToCheck) = direction switch
                    {
                        Direction.Up => (row - i, col),
                        Direction.UpAndRight => (row - i, col + i),
                        Direction.Right => (row, col + i),
                        Direction.DownAndRight => (row + i, col + i),
                        Direction.Down => (row + i, col),
                        Direction.DownAndLeft => (row + i, col - i),
                        Direction.Left => (row, col - i),
                        Direction.UpAndLeft => (row - i, col - i),
                        _ => throw new NotImplementedException(),
                    };

                    if (word[i] == Puzzle[rowToCheck, colToCheck])
                    {
                        coords.Add(new Coords() { X = colToCheck, Y = rowToCheck });
                    }
                    else
                    {
                        foundWord = false;
                        break;
                    }
                }
                if (foundWord)
                    answer = new Answer()
                    {
                        Word = word,
                        Coords = coords
                    };
                return foundWord;
            }
        }
    }

    public enum Direction
    {
        Up,
        UpAndRight,
        Right,
        DownAndRight,
        Down,
        DownAndLeft,
        Left,
        UpAndLeft
    }

    public class Answer
    {
        public string Word { get; set; }
        public List<Coords> Coords { get; set; }
    }

    public struct Coords
    {
        public int X { get; set; }
        public int Y { get; set; }
    }

    public struct DirPossibility
    {
        public Direction Direction { get; set; }
        public bool IsPossible { get; set; }
    }
}
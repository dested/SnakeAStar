﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ConsoleApplication2
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {

                Console.Clear();

                int ticks = 0;
                var board = Board.Start(30, 30, 3, 3, Facing.Up);
                Draw(board);
                while (true)
                {
                    var facing = GetInput(board);
                    if (facing == Facing.None)
                    {
                        Console.SetCursorPosition(0, board.Height + 2);
                        Console.WriteLine($"Dead! {board.Snake.Points.Count} Length in {ticks} ticks.");
                        break;
                    }
                    board.Snake.SetFacing(facing);
                    if (!board.Tick())
                    {
                        Console.SetCursorPosition(0, board.Height + 2);
                        Console.WriteLine($"Dead! {board.Snake.Points.Count} Length in {ticks} ticks.");
                        break;
                    }
                    Draw(board);
                    //                    Thread.Sleep(20);
                    //                    Console.ReadLine();
                    ticks++;
                }
                if (board.Snake.Points.Count > 200)
                    Console.ReadLine();
            }
        }



        private static Facing GetInput(Board board)
        {
            var start = board.Snake.Head;
            var goal = board.Dot;

            var fakeBoard = new Board(board);
            var startSnake = new Snake(board.Snake);

            HashSet<int> closedSet = new HashSet<int>();
            HashSet<int> openSet = new HashSet<int> { start.hashCode };
            Dictionary<int, FacingPoint> cameFrom = new Dictionary<int, FacingPoint>();

            var gScore = new Dictionary<int, double>();
            gScore[start.hashCode] = 0;

            var fScore = new List<Tuple<FacingPoint, Snake, double>>();
            fScore.Add(Tuple.Create(start, startSnake, distance(start, goal)));


            while (openSet.Count > 0)
            {

                var lowest = double.MaxValue;
                int itemIndex = -1;
                Tuple<FacingPoint, Snake, double> item = null;
                for (var index = 0; index < fScore.Count; index++)
                {
                    var tuple = fScore[index];
                    if (tuple.Item3 <= lowest)
                    {
                        item = tuple;
                        lowest = tuple.Item3;
                        itemIndex = index;
                    }
                }

                var currentPoint = item.Item1;
                var currentSnake = item.Item2;
                //                Console.WriteLine(currentPoint + " " + keyValuePair.Key);
                //                Console.ReadLine();

                if (currentPoint.hashCodeNoFacing == goal.hashCodeNoFacing)
                {
                    return reconstruct(cameFrom, currentPoint)[0].Facing;
                }
                openSet.Remove(currentPoint.hashCode);
                closedSet.Add(currentPoint.hashCode);
                var newPoint = false;
                foreach (var neighbor in neighbors(currentPoint))
                {
                    var newSnake = new Snake(currentSnake);
                    newSnake.SetFacing(neighbor.Facing);

                    fakeBoard.Snake = newSnake;

                    if (fakeBoard.Tick(false))
                    {
                        if (closedSet.Contains(neighbor.hashCode))
                        {
                            continue;
                        }
                        var tentative_gScore = gScore[currentPoint.hashCode] + distance(currentPoint, neighbor);

                        if (!openSet.Contains(neighbor.hashCode))
                        {
                            openSet.Add(neighbor.hashCode);
                        }
                        else if (tentative_gScore >= gScore[neighbor.hashCode])
                        {
                            continue;
                        }

                        cameFrom[neighbor.hashCode] = currentPoint;
                        gScore[neighbor.hashCode] = tentative_gScore;

                        fScore.Add(Tuple.Create(neighbor, newSnake, distance(neighbor, goal)));
                        newPoint = true;
                    }
                }
                if (!newPoint)
                {
                    fScore.RemoveAt(itemIndex);
                }

            }

            return Facing.None;
        }

        private static List<FacingPoint> reconstruct(Dictionary<int, FacingPoint> cameFrom, FacingPoint current)
        {
            List<FacingPoint> points = new List<FacingPoint>();
            FacingPoint now;
            points.Add(current);
            now = cameFrom[current.hashCode];

            while (cameFrom.ContainsKey(now.hashCode))
            {
                points.Add(now);
                now = cameFrom[now.hashCode];
            }
            points.Reverse();
            return points;

            /*       total_path := [current]
       while current in cameFrom.Keys:
           current:= cameFrom[current]
           total_path.append(current)
       return total_path*/

        }

        private static FacingPoint[] neighborItems = new FacingPoint[3];
        private static FacingPoint[] neighbors(FacingPoint current)
        {
            switch (current.Facing)
            {
                case Facing.Up:
                    neighborItems[0] = new FacingPoint(current.X, current.Y - 1, Facing.Up);
                    neighborItems[1] = new FacingPoint(current.X - 1, current.Y, Facing.Left);
                    neighborItems[2] = new FacingPoint(current.X + 1, current.Y, Facing.Right);
                    break;
                case Facing.Down:
                    neighborItems[0] = new FacingPoint(current.X, current.Y + 1, Facing.Down);
                    neighborItems[1] = new FacingPoint(current.X - 1, current.Y, Facing.Left);
                    neighborItems[2] = new FacingPoint(current.X + 1, current.Y, Facing.Right);
                    break;
                case Facing.Left:
                    neighborItems[0] = new FacingPoint(current.X - 1, current.Y, Facing.Left);
                    neighborItems[1] = new FacingPoint(current.X, current.Y - 1, Facing.Up);
                    neighborItems[2] = new FacingPoint(current.X, current.Y + 1, Facing.Down);
                    break;
                case Facing.Right:
                    neighborItems[0] = new FacingPoint(current.X + 1, current.Y, Facing.Right);
                    neighborItems[1] = new FacingPoint(current.X, current.Y - 1, Facing.Up);
                    neighborItems[2] = new FacingPoint(current.X, current.Y + 1, Facing.Down);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return neighborItems;
        }

        private static double distance(FacingPoint start, Point goal)
        {
            var x1 = (goal.X - start.X);
            var y1 = (goal.Y - start.Y);

            var x2 = (Board._Width - (goal.X - start.X));
            var y2 = (Board._Height - (goal.Y - start.Y));
            var x = Math.Min(x1 * x1, x2 * x2);
            var y = Math.Min(y1 * y1, y2 * y2);

            var result = Math.Sqrt(x + y);


            return result;
        }

        private static void Draw(Board board)
        {
            for (int y = 0; y < board.Height; y++)
            {
                for (int x = 0; x < board.Width; x++)
                {

                    if (board.Dot.X == x && board.Dot.Y == y)
                    {
                        ConsoleManager.SetPosition(x, y, 'X');
                    }
                    else if (board.Snake.ContainsPoint(x, y))
                    {
                        if (board.Snake.Head.X == x && board.Snake.Head.Y == y)
                        {
                            ConsoleManager.SetPosition(x, y, 'Z');
                        }
                        else
                        {
                            ConsoleManager.SetPosition(x, y, 'Y');
                        }
                    }
                    else
                    {
                        ConsoleManager.SetPosition(x, y, ' ');
                    }

                }
            }
        }
    }

    public class ConsoleManager
    {
        private static char[,] console = new char[100, 100];

        static ConsoleManager()
        {
            for (int x = 0; x < 100; x++)
            {
                for (int y = 0; y < 100; y++)
                {
                    console[x, y] = ' ';
                }
            }
        }

        public static void SetPosition(int x, int y, char c)
        {
            if (console[x, y] != c)
            {
                console[x, y] = c;
                Console.SetCursorPosition(x, y);
                Console.Write(c);
            }
        }

    }

    public class Board
    {
        public static int _Width;
        public static int _Height;
        public int Width { get; }
        public int Height { get; }

        public static Board Start(int width, int height, int startX, int startY, Facing facing)
        {
            _Width = width;
            _Height = height;
            var board = new Board(width, height);
            board.Snake = new Snake(startX, startY, facing);
            board.newDot();
            return board;
        }

        private Board(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public Board(Board original)
        {
            Width = original.Width;
            Height = original.Height;
            Dot = original.Dot;
            Snake = new Snake(original.Snake);
        }
        public Snake Snake { get; set; }
        public Point Dot { get; set; }

        public bool Tick(bool real = true)
        {
            FacingPoint movePoint;
            switch (Snake.Head.Facing)
            {
                case Facing.Up:
                    movePoint = new FacingPoint(Snake.Head.X, Snake.Head.Y - 1, Snake.Head.Facing);
                    break;
                case Facing.Down:
                    movePoint = new FacingPoint(Snake.Head.X, Snake.Head.Y + 1, Snake.Head.Facing);
                    break;
                case Facing.Left:
                    movePoint = new FacingPoint(Snake.Head.X - 1, Snake.Head.Y, Snake.Head.Facing);
                    break;
                case Facing.Right:
                    movePoint = new FacingPoint(Snake.Head.X + 1, Snake.Head.Y, Snake.Head.Facing);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (Snake.ContainsPoint(movePoint))
            {
                return false;
            }
            Snake.InsertPoint(movePoint);

            if (movePoint.hashCodeNoFacing == Dot.hashCodeNoFacing)
            {
                if (real)
                {
                    newDot();
                }
            }
            else
            {
                Snake.RemoveLastPoint();
            }
            return true;
        }

        static Random r = new Random(15695);

        private void newDot()
        {
            while (true)
            {
                Dot = new Point(r.Next(0, Width), r.Next(0, Height));
                if (!Snake.ContainsPoint(Dot))
                {
                    break;
                }
            }
        }
    }

    public class Snake
    {
        public Snake(int x, int y, Facing facing)
        {
            var facingPoint = new FacingPoint(x, y, facing);
            Points = new List<FacingPoint>(2) { facingPoint };
            PointsHash = new List<int>(2) { facingPoint.hashCodeNoFacing };
        }

        public Snake(Snake original)
        {
            Points = new List<FacingPoint>(original.Points.Count + 1);
            Points.AddRange(original.Points);
            PointsHash = new List<int>(original.PointsHash.Count + 1);
            PointsHash.AddRange(original.PointsHash);
        }

        public FacingPoint Head
        {
            get { return Points[0]; }
            set { Points[0] = value; }
        }

        public List<FacingPoint> Points { get; set; }
        private List<int> PointsHash { get; set; }

        public bool ContainsPoint(Point point)
        {
            var pointHashCodeNoFacing = point.hashCodeNoFacing;
            var pointsHashCount = PointsHash.Count;
            for (int i = 0; i < pointsHashCount; i++)
            {
                if (PointsHash[i] == pointHashCodeNoFacing)
                    return true;
            }
            return false;
        }

        internal bool ContainsPoint(int x, int y)
        {
            var pointHashCodeNoFacing = x * 1000 + y;
            var pointsHashCount = PointsHash.Count;
            for (int i = 0; i < pointsHashCount; i++)
            {
                if (PointsHash[i] == pointHashCodeNoFacing)
                    return true;
            }
            return false;
        }
        public void InsertPoint(FacingPoint movePoint)
        {
            Points.Insert(0, movePoint);
            PointsHash.Insert(0, movePoint.hashCodeNoFacing);
        }

        public void RemoveLastPoint()
        {
            var last = Points.Count - 1;
            Points.RemoveAt(last);
            PointsHash.RemoveAt(last);

        }

        public void SetFacing(Facing facing)
        {
            switch (Head.Facing)
            {
                case Facing.Up:

                    switch (facing)
                    {
                        case Facing.Up:
                        case Facing.Left:
                        case Facing.Right:
                            Head = new FacingPoint(Head.X, Head.Y, facing);
                            break;
                        case Facing.Down:
                            throw new Exception("Cannot set this facing");
                        default:
                            throw new ArgumentOutOfRangeException(nameof(facing), facing, null);
                    }

                    break;
                case Facing.Down:
                    switch (facing)
                    {
                        case Facing.Down:
                        case Facing.Left:
                        case Facing.Right:
                            Head = new FacingPoint(Head.X, Head.Y, facing);
                            break;
                        case Facing.Up:
                            throw new Exception("Cannot set this facing");
                        default:
                            throw new ArgumentOutOfRangeException(nameof(facing), facing, null);
                    }
                    break;
                case Facing.Left:
                    switch (facing)
                    {
                        case Facing.Up:
                        case Facing.Left:
                        case Facing.Down:
                            Head = new FacingPoint(Head.X, Head.Y, facing);
                            break;
                        case Facing.Right:
                            throw new Exception("Cannot set this facing");
                        default:
                            throw new ArgumentOutOfRangeException(nameof(facing), facing, null);
                    }
                    break;
                case Facing.Right:
                    switch (facing)
                    {
                        case Facing.Up:
                        case Facing.Right:
                        case Facing.Down:
                            Head = new FacingPoint(Head.X, Head.Y, facing);
                            break;
                        case Facing.Left:
                            throw new Exception("Cannot set this facing");
                        default:
                            throw new ArgumentOutOfRangeException(nameof(facing), facing, null);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

    }

    public class Point
    {
        public int hashCodeNoFacing;
        public override string ToString()
        {
            return $"{nameof(X)}: {X}, {nameof(Y)}: {Y}";
        }

        protected bool Equals(Point other) => other.hashCodeNoFacing == hashCodeNoFacing;

        public override bool Equals(object obj)
        {
            return hashCodeNoFacing == obj.GetHashCode();
        }

        public override int GetHashCode()
        {
            return hashCodeNoFacing;
        }

        public Point(Point point)
        {
            X = point.X;
            Y = point.Y;
            hashCodeNoFacing = X * 1000 + Y;
        }
        public Point(int x, int y)
        {
            if (x < 0)
            {
                x += Board._Width;
            }
            if (x >= Board._Width)
            {
                x -= Board._Width;
            }


            if (y < 0)
            {
                y += Board._Height;
            }
            if (y >= Board._Height)
            {
                y -= Board._Height;
            }

            X = x;
            Y = y;
            hashCodeNoFacing = X * 1000 + Y;
        }

        public int X { get; }
        public int Y { get; }
    }

    public class FacingPoint : Point
    {
        public int hashCode;
        public new int hashCodeNoFacing;

        public override string ToString()
        {
            return $"{nameof(Facing)}: {Facing} {base.ToString()}";
        }

        protected bool Equals(FacingPoint other) => other.hashCode == hashCode;


        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            return Equals((FacingPoint)obj);
        }

        public override int GetHashCode()
        {
            return hashCode;
        }
        public FacingPoint(FacingPoint point) : base(point)
        {
            Facing = point.Facing;
            generateHashCodes();
        }
        public FacingPoint(int x, int y, Facing facing) : base(x, y)
        {
            Facing = facing;
            generateHashCodes();
        }

        private void generateHashCodes()
        {
            hashCode = X * 1000 + Y * 50 + (int)Facing;
            hashCodeNoFacing = X * 1000 + Y;
        }

        public Facing Facing { get; }
    }
    public enum Facing
    {
        Up = 1, Down = 2, Left = 3, Right = 4, None = 1000
    }
}

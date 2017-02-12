using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication2
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                int ticks = 0;
                var board = Board.Start(10, 10, 3, 3, Facing.Up);
                Draw(board);
                while (true)
                {
                    board.Snake.SetFacing(GetInput(board));
                    if (!board.Tick())
                    {
                        Draw(board);
                        Console.WriteLine($"Dead! {board.Snake.Points.Count} Length in {ticks} ticks.");
                        Console.ReadLine();
                        break;
                    }
                    Draw(board);
                    Console.ReadLine();
                    ticks++;
                }
            }
        }



        private static Facing GetInput(Board board)
        {
            var start = board.Snake.Head;
            var goal = board.Dot;

            HashSet<FacingPoint> closedSet = new HashSet<FacingPoint>();
            HashSet<FacingPoint> openSet = new HashSet<FacingPoint>() { start };
            Dictionary<FacingPoint, FacingPoint> cameFrom = new Dictionary<FacingPoint, FacingPoint>();

            var gScore = new Dictionary<FacingPoint, double>();
            gScore[start] = 0;

            var fScore = new Dictionary<FacingPoint, double>();
            fScore[start] = distance(start, goal);
    

            while (openSet.Count > 0)
            {
                var current = fScore.OrderBy(a => a.Value).First().Key;
                if (current.EqualsNoFacing(goal))
                {
                    return cameFrom.First().Key.Facing;
                }
                openSet.Remove(current);
                closedSet.Add(current);

                foreach (var neighbor in neighbors(current))
                {
                    if (closedSet.Contains(neighbor))
                    {
                        continue;
                    }
                    var tentative_gScore = gScore[current] + distance(current, neighbor);

                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                    else if (tentative_gScore >= gScore[neighbor])
                    {
                        continue;
                    }
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentative_gScore;
                    fScore[neighbor] = gScore[neighbor] + distance(neighbor, goal);
                }
                fScore.Remove(current);

            }
            


            return Facing.Down;


            if (board.Dot.X < board.Snake.Head.X)
            {
                if (board.Snake.Head.Facing != Facing.Right)
                {

                    return Facing.Left;
                }
            }
            else if (board.Dot.X > board.Snake.Head.X)
            {
                if (board.Snake.Head.Facing != Facing.Left)
                {
                    return Facing.Right;
                }
            }


            if (board.Dot.Y < board.Snake.Head.Y)
            {
                if (board.Snake.Head.Facing != Facing.Down)
                {
                    return Facing.Up;
                }
            }
            else if (board.Dot.Y > board.Snake.Head.Y)
            {
                if (board.Snake.Head.Facing != Facing.Up)
                {
                    return Facing.Down;
                }
            }
            return Facing.Down;
        }

        private static IEnumerable<FacingPoint> neighbors(FacingPoint current)
        {
            switch (current.Facing)
            {
                case Facing.Up:
                    yield return new FacingPoint(current.X, current.Y - 1, Facing.Up);
                    yield return new FacingPoint(current.X - 1, current.Y, Facing.Left);
                    yield return new FacingPoint(current.X + 1, current.Y, Facing.Right);
                    break;
                case Facing.Down:
                    yield return new FacingPoint(current.X, current.Y + 1, Facing.Down);
                    yield return new FacingPoint(current.X - 1, current.Y, Facing.Left);
                    yield return new FacingPoint(current.X + 1, current.Y, Facing.Right);
                    break;
                case Facing.Left:
                    yield return new FacingPoint(current.X - 1, current.Y, Facing.Left);
                    yield return new FacingPoint(current.X, current.Y - 1, Facing.Up);
                    yield return new FacingPoint(current.X, current.Y + 1, Facing.Down);
                    break;
                case Facing.Right:
                    yield return new FacingPoint(current.X + 1, current.Y, Facing.Right);
                    yield return new FacingPoint(current.X, current.Y - 1, Facing.Up);
                    yield return new FacingPoint(current.X, current.Y + 1, Facing.Down);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

        }

        private static double distance(FacingPoint start, Point goal)
        {
            var x = (goal.X - start.X);
            var y = (goal.Y - start.Y);
            return Math.Sqrt(x * x + y * y);
        }

        private static void Draw(Board board)
        {
            Console.Clear();
            for (int y = 0; y < board.Height; y++)
            {
                for (int x = 0; x < board.Width; x++)
                {

                    if (board.Dot.X == x && board.Dot.Y == y)
                    {
                        Console.Write("X");
                    }
                    else if (board.Snake.Points.Any(a => a.X == x && a.Y == y))
                    {
                        if (board.Snake.Head.X == x && board.Snake.Head.Y == y)
                        {
                            Console.Write("Z");
                        }
                        else
                        {
                            Console.Write("Y");
                        }
                    }
                    else
                    {
                        Console.Write(" ");
                    }

                }
                Console.WriteLine();
            }
        }
    }

    public class Board
    {
        public int Width { get; }
        public int Height { get; }

        public static Board Start(int width, int height, int startX, int startY, Facing facing)
        {
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

        public Snake Snake { get; set; }
        public Point Dot { get; set; }

        public Board(Board original)
        {
            this.Width = original.Width;
            this.Height = original.Height;
            Dot = original.Dot;
            Snake = new Snake(original.Snake);
        }

        public bool Tick()
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

            if (movePoint.X < 0 || movePoint.Y < 0 || movePoint.X >= Width || movePoint.Y >= Height)
            {
                return false;
            }
            foreach (var snakePoint in Snake.Points)
            {
                if (snakePoint.EqualsNoFacing(movePoint))
                {
                    return false;
                }
            }
            Snake.Points.Insert(0, movePoint);

            if (movePoint.EqualsNoFacing(Dot))
            {
                this.newDot();
            }
            else
            {
                Snake.Points.RemoveAt(Snake.Points.Count - 1);
            }
            return true;
        }

        Random r = new Random();

        private void newDot()
        {
            while (true)
            {
                var good = true;
                this.Dot = new Point(r.Next(0, Width), r.Next(0, Height));
                foreach (var snakePoint in Snake.Points)
                {
                    if (snakePoint.Equals(this.Dot))
                    {
                        good = false;
                        break;
                    }
                }
                if (good)
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
            Points = new List<FacingPoint>() { new FacingPoint(x, y, facing), };
        }

        public Snake(Snake original)
        {
            this.Points = original.Points.Select(a => new FacingPoint(a)).ToList();
        }

        public FacingPoint Head => Points[0];
        public List<FacingPoint> Points { get; set; }

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
                            Head.Facing = facing;
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
                            Head.Facing = facing;
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
                            Head.Facing = facing;
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
                            Head.Facing = facing;
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
        protected bool Equals(Point other) => other.X == X && other.Y == Y;

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Point)obj);
        }

        public override int GetHashCode()
        {
            return this.X * 1000 + this.Y;
        }

        public Point(Point point)
        {
            this.X = point.X;
            this.Y = point.Y;
        }
        public Point(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public int X { get; set; }
        public int Y { get; set; }
    }

    public class FacingPoint : Point
    {
        protected bool Equals(FacingPoint other) =>
            other.X == X && other.Y == Y && other.Facing == Facing;

        public bool EqualsNoFacing(Point other) =>
                   other.X == X && other.Y == Y;

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Point)obj);
        }

        public override int GetHashCode()
        {
            return this.X * 1000 + this.Y * 50 + (int)Facing;
        }

        public FacingPoint(FacingPoint point) : base(point)
        {
            this.Facing = point.Facing;
        }
        public FacingPoint(int x, int y, Facing facing) : base(x, y)
        {
            Facing = facing;
        }

        public Facing Facing { get; set; }
    }
    public enum Facing
    {
        Up = 1, Down = 2, Left = 3, Right = 4
    }
}

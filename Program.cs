using System;
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
            var board = Board.Start(10, 10, 3, 3, Facing.Up);
            Draw(board);
            while (true)
            {
                GetInput(board);
                board.Tick();
                Draw(board);
            }
        }

        private static void GetInput(Board board)
        {
            if (board.Dot.X < board.Snake.Head.X)
            {
                if (board.Snake.Facing != Facing.Left)
                {
                    board.Snake.SetFacing(Facing.Left);
                    return;
                }
            }
            else if (board.Dot.X > board.Snake.Head.X)
            {
                if (board.Snake.Facing != Facing.Right)
                {
                    board.Snake.SetFacing(Facing.Right);
                    return;
                }
            }


            if (board.Dot.Y < board.Snake.Head.Y)
            {
                if (board.Snake.Facing != Facing.Up)
                {
                    board.Snake.SetFacing(Facing.Up);
                    return;
                }
            }
            else if (board.Dot.Y > board.Snake.Head.Y)
            {
                if (board.Snake.Facing != Facing.Down)
                {
                    board.Snake.SetFacing(Facing.Right);
                    return;
                }
            }
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

        public void Tick()
        {
            Point movePoint;
            switch (Snake.Facing)
            {
                case Facing.Up:
                    movePoint = new Point(Snake.Head.X, Snake.Head.Y - 1);
                    break;
                case Facing.Down:
                    movePoint = new Point(Snake.Head.X, Snake.Head.Y + 1);
                    break;
                case Facing.Left:
                    movePoint = new Point(Snake.Head.X - 1, Snake.Head.Y);
                    break;
                case Facing.Right:
                    movePoint = new Point(Snake.Head.X + 1, Snake.Head.Y);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (movePoint.X < 0 || movePoint.Y < 0 || movePoint.X >= Width || movePoint.Y >= Height)
            {
                throw new Exception("Dead");
            }
            foreach (var snakePoint in Snake.Points)
            {
                if (snakePoint.Equals(movePoint))
                {
                    throw new Exception("Dead");
                }
            }
            Snake.Points.Insert(0, movePoint);

            if (movePoint.Equals(Dot))
            {
                this.newDot();
            }
            else
            {
                Snake.Points.RemoveAt(Snake.Points.Count - 1);
            }
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
            Points = new List<Point>() { new Point(x, y), };
            Facing = facing;
        }

        public Snake(Snake original)
        {
            this.Points = original.Points.Select(a => new Point(a)).ToList();
            this.Facing = original.Facing;
        }

        public Point Head => Points[0];
        public List<Point> Points { get; set; }
        public Facing Facing { get; set; }

        public void SetFacing(Facing facing)
        {
            switch (Facing)
            {
                case Facing.Up:

                    switch (facing)
                    {
                        case Facing.Up:
                        case Facing.Left:
                        case Facing.Right:
                            this.Facing = facing;
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
                            this.Facing = facing;
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
                            this.Facing = facing;
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
                            this.Facing = facing;
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
            throw new NotImplementedException();
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
    public enum Facing
    {
        Up, Down, Left, Right
    }
}

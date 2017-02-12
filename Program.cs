//#define check
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Bridge.Html5;

namespace SnakeAStar
{
    class Program
    {
        public const int Width = 80;
        public const int Height = 80;
        private static CanvasRenderingContext2D context;
        static void Main(string[] args)
        {
            //                Console.Clear();


            var canvas = (HTMLCanvasElement)Document.CreateElement("canvas");
            canvas.Width = Width * blockSize;
            canvas.Height = Height * blockSize;
            canvas.Style.Border = $"solid {blockSize}px black";
            context = canvas.GetContext(CanvasTypes.CanvasContext2DType.CanvasRenderingContext2D);

            Document.Body.AppendChild(canvas);

            int ticks = 0;
            var board = Board.Start(Width, Height, 3, 3, Facing.Up);
            Draw(board);

            int interval = 0;
            interval = Window.SetInterval(() =>
            {
                var facing = GetInput(board);
                if (facing == Facing.None)
                {
                    //                        Console.SetCursorPosition(0, board.Height + 2);
                    Console.WriteLine($"Dead! {board.Snake.Points.Count} Length in {ticks} ticks.");
                    Window.ClearInterval(interval);
                    return;
                }
                board.Snake.SetFacing(facing);
                if (!board.Tick())
                {
                    //                        Console.SetCursorPosition(0, board.Height + 2);
                    Console.WriteLine($"Dead! {board.Snake.Points.Count} Length in {ticks} ticks.");
                    Window.ClearInterval(interval);
                    return;
                }
                Draw(board);
                //                                                            Thread.Sleep(20);
                ticks++;

            }, 0);
        }


        static List<Facing> cachedPoints = new List<Facing>();

        private static Facing GetInput(Board board)
        {

            if (cachedPoints.Count > 0)
            {
                var point = cachedPoints[0];
                cachedPoints.RemoveAt(0);
                return point;
            }

            var start = board.Snake.Head;
            var goal = board.Dot;

            var fakeBoard = new Board(board);
            var startSnake = new Snake(board.Snake);

            List<int> closedSet = new List<int>();
            List<int> openSet = new List<int> { start.hashCode };
            Dictionary<int, Point> cameFrom = new Dictionary<int, Point>();

            var gScore = new Dictionary<int, double>();
            gScore[start.hashCode] = 0;

            var fScore = new List<Tuple<Point, Snake, double>>();
            fScore.Add(Tuple.Create(start, startSnake, distance(start, goal)));


            while (openSet.Count > 0)
            {

                var lowest = double.MaxValue;
                int itemIndex = -1;
                Tuple<Point, Snake, double> item = null;
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
                    cachedPoints = reconstruct(cameFrom, currentPoint);
                    return GetInput(board);
                }
                var currentPointHashCode = currentPoint.hashCode;

                openSet.Remove(currentPointHashCode);
                closedSet.Add(currentPointHashCode);
                var newPoint = false;
                foreach (var neighbor in neighbors(currentPoint))
                {
                    var newSnake = new Snake(currentSnake);
                    newSnake.SetFacing(neighbor.Facing);

                    fakeBoard.Snake = newSnake;

                    if (fakeBoard.Tick(false))
                    {
                        var neighborHashCode = neighbor.hashCode;
                        if (closedSet.Contains(neighborHashCode))
                        {
                            continue;
                        }
                        var tentative_gScore = gScore[currentPointHashCode] + distance(currentPoint, neighbor);

                        if (!openSet.Contains(neighborHashCode))
                        {
                            openSet.Add(neighborHashCode);
                        }
                        else if (tentative_gScore >= gScore[neighborHashCode])
                        {
                            continue;
                        }

                        cameFrom[neighborHashCode] = currentPoint;
                        gScore[neighborHashCode] = tentative_gScore;

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

        private static List<Facing> reconstruct(Dictionary<int, Point> cameFrom, Point current)
        {
            List<Facing> points = new List<Facing>();
            Point now;
            points.Add(current.Facing);
            now = cameFrom[current.hashCode];

            while (cameFrom.ContainsKey(now.hashCode))
            {
                points.Add(now.Facing);
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

        private static Point[] neighborItems = new Point[3];
        private static Point[] neighbors(Point current)
        {
            switch (current.Facing)
            {
                case Facing.Up:
                    neighborItems[0] = Point.GetPoint(current.X, current.Y - 1, Facing.Up);
                    neighborItems[1] = Point.GetPoint(current.X - 1, current.Y, Facing.Left);
                    neighborItems[2] = Point.GetPoint(current.X + 1, current.Y, Facing.Right);
                    break;
                case Facing.Down:
                    neighborItems[0] = Point.GetPoint(current.X, current.Y + 1, Facing.Down);
                    neighborItems[1] = Point.GetPoint(current.X - 1, current.Y, Facing.Left);
                    neighborItems[2] = Point.GetPoint(current.X + 1, current.Y, Facing.Right);
                    break;
                case Facing.Left:
                    neighborItems[0] = Point.GetPoint(current.X - 1, current.Y, Facing.Left);
                    neighborItems[1] = Point.GetPoint(current.X, current.Y - 1, Facing.Up);
                    neighborItems[2] = Point.GetPoint(current.X, current.Y + 1, Facing.Down);
                    break;
                case Facing.Right:
                    neighborItems[0] = Point.GetPoint(current.X + 1, current.Y, Facing.Right);
                    neighborItems[1] = Point.GetPoint(current.X, current.Y - 1, Facing.Up);
                    neighborItems[2] = Point.GetPoint(current.X, current.Y + 1, Facing.Down);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return neighborItems;
        }

        private static double distance(Point start, Point goal)
        {
            var x1 = (goal.X - start.X);
            var y1 = (goal.Y - start.Y);

            var x2 = (Width - (goal.X - start.X));
            var y2 = (Height - (goal.Y - start.Y));
            var x = Math.Min(x1 * x1, x2 * x2);
            var y = Math.Min(y1 * y1, y2 * y2);

            var result = Math.Sqrt(x + y);

            return result;
        }

        private const int blockSize = 5;
        private static void Draw(Board board)
        {
            context.ClearRect(0, 0, blockSize * Width, blockSize * Height);

            var snakeHead = board.Snake.Head;
            for (int y = 0; y < board.Height; y++)
            {
                for (int x = 0; x < board.Width; x++)
                {
                    if (board.Dot.X == x && board.Dot.Y == y)
                    {
                        context.FillStyle = "red";
                        context.FillRect(x * blockSize, y * blockSize, blockSize, blockSize);
                    }
                    else if (board.Snake.ContainsPoint(x, y))
                    {
                        if (snakeHead.X == x && snakeHead.Y == y)
                        {
                            context.FillStyle = "light blue";
                            context.FillRect(x * blockSize, y * blockSize, blockSize, blockSize);
                        }
                        else
                        {
                            context.FillStyle = "blue";
                            context.FillRect(x * blockSize, y * blockSize, blockSize, blockSize);
                        }
                    }
                    else
                    {
                        //                        ConsoleManager.SetPosition(x, y, ' ');
                    }

                }
            }
        }
    }


    public class Board
    {
        public readonly int Width;
        public readonly int Height;

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

        public Board(Board original)
        {
            Width = original.Width;
            Height = original.Height;
            Dot = original.Dot;
            Snake = new Snake(original.Snake);
        }

        public Snake Snake;
        public Point Dot;

        public bool Tick(bool real = true)
        {
            Point movePoint;
            var snakeHead = Snake.Head;

            switch (snakeHead.Facing)
            {
                case Facing.Up:
                    movePoint = Point.GetPoint(snakeHead.X, snakeHead.Y - 1, snakeHead.Facing);
                    break;
                case Facing.Down:
                    movePoint = Point.GetPoint(snakeHead.X, snakeHead.Y + 1, snakeHead.Facing);
                    break;
                case Facing.Left:
                    movePoint = Point.GetPoint(snakeHead.X - 1, snakeHead.Y, snakeHead.Facing);
                    break;
                case Facing.Right:
                    movePoint = Point.GetPoint(snakeHead.X + 1, snakeHead.Y, snakeHead.Facing);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (Snake.ContainsPoint(movePoint.hashCodeNoFacing))
            {
                return false;
            }

            if (movePoint.hashCodeNoFacing == Dot.hashCodeNoFacing)
            {
                Snake.InsertPoint(movePoint);
                if (real)
                {
                    newDot();
                }
            }
            else
            {
                Snake.InsertAndMove(movePoint);
            }
            return true;
        }

        static Random r = new Random(15659);

        private void newDot()
        {
            while (true)
            {
                Dot = Point.GetPoint(r.Next(0, Width), r.Next(0, Height), Facing.None);
                if (!Snake.ContainsPoint(Dot.hashCodeNoFacing))
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
            var facingPoint = Point.GetPoint(x, y, facing);
            Points = new List<Point>(2) { facingPoint };
        }

        public Snake(Snake original)
        {
            Points = new List<Point>(original.Points.Count + 1);
            Points.AddRange(original.Points);
        }

        public Point Head
        {
            get { return Points[0]; }
            set { Points[0] = value; }
        }

        public List<Point> Points;

        internal bool ContainsPoint(int x, int y)
        {
            var pointHashCodeNoFacing = x * 100000 + y;
            return ContainsPoint(pointHashCodeNoFacing);
        }
        internal bool ContainsPoint(int hashCodeNoFacing)
        {
            var pointCount = Points.Count;
            for (int i = 0; i < pointCount; i++)
            {
                if (Points[i].hashCodeNoFacing == hashCodeNoFacing)
                    return true;
            }
            return false;
        }

        public void InsertPoint(Point movePoint)
        {
            Points.Insert(0, movePoint);
        }

        public void InsertAndMove(Point movePoint)
        {
            Points.Insert(0, movePoint);
            var last = Points.Count - 1;
            Points.RemoveAt(last);
        }

        public void SetFacing(Facing facing)
        {
#if check
            switch (Head.Facing)
            {
                case Facing.Up:
                    switch (facing)
                    {
                        case Facing.Up:
                        case Facing.Left:
                        case Facing.Right:
                            Head = Point.GetPoint(Head.X, Head.Y, facing);
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
                            Head = Point.GetPoint(Head.X, Head.Y, facing);
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
                            Head = Point.GetPoint(Head.X, Head.Y, facing);
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
                            Head = Point.GetPoint(Head.X, Head.Y, facing);
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
#else
            Head = Point.GetPoint(Head.X, Head.Y, facing);
#endif

        }

    }


    public class Point
    {
        static Dictionary<int, Point> facingPoints = new Dictionary<int, Point>();

        static Point()
        {
            for (int x = 0; x < Program.Width; x++)
            {
                for (int y = 0; y < Program.Height; y++)
                {
                    facingPoints.Add(x * 100000 + y * 10 + 1, new Point(x, y, Facing.Up));
                    facingPoints.Add(x * 100000 + y * 10 + 2, new Point(x, y, Facing.Down));
                    facingPoints.Add(x * 100000 + y * 10 + 3, new Point(x, y, Facing.Left));
                    facingPoints.Add(x * 100000 + y * 10 + 4, new Point(x, y, Facing.Right));
                    facingPoints.Add(x * 100000 + y * 10 + 5, new Point(x, y, Facing.None));
                }
            }
        }

        public static Point GetPoint(int x, int y, Facing facing)
        {
            if (x < 0)
            {
                x += Program.Width;
            }
            else if (x >= Program.Width)
            {
                x -= Program.Width;
            }

            if (y < 0)
            {
                y += Program.Height;
            }
            else if (y >= Program.Height)
            {
                y -= Program.Height;
            }

            return facingPoints[x * 100000 + y * 10 + (int)facing];
        }


        public int hashCode;
        public int hashCodeNoFacing;

        public override string ToString()
        {
            return $"{nameof(Facing)}: {Facing} {X} {Y}";
        }


        private Point(int x, int y, Facing facing)
        {
            X = x;
            Y = y;
            Facing = facing;
            generateHashCodes();
        }

        private void generateHashCodes()
        {
            hashCode = X * 100000 + Y * 10 + (int)Facing;
            hashCodeNoFacing = X * 100000 + Y;
        }

        public readonly int X;
        public readonly int Y;
        public readonly Facing Facing;
    }
    public enum Facing
    {
        Up = 1, Down = 2, Left = 3, Right = 4, None = 5
    }
}

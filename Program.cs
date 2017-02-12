using System;
using System.Collections.Generic;
using System.Linq;

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
                    //                    Console.ReadLine();
                    ticks++;
                }
                Console.ReadLine();
            }
        }



        private static Facing GetInput(Board board)
        {
            var start = board.Snake.Head;
            var goal = board.Dot;

            var startBoard = new Board(board);

            HashSet<FacingPoint> closedSet = new HashSet<FacingPoint>();
            HashSet<FacingPoint> openSet = new HashSet<FacingPoint> { start };
            Dictionary<FacingPoint, FacingPoint> cameFrom = new Dictionary<FacingPoint, FacingPoint>();

            var gScore = new Dictionary<FacingPoint, double>();
            gScore[start] = 0;

            var fScore = new Dictionary<FacingPoint, Tuple<FacingPoint, Board, double>>();
            fScore[start] = Tuple.Create(start, startBoard, distance(start, goal));


            while (openSet.Count > 0)
            {
                var keyValuePair = fScore.OrderBy(a => a.Value.Item3).First();
                var currentItem = keyValuePair.Value;
                var currentPoint = currentItem.Item1;
                var currentBoard = currentItem.Item2;
                //                Console.WriteLine(currentPoint + " " + keyValuePair.Key);
                //                Console.ReadLine();

                if (currentPoint.EqualsNoFacing(goal))
                {
                    return reconstruct(cameFrom, currentPoint)[0].Facing;
                }
                openSet.Remove(currentPoint);
                closedSet.Add(currentPoint);
                var newPoint = false;
                foreach (var neighbor in neighbors(currentPoint))
                {
                    var newBoard = new Board(currentBoard);
                    newBoard.Snake.SetFacing(neighbor.Facing);
                    if (newBoard.Tick(false))
                    {
                        if (closedSet.Contains(neighbor))
                        {
                            continue;
                        }
                        var tentative_gScore = gScore[currentPoint] + distance(currentPoint, neighbor);

                        if (!openSet.Contains(neighbor))
                        {
                            openSet.Add(neighbor);
                        }
                        else if (tentative_gScore >= gScore[neighbor])
                        {
                            continue;
                        }

                        cameFrom[neighbor] = currentPoint;
                        gScore[neighbor] = tentative_gScore;

                        fScore[neighbor] = Tuple.Create(neighbor, newBoard, distance(neighbor, goal));
                        newPoint = true;
                    }
                }
                if (!newPoint)
                {
                    fScore.Remove(keyValuePair.Key);
                }

            }

            return Facing.None;
        }

        private static List<FacingPoint> reconstruct(Dictionary<FacingPoint, FacingPoint> cameFrom, FacingPoint current)
        {
            List<FacingPoint> points = new List<FacingPoint>();
            FacingPoint now;
            points.Add(current);
            now = cameFrom[current];

            while (cameFrom.ContainsKey(now))
            {
                points.Add(now);
                now = cameFrom[now];
            }
            points.Reverse();
            return points;

            /*       total_path := [current]
       while current in cameFrom.Keys:
           current:= cameFrom[current]
           total_path.append(current)
       return total_path*/

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

            var result = Math.Sqrt(x * x + y * y);
            if (goal.X - start.X == 0)
            {
                if (goal.Y > start.Y && start.Facing == Facing.Up) return result + 3;
                if (goal.Y < start.Y && start.Facing == Facing.Down) return result + 3;
            }

            if (goal.Y - start.Y == 0)
            {
                if (goal.X > start.X && start.Facing == Facing.Left) return result + 3;
                if (goal.X < start.X && start.Facing == Facing.Right) return result + 3;
            }

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

        public Snake Snake { get; set; }
        public Point Dot { get; set; }

        public Board(Board original)
        {
            Width = original.Width;
            Height = original.Height;
            Dot = original.Dot;
            Snake = new Snake(original.Snake);
        }

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

            if (movePoint.EqualsNoFacing(Dot))
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

        static Random r = new Random(15694);

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
            PointsHash = new List<int>(2) { facingPoint.GetHashCodeNoFacing() };
        }

        public Snake(Snake original)
        {
            Points = new List<FacingPoint>(original.Points.Count + 1);
            Points.AddRange(original.Points);
            PointsHash = new List<int>(original.PointsHash.Count);
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
            return PointsHash.Contains(point.X * 1000 + point.Y);
        }

        internal bool ContainsPoint(int x, int y)
        {
            return PointsHash.Contains(x * 1000 + y);
        }
        public void InsertPoint(FacingPoint movePoint)
        {
            Points.Insert(0, movePoint);
            PointsHash.Add(movePoint.GetHashCodeNoFacing());
        }

        public void RemoveLastPoint()
        {
            var point = Points[Points.Count - 1];
            Points.RemoveAt(Points.Count - 1);
            PointsHash.Remove(point.GetHashCodeNoFacing());

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
        public override string ToString()
        {
            return $"{nameof(X)}: {X}, {nameof(Y)}: {Y}";
        }

        protected bool Equals(Point other) => other.X == X && other.Y == Y;

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            return Equals((Point)obj);
        }

        public override int GetHashCode()
        {
            return X * 1000 + Y;
        }

        public Point(Point point)
        {
            X = point.X;
            Y = point.Y;
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
        }

        public int X { get; }
        public int Y { get; }
    }

    public class FacingPoint : Point
    {
        public override string ToString()
        {
            return $"{nameof(Facing)}: {Facing} {base.ToString()}";
        }

        protected bool Equals(FacingPoint other) =>
            other.X == X && other.Y == Y && other.Facing == Facing;

        public bool EqualsNoFacing(Point other) =>
                   other.X == X && other.Y == Y;

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            return Equals((Point)obj);
        }

        public override int GetHashCode()
        {
            return X * 1000 + Y * 50 + (int)Facing;
        }
        public int GetHashCodeNoFacing()
        {
            return X * 1000 + Y;
        }

        public FacingPoint(FacingPoint point) : base(point)
        {
            Facing = point.Facing;
        }
        public FacingPoint(int x, int y, Facing facing) : base(x, y)
        {
            Facing = facing;
        }

        public Facing Facing { get; }
    }
    public enum Facing
    {
        Up = 1, Down = 2, Left = 3, Right = 4, None = 1000
    }
}

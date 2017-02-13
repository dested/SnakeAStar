using System;
using System.Collections.Generic;

namespace SnakeAStar
{
    public static class ASTarSolver
    {

        static List<Facing> cachedPoints = new List<Facing>();

        public static Facing GetInput(Board board)
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

            var x2 = (Program.Width - (goal.X - start.X));
            var y2 = (Program.Height - (goal.Y - start.Y));
            var x = Math.Min(x1 * x1, x2 * x2);
            var y = Math.Min(y1 * y1, y2 * y2);

            var result = Math.Sqrt(x + y);

            return result;
        }

    }
}
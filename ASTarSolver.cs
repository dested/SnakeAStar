using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Bridge.Html5;

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
            var startTicks = Window.Performance.Now();
            var start = board.Snake.Head;
            var goal = board.Dot;

            var fakeBoard = new Board(board);

            List<int> closedSet = new List<int>();
            List<int> openSet = new List<int> { start.hashCode };
            Dictionary<int, Point> cameFrom = new Dictionary<int, Point>();

            var gScore = new Dictionary<int, double>();
            gScore[start.hashCode] = 0;

            var fScore = new List<Tuple<Snake, double>>();
            fScore.Add(Tuple.Create(new Snake(board.Snake), distance(start, goal)));

            while (openSet.Count > 0)
            {

                var lowest = double.MaxValue;
                int itemIndex = -1;
                Snake currentSnake = null;
                for (var index = 0; index < fScore.Count; index++)
                {
                    var tuple = fScore[index];
                    if (tuple.Item2 <= lowest)
                    {
                        currentSnake = tuple.Item1;
                        lowest = tuple.Item2;
                        itemIndex = index;
                    }
                }

                var currentHead = currentSnake.Head;
                if (currentHead.hashCodeNoFacing == goal.hashCodeNoFacing)
                {
                    cachedPoints = reconstruct(cameFrom, currentHead);

                    var endTicks = Window.Performance.Now();
                    Console.WriteLine($"{endTicks - startTicks} ticks with {cachedPoints.Count} moves {currentSnake.Points.Count} length");

//                    return cachedPoints.First();
                    return GetInput(board);
                }
                var currentPointHashCode = currentHead.hashCode;

                openSet.Remove(currentPointHashCode);
                closedSet.Add(currentPointHashCode);
                var newPoint = false;
                foreach (var neighbor in Neighbors(currentHead))
                {
                    var newSnake = new Snake(currentSnake);
                    newSnake.SetFacing(neighbor);

                    fakeBoard.Snake = newSnake;

                    if (fakeBoard.Tick(false))
                    {
                        var newHead = newSnake.Head;

                        var neighborHashCode = newHead.hashCode;
                        if (closedSet.Contains(neighborHashCode))
                        {
                            continue;
                        }
                        var tentative_gScore = gScore[currentPointHashCode] + distance(currentHead, newHead);

                        if (!openSet.Contains(neighborHashCode))
                        {
                            openSet.Add(neighborHashCode);
                        }
                        else if (tentative_gScore >= gScore[neighborHashCode])
                        {
                            continue;
                        }

                        cameFrom[neighborHashCode] = currentHead;
                        gScore[neighborHashCode] = tentative_gScore;

                        fScore.Add(Tuple.Create(newSnake, distance(newHead, goal)));
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
            points.Add(current.Facing);

            while (cameFrom.ContainsKey(current.hashCode))
            {
                current = cameFrom[current.hashCode];
                points.Add(current.Facing);
            }
            points.Reverse();
            points.RemoveAt(0);
            return points;
        }

        private static readonly Facing[] neighborCache = new Facing[3];
        private static Facing[] Neighbors(Point current)
        {
            switch (current.Facing)
            {
                case Facing.Up:
                    neighborCache[0] = Facing.Up;
                    neighborCache[1] = Facing.Left;
                    neighborCache[2] = Facing.Right;
                    break;
                case Facing.Down:
                    neighborCache[0] = Facing.Down;
                    neighborCache[1] = Facing.Left;
                    neighborCache[2] = Facing.Right;
                    break;
                case Facing.Left:
                    neighborCache[0] = Facing.Left;
                    neighborCache[1] = Facing.Up;
                    neighborCache[2] = Facing.Down;
                    break;
                case Facing.Right:
                    neighborCache[0] = Facing.Right;
                    neighborCache[1] = Facing.Up;
                    neighborCache[2] = Facing.Down;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return neighborCache;
        }

        private static double distance(Point start, Point goal)
        {

            var x1 = ((goal.X - start.X) + Program.Width) % Program.Width;
            var y1 = ((goal.Y - start.Y) + Program.Height) % Program.Height;
            var x2 = ((start.X - goal.X) + Program.Width) % Program.Width;
            var y2 = ((start.Y - goal.Y) + Program.Height) % Program.Height;


            var result = Math.Sqrt(Math.Min(x1 * x1, x2 * x2) + Math.Min(y1 * y1, y2 * y2));
            /*
                        var x1 = goal.X - start.X;
                        var y1 = goal.Y - start.Y;

                        var result = Math.Sqrt(x1 * x1 + y1 * y1);
            */

            return result;
        }

    }
}
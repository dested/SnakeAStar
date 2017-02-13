using System.Collections.Generic;

namespace SnakeAStar
{
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

        public int[] Tails
        {
            get
            {
                List<int> points=new List<int>();
                for (var index = Points.Count-3;index>=0 && index < Points.Count; index++)
                {
                    var point = Points[index];
                    points.Add(point.hashCodeNoFacing);
                }
                return points.ToArray();
            }
        }

        public List<Point> Points;

        internal bool ContainsPoint(int x, int y)
        {
            var pointHashCodeNoFacing = x * 100000 + y;
            return ContainsPointWithOffset(pointHashCodeNoFacing,0);
        }
        internal bool ContainsPointWithOffset(int hashCodeNoFacing,int offset)
        {
            var pointCount = Points.Count - offset;
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
}
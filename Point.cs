using System.Collections.Generic;

namespace SnakeAStar
{
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
}
using System;

namespace SnakeAStar
{
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

        static Random r = new Random(/*15659*/);

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
}
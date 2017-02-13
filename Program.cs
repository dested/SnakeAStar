//#define check
using System;
using System.Linq;
using System.Threading;
using Bridge.Html5;

namespace SnakeAStar
{
    class Program
    {
        public const int Width = 80;
        public const int Height = 80;
        public const int BlockSize = 5;

        private static CanvasRenderingContext2D context;
        static void Main(string[] args)
        {
            //                Console.Clear();


            var canvas = (HTMLCanvasElement)Document.CreateElement("canvas");
            canvas.Width = Width * BlockSize;
            canvas.Height = Height * BlockSize;
            context = canvas.GetContext(CanvasTypes.CanvasContext2DType.CanvasRenderingContext2D);
            ((dynamic)context).mozImageSmoothingEnabled = false; /// future
            ((dynamic)context).msImageSmoothingEnabled = false; /// future
            ((dynamic)context).imageSmoothingEnabled = false; /// future
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
                    Console.WriteLine($"Dead! {board.Snake.Points.Count} Length in {ticks} ticks.");
                    Window.ClearInterval(interval);
                    return;
                }
                board.Snake.SetFacing(facing);
                if (!board.Tick())
                {
                    Console.WriteLine($"Dead! {board.Snake.Points.Count} Length in {ticks} ticks.");
                    Window.ClearInterval(interval);
                    return;
                }
                Draw(board);
                //                                                            Thread.Sleep(20);
                ticks++;

            }, 0);
        }


        private static Facing GetInput(Board board)
        {
            return ASTarSolver.GetInput(board);
        }

        private static void Draw(Board board)
        {
            var snakeHead = board.Snake.Head;
            for (int y = 0; y < board.Height; y++)
            {
                for (int x = 0; x < board.Width; x++)
                {
                    if (board.Dot.X == x && board.Dot.Y == y)
                    {
                        ScreenManager.SetPosition(context, x, y, "red");

                    }
                    else if (board.Snake.ContainsPoint(x, y))
                    {
                        if (snakeHead.X == x && snakeHead.Y == y)
                        {
                            ScreenManager.SetPosition(context, x, y, "green");
                        }
                        else
                        {
                            ScreenManager.SetPosition(context, x, y, "blue");
                        }
                    }
                    else
                    {
                        ScreenManager.SetPosition(context, x, y, "white");
                    }

                }
            }
        }
    }
}

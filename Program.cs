//#define check
using System;
using System.Linq;
using System.Threading;
using Bridge;
using Bridge.Html5;

namespace SnakeAStar
{
    class Program
    {
        public const int Width = 100;
        public const int Height = 100;
        public const int BlockSize = 1;

        private static CanvasRenderingContext2D context;
        static void Main(string[] args)
        {
            //                Console.Clear();


            var canvas = (HTMLCanvasElement)Document.CreateElement("canvas");
            canvas.Width = Width * BlockSize;
            canvas.Height = Height * BlockSize;
            context = canvas.GetContext(CanvasTypes.CanvasContext2DType.CanvasRenderingContext2D);
            ((dynamic)context).mozImageSmoothingEnabled = false;
            ((dynamic)context).msImageSmoothingEnabled = false;
            ((dynamic)context).imageSmoothingEnabled = false;
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
                    Draw(board);

                    /*  
                     Window.Alert($"Dead, no moves! {board.Snake.Points.Count} Length in {ticks} ticks.");
                     Window.ClearInterval(interval);
                      return;*/
                    board = Board.Start(Width, Height, 3, 3, Facing.Up);
                    return;
                }
                board.Snake.SetFacing(facing);
                if (!board.Tick())
                {
                    Draw(board);
                    Window.Alert($"Dead collided! {board.Snake.Points.Count} Length in {ticks} ticks.");
                    Window.ClearInterval(interval);
                    return;
                }
                Draw(board);
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
//            var tails = board.Snake.Tails;
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
                       /* else if (tails.Contains(x * 100000 + y))
                        {
                            ScreenManager.SetPosition(context, x, y, "yellow");
                        }*/
                        else
                        {
                            ScreenManager.SetPosition(context, x, y, "blue");

                        }
                    }
                    else
                    {
                        ScreenManager.SetPosition(context, x, y, "transparent");
                    }

                }
            }
        }
        [Init(InitPosition.Top)]
        private static void DisableConsole()
        {
            /*@
            Bridge.Console.log = function(message) { console.log(message); };
            Bridge.Console.error = function(message) { console.error(message); };
            Bridge.Console.debug = function(message) { console.debug(message); };
            */
        }
    }


}

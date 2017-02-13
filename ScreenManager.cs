using Bridge.Html5;

namespace SnakeAStar
{
    public class ScreenManager
    {
        private static string[] console = new string[Program.Width * Program.Height];
        private static string currentFillStyle = null;
        static ScreenManager()
        {
            for (int x = 0; x < Program.Width; x++)
            {
                for (int y = 0; y < Program.Height; y++)
                {
                    console[x * Program.Width + y] = "transparent";
                }
            }
        }

        public static void SetPosition(CanvasRenderingContext2D context, int x, int y, string color)
        {
            if (console[x * Program.Width + y] != color)
            {
                console[x * Program.Width + y] = color;

                if (color == "transparent")
                {
                    context.ClearRect(x * Program.BlockSize, y * Program.BlockSize, Program.BlockSize, Program.BlockSize);
                }
                else
                {
                    if (currentFillStyle != color)
                    {
                        context.FillStyle = color;
                        currentFillStyle = color;
                    }
                    context.FillRect(x * Program.BlockSize, y * Program.BlockSize, Program.BlockSize, Program.BlockSize);
                }

            }
        }

    }
}
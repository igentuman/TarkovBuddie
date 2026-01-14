using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Imaging;

namespace TARKIT.Services;

public class ScreenCaptureService : IDisposable
{
    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

    [StructLayout(LayoutKind.Sequential)]
    private struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    public struct CaptureRegion
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public CaptureRegion(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }
    }

    private readonly int _captureIntervalMs;
    private bool _disposed;

    public ScreenCaptureService(int captureIntervalMs = 2000)
    {
        _captureIntervalMs = captureIntervalMs;
    }

    public Bitmap? CaptureScreenRegion(CaptureRegion region)
    {
        try
        {
            if (region.Width <= 0 || region.Height <= 0)
                return null;

            var bitmap = new Bitmap(region.Width, region.Height, PixelFormat.Format32bppArgb);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.CopyFromScreen(
                    new Point(region.X, region.Y),
                    Point.Empty,
                    new Size(region.Width, region.Height));
            }

            return bitmap;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error capturing region: {ex.Message}");
            return null;
        }
    }

    public Bitmap? CaptureMapExitsRegion()
    {
        try
        {
            var screenBounds = System.Windows.Forms.Screen.PrimaryScreen?.Bounds;
            if (screenBounds == null)
                return null;

            int screenWidth = screenBounds.Value.Width;
            int screenHeight = screenBounds.Value.Height;

            int captureWidth = (int)(screenWidth * 0.20);
            int captureHeight = (int)(screenHeight * 0.5);
            int captureX = screenWidth - (int)(captureWidth*1.5);
            int captureY = 0;

            Bitmap capture = CaptureScreenRegion(new CaptureRegion(captureX, captureY, captureWidth, captureHeight));
            //leave only white and black pixels
            if (capture != null)
            {
                for (int i = 0; i < capture.Width; i++)
                {
                    for (int j = 0; j < capture.Height; j++)
                    {
                        Color pixelColor = capture.GetPixel(i, j);
                        if (pixelColor.R >= 240 && pixelColor.G >= 240 && pixelColor.B >= 240) //white color
                            continue;
                        if (pixelColor.R <= 5 && pixelColor.G <= 5 && pixelColor.B <= 5) //black color
                            continue;
                        capture.SetPixel(i, j, Color.LawnGreen);
                    }
                }
            }

            return capture;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error capturing map exits region: {ex.Message}");
            return null;
        }
    }

    public void SaveBitmapToFile(Bitmap bitmap, string filePath)
    {
        try
        {
            bitmap.Save(filePath, ImageFormat.Png);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error saving bitmap: {ex.Message}");
        }
    }

    public int CaptureIntervalMs => _captureIntervalMs;

    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;
    }
}

using System.Drawing.Imaging;

namespace TARKIT.Services;

public class ExitTrackerService
{
    private static ExitTrackerService? _instance;
    private static readonly object _lock = new();

    private readonly ScreenCaptureService _captureService;
    private readonly OcrProcessingService _ocrService;
    private Thread? _captureThread;
    private CancellationTokenSource? _cancellationTokenSource;
    private bool _isRunning;

    public event Action<List<string>>? ExitsDetected;
    public event Action<Exception>? ErrorOccurred;

    public bool IsRunning => _isRunning;

    public static ExitTrackerService Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    _instance ??= new ExitTrackerService();
                }
            }
            return _instance;
        }
    }

    private ExitTrackerService()
    {
        _captureService = new ScreenCaptureService(captureIntervalMs: 2000);
        _ocrService = new OcrProcessingService();
    }

    public void Start()
    {
        if (_isRunning)
            return;

        _isRunning = true;
        _cancellationTokenSource = new CancellationTokenSource();

        _captureThread = new Thread(() => CaptureAndProcessLoop(_cancellationTokenSource.Token))
        {
            Name = "ExitTrackerThread",
            IsBackground = true,
            Priority = ThreadPriority.BelowNormal
        };

        _captureThread.Start();
        System.Diagnostics.Debug.WriteLine("Exit Tracker started");
    }

    public void Stop()
    {
        if (!_isRunning)
            return;

        _isRunning = false;
        _cancellationTokenSource?.Cancel();

        if (_captureThread != null && _captureThread.IsAlive)
        {
            _captureThread.Join(5000);
        }

        System.Diagnostics.Debug.WriteLine("Exit Tracker stopped");
    }

    private void CaptureAndProcessLoop(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var screenshot = _captureService.CaptureMapExitsRegion();
                if (screenshot == null)
                {
                    Thread.Sleep(_captureService.CaptureIntervalMs);
                    continue;
                }
                screenshot.Save("test.png", ImageFormat.Png);
                //return;
                string ocrText = _ocrService.ProcessImage(screenshot);
                
                var detectedExits = _ocrService.ExtractExits(ocrText);

                if (detectedExits.Count > 0)
                {
                    ExitsDetected?.Invoke(detectedExits);
                }

                screenshot.Dispose();
                Thread.Sleep(_captureService.CaptureIntervalMs);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in capture loop: {ex.Message}");
                ErrorOccurred?.Invoke(ex);
                Thread.Sleep(_captureService.CaptureIntervalMs);
            }
        }
    }

    public void Dispose()
    {
        Stop();
        _captureService?.Dispose();
        _ocrService?.Dispose();
    }
}

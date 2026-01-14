using System.Drawing;
using System.IO;
using System.Text.Json;
using Tesseract;

namespace TARKIT.Services;

public class OcrProcessingService : IDisposable
{
    private readonly string _tesseractDataPath;
    private readonly string _exitsDataPath;
    private TesseractEngine? _engine;
    private bool _disposed;
    private readonly object _lockObject = new();
    private Dictionary<string, List<string>> _mapExits = new();
    private readonly string[] _mapNames = { "customs", "factory", "interchange", "lab", "labrynth", 
        "lighthouse", "reserve", "shoreline", "streets", "terminal", "woods" };

    public OcrProcessingService(string? customTesseractPath = null)
    {
        _tesseractDataPath = customTesseractPath ?? 
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data/ocr");
        _exitsDataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data/ru/exits");
        
        InitializeEngine();
        LoadExitData();
    }

    private void InitializeEngine()
    {
        lock (_lockObject)
        {
            try
            {
                if (!Directory.Exists(_tesseractDataPath))
                {
                    System.Diagnostics.Debug.WriteLine(
                        $"Warning: Tesseract data path not found: {_tesseractDataPath}");
                    return;
                }

                System.Diagnostics.Debug.WriteLine($"Initializing Tesseract engine with path: {_tesseractDataPath}");
                _engine = new TesseractEngine(_tesseractDataPath, "rus", EngineMode.Default);
                System.Diagnostics.Debug.WriteLine("Tesseract engine initialized successfully");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error initializing OCR engine: {ex.Message}");
                _engine = null;
            }
        }
    }

    private void LoadExitData()
    {
        try
        {
            if (!Directory.Exists(_exitsDataPath))
            {
                System.Diagnostics.Debug.WriteLine($"Warning: Exits data path not found: {_exitsDataPath}");
                return;
            }

            foreach (var mapName in _mapNames)
            {
                var filePath = Path.Combine(_exitsDataPath, $"{mapName}.json");
                if (!File.Exists(filePath))
                {
                    System.Diagnostics.Debug.WriteLine($"Warning: Exit file not found for map '{mapName}'");
                    continue;
                }

                try
                {
                    var json = File.ReadAllText(filePath);
                    var exits = JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>();
                    _mapExits[mapName] = exits.Where(e => !string.IsNullOrWhiteSpace(e)).ToList();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error loading exits for map '{mapName}': {ex.Message}");
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading exit data: {ex.Message}");
        }
    }

    public string ProcessImage(Bitmap image)
    {
        try
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image));

            lock (_lockObject)
            {
                if (_engine == null)
                {
                    System.Diagnostics.Debug.WriteLine("OCR engine is not initialized");
                    return string.Empty;
                }

                using (var ms = new MemoryStream())
                {
                    image.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                    var imageBytes = ms.ToArray();
                    
                    using (var pix = Pix.LoadFromMemory(imageBytes))
                    using (var page = _engine.Process(pix))
                    {
                        var text = page.GetText();
                        System.Diagnostics.Debug.WriteLine($"OCR confidence: {page.GetMeanConfidence()}");
                        return text;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error processing OCR: {ex.Message}");
            return string.Empty;
        }
    }

    public List<string> ExtractExits(string ocrText)
    {
        var foundExits = new List<string>();

        foreach (var (mapName, exits) in _mapExits)
        {
            foreach (var exit in exits)
            {
                if (ocrText.Contains(exit, StringComparison.OrdinalIgnoreCase))
                {
                    foundExits.Add(exit);
                }
            }
        }

        return foundExits;
    }

    public string DetermineMapType(string ocrText)
    {
        var exitCounts = new Dictionary<string, int>();

        foreach (var (mapName, exits) in _mapExits)
        {
            var matchCount = 0;
            foreach (var exit in exits)
            {
                if (ocrText.Contains(exit, StringComparison.OrdinalIgnoreCase))
                {
                    matchCount++;
                }
            }

            if (matchCount > 0)
            {
                exitCounts[mapName] = matchCount;
            }
        }

        if (exitCounts.Count == 0)
            return string.Empty;

        return exitCounts.OrderByDescending(x => x.Value).First().Key;
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        lock (_lockObject)
        {
            _engine?.Dispose();
            _engine = null;
        }

        _disposed = true;
    }
}

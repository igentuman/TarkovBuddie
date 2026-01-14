using System.Windows.Markup;
using System.Windows;
using System.Windows.Data;
using TARKIT.Services;

namespace TARKIT.Views;

public class TranslateExtension : MarkupExtension
{
    public string Category { get; set; } = "";
    public string Key { get; set; } = "";
    public string? Fallback { get; set; }

    public TranslateExtension() { }

    public TranslateExtension(string category, string key)
    {
        Category = category;
        Key = key;
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        if (string.IsNullOrEmpty(Category) || string.IsNullOrEmpty(Key))
        {
            System.Diagnostics.Debug.WriteLine($"TranslateExtension: Empty category or key");
            return $"[{Category}.{Key}]";
        }

        try
        {
            var binding = new System.Windows.Data.Binding("CurrentLanguage")
            {
                Source = LocalizationService.Instance,
                Converter = new TranslationConverter(),
                ConverterParameter = $"{Category}|{Key}|{Fallback ?? ""}",
                FallbackValue = Fallback ?? $"[{Category}.{Key}]",
                Mode = System.Windows.Data.BindingMode.OneWay,
                UpdateSourceTrigger = System.Windows.Data.UpdateSourceTrigger.PropertyChanged
            };
            
            System.Diagnostics.Debug.WriteLine($"TranslateExtension created binding for {Category}.{Key}");
            return binding.ProvideValue(serviceProvider);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"TranslateExtension error for {Category}.{Key}: {ex.Message}");
            return Fallback ?? $"[{Category}.{Key}]";
        }
    }
}

public class TranslationConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, 
        System.Globalization.CultureInfo culture)
    {
        try
        {
            if (parameter is not string param)
            {
                System.Diagnostics.Debug.WriteLine("TranslationConverter: No parameter provided");
                return "[No Parameter]";
            }

            var parts = param.Split('|');
            if (parts.Length < 2)
            {
                System.Diagnostics.Debug.WriteLine("TranslationConverter: Invalid parameter format");
                return "[Invalid Format]";
            }

            var category = parts[0];
            var key = parts[1];
            var fallback = parts.Length > 2 && !string.IsNullOrEmpty(parts[2]) ? parts[2] : null;

            var result = LocalizationService.Instance.Translate(category, key, fallback);
            return result;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"TranslationConverter exception: {ex.Message}");
            return "[Converter Error]";
        }
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, 
        System.Globalization.CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using TarkovBuddie.ViewModels;

namespace TarkovBuddie.Views;

public partial class MapView
{
    private MapViewModel? _viewModel;
    private bool _isPanning;
    private System.Windows.Point _lastMousePosition;
    private const double ZoomFactor = 1.15;
    private double _minZoom = 0.125;
    private const double MaxZoom = 10.0;

    public MapView()
    {
        InitializeComponent();
        this.Loaded += MapView_Loaded;
    }

    private void MapView_Loaded(object sender, RoutedEventArgs e)
    {
        _viewModel = this.DataContext as MapViewModel;
        if (_viewModel != null)
        {
            CalculateMinZoom();
            ApplyViewportState();

            if (_viewModel.ZoomLevel < _minZoom)
            {
                _viewModel.ZoomLevel = _minZoom;
                _viewModel.PanX = 0;
                _viewModel.PanY = 0;
                ApplyViewportState();
            }
        }
    }

    private void CalculateMinZoom()
    {
        if (_viewModel == null)
            return;

        double mapWidth = _viewModel.MapWidth;
        double mapHeight = _viewModel.MapHeight;
        double canvasWidth = MapCanvas.ActualWidth;
        double canvasHeight = MapCanvas.ActualHeight;

        if (canvasWidth > 0 && canvasHeight > 0 && mapWidth > 0 && mapHeight > 0)
        {
            double zoomX = canvasWidth / mapWidth;
            double zoomY = canvasHeight / mapHeight;
            _minZoom = Math.Min(zoomX, zoomY);
        }
    }

    private void ApplyViewportState()
    {
        if (_viewModel == null)
            return;

        var transform = new TransformGroup();
        transform.Children.Add(new ScaleTransform(_viewModel.ZoomLevel, _viewModel.ZoomLevel));
        transform.Children.Add(new TranslateTransform(_viewModel.PanX, _viewModel.PanY));
        MapImage.RenderTransform = transform;
    }

    private void MapCanvas_MouseWheel(object sender, MouseWheelEventArgs e)
    {
        if (_viewModel == null)
            return;

        double zoomMultiplier = e.Delta > 0 ? ZoomFactor : 1 / ZoomFactor;
        double newZoom = _viewModel.ZoomLevel * zoomMultiplier;

        if (newZoom < _minZoom || newZoom > MaxZoom)
        {
            e.Handled = true;
            return;
        }

        var mousePosition = e.GetPosition(MapCanvas);

        double scaleDifference = newZoom - _viewModel.ZoomLevel;
        double newPanX = _viewModel.PanX - (mousePosition.X * scaleDifference);
        double newPanY = _viewModel.PanY - (mousePosition.Y * scaleDifference);

        _viewModel.ZoomLevel = newZoom;
        _viewModel.PanX = newPanX;
        _viewModel.PanY = newPanY;

        ApplyViewportState();
        e.Handled = true;
    }

    private void MapCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        _isPanning = true;
        _lastMousePosition = e.GetPosition(MapCanvas);
        MapCanvas.CaptureMouse();
    }

    private void MapCanvas_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
    {
        if (!_isPanning || _viewModel == null)
            return;

        var currentPosition = e.GetPosition(MapCanvas);
        var delta = new System.Windows.Point(
            currentPosition.X - _lastMousePosition.X,
            currentPosition.Y - _lastMousePosition.Y
        );

        _viewModel.PanX += delta.X;
        _viewModel.PanY += delta.Y;

        ApplyViewportState();
        _lastMousePosition = currentPosition;
    }

    private void MapCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        _isPanning = false;
        MapCanvas.ReleaseMouseCapture();
        _viewModel?.SaveViewportState();
    }
}

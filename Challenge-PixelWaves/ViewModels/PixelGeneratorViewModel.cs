using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Challenge_PixelWaves.Services.Interfaces;
using Challenge_PixelWaves.Utils;

namespace Challenge_PixelWaves.ViewModels
{
    public class PixelGeneratorViewModel : BaseViewModel
    {
        private readonly IPixelGeneratorService _pixelGeneratorService;
        private readonly INavigationService _navigationService;

        private WriteableBitmap _bitmap;
        private byte[] _pixels;
        private int _stride;

        private int _width;
        private int _height;

        private int _stepsPerFrame = 10;
        public int StepsPerFrame
        {
            get => _stepsPerFrame;
            set
            {
                if (_stepsPerFrame != value)
                {
                    _stepsPerFrame = value;
                    OnPropertyChanged(nameof(StepsPerFrame));
                }
            }
        }


        private DateTime _lastFrameTime = DateTime.Now;
        private int _frameCount = 0;
        private DateTime _lastFpsReport = DateTime.Now;


        public WriteableBitmap Bitmap
        {
            get => _bitmap;
            set
            {
                _bitmap = value;
                OnPropertyChanged(nameof(Bitmap));
            }
        }

        private DispatcherTimer _timer;

        public PixelGeneratorViewModel(IPixelGeneratorService pixelGeneratorService, INavigationService navigationService)
        {
            _pixelGeneratorService = pixelGeneratorService;
            _navigationService = navigationService;

            _width = _pixelGeneratorService.Width;
            _height = _pixelGeneratorService.Height;

            Bitmap = new WriteableBitmap(_width, _height, 96, 96, PixelFormats.Bgra32, null);
            _stride = _width * 4;
            _pixels = new byte[_stride * _height];

            double fps = 60.0;
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(Math.Round(1000 / fps))
            };
            _timer.Tick += UpdateFrame;
            _timer.Start();
        }

        private void UpdateFrame(object? sender, EventArgs e)
        {
            // Calcul du temps écoulé depuis la dernière frame
            var now = DateTime.Now;
            double ms = (now - _lastFrameTime).TotalMilliseconds;
            _lastFrameTime = now;

            // Incrémente le compteur de frames
            _frameCount++;

            // Affiche le temps de la frame courante (en ms)
            System.Diagnostics.Debug.WriteLine($"Frame time: {ms:F2} ms");

            // Affiche le FPS toutes les secondes
            if ((now - _lastFpsReport).TotalSeconds >= 1.0)
            {
                System.Diagnostics.Debug.WriteLine($"FPS: {_frameCount}");
                _frameCount = 0;
                _lastFpsReport = now;
            }

            // Simulation et rendu
            // Simulation et rendu
            for (int i = 0; i < StepsPerFrame; i++)
            {
                _pixelGeneratorService.StepSimulation();
            }
            var eta = _pixelGeneratorService.Eta_n;
            int srcWidth = eta.GetLength(0);
            int srcHeight = eta.GetLength(1);
            int dstWidth = _width;
            int dstHeight = _height;

            // Trouver min/max pour normalisation
            double min = double.MaxValue, max = double.MinValue;
            for (int y = 0; y < srcHeight; y++)
                for (int x = 0; x < srcWidth; x++)
                {
                    double v = eta[x, y];
                    if (v < min) min = v;
                    if (v > max) max = v;
                }

            // Pour chaque pixel du bitmap d'affichage
            for (int y = 0; y < dstHeight; y++)
            {
                double srcY = (double)y / (dstHeight - 1) * (srcHeight - 1);
                int y0 = (int)Math.Floor(srcY);
                int y1 = Math.Min(y0 + 1, srcHeight - 1);
                double wy = srcY - y0;

                for (int x = 0; x < dstWidth; x++)
                {
                    double srcX = (double)x / (dstWidth - 1) * (srcWidth - 1);
                    int x0 = (int)Math.Floor(srcX);
                    int x1 = Math.Min(x0 + 1, srcWidth - 1);
                    double wx = srcX - x0;

                    // Interpolation bilinéaire
                    double v00 = eta[x0, y0];
                    double v10 = eta[x1, y0];
                    double v01 = eta[x0, y1];
                    double v11 = eta[x1, y1];
                    double v = (1 - wx) * (1 - wy) * v00 +
                               wx * (1 - wy) * v10 +
                               (1 - wx) * wy * v01 +
                               wx * wy * v11;

                    byte intensity = CalculateIntensity(v, min, max, 16);
                    int index = (y * _stride) + (x * 4);
                    byte blue = (byte)(255);
                    byte green = intensity;
                    byte red = intensity;

                    _pixels[index] = blue;    // B
                    _pixels[index + 1] = green; // G
                    _pixels[index + 2] = red;   // R
                    _pixels[index + 3] = 255;   // A
                }
            }

            Bitmap.Lock();
            Bitmap.WritePixels(new System.Windows.Int32Rect(0, 0, _width, _height), _pixels, _stride, 0);
            Bitmap.Unlock();
        }

        private byte CalculateIntensity(double v, double min, double max, int discretization = 1)
        {
            double range = max - min;
            if (range < 1e-8) range = 1.0;

            // Calcul de l'intensité normalisée
            double norm = (v - min) / range;
            norm = Math.Clamp(norm, 0.0, 1.0);

            // Discrétisation si demandé
            if (discretization > 1)
            {
                norm = Math.Round(norm * (discretization - 1)) / (discretization - 1);
            }

            int intensity = (int)Math.Round(norm * 255.0);
            return (byte)intensity;
        }
    }
}


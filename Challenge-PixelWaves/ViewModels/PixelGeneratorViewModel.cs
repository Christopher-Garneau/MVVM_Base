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
            // Simulation et rendu
            for (int i = 0; i < StepsPerFrame; i++)
            {
                _pixelGeneratorService.StepSimulation();
            }

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
            _pixelGeneratorService.StepSimulation();
            var eta = _pixelGeneratorService.Eta_n;
            int width = eta.GetLength(0);
            int height = eta.GetLength(1);

            double min = double.MaxValue, max = double.MinValue;
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                {
                    double v = eta[x, y];
                    if (v < min) min = v;
                    if (v > max) max = v;
                }
            double range = max - min;
            if (range < 1e-8) range = 1.0;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int index = (y * _stride) + (x * 4);
                    byte intensity = (byte)(255.0 * (eta[x, y] - min) / range);
                    _pixels[index] = intensity;
                    _pixels[index + 1] = intensity;
                    _pixels[index + 2] = intensity;
                    _pixels[index + 3] = 255;
                }
            }

            Bitmap.Lock();
            Bitmap.WritePixels(new System.Windows.Int32Rect(0, 0, width, height), _pixels, _stride, 0);
            Bitmap.Unlock();
        }


    }
}


using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Threading;
using Challenge_PixelWaves.Services.Interfaces;
using Challenge_PixelWaves.Utils;

namespace Challenge_PixelWaves.ViewModels
{
    public class PixelGeneratorViewModel : BaseViewModel
    {
        private IPixelGeneratorService _pixelGeneratorService;
        private INavigationService _navigationService;

        private WriteableBitmap _bitmap;
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
        private byte _redIntensity = 255; private bool _increasing = false;
        public PixelGeneratorViewModel(IPixelGeneratorService pixelGeneratorService, INavigationService navigationService)
        {
            _pixelGeneratorService = pixelGeneratorService;
            _navigationService = navigationService;

            Bitmap = new WriteableBitmap(800, 450, 96, 96, PixelFormats.Bgra32, null);

            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(16)
            };
            _timer.Tick += UpdateFrame;
            _timer.Start();

            System.Diagnostics.Debug.WriteLine("PixelGeneratorViewModel instancié");
        }

        private void UpdateFrame(object? sender, EventArgs e)
        {
            if (_increasing)
            {
                _redIntensity++;
                if (_redIntensity >= 255) _increasing = false;
            }
            else
            {
                _redIntensity--;
                if (_redIntensity <= 50) _increasing = true;
            }

            DrawSquare(Bitmap.PixelWidth, Bitmap.PixelHeight, Color.FromRgb(_redIntensity, 0, 0));
        }

        private void DrawSquare(int width, int height, Color color)
        {
            int stride = width * 4;
            byte[] pixels = new byte[stride * height];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int index = (y * stride) + (x * 4);
                    pixels[index] = color.B;
                    pixels[index + 1] = color.G;
                    pixels[index + 2] = color.R;
                    pixels[index + 3] = 255;
                }
            }

            Bitmap.Lock();
            Bitmap.WritePixels(new System.Windows.Int32Rect(0, 0, width, height), pixels, stride, 0);
            Bitmap.Unlock();
        }
    }
}
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
        private int _rainbowOffset = 0;

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

            int width = 800;
            int height = 450;

            Bitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, null);
            _stride = width * 4;
            _pixels = new byte[_stride * height];

            double fps = 60.0;
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(Math.Round(1000 / fps))
            };
            _timer.Tick += UpdateFrame;
            _timer.Start();

            System.Diagnostics.Debug.WriteLine("PixelGeneratorViewModel instancié");
        }

        private void UpdateFrame(object? sender, EventArgs e)
        {
            _rainbowOffset = (_rainbowOffset + 5) % Bitmap.PixelWidth;
            _pixelGeneratorService.UpdateRainbowEffect(_pixels, Bitmap.PixelWidth, Bitmap.PixelHeight, _stride, _rainbowOffset);

            Bitmap.Lock();
            Bitmap.WritePixels(new System.Windows.Int32Rect(0, 0, Bitmap.PixelWidth, Bitmap.PixelHeight), _pixels, _stride, 0);
            Bitmap.Unlock();
        }
    }
}

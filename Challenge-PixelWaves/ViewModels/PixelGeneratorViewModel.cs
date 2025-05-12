using System.Windows.Media.Imaging;
using System.Windows.Media;
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

        public PixelGeneratorViewModel(IPixelGeneratorService pixelGeneratorService, INavigationService navigationService)
        {
            _pixelGeneratorService = pixelGeneratorService;
            _navigationService = navigationService;

            Bitmap = new WriteableBitmap(800, 450, 96, 96, PixelFormats.Bgra32, null);

            for (int i = 0; i < 200; i++)
            {
                for (int j = 0; i < 200; i++)
                {
                    SetPixel(i, j, Colors.Red);
                }
            }
            System.Diagnostics.Debug.WriteLine("PixelGeneratorViewModel instancié");

        }

        public void SetPixel(int x, int y, Color color)
        {
            if (x < 0 || x >= Bitmap.PixelWidth || y < 0 || y >= Bitmap.PixelHeight) return;

            Bitmap.Lock();
            unsafe
            {
                IntPtr pBackBuffer = Bitmap.BackBuffer;
                int stride = Bitmap.BackBufferStride;
                byte* p = (byte*)pBackBuffer + y * stride + x * 4;
                p[0] = color.B;
                p[1] = color.G;
                p[2] = color.R;
                p[3] = color.A;
            }
            Bitmap.AddDirtyRect(new System.Windows.Int32Rect(x, y, 1, 1));
            Bitmap.Unlock();
        }
    }
}

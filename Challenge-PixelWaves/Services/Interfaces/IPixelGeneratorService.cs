using System.Windows.Media;

namespace Challenge_PixelWaves.Services.Interfaces
{
    public interface IPixelGeneratorService
    {
        void UpdateRainbowEffect(byte[] pixels, int width, int height, int stride, int offset);
        Color GetRainbowColor(int position, int width);
    }
}

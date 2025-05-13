using System;
using System.Windows.Media;
using Challenge_PixelWaves.Services.Interfaces;

namespace Challenge_PixelWaves.Services
{
    public class PixelGeneratorService : IPixelGeneratorService
    {
        public void UpdateRainbowEffect(byte[] pixels, int width, int height, int stride, int offset)
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int index = (y * stride) + (x * 4);
                    Color color = GetRainbowColor((x + offset) % width, width);
                    pixels[index] = color.B;
                    pixels[index + 1] = color.G;
                    pixels[index + 2] = color.R;
                    pixels[index + 3] = 255;
                }
            }
        }

        public Color GetRainbowColor(int position, int width)
        {
            double ratio = (double)position / width;
            byte r = (byte)(Math.Sin(2 * Math.PI * ratio) * 127 + 128);
            byte g = (byte)(Math.Sin(2 * Math.PI * ratio + 2 * Math.PI / 3) * 127 + 128);
            byte b = (byte)(Math.Sin(2 * Math.PI * ratio + 4 * Math.PI / 3) * 127 + 128);
            return Color.FromRgb(r, g, b);
        }
    }
}

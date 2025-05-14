using System.Windows.Media;

namespace Challenge_PixelWaves.Services.Interfaces
{
    public interface IPixelGeneratorService
    {
        double[,] Eta_n { get; }
        int Width { get; }
        int Height { get; }
        void StepSimulation();
    }
}

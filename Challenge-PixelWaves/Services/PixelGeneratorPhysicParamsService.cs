using Challenge_PixelWaves.Services.Interfaces;
using System.Windows;

namespace Challenge_PixelWaves.Services
{
    public class PixelGeneratorPhysicParamsService : IPixelGeneratorPhysicParamsService
    {
        public double DomainLengthX => SystemParameters.PrimaryScreenWidth * 1e3;
        public double DomainLengthY => SystemParameters.PrimaryScreenHeight * 1e3;
        public double Gravity => 9.81;
        public double FluidDepth => 100.0;
        public double CoriolisBase => 1e-4;
        public double CoriolisGradient => 2e-11;
        public double FluidDensity => 1024.0;
        public double WindStressAmplitude => 0.1;
        public bool UseCoriolis => false;
        public bool UseFriction => false;
        public bool UseWind => false;
        public bool UseBetaVariation => true;
        public bool UseSource => false;
        public bool UseSink => false;
    }
}

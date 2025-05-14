using Challenge_PixelWaves.Services.Interfaces;

namespace Challenge_PixelWaves.Services
{
    public class PixelGeneratorPhysicParamsService : IPixelGeneratorPhysicParamsService
    {
        public double DomainLengthX => 800e3;
        public double DomainLengthY => 450e3;
        public double Gravity => 9.81;
        public double FluidDepth => 100.0;
        public double CoriolisBase => 1e-4;
        public double CoriolisGradient => 2e-11;
        public double FluidDensity => 1024.0;
        public double WindStressAmplitude => 0.1;
        public bool UseCoriolis => true;
        public bool UseFriction => false;
        public bool UseWind => false;
        public bool UseBetaVariation => true;
        public bool UseSource => false;
        public bool UseSink => false;
    }
}

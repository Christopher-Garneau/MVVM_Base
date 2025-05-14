using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Challenge_PixelWaves.Services.Interfaces
{
    public interface IPixelGeneratorPhysicParamsService
    {
        double DomainLengthX { get; }
        double DomainLengthY { get; }
        double Gravity { get; }
        double FluidDepth { get; }
        double CoriolisBase { get; }
        double CoriolisGradient { get; }
        double FluidDensity { get; }
        double WindStressAmplitude { get; }
        bool UseCoriolis { get; }
        bool UseFriction { get; }
        bool UseWind { get; }
        bool UseBetaVariation { get; }
        bool UseSource { get; }
        bool UseSink { get; }
    }
}

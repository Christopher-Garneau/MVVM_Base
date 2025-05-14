using System;
using System.Linq;
using Challenge_PixelWaves.Services.Interfaces;

namespace Challenge_PixelWaves.Services
{
    public class PixelGeneratorComputationalParamsService : IPixelGeneratorComputationalParamsService
    {
        public int GridPointsX { get; }
        public int GridPointsY { get; }
        public double GridSpacingX { get; }
        public double GridSpacingY { get; }
        public double TimeStep { get; }
        public int CurrentTimeStep { get; set; }
        public int MaxTimeStep { get; }
        public double[] XCoordinates { get; }
        public double[] YCoordinates { get; }
        public double[,] XMesh { get; }
        public double[,] YMesh { get; }
        public double[,] FrictionArray { get; }
        public double[,] WindStressX { get; }
        public double[,] WindStressY { get; }
        public double[] CoriolisArray { get; }
        public double[,] SourceArray { get; }
        public double[,] SinkArray { get; }
        public double RossbyDeformationRadius { get; }
        public double LongRossbyWaveSpeed { get; }
        public double[] Alpha { get; }
        public double[] BetaC { get; }
        public string ParameterSummary { get; }

        public PixelGeneratorComputationalParamsService(IPixelGeneratorPhysicParamsService param)
        {
            GridPointsX = 200;
            GridPointsY = 200;
            double Lx = param.DomainLengthX;
            double Ly = param.DomainLengthY;
            double g = param.Gravity;
            double H = param.FluidDepth;
            double f0 = param.CoriolisBase;
            double beta = param.CoriolisGradient;
            double tau0 = param.WindStressAmplitude;
            double rho0 = param.FluidDensity;

            GridSpacingX = Lx / (GridPointsX - 1);
            GridSpacingY = Ly / (GridPointsY - 1);

            TimeStep = 0.1 * Math.Min(GridSpacingX, GridSpacingY) / Math.Sqrt(g * H);
            CurrentTimeStep = 1;
            MaxTimeStep = 50000;

            XCoordinates = Linspace(-Lx / 2, Lx / 2, GridPointsX);
            YCoordinates = Linspace(-Ly / 2, Ly / 2, GridPointsY);

            (XMesh, YMesh) = Meshgrid(XCoordinates, YCoordinates);

            // Friction
            if (param.UseFriction)
            {
                double kappa0 = 1.0 / (5 * 24 * 3600);
                FrictionArray = new double[GridPointsX, GridPointsY];
                for (int y = 0; y < GridPointsY; y++)
                    for (int x = 0; x < GridPointsX; x++)
                        FrictionArray[x, y] = kappa0;
            }
            else
            {
                FrictionArray = new double[GridPointsX, GridPointsY];
            }

            // Wind stress
            if (param.UseWind)
            {
                WindStressX = new double[GridPointsX, GridPointsY];
                WindStressY = new double[GridPointsX, GridPointsY];
                for (int y = 0; y < GridPointsY; y++)
                {
                    double yVal = YCoordinates[y];
                    double tauX = -tau0 * Math.Cos(Math.PI * yVal / Ly) * 0;
                    for (int x = 0; x < GridPointsX; x++)
                    {
                        WindStressX[x, y] = tauX;
                        WindStressY[x, y] = 0.0;
                    }
                }
            }
            else
            {
                WindStressX = new double[GridPointsX, GridPointsY];
                WindStressY = new double[GridPointsX, GridPointsY];
            }

            // Coriolis
            if (param.UseCoriolis)
            {
                if (param.UseBetaVariation)
                {
                    CoriolisArray = YCoordinates.Select(y => f0 + beta * y).ToArray();
                    RossbyDeformationRadius = Math.Sqrt(g * H) / f0;
                    LongRossbyWaveSpeed = beta * g * H / (f0 * f0);
                }
                else
                {
                    CoriolisArray = Enumerable.Repeat(f0, GridPointsY).ToArray();
                    RossbyDeformationRadius = Math.Sqrt(g * H) / f0;
                    LongRossbyWaveSpeed = 0;
                }
                Alpha = CoriolisArray.Select(f => TimeStep * f).ToArray();
                BetaC = Alpha.Select(a => a * a / 4).ToArray();
            }
            else
            {
                CoriolisArray = new double[GridPointsY];
                Alpha = new double[GridPointsY];
                BetaC = new double[GridPointsY];
                RossbyDeformationRadius = 0;
                LongRossbyWaveSpeed = 0;
            }

            ParameterSummary = $"dx = {GridSpacingX / 1000:F2} km\ndy = {GridSpacingY / 1000:F2} km\ndt = {TimeStep:F2} s";
            if (param.UseFriction)
            {
                double kappa0 = 1.0 / (5 * 24 * 3600);
                ParameterSummary += $"\nkappa = {kappa0:G}\nkappa/beta = {(beta != 0 ? kappa0 / (beta * 1000) : 0):G} km";
            }
            if (param.UseWind)
            {
                ParameterSummary += $"\ntau_0 = {tau0:G}\nrho_0 = {rho0:G} kg/m^3";
            }

            if (param.UseSource)
            {
                SourceArray = new double[GridPointsX, GridPointsY];
                double sigma0 = 0.0001;
                double x0 = Lx / 2;
                double y0 = Ly / 2;
                double sigmaX = 1E+5;
                double sigmaY = 1E+5;
                for (int y = 0; y < GridPointsY; y++)
                {
                    for (int x = 0; x < GridPointsX; x++)
                    {
                        double X = XMesh[x, y];
                        double Y = YMesh[x, y];
                        SourceArray[x, y] = sigma0 * Math.Exp(
                            -((X - x0) * (X - x0) / (2 * sigmaX * sigmaX) +
                              (Y - y0) * (Y - y0) / (2 * sigmaY * sigmaY))
                        );
                    }
                }
            }
            else
            {
                SourceArray = new double[GridPointsX, GridPointsY];
            }

            // Sink (w)
            if (param.UseSink)
            {
                double sumSigma = 0.0;
                for (int y = 0; y < GridPointsY; y++)
                    for (int x = 0; x < GridPointsX; x++)
                        sumSigma += SourceArray[x, y];

                double wVal = sumSigma / (GridPointsX * GridPointsY);
                SinkArray = new double[GridPointsX, GridPointsY];
                for (int y = 0; y < GridPointsY; y++)
                    for (int x = 0; x < GridPointsX; x++)
                        SinkArray[x, y] = wVal;
            }
            else
            {
                SinkArray = new double[GridPointsX, GridPointsY];
            }
        }

        private static double[] Linspace(double start, double end, int num)
        {
            double[] result = new double[num];
            double step = (end - start) / (num - 1);
            for (int i = 0; i < num; i++)
                result[i] = start + step * i;
            return result;
        }

        private static (double[,], double[,]) Meshgrid(double[] x, double[] y)
        {
            int nx = x.Length;
            int ny = y.Length;
            var X = new double[nx, ny];
            var Y = new double[nx, ny];
            for (int i = 0; i < nx; i++)
                for (int j = 0; j < ny; j++)
                {
                    X[i, j] = x[i];
                    Y[i, j] = y[j];
                }
            return (X, Y);
        }
    }
}

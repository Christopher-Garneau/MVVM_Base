using System;
using System.Collections.ObjectModel;
using System.Windows.Media;
using Challenge_PixelWaves.Services.Interfaces;

namespace Challenge_PixelWaves.Services
{
    public class PixelGeneratorService : IPixelGeneratorService
    {
        private readonly IPixelGeneratorPhysicParamsService _physicParams;
        private readonly IPixelGeneratorComputationalParamsService _compParams;

        private double[,] _uN;
        private double[,] _uNp1;
        private double[,] _vN;
        private double[,] _vNp1;
        private double[,] _etaN;
        private double[,] _etaNp1;
        private double[,] _hE;
        private double[,] _hW;
        private double[,] _hN;
        private double[,] _hS;
        private double[,] _uhwe;
        private double[,] _vhns;

        public double[,] U_n => _uN;
        public double[,] U_np1 => _uNp1;
        public double[,] V_n => _vN;
        public double[,] V_np1 => _vNp1;
        public double[,] Eta_n => _etaN;
        public double[,] Eta_np1 => _etaNp1;
        public double[,] H_e => _hE;
        public double[,] H_w => _hW;
        public double[,] H_n => _hN;
        public double[,] H_s => _hS;
        public double[,] Uhwe => _uhwe;
        public double[,] Vhns => _vhns;

        private int _width;
        private int _height;
        
        public int Width => _width;
        public int Height => _height;

        private int nx;
        private int ny;
        private double[,] X;
        private double[,] Y;
        private double Lx;
        private double Ly;

        public PixelGeneratorService(
            IPixelGeneratorPhysicParamsService physicParams,
            IPixelGeneratorComputationalParamsService compParams)
        {
            _physicParams = physicParams;
            _compParams = compParams;

            _width = (int)Math.Round(physicParams.DomainLengthX / 1e3);
            _height = (int)Math.Round(physicParams.DomainLengthY / 1e3);

            InitializeMeshVariables();
            AllocateArrays();
            InitializeWaveSimulation();
        }

        private void InitializeMeshVariables()
        {
            nx = _compParams.GridPointsX;
            ny = _compParams.GridPointsY;
            X = _compParams.XMesh;
            Y = _compParams.YMesh;
            Lx = _physicParams.DomainLengthX;
            Ly = _physicParams.DomainLengthY;
        }

        private void AllocateArrays()
        {
            _uN = new double[nx, ny];
            _uNp1 = new double[nx, ny];
            _vN = new double[nx, ny];
            _vNp1 = new double[nx, ny];
            _etaN = new double[nx, ny];
            _etaNp1 = new double[nx, ny];
            _hE = new double[nx, ny];
            _hW = new double[nx, ny];
            _hN = new double[nx, ny];
            _hS = new double[nx, ny];
            _uhwe = new double[nx, ny];
            _vhns = new double[nx, ny];
        }

        private void InitializeWaveSimulation()
        {
            for (int y = 0; y < ny; y++)
            {
                for (int x = 0; x < nx; x++)
                {
                    _uN[x, y] = 0.0;
                    _vN[x, y] = 0.0;
                }
            }
            for (int y = 0; y < ny; y++)
                _uN[nx - 1, y] = 0.0;
            for (int x = 0; x < nx; x++)
                _vN[x, ny - 1] = 0.0;

            double x0 = 0.0 * 1e3;
            double y0 = 0.0 * 1e3;
            double sigma = 0.05E+6;
            for (int y = 0; y < ny; y++)
            {
                for (int x = 0; x < nx; x++)
                {
                    _etaN[x, y] = Math.Exp(
                        -(
                            Math.Pow(X[x, y] - x0, 2) / (2 * sigma * sigma) +
                            Math.Pow(Y[x, y] - y0, 2) / (2 * sigma * sigma)
                        )
                    );
                }
            }
        }

        public void StepSimulation()
        {
            double g = _physicParams.Gravity;
            double H = _physicParams.FluidDepth;
            double dt = _compParams.TimeStep;
            double dx = _compParams.GridSpacingX;
            double dy = _compParams.GridSpacingY;
            double rho0 = _physicParams.FluidDensity;
            int maxTimeStep = _compParams.MaxTimeStep;
            int sampleInterval = 1000;
            int animInterval = 20;

            var kappa = _compParams.FrictionArray;
            var tau_x = _compParams.WindStressX;
            var tau_y = _compParams.WindStressY;
            var sigma = _compParams.SourceArray;
            var w = _compParams.SinkArray;
            var alpha = _compParams.Alpha;
            var beta_c = _compParams.BetaC;

            bool useFriction = _physicParams.UseFriction;
            bool useWind = _physicParams.UseWind;
            bool useCoriolis = _physicParams.UseCoriolis;
            bool useSource = _physicParams.UseSource;
            bool useSink = _physicParams.UseSink;

            // --- u_np1 and v_np1 ---
            for (int y = 0; y < ny; y++)
            {
                for (int x = 0; x < nx - 1; x++)
                    _uNp1[x, y] = _uN[x, y] - g * dt / dx * (_etaN[x + 1, y] - _etaN[x, y]);
            }
            for (int y = 0; y < ny - 1; y++)
            {
                for (int x = 0; x < nx; x++)
                    _vNp1[x, y] = _vN[x, y] - g * dt / dy * (_etaN[x, y + 1] - _etaN[x, y]);
            }

            // Friction
            if (useFriction)
            {
                for (int y = 0; y < ny; y++)
                    for (int x = 0; x < nx - 1; x++)
                        _uNp1[x, y] -= dt * kappa[x, y] * _uN[x, y];
                for (int y = 0; y < ny; y++)
                    for (int x = 0; x < nx - 1; x++)
                        _vNp1[x, y] -= dt * kappa[x, y] * _vN[x, y];
            }

            // Wind
            if (useWind)
            {
                for (int y = 0; y < ny; y++)
                    for (int x = 0; x < nx - 1; x++)
                        _uNp1[x, y] += dt * tau_x[x, y] / (rho0 * H);
                for (int y = 0; y < ny; y++)
                    for (int x = 0; x < nx - 1; x++)
                        _vNp1[x, y] += dt * tau_y[x, y] / (rho0 * H);
            }

            // Coriolis
            if (useCoriolis)
            {
                for (int y = 0; y < ny; y++)
                {
                    for (int x = 0; x < nx; x++)
                    {
                        double a = (alpha.Length == 1) ? alpha[0] : alpha[y];
                        double b = (beta_c.Length == 1) ? beta_c[0] : beta_c[y];
                        double denom = 1 + b;
                        double u = _uNp1[x, y];
                        double v = _vNp1[x, y];
                        _uNp1[x, y] = (u - b * _uN[x, y] + a * _vN[x, y]) / denom;
                        _vNp1[x, y] = (v - b * _vN[x, y] - a * _uN[x, y]) / denom;
                    }
                }
            }

            // Boundary conditions
            for (int y = 0; y < ny; y++)
                _uNp1[nx - 1, y] = 0.0;
            for (int x = 0; x < nx; x++)
                _vNp1[x, ny - 1] = 0.0;

            // --- Upwind scheme for eta ---
            // h_e, h_w, h_n, h_s
            for (int y = 0; y < ny; y++)
            {
                for (int x = 0; x < nx - 1; x++)
                    _hE[x, y] = _uNp1[x, y] > 0 ? _etaN[x, y] + H : _etaN[x + 1, y] + H;
                _hE[nx - 1, y] = _etaN[nx - 1, y] + H;
            }
            for (int y = 0; y < ny; y++)
            {
                _hW[0, y] = _etaN[0, y] + H;
                for (int x = 1; x < nx; x++)
                    _hW[x, y] = _uNp1[x - 1, y] > 0 ? _etaN[x - 1, y] + H : _etaN[x, y] + H;
            }
            for (int x = 0; x < nx; x++)
            {
                for (int y = 0; y < ny - 1; y++)
                    _hN[x, y] = _vNp1[x, y] > 0 ? _etaN[x, y] + H : _etaN[x, y + 1] + H;
                _hN[x, ny - 1] = _etaN[x, ny - 1] + H;
            }
            for (int x = 0; x < nx; x++)
            {
                _hS[x, 0] = _etaN[x, 0] + H;
                for (int y = 1; y < ny; y++)
                    _hS[x, y] = _vNp1[x, y - 1] > 0 ? _etaN[x, y - 1] + H : _etaN[x, y] + H;
            }

            // uhwe
            for (int y = 0; y < ny; y++)
            {
                _uhwe[0, y] = _uNp1[0, y] * _hE[0, y];
                for (int x = 1; x < nx; x++)
                    _uhwe[x, y] = _uNp1[x, y] * _hE[x, y] - _uNp1[x - 1, y] * _hW[x, y];
            }
            // vhns
            for (int x = 0; x < nx; x++)
            {
                _vhns[x, 0] = _vNp1[x, 0] * _hN[x, 0];
                for (int y = 1; y < ny; y++)
                    _vhns[x, y] = _vNp1[x, y] * _hN[x, y] - _vNp1[x, y - 1] * _hS[x, y];
            }

            // eta_np1
            for (int y = 0; y < ny; y++)
            {
                for (int x = 0; x < nx; x++)
                {
                    _etaNp1[x, y] = _etaN[x, y] - dt * (_uhwe[x, y] / dx + _vhns[x, y] / dy);
                    if (useSource)
                        _etaNp1[x, y] += dt * sigma[x, y];
                    if (useSink)
                        _etaNp1[x, y] -= dt * w[x, y];
                }
            }

            // Swap for next step
            Array.Copy(_uNp1, _uN, _uN.Length);
            Array.Copy(_vNp1, _vN, _vN.Length);
            Array.Copy(_etaNp1, _etaN, _etaN.Length);
        }
    }
}

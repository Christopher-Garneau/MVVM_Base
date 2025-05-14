namespace Challenge_PixelWaves.Services.Interfaces
{
    public interface IPixelGeneratorComputationalParamsService
    {
        int GridPointsX { get; }
        int GridPointsY { get; }
        double GridSpacingX { get; }
        double GridSpacingY { get; }
        double TimeStep { get; }
        int CurrentTimeStep { get; set; }
        int MaxTimeStep { get; }
        double[] XCoordinates { get; }
        double[] YCoordinates { get; }
        double[,] XMesh { get; }
        double[,] YMesh { get; }
        double[,] FrictionArray { get; }
        double[,] WindStressX { get; }
        double[,] WindStressY { get; }
        double[] CoriolisArray { get; }
        double[,] SourceArray { get; }
        double[,] SinkArray { get; }

        double RossbyDeformationRadius { get; }
        double LongRossbyWaveSpeed { get; }
        double[] Alpha { get; }
        double[] BetaC { get; }
        string ParameterSummary { get; }
    }
}

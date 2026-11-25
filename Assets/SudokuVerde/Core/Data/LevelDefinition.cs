using System;

namespace SudokuVerde.Core
{
    [Serializable]
    public sealed class LevelDefinition
    {
        public int LevelNumber { get; }
        public int GridSize { get; }
        public int[] RegionMap { get; }
        public int[] SolutionIndices { get; }
        public int InitialHints { get; }

        public LevelDefinition(int levelNumber, int gridSize, int[] regionMap, int[] solutionIndices, int initialHints = 3)
        {
            LevelNumber = levelNumber;
            GridSize = gridSize;
            RegionMap = regionMap;
            SolutionIndices = solutionIndices;
            InitialHints = initialHints;
        }
    }
}

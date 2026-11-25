using System;

namespace SudokuVerde.Core
{
    public static class LevelCatalog
    {
        // All levels use a 4x4 grid with 4 contiguous color regions (0=Orange, 1=Blue, 2=Green, 3=LightGreen)
        // and a mathematically unique solution under the three Sudoku Verde rules.
        private static readonly LevelDefinition[] Levels =
        {
            new LevelDefinition(
                levelNumber: 1,
                gridSize: 4,
                regionMap: new[]
                {
                    0, 0, 1, 1,
                    2, 0, 1, 1,
                    2, 2, 3, 1,
                    2, 3, 3, 3
                },
                solutionIndices: new[] { 1, 7, 8, 14 },
                initialHints: 3
            ),
            new LevelDefinition(
                levelNumber: 2,
                gridSize: 4,
                regionMap: new[]
                {
                    0, 0, 1, 1,
                    2, 0, 0, 3,
                    2, 0, 3, 3,
                    2, 0, 3, 3
                },
                solutionIndices: new[] { 2, 4, 11, 13 },
                initialHints: 3
            ),
            new LevelDefinition(
                levelNumber: 3,
                gridSize: 4,
                regionMap: new[]
                {
                    0, 1, 1, 1,
                    0, 2, 2, 2,
                    0, 3, 3, 3,
                    3, 3, 3, 3
                },
                solutionIndices: new[] { 1, 7, 8, 14 },
                initialHints: 3
            ),
            new LevelDefinition(
                levelNumber: 4,
                gridSize: 4,
                regionMap: new[]
                {
                    1, 1, 2, 2,
                    0, 1, 3, 3,
                    0, 1, 3, 3,
                    0, 1, 1, 3
                },
                solutionIndices: new[] { 2, 4, 11, 13 },
                initialHints: 3
            ),
            new LevelDefinition(
                levelNumber: 5,
                gridSize: 4,
                regionMap: new[]
                {
                    2, 2, 2, 0,
                    3, 3, 0, 0,
                    1, 3, 3, 3,
                    1, 1, 3, 3
                },
                solutionIndices: new[] { 1, 7, 8, 14 },
                initialHints: 3
            )
        };

        public static int LevelCount => Levels.Length;

        public static LevelDefinition GetLevel(int levelNumber)
        {
            int safeNumber = Math.Max(1, levelNumber);
            int index = (safeNumber - 1) % Levels.Length;
            LevelDefinition source = Levels[index];
            return new LevelDefinition(
                safeNumber,
                source.GridSize,
                source.RegionMap,
                source.SolutionIndices,
                source.InitialHints
            );
        }

        public static LevelDefinition GetDailyChallengeLevel(DateTime date)
        {
            int index = Math.Abs(date.DayOfYear) % Levels.Length;
            LevelDefinition source = Levels[index];
            return new LevelDefinition(
                source.LevelNumber,
                source.GridSize,
                source.RegionMap,
                source.SolutionIndices,
                source.InitialHints
            );
        }
    }
}

using System;
using System.Collections.Generic;

namespace SudokuVerde.Core
{
    public static class SudokuBoardValidator
    {
        public static HashSet<int> GetConflictingCells(CellState[] cells, LevelDefinition level)
        {
            var conflicts = new HashSet<int>();
            if (cells == null || level == null)
            {
                return conflicts;
            }

            int size = level.GridSize;
            var plantIndices = new List<int>(size);
            for (int i = 0; i < cells.Length; i++)
            {
                if (cells[i] == CellState.Plant)
                {
                    plantIndices.Add(i);
                }
            }

            for (int i = 0; i < plantIndices.Count; i++)
            {
                int idxA = plantIndices[i];
                int rowA = idxA / size;
                int colA = idxA % size;
                int regionA = level.RegionMap[idxA];

                for (int j = i + 1; j < plantIndices.Count; j++)
                {
                    int idxB = plantIndices[j];
                    int rowB = idxB / size;
                    int colB = idxB % size;
                    int regionB = level.RegionMap[idxB];

                    bool sameRegion = regionA == regionB;
                    bool sameRowOrCol = rowA == rowB || colA == colB;
                    bool adjacent = Math.Abs(rowA - rowB) <= 1 && Math.Abs(colA - colB) <= 1;

                    if (sameRegion || sameRowOrCol || adjacent)
                    {
                        conflicts.Add(idxA);
                        conflicts.Add(idxB);
                    }
                }
            }

            return conflicts;
        }

        public static int CountPlants(CellState[] cells)
        {
            if (cells == null)
            {
                return 0;
            }

            int count = 0;
            for (int i = 0; i < cells.Length; i++)
            {
                if (cells[i] == CellState.Plant)
                {
                    count++;
                }
            }

            return count;
        }

        public static bool IsSolved(CellState[] cells, LevelDefinition level)
        {
            if (cells == null || level == null || cells.Length != level.GridSize * level.GridSize)
            {
                return false;
            }

            int size = level.GridSize;
            var rows = new HashSet<int>();
            var cols = new HashSet<int>();
            var regions = new HashSet<int>();
            int plantCount = 0;

            for (int i = 0; i < cells.Length; i++)
            {
                if (cells[i] != CellState.Plant)
                {
                    continue;
                }

                plantCount++;
                rows.Add(i / size);
                cols.Add(i % size);
                regions.Add(level.RegionMap[i]);
            }

            // All 3 rules must hold simultaneously:
            // 1) 1 plant per color region, 2) 1 plant per row and column, 3) no orthogonal/diagonal adjacency.
            return plantCount == size
                && regions.Count == size
                && rows.Count == size
                && cols.Count == size
                && GetConflictingCells(cells, level).Count == 0;
        }
    }
}

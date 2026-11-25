using System;
using System.Collections.Generic;
using Unity.Properties;
using SudokuVerde.Core;

namespace SudokuVerde.UI.Shared.ViewModels
{
    public sealed class GameViewModel : BindableViewModelBase
    {
        public event Action<bool> LevelCompleted;

        private LevelDefinition _currentLevel;
        private CellState[] _rawCells;
        private bool _progressAlreadyAwardedForBoard;

        private int _currentLevelNumber;
        private string _levelTitleText = "NIVEL 01";
        private int _plantsRemaining;
        private string _plantsRemainingText = "4";
        private string _plantsProgressText = "0/4";
        private int _hintsRemaining;
        private string _hintsRemainingText = "3";
        private bool _isDailyChallenge;
        private bool _isCompleted;
        private ToolMode _activeTool = ToolMode.Plant;
        private string _activeToolLabel = "PLANTA";

        public CellViewModel[] Cells { get; } = new CellViewModel[16];

        [CreateProperty]
        public int CurrentLevelNumber
        {
            get => _currentLevelNumber;
            private set => SetProperty(ref _currentLevelNumber, value);
        }

        [CreateProperty]
        public string LevelTitleText
        {
            get => _levelTitleText;
            private set => SetProperty(ref _levelTitleText, value);
        }

        [CreateProperty]
        public int PlantsRemaining
        {
            get => _plantsRemaining;
            private set
            {
                if (SetProperty(ref _plantsRemaining, value))
                {
                    PlantsRemainingText = value.ToString();
                }
            }
        }

        [CreateProperty]
        public string PlantsRemainingText
        {
            get => _plantsRemainingText;
            private set => SetProperty(ref _plantsRemainingText, value);
        }

        [CreateProperty]
        public string PlantsProgressText
        {
            get => _plantsProgressText;
            private set => SetProperty(ref _plantsProgressText, value);
        }

        [CreateProperty]
        public int HintsRemaining
        {
            get => _hintsRemaining;
            private set
            {
                if (SetProperty(ref _hintsRemaining, value))
                {
                    HintsRemainingText = value.ToString();
                }
            }
        }

        [CreateProperty]
        public string HintsRemainingText
        {
            get => _hintsRemainingText;
            private set => SetProperty(ref _hintsRemainingText, value);
        }

        [CreateProperty]
        public bool IsDailyChallenge
        {
            get => _isDailyChallenge;
            private set => SetProperty(ref _isDailyChallenge, value);
        }

        [CreateProperty]
        public bool IsCompleted
        {
            get => _isCompleted;
            private set => SetProperty(ref _isCompleted, value);
        }

        [CreateProperty]
        public ToolMode ActiveTool
        {
            get => _activeTool;
            private set
            {
                if (SetProperty(ref _activeTool, value))
                {
                    ActiveToolLabel = value == ToolMode.Plant ? "PLANTA" : "MARCA X";
                }
            }
        }

        [CreateProperty]
        public string ActiveToolLabel
        {
            get => _activeToolLabel;
            private set => SetProperty(ref _activeToolLabel, value);
        }

        public GameViewModel()
        {
            _rawCells = new CellState[16];
            for (int i = 0; i < 16; i++)
            {
                Cells[i] = new CellViewModel(i, 0);
            }
        }

        public void LoadLevel(LevelDefinition level, bool isDailyChallenge)
        {
            _progressAlreadyAwardedForBoard = false;
            LoadBoardInternal(level ?? LevelCatalog.GetLevel(1), isDailyChallenge);
        }

        public void RestartLevel()
        {
            if (_currentLevel != null)
            {
                LoadBoardInternal(_currentLevel, IsDailyChallenge);
            }
        }

        private void LoadBoardInternal(LevelDefinition level, bool isDailyChallenge)
        {
            _currentLevel = level;
            IsDailyChallenge = isDailyChallenge;
            CurrentLevelNumber = _currentLevel.LevelNumber;
            LevelTitleText = isDailyChallenge
                ? "DESAFIO DIARIO"
                : $"NIVEL {_currentLevel.LevelNumber:00}";

            ActiveTool = ToolMode.Plant;
            IsCompleted = false;
            HintsRemaining = _currentLevel.InitialHints;

            for (int i = 0; i < Cells.Length; i++)
            {
                _rawCells[i] = CellState.Empty;
                Cells[i].RegionId = _currentLevel.RegionMap[i];
                Cells[i].State = CellState.Empty;
                Cells[i].HasConflict = false;
                Cells[i].IsHinted = false;
            }

            RecalculateBoardState();
        }

        public void ToggleTool()
        {
            ActiveTool = ActiveTool == ToolMode.Plant ? ToolMode.Cross : ToolMode.Plant;
        }

        public void SetTool(ToolMode tool)
        {
            ActiveTool = tool;
        }

        public void InteractCell(int index)
        {
            if (ActiveTool == ToolMode.Plant)
            {
                PlaceOrTogglePlant(index);
            }
            else
            {
                PlaceOrToggleCross(index);
            }
        }

        public void PlaceOrTogglePlant(int index)
        {
            if (IsCompleted || _currentLevel == null || index < 0 || index >= Cells.Length)
            {
                return;
            }

            CellState next = _rawCells[index] == CellState.Plant ? CellState.Empty : CellState.Plant;
            ApplyCellState(index, next);
        }

        public void PlaceOrToggleCross(int index)
        {
            if (IsCompleted || _currentLevel == null || index < 0 || index >= Cells.Length)
            {
                return;
            }

            CellState next = _rawCells[index] == CellState.Cross ? CellState.Empty : CellState.Cross;
            ApplyCellState(index, next);
        }

        private void ApplyCellState(int index, CellState next)
        {
            _rawCells[index] = next;
            Cells[index].State = next;
            Cells[index].IsHinted = false;
            RecalculateBoardState();
        }

        public bool UseHint()
        {
            if (IsCompleted || _currentLevel == null || HintsRemaining <= 0)
            {
                return false;
            }

            // First try to place a missing solution plant; otherwise clear an incorrect plant.
            int targetIndex = -1;
            foreach (int solIdx in _currentLevel.SolutionIndices)
            {
                if (_rawCells[solIdx] != CellState.Plant)
                {
                    targetIndex = solIdx;
                    break;
                }
            }

            if (targetIndex >= 0)
            {
                _rawCells[targetIndex] = CellState.Plant;
                Cells[targetIndex].State = CellState.Plant;
                Cells[targetIndex].IsHinted = true;
                HintsRemaining--;
                RecalculateBoardState();
                return true;
            }

            // If all solution indices already have plants, remove one non-solution plant.
            var solutionSet = new HashSet<int>(_currentLevel.SolutionIndices);
            for (int i = 0; i < _rawCells.Length; i++)
            {
                if (_rawCells[i] == CellState.Plant && !solutionSet.Contains(i))
                {
                    _rawCells[i] = CellState.Cross;
                    Cells[i].State = CellState.Cross;
                    Cells[i].IsHinted = true;
                    HintsRemaining--;
                    RecalculateBoardState();
                    return true;
                }
            }

            return false;
        }

        public bool ShouldAwardProgressOnCompletion()
        {
            if (_progressAlreadyAwardedForBoard)
            {
                return false;
            }

            _progressAlreadyAwardedForBoard = true;
            return true;
        }

        private void RecalculateBoardState()
        {
            if (_currentLevel == null)
            {
                return;
            }

            int placedPlants = SudokuBoardValidator.CountPlants(_rawCells);
            PlantsRemaining = Math.Max(0, _currentLevel.GridSize - placedPlants);
            PlantsProgressText = $"{placedPlants}/{_currentLevel.GridSize}";

            HashSet<int> conflicts = SudokuBoardValidator.GetConflictingCells(_rawCells, _currentLevel);
            for (int i = 0; i < Cells.Length; i++)
            {
                Cells[i].HasConflict = conflicts.Contains(i);
            }

            if (!IsCompleted && SudokuBoardValidator.IsSolved(_rawCells, _currentLevel))
            {
                IsCompleted = true;
                LevelCompleted?.Invoke(IsDailyChallenge);
            }
        }
    }
}

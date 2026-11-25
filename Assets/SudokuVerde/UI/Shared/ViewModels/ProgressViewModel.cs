using System;
using Unity.Properties;
using SudokuVerde.Core;

namespace SudokuVerde.UI.Shared.ViewModels
{
    public sealed class ProgressViewModel : BindableViewModelBase
    {
        private int _dailyStreak;
        private string _dailyStreakText = "0";
        private int _currentLevel;
        private string _currentLevelLabelText = "Nivel 1";
        private int _completedLevelsCount;

        [CreateProperty]
        public int DailyStreak
        {
            get => _dailyStreak;
            private set
            {
                if (SetProperty(ref _dailyStreak, value))
                {
                    DailyStreakText = value.ToString();
                }
            }
        }

        [CreateProperty]
        public string DailyStreakText
        {
            get => _dailyStreakText;
            private set => SetProperty(ref _dailyStreakText, value);
        }

        [CreateProperty]
        public int CurrentLevel
        {
            get => _currentLevel;
            private set
            {
                if (SetProperty(ref _currentLevel, value))
                {
                    CurrentLevelLabelText = $"Nivel {value}";
                }
            }
        }

        [CreateProperty]
        public string CurrentLevelLabelText
        {
            get => _currentLevelLabelText;
            private set => SetProperty(ref _currentLevelLabelText, value);
        }

        [CreateProperty]
        public int CompletedLevelsCount
        {
            get => _completedLevelsCount;
            private set => SetProperty(ref _completedLevelsCount, value);
        }

        public ProgressViewModel()
        {
            var saved = SudokuPersistence.LoadProgress();
            _currentLevel = saved.currentLevel;
            _currentLevelLabelText = $"Nivel {_currentLevel}";
            _dailyStreak = saved.dailyStreak;
            _dailyStreakText = _dailyStreak.ToString();
            _completedLevelsCount = saved.completedCount;
        }

        public void RecordLevelCompleted(bool isDailyChallenge)
        {
            CompletedLevelsCount++;
            if (isDailyChallenge)
            {
                DailyStreak = SudokuPersistence.RecordDailyCompletion(DateTime.Now, DailyStreak);
            }
            else
            {
                CurrentLevel++;
            }

            SudokuPersistence.SaveProgress(CurrentLevel, DailyStreak, CompletedLevelsCount);
        }
    }
}

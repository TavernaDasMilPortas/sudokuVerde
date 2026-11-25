using System;
using System.Globalization;
using UnityEngine;

namespace SudokuVerde.Core
{
    public static class SudokuPersistence
    {
        private const string KeyMusicEnabled = "SudokuVerde_MusicEnabled";
        private const string KeyVibrationEnabled = "SudokuVerde_VibrationEnabled";
        private const string KeyVoiceEnabled = "SudokuVerde_VoiceEnabled";
        private const string KeyCurrentLevel = "SudokuVerde_CurrentLevel";
        private const string KeyDailyStreak = "SudokuVerde_DailyStreak";
        private const string KeyCompletedLevels = "SudokuVerde_CompletedLevels";
        private const string KeyLastDailyDate = "SudokuVerde_LastDailyDate";
        private const string KeySelectedVariant = "SudokuVerde_SelectedVariant";
        private const string KeyVariantBInstructionsSeen = "SudokuVerde_VariantBInstructionsSeen";

        public static UIVariant LoadSelectedVariant()
        {
            int raw = PlayerPrefs.GetInt(KeySelectedVariant, (int)UIVariant.A);
            return raw == (int)UIVariant.B ? UIVariant.B : UIVariant.A;
        }

        public static void SaveSelectedVariant(UIVariant variant)
        {
            PlayerPrefs.SetInt(KeySelectedVariant, (int)variant);
            PlayerPrefs.Save();
        }

        public static bool HasSeenVariantBInstructions()
        {
            return PlayerPrefs.GetInt(KeyVariantBInstructionsSeen, 0) == 1;
        }

        public static void MarkVariantBInstructionsSeen()
        {
            PlayerPrefs.SetInt(KeyVariantBInstructionsSeen, 1);
            PlayerPrefs.Save();
        }

        public static (bool music, bool vibration, bool voice) LoadSettings()
        {
            bool music = PlayerPrefs.GetInt(KeyMusicEnabled, 1) == 1;
            bool vibration = PlayerPrefs.GetInt(KeyVibrationEnabled, 0) == 1;
            bool voice = PlayerPrefs.GetInt(KeyVoiceEnabled, 1) == 1;
            return (music, vibration, voice);
        }

        public static void SaveSettings(bool music, bool vibration, bool voice)
        {
            PlayerPrefs.SetInt(KeyMusicEnabled, music ? 1 : 0);
            PlayerPrefs.SetInt(KeyVibrationEnabled, vibration ? 1 : 0);
            PlayerPrefs.SetInt(KeyVoiceEnabled, voice ? 1 : 0);
            PlayerPrefs.Save();
        }

        public static (int currentLevel, int dailyStreak, int completedCount) LoadProgress()
        {
            int currentLevel = Math.Max(1, PlayerPrefs.GetInt(KeyCurrentLevel, 1));
            int dailyStreak = Math.Max(0, PlayerPrefs.GetInt(KeyDailyStreak, 0));
            int completedCount = Math.Max(0, PlayerPrefs.GetInt(KeyCompletedLevels, 0));
            return (currentLevel, dailyStreak, completedCount);
        }

        public static void SaveProgress(int currentLevel, int dailyStreak, int completedCount)
        {
            PlayerPrefs.SetInt(KeyCurrentLevel, Math.Max(1, currentLevel));
            PlayerPrefs.SetInt(KeyDailyStreak, Math.Max(0, dailyStreak));
            PlayerPrefs.SetInt(KeyCompletedLevels, Math.Max(0, completedCount));
            PlayerPrefs.Save();
        }

        public static int RecordDailyCompletion(DateTime today, int currentStreak)
        {
            string todayKey = today.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            string lastDateKey = PlayerPrefs.GetString(KeyLastDailyDate, string.Empty);

            if (lastDateKey == todayKey)
            {
                return Math.Max(1, currentStreak);
            }

            int updatedStreak = 1;
            if (DateTime.TryParseExact(lastDateKey, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime lastDate))
            {
                if ((today.Date - lastDate.Date).Days == 1)
                {
                    updatedStreak = Math.Max(1, currentStreak + 1);
                }
            }

            PlayerPrefs.SetString(KeyLastDailyDate, todayKey);
            PlayerPrefs.SetInt(KeyDailyStreak, updatedStreak);
            PlayerPrefs.Save();
            return updatedStreak;
        }
    }
}

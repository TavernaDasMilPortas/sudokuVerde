using System;
using Unity.Properties;
using SudokuVerde.Core;

namespace SudokuVerde.UI.Shared.ViewModels
{
    public sealed class SettingsViewModel : BindableViewModelBase
    {
        public event Action<UIVariant> VariantChanged;

        private bool _musicEnabled;
        private bool _vibrationEnabled;
        private bool _voiceEnabled;
        private string _musicStatusText;
        private string _vibrationStatusText;
        private string _voiceStatusText;
        private UIVariant _selectedVariant;
        private bool _isVariantB;
        private string _selectedVariantText;

        [CreateProperty]
        public bool MusicEnabled
        {
            get => _musicEnabled;
            set
            {
                if (SetProperty(ref _musicEnabled, value))
                {
                    MusicStatusText = value ? "ON" : "OFF";
                    Persist();
                }
            }
        }

        [CreateProperty]
        public string MusicStatusText
        {
            get => _musicStatusText;
            private set => SetProperty(ref _musicStatusText, value);
        }

        [CreateProperty]
        public bool VibrationEnabled
        {
            get => _vibrationEnabled;
            set
            {
                if (SetProperty(ref _vibrationEnabled, value))
                {
                    VibrationStatusText = value ? "ON" : "OFF";
                    Persist();
                }
            }
        }

        [CreateProperty]
        public string VibrationStatusText
        {
            get => _vibrationStatusText;
            private set => SetProperty(ref _vibrationStatusText, value);
        }

        [CreateProperty]
        public bool VoiceEnabled
        {
            get => _voiceEnabled;
            set
            {
                if (SetProperty(ref _voiceEnabled, value))
                {
                    VoiceStatusText = value ? "JOGAR" : "OFF";
                    Persist();
                }
            }
        }

        [CreateProperty]
        public string VoiceStatusText
        {
            get => _voiceStatusText;
            private set => SetProperty(ref _voiceStatusText, value);
        }

        [CreateProperty]
        public UIVariant SelectedVariant
        {
            get => _selectedVariant;
            set
            {
                if (SetProperty(ref _selectedVariant, value))
                {
                    bool isB = value == UIVariant.B;
                    SetProperty(ref _isVariantB, isB, nameof(IsVariantB));
                    SelectedVariantText = isB ? "VERSAO B" : "VERSAO A";
                    SudokuPersistence.SaveSelectedVariant(value);
                    VariantChanged?.Invoke(value);
                }
            }
        }

        [CreateProperty]
        public bool IsVariantB
        {
            get => _isVariantB;
            set
            {
                UIVariant target = value ? UIVariant.B : UIVariant.A;
                SelectedVariant = target;
            }
        }

        [CreateProperty]
        public string SelectedVariantText
        {
            get => _selectedVariantText;
            private set => SetProperty(ref _selectedVariantText, value);
        }

        public SettingsViewModel()
        {
            var saved = SudokuPersistence.LoadSettings();
            _musicEnabled = saved.music;
            _vibrationEnabled = saved.vibration;
            _voiceEnabled = saved.voice;
            _musicStatusText = _musicEnabled ? "ON" : "OFF";
            _vibrationStatusText = _vibrationEnabled ? "ON" : "OFF";
            _voiceStatusText = _voiceEnabled ? "JOGAR" : "OFF";

            _selectedVariant = SudokuPersistence.LoadSelectedVariant();
            _isVariantB = _selectedVariant == UIVariant.B;
            _selectedVariantText = _isVariantB ? "VERSAO B" : "VERSAO A";
        }

        public void ToggleMusic() => MusicEnabled = !MusicEnabled;

        public void ToggleVibration() => VibrationEnabled = !VibrationEnabled;

        public void ToggleVoice() => VoiceEnabled = !VoiceEnabled;

        private void Persist()
        {
            SudokuPersistence.SaveSettings(_musicEnabled, _vibrationEnabled, _voiceEnabled);
        }
    }
}

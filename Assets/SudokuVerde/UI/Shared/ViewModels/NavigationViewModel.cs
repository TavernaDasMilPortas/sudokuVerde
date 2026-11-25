using Unity.Properties;

namespace SudokuVerde.UI.Shared.ViewModels
{
    public enum ScreenId
    {
        MainMenu = 0,
        Gameplay = 1,
        Settings = 2
    }

    public enum OverlayId
    {
        None = 0,
        Pause = 1,
        Tutorial = 2,
        Victory = 3
    }

    public sealed class NavigationViewModel : BindableViewModelBase
    {
        private ScreenId _currentScreen = ScreenId.MainMenu;
        private ScreenId _previousScreen = ScreenId.MainMenu;
        private OverlayId _currentOverlay = OverlayId.None;
        private OverlayId _overlayBeforeSettings = OverlayId.None;
        private bool _isPaused;

        [CreateProperty]
        public ScreenId CurrentScreen
        {
            get => _currentScreen;
            private set => SetProperty(ref _currentScreen, value);
        }

        [CreateProperty]
        public ScreenId PreviousScreen
        {
            get => _previousScreen;
            private set => SetProperty(ref _previousScreen, value);
        }

        [CreateProperty]
        public OverlayId CurrentOverlay
        {
            get => _currentOverlay;
            private set
            {
                if (SetProperty(ref _currentOverlay, value))
                {
                    IsPaused = value != OverlayId.None;
                }
            }
        }

        [CreateProperty]
        public bool IsPaused
        {
            get => _isPaused;
            private set => SetProperty(ref _isPaused, value);
        }

        public void OpenMainMenu()
        {
            CurrentOverlay = OverlayId.None;
            _overlayBeforeSettings = OverlayId.None;
            CurrentScreen = ScreenId.MainMenu;
        }

        public void OpenGameplay()
        {
            CurrentOverlay = OverlayId.None;
            _overlayBeforeSettings = OverlayId.None;
            CurrentScreen = ScreenId.Gameplay;
        }

        public void OpenSettings(ScreenId returnScreen)
        {
            PreviousScreen = returnScreen;
            _overlayBeforeSettings = returnScreen == ScreenId.Gameplay ? CurrentOverlay : OverlayId.None;
            CurrentOverlay = OverlayId.None;
            CurrentScreen = ScreenId.Settings;
        }

        public void ReturnFromSettings()
        {
            CurrentScreen = PreviousScreen;
            CurrentOverlay = PreviousScreen == ScreenId.Gameplay ? _overlayBeforeSettings : OverlayId.None;
            _overlayBeforeSettings = OverlayId.None;
        }

        public void OpenPause()
        {
            if (CurrentScreen == ScreenId.Gameplay)
            {
                CurrentOverlay = OverlayId.Pause;
            }
        }

        public void OpenTutorial()
        {
            if (CurrentScreen == ScreenId.Gameplay)
            {
                CurrentOverlay = OverlayId.Tutorial;
            }
        }

        public void OpenVictory()
        {
            if (CurrentScreen == ScreenId.Gameplay)
            {
                CurrentOverlay = OverlayId.Victory;
            }
        }

        public void CloseOverlay()
        {
            CurrentOverlay = OverlayId.None;
        }
    }
}

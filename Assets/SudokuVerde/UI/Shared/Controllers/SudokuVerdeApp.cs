using System;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;
using SudokuVerde.Core;
using SudokuVerde.UI.Shared.ViewModels;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SudokuVerde.UI.Shared.Controllers
{
    [RequireComponent(typeof(UIDocument))]
    public sealed class SudokuVerdeApp : MonoBehaviour
    {
        private const string VariantARootPath = "Assets/SudokuVerde/UI/VariantA/Layouts/VariantARoot.uxml";
        private const string VariantACellPath = "Assets/SudokuVerde/UI/VariantA/Templates/BoardCell.uxml";
        private const string VariantBRootPath = "Assets/SudokuVerde/UI/VariantB/Layouts/VariantBRoot.uxml";
        private const string VariantBCellPath = "Assets/SudokuVerde/UI/VariantB/Templates/BoardCellB.uxml";

        [Header("Variant A Assets")]
        [SerializeField] private VisualTreeAsset rootVisualTree;
        [SerializeField] private VisualTreeAsset boardCellTemplate;

        [Header("Variant B Assets")]
        [SerializeField] private VisualTreeAsset variantBRootVisualTree;
        [SerializeField] private VisualTreeAsset variantBCellTemplate;

        [Header("Panel & Interaction")]
        [SerializeField] private PanelSettings panelSettingsAsset;
        [SerializeField] private bool enforceReferenceResolution = true;
        [SerializeField, Range(350, 450)] private int doubleClickWindowMs = 400;

        private UIDocument _uiDocument;
        private PanelSettings _runtimePanelSettings;

        private ProgressViewModel _progressVm;
        private SettingsViewModel _settingsVm;
        private GameViewModel _gameVm;
        private NavigationViewModel _navigationVm;

        private VisualElement _mainMenuInstance;
        private VisualElement _gameplayInstance;
        private VisualElement _settingsInstance;
        private VisualElement _pauseInstance;
        private VisualElement _tutorialInstance;
        private VisualElement _victoryInstance;

        private VisualElement _mainMenuScreen;
        private VisualElement _gameplayScreen;
        private VisualElement _settingsScreen;
        private VisualElement _pauseOverlay;
        private VisualElement _tutorialOverlay;
        private VisualElement _victoryOverlay;

        private VisualElement _boardGrid;
        private Button _toolPlantButton;
        private Button _toolCrossButton;
        private Button _hintButton;
        private VisualElement _victorySparkles;
        private VisualElement _victoryPlant;

        private Toggle _musicToggle;
        private Toggle _vibrationToggle;
        private Toggle _voiceToggle;
        private Toggle _variantToggle;

        private Label _musicStatusLabel;
        private Label _vibrationStatusLabel;
        private Label _voiceStatusLabel;
        private Label _variantStatusLabel;

        private IVisualElementScheduledItem _idleTicker;
        private int _idleSeconds;

        private IVisualElementScheduledItem _pendingTapSchedule;
        private int _pendingTapCellIndex = -1;

        private readonly List<Action> _visualCleanupActions = new();

        public ProgressViewModel ProgressViewModel => _progressVm;
        public SettingsViewModel SettingsViewModel => _settingsVm;
        public GameViewModel GameViewModel => _gameVm;
        public NavigationViewModel NavigationViewModel => _navigationVm;

#if UNITY_EDITOR
        private void Reset()
        {
            TryAssignDefaultAssetsInEditor();
        }

        private void OnValidate()
        {
            TryAssignDefaultAssetsInEditor();
        }

        private void TryAssignDefaultAssetsInEditor()
        {
            if (rootVisualTree == null)
            {
                rootVisualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(VariantARootPath);
            }

            if (boardCellTemplate == null)
            {
                boardCellTemplate = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(VariantACellPath);
            }

            if (variantBRootVisualTree == null)
            {
                variantBRootVisualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(VariantBRootPath);
            }

            if (variantBCellTemplate == null)
            {
                variantBCellTemplate = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(VariantBCellPath);
            }
        }
#endif

        private void OnEnable()
        {
            _uiDocument = GetComponent<UIDocument>();
            EnsurePanelSettings();
            InitializeViewModels();
            SubscribeModelEvents();

            _navigationVm.OpenMainMenu();
            LoadActiveVariantUI();
        }

        private void OnDisable()
        {
            TeardownVisualTreeBindings();
            UnsubscribeModelEvents();

            if (_runtimePanelSettings != null)
            {
                Destroy(_runtimePanelSettings);
                _runtimePanelSettings = null;
            }
        }

        private void EnsurePanelSettings()
        {
            if (_uiDocument.panelSettings == null)
            {
                if (panelSettingsAsset != null)
                {
                    _uiDocument.panelSettings = panelSettingsAsset;
                }
                else
                {
                    _runtimePanelSettings = ScriptableObject.CreateInstance<PanelSettings>();
                    _uiDocument.panelSettings = _runtimePanelSettings;
                }
            }

            if (enforceReferenceResolution && _uiDocument.panelSettings != null)
            {
                _uiDocument.panelSettings.scaleMode = PanelScaleMode.ScaleWithScreenSize;
                _uiDocument.panelSettings.referenceResolution = new Vector2Int(1080, 1920);
                _uiDocument.panelSettings.screenMatchMode = PanelScreenMatchMode.MatchWidthOrHeight;
                _uiDocument.panelSettings.match = 0.5f;
            }
        }

        private void InitializeViewModels()
        {
            _progressVm = new ProgressViewModel();
            _settingsVm = new SettingsViewModel();
            _gameVm = new GameViewModel();
            _navigationVm = new NavigationViewModel();
        }

        private void SubscribeModelEvents()
        {
            _navigationVm.propertyChanged += OnNavigationPropertyChanged;
            _gameVm.propertyChanged += OnGamePropertyChanged;
            _gameVm.LevelCompleted += OnLevelCompleted;
            _settingsVm.propertyChanged += OnSettingsPropertyChanged;
            _settingsVm.VariantChanged += OnVariantChanged;
        }

        private void UnsubscribeModelEvents()
        {
            if (_navigationVm != null)
            {
                _navigationVm.propertyChanged -= OnNavigationPropertyChanged;
            }

            if (_gameVm != null)
            {
                _gameVm.propertyChanged -= OnGamePropertyChanged;
                _gameVm.LevelCompleted -= OnLevelCompleted;
            }

            if (_settingsVm != null)
            {
                _settingsVm.propertyChanged -= OnSettingsPropertyChanged;
                _settingsVm.VariantChanged -= OnVariantChanged;
            }
        }

        private void OnVariantChanged(UIVariant newVariant)
        {
            // Defer swap by one tick so the Toggle's pointer/change event finishes cleanly before tree rebuild.
            VisualElement currentRoot = _uiDocument.rootVisualElement;
            if (currentRoot != null)
            {
                currentRoot.schedule.Execute(LoadActiveVariantUI).StartingIn(0);
            }
            else
            {
                LoadActiveVariantUI();
            }
        }

        private void LoadActiveVariantUI()
        {
            TeardownVisualTreeBindings();

            bool isVariantB = _settingsVm.SelectedVariant == UIVariant.B;
            VisualTreeAsset targetRoot = isVariantB && variantBRootVisualTree != null
                ? variantBRootVisualTree
                : rootVisualTree;

            if (targetRoot != null)
            {
                _uiDocument.visualTreeAsset = targetRoot;
            }

            VisualElement root = _uiDocument.rootVisualElement;
            if (root == null)
            {
                Debug.LogWarning("[SudokuVerdeApp] Root VisualElement is null. Assign VariantARoot.uxml / VariantBRoot.uxml.");
                return;
            }

            BindScreensAndControls(root);
            BuildBoardCells(root, isVariantB);
            SyncNavigationVisibility();
            SyncToolVisuals();
            SyncVariantBSegmentedToggles();

            _idleSeconds = 0;
            _idleTicker = root.schedule.Execute(OnIdleTick).Every(1000);
        }

        private void TeardownVisualTreeBindings()
        {
            CancelPendingTap();
            _idleTicker?.Pause();
            _idleTicker = null;

            for (int i = 0; i < _visualCleanupActions.Count; i++)
            {
                _visualCleanupActions[i]?.Invoke();
            }

            _visualCleanupActions.Clear();
        }

        private void BindScreensAndControls(VisualElement root)
        {
            _mainMenuInstance = root.Q<VisualElement>("main-menu-instance");
            _gameplayInstance = root.Q<VisualElement>("gameplay-instance");
            _settingsInstance = root.Q<VisualElement>("settings-instance");
            _pauseInstance = root.Q<VisualElement>("pause-instance");
            _tutorialInstance = root.Q<VisualElement>("tutorial-instance");
            _victoryInstance = root.Q<VisualElement>("victory-instance");

            _mainMenuScreen = root.Q<VisualElement>("main-menu-screen");
            _gameplayScreen = root.Q<VisualElement>("gameplay-screen");
            _settingsScreen = root.Q<VisualElement>("settings-screen");
            _pauseOverlay = root.Q<VisualElement>("pause-overlay");
            _tutorialOverlay = root.Q<VisualElement>("tutorial-overlay");
            _victoryOverlay = root.Q<VisualElement>("victory-overlay");

            // Main Menu (One-way bindings to ProgressViewModel)
            if (_mainMenuScreen != null)
            {
                _mainMenuScreen.dataSource = _progressVm;
                BindText(_mainMenuScreen.Q<Label>("menu-streak-value"), nameof(ProgressViewModel.DailyStreakText));
                BindText(_mainMenuScreen.Q<Label>("menu-level-label"), nameof(ProgressViewModel.CurrentLevelLabelText));
            }

            RegisterClick(root, "btn-menu-settings", () => _navigationVm.OpenSettings(ScreenId.MainMenu));
            RegisterClick(root, "btn-play", StartStandardLevel);
            RegisterClick(root, "btn-daily-challenge", StartDailyChallenge);
            VisualElement quitConfirmOverlay = root.Q<VisualElement>("quit-confirm-overlay");
            RegisterClick(root, "btn-quit", () => quitConfirmOverlay?.RemoveFromClassList("is-hidden"));
            RegisterClick(root, "btn-quit-confirm", QuitGame);
            RegisterClick(root, "btn-quit-cancel", () => quitConfirmOverlay?.AddToClassList("is-hidden"));
            RegisterClick(root, "btn-prev-level", () => _progressVm.SelectPreviousLevel());
            RegisterClick(root, "btn-next-level", () => _progressVm.SelectNextLevel());

            // Gameplay (One-way bindings to GameViewModel)
            if (_gameplayScreen != null)
            {
                _gameplayScreen.dataSource = _gameVm;
                BindText(_gameplayScreen.Q<Label>("gameplay-level-label"), nameof(GameViewModel.LevelTitleText));
                BindText(_gameplayScreen.Q<Label>("plants-progress-label"), nameof(GameViewModel.PlantsProgressText));
                BindText(_gameplayScreen.Q<Label>("hints-remaining-label"), nameof(GameViewModel.HintsRemainingText));
            }

            _toolPlantButton = root.Q<Button>("btn-tool-plant");
            _toolCrossButton = root.Q<Button>("btn-tool-cross");
            _hintButton = root.Q<Button>("btn-hint");

            RegisterClick(root, "btn-gameplay-pause", () =>
            {
                CancelPendingTap();
                ResetIdleTimer();
                _navigationVm.OpenPause();
            });
            RegisterClick(root, "btn-gameplay-help", () =>
            {
                CancelPendingTap();
                ResetIdleTimer();
                _navigationVm.OpenTutorial();
            });
            RegisterClick(root, "btn-tool-plant", () =>
            {
                ResetIdleTimer();
                _gameVm.SetTool(ToolMode.Plant);
            });
            RegisterClick(root, "btn-tool-cross", () =>
            {
                ResetIdleTimer();
                _gameVm.SetTool(ToolMode.Cross);
            });
            RegisterClick(root, "btn-hint", () =>
            {
                CancelPendingTap();
                ResetIdleTimer();
                _gameVm.UseHint();
            });

            // Settings (Two-way bindings for toggles + One-way bindings for status labels)
            if (_settingsScreen != null)
            {
                _settingsScreen.dataSource = _settingsVm;
                _musicToggle = _settingsScreen.Q<Toggle>("toggle-music");
                _vibrationToggle = _settingsScreen.Q<Toggle>("toggle-vibration");
                _voiceToggle = _settingsScreen.Q<Toggle>("toggle-voice");
                _variantToggle = _settingsScreen.Q<Toggle>("toggle-variant");

                _musicStatusLabel = _settingsScreen.Q<Label>("music-status-label");
                _vibrationStatusLabel = _settingsScreen.Q<Label>("vibration-status-label");
                _voiceStatusLabel = _settingsScreen.Q<Label>("voice-status-label");
                _variantStatusLabel = _settingsScreen.Q<Label>("variant-status-label");

                BindToggleTwoWay(_musicToggle, nameof(SettingsViewModel.MusicEnabled));
                BindToggleTwoWay(_vibrationToggle, nameof(SettingsViewModel.VibrationEnabled));
                BindToggleTwoWay(_voiceToggle, nameof(SettingsViewModel.VoiceEnabled));
                BindToggleTwoWay(_variantToggle, nameof(SettingsViewModel.IsVariantB));

                BindText(_musicStatusLabel, nameof(SettingsViewModel.MusicStatusText));
                BindText(_vibrationStatusLabel, nameof(SettingsViewModel.VibrationStatusText));
                BindText(_voiceStatusLabel, nameof(SettingsViewModel.VoiceStatusText));
                BindText(_variantStatusLabel, nameof(SettingsViewModel.SelectedVariantText));

                RegisterToggleSync(_musicToggle, v => _settingsVm.MusicEnabled = v);
                RegisterToggleSync(_vibrationToggle, v => _settingsVm.VibrationEnabled = v);
                RegisterToggleSync(_voiceToggle, v => _settingsVm.VoiceEnabled = v);
                RegisterToggleSync(_variantToggle, v => _settingsVm.IsVariantB = v);
            }

            RegisterClick(root, "btn-settings-back", () => _navigationVm.ReturnFromSettings());

            // Pause Overlay
            RegisterClick(root, "btn-pause-resume", () => _navigationVm.CloseOverlay());
            RegisterClick(root, "btn-pause-restart", () =>
            {
                CancelPendingTap();
                ResetIdleTimer();
                _gameVm.RestartLevel();
                _navigationVm.CloseOverlay();
            });
            RegisterClick(root, "btn-pause-help", () => _navigationVm.OpenTutorial());
            RegisterClick(root, "btn-pause-settings", () => _navigationVm.OpenSettings(ScreenId.Gameplay));
            RegisterClick(root, "btn-pause-menu", () => _navigationVm.OpenMainMenu());

            // Tutorial / Instructions Overlay
            RegisterClick(root, "btn-tutorial-back", () => _navigationVm.CloseOverlay());
            RegisterClick(root, "btn-tutorial-confirm", () => _navigationVm.CloseOverlay());

            // Victory Overlay
            _victorySparkles = root.Q<VisualElement>("victory-sparkles");
            _victoryPlant = root.Q<VisualElement>("victory-plant");

            RegisterClick(root, "btn-victory-close", () => _navigationVm.OpenMainMenu());
            RegisterClick(root, "btn-victory-menu", () => _navigationVm.OpenMainMenu());
            RegisterClick(root, "btn-victory-restart", () =>
            {
                CancelPendingTap();
                ResetIdleTimer();
                _gameVm.RestartLevel();
                _navigationVm.CloseOverlay();
            });
            RegisterClick(root, "btn-victory-next", StartNextLevel);
        }

        private void BuildBoardCells(VisualElement root, bool isVariantB)
        {
            _boardGrid = root.Q<VisualElement>("board-grid");
            if (_boardGrid == null)
            {
                return;
            }

            _boardGrid.Clear();
            VisualTreeAsset cellAsset = isVariantB && variantBCellTemplate != null
                ? variantBCellTemplate
                : boardCellTemplate;
            float cellPct = isVariantB ? 23.8f : 23.2f;

            for (int i = 0; i < _gameVm.Cells.Length; i++)
            {
                int cellIndex = i;
                CellViewModel cellVm = _gameVm.Cells[i];

                VisualElement cellRoot = cellAsset != null
                    ? cellAsset.Instantiate()
                    : CreateFallbackCellElement();

                Button cellButton = cellRoot as Button ?? cellRoot.Q<Button>("board-cell");
                if (cellButton == null)
                {
                    continue;
                }

                if (cellRoot != cellButton)
                {
                    cellRoot.style.width = new Length(cellPct, LengthUnit.Percent);
                    cellRoot.style.height = new Length(cellPct, LengthUnit.Percent);
                    cellButton.style.width = new Length(100f, LengthUnit.Percent);
                    cellButton.style.height = new Length(100f, LengthUnit.Percent);
                }

                cellButton.dataSource = cellVm;

                Action clickHandler = isVariantB
                    ? () => HandleVariantBCellTap(cellIndex)
                    : () =>
                    {
                        ResetIdleTimer();
                        _gameVm.InteractCell(cellIndex);
                    };

                cellButton.clicked += clickHandler;
                _visualCleanupActions.Add(() => cellButton.clicked -= clickHandler);

                VisualElement plantIcon = cellButton.Q<VisualElement>("cell-plant-icon");
                VisualElement crossIcon = cellButton.Q<VisualElement>("cell-cross-icon");

                EventHandler<BindablePropertyChangedEventArgs> cellChangedHandler = (_, __) =>
                    SyncCellVisual(cellButton, plantIcon, crossIcon, cellVm);
                cellVm.propertyChanged += cellChangedHandler;
                _visualCleanupActions.Add(() => cellVm.propertyChanged -= cellChangedHandler);

                _boardGrid.Add(cellRoot);
                SyncCellVisual(cellButton, plantIcon, crossIcon, cellVm);
            }
        }

        private void HandleVariantBCellTap(int cellIndex)
        {
            ResetIdleTimer();
            if (_gameVm.IsCompleted || _navigationVm.CurrentScreen != ScreenId.Gameplay || _navigationVm.CurrentOverlay != OverlayId.None)
            {
                CancelPendingTap();
                return;
            }

            // Second tap on the same cell within the window -> Double-click places/toggles X.
            if (_pendingTapSchedule != null && _pendingTapCellIndex == cellIndex)
            {
                CancelPendingTap();
                _gameVm.PlaceOrToggleCross(cellIndex);
                return;
            }

            // Tap on a different cell while another tap was pending -> flush previous single-click immediately.
            if (_pendingTapSchedule != null && _pendingTapCellIndex >= 0)
            {
                int previousIndex = _pendingTapCellIndex;
                CancelPendingTap();
                _gameVm.PlaceOrTogglePlant(previousIndex);
                if (_gameVm.IsCompleted)
                {
                    return;
                }
            }

            _pendingTapCellIndex = cellIndex;
            VisualElement scheduleHost = _uiDocument.rootVisualElement;
            if (scheduleHost == null)
            {
                _pendingTapCellIndex = -1;
                _gameVm.PlaceOrTogglePlant(cellIndex);
                return;
            }

            _pendingTapSchedule = scheduleHost.schedule.Execute(() =>
            {
                int committedIndex = _pendingTapCellIndex;
                _pendingTapSchedule = null;
                _pendingTapCellIndex = -1;
                if (committedIndex >= 0 &&
                    _navigationVm.CurrentScreen == ScreenId.Gameplay &&
                    _navigationVm.CurrentOverlay == OverlayId.None)
                {
                    _gameVm.PlaceOrTogglePlant(committedIndex);
                }
            }).StartingIn(doubleClickWindowMs);
        }

        private void CancelPendingTap()
        {
            _pendingTapSchedule?.Pause();
            _pendingTapSchedule = null;
            _pendingTapCellIndex = -1;
        }

        private static VisualElement CreateFallbackCellElement()
        {
            var btn = new Button { name = "board-cell" };
            btn.AddToClassList("board-cell");

            var plant = new VisualElement { name = "cell-plant-icon", pickingMode = PickingMode.Ignore };
            plant.AddToClassList("cell-icon-plant");
            plant.AddToClassList("is-hidden");

            var cross = new VisualElement { name = "cell-cross-icon", pickingMode = PickingMode.Ignore };
            cross.AddToClassList("cell-icon-cross");
            cross.AddToClassList("is-hidden");

            btn.Add(plant);
            btn.Add(cross);
            return btn;
        }

        private void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        private void StartStandardLevel()
        {
            CancelPendingTap();
            ResetIdleTimer();
            LevelDefinition level = LevelCatalog.GetLevel(_progressVm.CurrentLevel);
            _gameVm.LoadLevel(level, isDailyChallenge: false);
            _navigationVm.OpenGameplay();
            ShowVariantBInstructionsIfFirstTime();
        }

        private void StartDailyChallenge()
        {
            CancelPendingTap();
            ResetIdleTimer();
            LevelDefinition level = LevelCatalog.GetDailyChallengeLevel(DateTime.Now);
            _gameVm.LoadLevel(level, isDailyChallenge: true);
            _navigationVm.OpenGameplay();
        }

        private void StartNextLevel()
        {
            CancelPendingTap();
            ResetIdleTimer();
            LevelDefinition next = LevelCatalog.GetLevel(_progressVm.CurrentLevel);
            _gameVm.LoadLevel(next, isDailyChallenge: false);
            _navigationVm.OpenGameplay();
        }

        private void ShowVariantBInstructionsIfFirstTime()
        {
            if (_settingsVm.SelectedVariant == UIVariant.B && !SudokuPersistence.HasSeenVariantBInstructions())
            {
                SudokuPersistence.MarkVariantBInstructionsSeen();
                _navigationVm.OpenTutorial();
            }
        }

        private void OnLevelCompleted(bool isDailyChallenge)
        {
            CancelPendingTap();
            ResetIdleTimer();
            if (_gameVm.ShouldAwardProgressOnCompletion())
            {
                _progressVm.RecordLevelCompleted(isDailyChallenge);
            }

            _navigationVm.OpenVictory();
            PlayVictoryPulse();
        }

        private void OnNavigationPropertyChanged(object sender, BindablePropertyChangedEventArgs args)
        {
            CancelPendingTap();
            ResetIdleTimer();
            SyncNavigationVisibility();
        }

        private void OnGamePropertyChanged(object sender, BindablePropertyChangedEventArgs args)
        {
            if (args.propertyName == nameof(GameViewModel.ActiveTool))
            {
                SyncToolVisuals();
            }
        }

        private void OnSettingsPropertyChanged(object sender, BindablePropertyChangedEventArgs args)
        {
            SyncVariantBSegmentedToggles();
        }

        private void SyncVariantBSegmentedToggles()
        {
            _musicToggle?.SetValueWithoutNotify(_settingsVm.MusicEnabled);
            _vibrationToggle?.SetValueWithoutNotify(_settingsVm.VibrationEnabled);
            _voiceToggle?.SetValueWithoutNotify(_settingsVm.VoiceEnabled);
            _variantToggle?.SetValueWithoutNotify(_settingsVm.IsVariantB);

            if (_musicStatusLabel != null) _musicStatusLabel.text = _settingsVm.MusicStatusText;
            if (_vibrationStatusLabel != null) _vibrationStatusLabel.text = _settingsVm.VibrationStatusText;
            if (_voiceStatusLabel != null) _voiceStatusLabel.text = _settingsVm.VoiceStatusText;
            if (_variantStatusLabel != null) _variantStatusLabel.text = _settingsVm.SelectedVariantText;

            ApplySegmentedState(_musicToggle, _settingsVm.MusicEnabled);
            ApplySegmentedState(_vibrationToggle, _settingsVm.VibrationEnabled);
            ApplySegmentedState(_variantToggle, _settingsVm.IsVariantB);
        }

        private static void ApplySegmentedState(Toggle toggle, bool isRightSelected)
        {
            if (toggle == null || !toggle.ClassListContains("vb-seg-toggle"))
            {
                return;
            }

            toggle.EnableInClassList("vb-seg-toggle--on", isRightSelected);
            toggle.EnableInClassList("vb-seg-toggle--off", !isRightSelected);
        }

        private void SyncNavigationVisibility()
        {
            bool showMenu = _navigationVm.CurrentScreen == ScreenId.MainMenu;
            bool showGameplay = _navigationVm.CurrentScreen == ScreenId.Gameplay;
            bool showSettings = _navigationVm.CurrentScreen == ScreenId.Settings;

            bool showPause = _navigationVm.CurrentOverlay == OverlayId.Pause;
            bool showTutorial = _navigationVm.CurrentOverlay == OverlayId.Tutorial;
            bool showVictory = _navigationVm.CurrentOverlay == OverlayId.Victory;

            SetVisible(_mainMenuInstance, showMenu);
            SetVisible(_mainMenuScreen, showMenu);

            SetVisible(_gameplayInstance, showGameplay);
            SetVisible(_gameplayScreen, showGameplay);

            SetVisible(_settingsInstance, showSettings);
            SetVisible(_settingsScreen, showSettings);

            SetVisible(_pauseInstance, showPause);
            SetVisible(_pauseOverlay, showPause);

            SetVisible(_tutorialInstance, showTutorial);
            SetVisible(_tutorialOverlay, showTutorial);

            SetVisible(_victoryInstance, showVictory);
            SetVisible(_victoryOverlay, showVictory);
        }

        private void SyncToolVisuals()
        {
            bool isPlant = _gameVm.ActiveTool == ToolMode.Plant;
            if (_toolPlantButton != null)
            {
                _toolPlantButton.EnableInClassList("tool-btn--active", isPlant);
            }

            if (_toolCrossButton != null)
            {
                _toolCrossButton.EnableInClassList("tool-btn--active", !isPlant);
            }
        }

        private static void SyncCellVisual(Button button, VisualElement plantIcon, VisualElement crossIcon, CellViewModel vm)
        {
            bool isPlant = vm.State == CellState.Plant;
            button.EnableInClassList("cell--orange", vm.RegionId == 0);
            button.EnableInClassList("cell--blue", vm.RegionId == 1);
            button.EnableInClassList("cell--green", vm.RegionId == 2);
            button.EnableInClassList("cell--light-green", vm.RegionId == 3);
            button.EnableInClassList("cell--player-plant", isPlant && !vm.IsHinted);
            button.EnableInClassList("cell--hinted", isPlant && vm.IsHinted);
            button.EnableInClassList("cell--conflict", vm.HasConflict);

            SetVisible(plantIcon, isPlant);
            SetVisible(crossIcon, vm.State == CellState.Cross);
        }

        private void PlayVictoryPulse()
        {
            if (_victorySparkles == null || _victoryPlant == null)
            {
                return;
            }

            _victorySparkles.AddToClassList("victory-celebrate");
            _victoryPlant.AddToClassList("victory-celebrate");
            _victoryPlant.schedule.Execute(() =>
            {
                _victorySparkles?.RemoveFromClassList("victory-celebrate");
                _victoryPlant?.RemoveFromClassList("victory-celebrate");
            }).StartingIn(380);
        }

        private void ResetIdleTimer()
        {
            _idleSeconds = 0;
            ClearNudgeClasses(_hintButton);
        }

        private void OnIdleTick()
        {
            if (_navigationVm == null || _gameVm == null ||
                _navigationVm.CurrentScreen != ScreenId.Gameplay ||
                _navigationVm.CurrentOverlay != OverlayId.None ||
                _gameVm.IsCompleted)
            {
                ResetIdleTimer();
                return;
            }

            _idleSeconds++;

            if (_idleSeconds % 60 == 0 && _gameVm.HintsRemaining > 0)
            {
                PlayNudgeAnimation(_hintButton);
            }
        }

        private void PlayNudgeAnimation(VisualElement target)
        {
            if (target == null)
            {
                return;
            }

            int nudgeVersion = _idleSeconds;
            target.RemoveFromClassList("nudge-right");
            target.AddToClassList("nudge-left");

            target.schedule.Execute(() =>
            {
                if (_idleSeconds < nudgeVersion) return;
                target.RemoveFromClassList("nudge-left");
                target.AddToClassList("nudge-right");
            }).StartingIn(130);

            target.schedule.Execute(() =>
            {
                if (_idleSeconds < nudgeVersion) return;
                target.RemoveFromClassList("nudge-right");
                target.AddToClassList("nudge-left");
            }).StartingIn(260);

            target.schedule.Execute(() => ClearNudgeClasses(target)).StartingIn(390);
        }

        private static void ClearNudgeClasses(VisualElement target)
        {
            if (target == null)
            {
                return;
            }

            target.RemoveFromClassList("nudge-left");
            target.RemoveFromClassList("nudge-right");
        }

        private static void SetVisible(VisualElement element, bool visible)
        {
            if (element == null)
            {
                return;
            }

            element.EnableInClassList("is-hidden", !visible);
        }

        private static void BindText(Label label, string propertyName)
        {
            if (label == null)
            {
                return;
            }

            label.SetBinding("text", new DataBinding
            {
                dataSourcePath = new PropertyPath(propertyName),
                bindingMode = BindingMode.ToTarget
            });
        }

        private static void BindToggleTwoWay(Toggle toggle, string propertyName)
        {
            if (toggle == null)
            {
                return;
            }

            toggle.SetBinding("value", new DataBinding
            {
                dataSourcePath = new PropertyPath(propertyName),
                bindingMode = BindingMode.TwoWay
            });
        }

        private void RegisterToggleSync(Toggle toggle, Action<bool> setter)
        {
            if (toggle == null || setter == null)
            {
                return;
            }

            EventCallback<ChangeEvent<bool>> handler = evt =>
            {
                setter(evt.newValue);
                SyncVariantBSegmentedToggles();
            };

            toggle.RegisterValueChangedCallback(handler);
            _visualCleanupActions.Add(() => toggle.UnregisterValueChangedCallback(handler));
        }

        private void RegisterClick(VisualElement root, string buttonName, Action callback)
        {
            Button button = root.Q<Button>(buttonName);
            if (button != null && callback != null)
            {
                button.clicked += callback;
                _visualCleanupActions.Add(() => button.clicked -= callback);
            }
        }
    }
}

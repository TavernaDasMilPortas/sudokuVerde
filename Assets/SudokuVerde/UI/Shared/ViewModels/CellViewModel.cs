using Unity.Properties;
using SudokuVerde.Core;

namespace SudokuVerde.UI.Shared.ViewModels
{
    public sealed class CellViewModel : BindableViewModelBase
    {
        private int _index;
        private int _regionId;
        private CellState _state;
        private bool _hasConflict;
        private bool _isHinted;

        [CreateProperty]
        public int Index
        {
            get => _index;
            set => SetProperty(ref _index, value);
        }

        [CreateProperty]
        public int RegionId
        {
            get => _regionId;
            set => SetProperty(ref _regionId, value);
        }

        [CreateProperty]
        public CellState State
        {
            get => _state;
            set => SetProperty(ref _state, value);
        }

        [CreateProperty]
        public bool HasConflict
        {
            get => _hasConflict;
            set => SetProperty(ref _hasConflict, value);
        }

        [CreateProperty]
        public bool IsHinted
        {
            get => _isHinted;
            set => SetProperty(ref _isHinted, value);
        }

        public CellViewModel(int index, int regionId)
        {
            _index = index;
            _regionId = regionId;
            _state = CellState.Empty;
        }
    }
}

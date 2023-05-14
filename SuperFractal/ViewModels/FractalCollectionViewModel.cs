using AlgebraicFractals;
using AlgebraicFractals.Fractals;
using SuperFractal.Commands;
using SuperFractal.Stores;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperFractal.ViewModels
{
    public class FractalCollectionViewModel : ViewModelBase
    {
        FractalStore _fractalStore;
        NavigationStore _navigationStore;
        Coord<double> LastMousePosInWorld { get; init; }
        private AlgebraicFractal selectedFractal;
        public AlgebraicFractal SelectedFractal
        {
            get
            {
                return selectedFractal;
            }
            set
            {
                selectedFractal = value;
                OnPropertyChanged(nameof(SelectedFractal));
            }
        }

        ObservableCollection<AlgebraicFractal> _algebraicFractals;

        public IEnumerable<ExistingFractals> ExistingFractals { get; set; }

        private ExistingFractals _fractalToAdd;
        public ExistingFractals FractalSelectedToAdd
        {
            get
            {
                return _fractalToAdd;
            }
            set
            {
                _fractalToAdd = value;
                OnPropertyChanged(nameof(FractalSelectedToAdd));
            }
        }

        public IEnumerable<AlgebraicFractal>  Fractals => _algebraicFractals;
        public FractalCollectionViewModel(Coord<double> lastMousePos, FractalStore fractals, NavigationStore navigation)
        {
            _fractalStore = fractals;
            _navigationStore = navigation;
            LastMousePosInWorld = lastMousePos;
            _algebraicFractals = new ObservableCollection<AlgebraicFractal>(_fractalStore.Fractals);
            ExistingFractals = Enum.GetValues(typeof(ExistingFractals)).Cast<ExistingFractals>();
        }
        private void DeleteItem(object e)
        {
            var obj = SelectedFractal;
            if (obj == null) return;
            _algebraicFractals.Remove(obj);
            _fractalStore.Fractals.Remove(obj);
        }

        private void AddItem(object e)
        {
            var obj = FractalSelectedToAdd;
            if (obj == null) return;
            switch(obj)
            {
                case AlgebraicFractals.Fractals.ExistingFractals.MandelbrotSet:
                    _navigationStore.CurrentViewModel = new AddMandelbrotSetViewModel(LastMousePosInWorld, _fractalStore,_navigationStore);
                break;
                case AlgebraicFractals.Fractals.ExistingFractals.MandelbrotSetNPow:
                    _navigationStore.CurrentViewModel = new AddMandelbrotSetNPowViewModel(LastMousePosInWorld, _fractalStore, _navigationStore);
                    break;
                case AlgebraicFractals.Fractals.ExistingFractals.JuliaSet:
                    _navigationStore.CurrentViewModel = new AddJuliaSetViewModel(LastMousePosInWorld, _fractalStore, _navigationStore);
                    break;
                case AlgebraicFractals.Fractals.ExistingFractals.Newton:
                    _navigationStore.CurrentViewModel = new AddNewtonViewModel(LastMousePosInWorld, _fractalStore, _navigationStore);
                    break;

            }
        }

        private void EditFractal(object e)
        {
            var obj = SelectedFractal;
            if (obj == null) return;
            if (obj is MandelbrotSet)
            {
                _navigationStore.CurrentViewModel = new AddMandelbrotSetViewModel(LastMousePosInWorld,
                    _fractalStore, _navigationStore, obj as MandelbrotSet);
            }
            else if (obj is MandelbrotSetNPow)
            {
                _navigationStore.CurrentViewModel = new AddMandelbrotSetNPowViewModel(LastMousePosInWorld,
                    _fractalStore, _navigationStore, obj as MandelbrotSetNPow);
            }
            else if(obj is JuliaSet) 
            {
                _navigationStore.CurrentViewModel = new AddJuliaSetViewModel(LastMousePosInWorld,
                        _fractalStore, _navigationStore, obj as JuliaSet);
            }
            else if(obj is Newton)
            {
                _navigationStore.CurrentViewModel = new AddNewtonViewModel(LastMousePosInWorld,
                    _fractalStore, _navigationStore, obj as Newton);
            }
        }
        private RelayCommand deleteSelected;
        public RelayCommand DeleteSelected
        {
            get
            {
                return deleteSelected ??
                    (deleteSelected = new RelayCommand(DeleteItem));
            }
        }
        private RelayCommand editSelected;
        public RelayCommand EditSelected
        {
            get
            {
                return editSelected ??
                    (editSelected = new RelayCommand(EditFractal));
            }
        }

        private RelayCommand addFractal;
        public RelayCommand AddFractal
        {
            get
            {
                return addFractal ??
                    (addFractal = new RelayCommand(AddItem));
            }
        }
    }
}

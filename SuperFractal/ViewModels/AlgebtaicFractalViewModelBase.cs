using AlgebraicFractals;
using SuperFractal.Commands;
using SuperFractal.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperFractal.ViewModels
{
    public abstract class AlgebtaicFractalViewModelBase : ViewModelBase
    {
        protected NavigationStore _navigation;
        protected FractalStore _fractalStore;
        private AlgebraicFractal Fractal { get; set; }
        public AlgebtaicFractalViewModelBase(Coord<double> mousePos, FractalStore fractals, NavigationStore navigation, AlgebraicFractal? fractal = null)
        {
            _fractalStore = fractals;
            _navigation = navigation;
            Fractal = fractal;
            CenterX = Fractal?.Center.X ?? mousePos.X;
            CenterY = Fractal?.Center.Y ?? mousePos.Y;
            Caption = Fractal?.Caption ?? "Фрактал Мандельброта";
        }
        private string caption;
        public string Caption
        {
            get
            {
                return caption;
            }
            set
            {
                caption = value;
                OnPropertyChanged(nameof(Caption));
            }
        }

        private double centerX;
        public double CenterX
        {
            get
            {
                return centerX;
            }
            set
            {
                centerX = value;
                OnPropertyChanged(nameof(CenterX));
            }
        }

        private double centerY;
        public double CenterY
        {
            get
            {
                return centerY;
            }
            set
            {
                centerY = value;
                OnPropertyChanged(nameof(CenterY));
            }
        }
        protected abstract AlgebraicFractal ModifyFractal(AlgebraicFractal fractal);

        private RelayCommand addFractal;
        public RelayCommand AddFractal
        {
            get
            {
                return addFractal ??
                    (addFractal = new RelayCommand(e =>
                    {
                        bool IAdd = Fractal is null;
                        Fractal = ModifyFractal(Fractal);
                        if (Caption != "") Fractal.Caption = Caption;
                        Fractal.Center = new Coord<double>(CenterX, CenterY);
                        if (IAdd) _fractalStore.Fractals.Add(Fractal);
                        _navigation.CurrentViewModel = new FractalCollectionViewModel(Fractal.Center, _fractalStore, _navigation);
                    }));
            }
        }

        private RelayCommand backPressed;
        public RelayCommand BackPressed
        {
            get
            {
                return backPressed ??
                    (backPressed = new RelayCommand(e =>
                    {
                        _navigation.CurrentViewModel = new FractalCollectionViewModel(new Coord<double>(CenterX,CenterY), _fractalStore, _navigation);
                    }));
            }
        }
    }
}

using AlgebraicFractals;
using AlgebraicFractals.Fractals;
using SuperFractal.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperFractal.ViewModels
{
    public class AddMandelbrotSetNPowViewModel : AlgebtaicFractalViewModelBase
    {

        private int pow;
        public int Power
        {
            get
            {
                return pow;
            }
            set
            {
                if (value <= 0 || value > 20) return;
                pow = value;
                OnPropertyChanged(nameof(Power));
            }
        }
        public AddMandelbrotSetNPowViewModel(Coord<double> mousePos, FractalStore fractals, NavigationStore navigation, MandelbrotSetNPow? fractal = null)
            : base(mousePos, fractals, navigation, fractal)
        {
            Power = fractal?.Power?? 3;
            Caption = fractal?.Caption ?? "Фрактал Mандельброту ступеня N";
        }

        protected override AlgebraicFractal ModifyFractal(AlgebraicFractal fractal)
        {
            var fr =  fractal as MandelbrotSetNPow ?? new MandelbrotSetNPow(Power);
            fr.Power = Power;
            return fr;
        }
    }
}

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
    internal class AddNewtonViewModel : AlgebtaicFractalViewModelBase
    {
		private double _eps;

        public AddNewtonViewModel(Coord<double> mousePos, FractalStore fractals, NavigationStore navigation, Newton? fractal = null) : base(mousePos, fractals, navigation, fractal)
        {
			Eps = fractal?.Eps?? 0.00001;
			Caption = fractal?.Caption ?? "Фрактал Ньютона";
        }

        public double Eps
		{
			get
			{
				return _eps;
			}
			set
			{
				if (value <= 0 || value >= 1) return;
				_eps = value;
				OnPropertyChanged(nameof(Eps));
			}
		}
		protected override AlgebraicFractal ModifyFractal(AlgebraicFractal fractal)
        {
            var fr = fractal as Newton ?? new Newton();
			fr.Eps = Eps;
			return fr;

        }
    }
}

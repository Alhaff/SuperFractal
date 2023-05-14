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
    internal class AddJuliaSetViewModel : AlgebtaicFractalViewModelBase
    {
		private double _CX;
		public double CX
		{
			get
			{
				return _CX;
			}
			set
			{
				_CX = value;
				OnPropertyChanged(nameof(CX));
			}
		}

		private double _CY;
		public double CY
		{
			get
			{
				return _CY;
			}
			set
			{
				_CY = value;
				OnPropertyChanged(nameof(CY));
			}
		}
		public AddJuliaSetViewModel(Coord<double> mousePos, FractalStore fractals, NavigationStore navigation, JuliaSet? fractal = null) : base(mousePos, fractals, navigation, fractal)
        {
			_CX = fractal?.CPos.X ?? 0.36;
			_CY = fractal?.CPos.Y ?? 0.36;
			Caption = fractal?.Caption ?? "Фрактал Жуліа";
        }

        protected override AlgebraicFractal ModifyFractal(AlgebraicFractal fractal)
        {
			var fr = fractal as JuliaSet?? new JuliaSet();
			fr.CPos = new Coord<double>(CX, CY);
			return fr;
        }
    }
}

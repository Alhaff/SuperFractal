using AlgebraicFractals;
using AlgebraicFractals.Fractals;
using SuperFractal.Commands;
using SuperFractal.Stores;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperFractal.ViewModels
{
    public class AddMandelbrotSetViewModel :AlgebtaicFractalViewModelBase
    {
        public AddMandelbrotSetViewModel(Coord<double> mousePos, FractalStore fractals, NavigationStore navigation, MandelbrotSet? fractal = null)
            : base(mousePos, fractals, navigation, fractal)
        {
        }

        protected override AlgebraicFractal ModifyFractal(AlgebraicFractal fractal)
        {
            fractal = fractal?? new MandelbrotSet();
            return fractal;
        }
    }
}

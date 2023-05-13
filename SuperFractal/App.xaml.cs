using AlgebraicFractals.Fractals;
using SuperFractal.Stores;
using SuperFractal.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace SuperFractal
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private FractalStore _FractalStore = new FractalStore();
        protected override void OnStartup(StartupEventArgs e)
        {
            _FractalStore.Fractals.Add(new MandelbrotSet() { Center = new AlgebraicFractals.Coord<double>(1, 1), ThreadCount = 64 });
            MainWindow = new MainWindow()
            {
                DataContext = new MainViewModel(_FractalStore)
            };
            MainWindow.Show();
           
        }
    }
}

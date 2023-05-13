using AlgebraicFractals;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperFractal.Stores
{
    public class FractalStore
    {
        public List<AlgebraicFractal> Fractals { get; set; }
        public CreateMultiFractal MultiFractalCreator { get; set; }
        public Dictionary<string,CreateMultiFractal> MultiFractalsCreators { get; init; }
        public FractalStore() 
        { 
            Fractals = new List<AlgebraicFractal>();
            MultiFractalsCreators = new Dictionary<string, CreateMultiFractal>()
            {
                {"Simple", AlgebraicFractal.CreateMultiFractalSimple},
                {"Simple in TreadPool", AlgebraicFractal.CreateMultiFractalSimpleInThreadPool },
                {"AVX2", AlgebraicFractal.CreateMultiFractalIntrinsics },
                {"AVX2 in ThreadPool", AlgebraicFractal.CreateMultiFractalIntrinsicsInThreadPool }
            };
            MultiFractalCreator = AlgebraicFractal.CreateMultiFractalIntrinsicsInThreadPool;
        }
        public int ThreadCount { get; set; } = 64;
        public int MaxIterations { get; set; } = 64;
    }
}

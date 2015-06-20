using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextMiningConsoleApp.PrincipleComponentAnalysis
{
    /// <summary>
    /// struct for eigen values and eigenvectors
    /// </summary>
    struct Eigen : IComparable<Eigen>
    {
        public double Value;
        public Vector<double> Vector;

        public Eigen(double value, Vector<double> vector)
        {
            Value = value;
            Vector = vector;
        }

        public int CompareTo(Eigen other)
        {
            return this.Value.CompareTo(other.Value);
        }
    }
    
}

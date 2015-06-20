using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextMiningConsoleApp.PrincipleComponentAnalysis
{
    class ICA
    {
        /// <summary>
        /// Term by Document Matrix.
        /// Terms in rows and Documents in cols.
        /// </summary>
        private double[,] termdocmatrix;
        private int rows;
        private int cols;

        private Eigen[] principalEigenValues;

        public ICA(double[,] termdocmatrix, Eigen[] principalEigenValues)
        {
            this.termdocmatrix = termdocmatrix;
            rows = termdocmatrix.GetLength(0);
            cols = termdocmatrix.GetLength(1);

            this.principalEigenValues = principalEigenValues;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Topic by Document Matrix</returns>
        public double[,] Compute()
        {

            int topics = principalEigenValues.Length;
            ////// whitening

            // Z = D*R*X
            
            Matrix<double> Z = Matrix<double>.Build.DenseIdentity(topics);

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    Z[i, j] = Math.Pow(principalEigenValues[i].Value, -0.5) * principalEigenValues[i].Vector[j];
                }

            }

            Matrix<double> X = Matrix<double>.Build.DenseOfArray(termdocmatrix);

            Z = Z.Multiply(X);

            // random weight matrix
            Matrix<double> W = Matrix<double>.Build.Random(topics, X.RowCount);

            W = W / Math.Sqrt(W.Multiply(W.Transpose()).L2Norm());

            // iterate until convergence
            int iter = 0;
            Matrix<double> Wold;
            do
            {
                Wold = W;
                W = 1.5 * W - 0.5 * W.Multiply(W.Transpose()).Multiply(W);
            }
            while (converge(W, Wold) > 0.000000001 && iter < 1000000);
            //while(1 - (W.Transpose().Multiply(Wold).Diagonal().AbsoluteMinimum()) < 0.00000000000000001);


            //// iterate
            //W = W.Multiply(Z).Power(3).Multiply(Z.Transpose()) - ;

            return W.Multiply(X).ToArray();

        }

        private double converge(Matrix<double> m1, Matrix<double> m2)
        {
            Matrix<double> sub = m1 - m2;
            double delta=0;
            for (int i = 0; i < sub.RowCount; i++)
                for (int j = 0; j < sub.ColumnCount; j++)
                    delta += sub[i, j];
            double r = delta / (sub.RowCount * sub.ColumnCount);
            Console.WriteLine(r);
            return r;
        }
    }

    
}

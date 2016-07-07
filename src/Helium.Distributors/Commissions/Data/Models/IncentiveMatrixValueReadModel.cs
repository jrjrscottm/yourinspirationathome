using System;
using System.Collections.Generic;
using System.Linq;
using Helium.Distributors.Commissions.Incentives;

namespace Helium.Distributors.Commissions.Data.Models
{
    public class IncentiveMatrixValueReadModel : IncentiveReadModel, IIncentiveMatrix
    {
        public IncentiveMatrixValueReadModel(IEnumerable<List<decimal>> yaxes, int size)
        {
            ValueType = IncentiveValueType.Matrix;
            ParseMatrix(yaxes, size);
        }
        public decimal[,] Values { get; set; }

        public decimal this[int x, int y]
        {
            get
            {
                if (x <= 0 || y <= 0)
                {
                    throw new ArgumentException("Indexes are 1 based, not 0 based.");
                }

                return Values[x, y];
            }  
        } 

        private void ParseMatrix(IEnumerable<List<decimal>> yaxes, int size)
        {
            var matrix = new decimal[yaxes.Count() + 1,size + 1];
            var x = 1;

            foreach (var xaxes in yaxes)
            {
                var y = 1;
                foreach (var value in xaxes)
                {
                    matrix[x, y] = value;
                    y++;
                }
                x++;
            }

            Values = matrix;
        }
    }
}
//
// Encog(tm) Core v3.0 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2011 Heaton Research, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//   
// For more information on Heaton Research copyrights, licenses 
// and trademarks visit:
// http://www.heatonresearch.com/copyright
//
using Encog.MathUtil.Matrices;
using Encog.ML;
using Encog.ML.Data;
using Encog.Neural.Networks;
using System;

namespace Encog.Neural.BAM
{
    /// <summary>
    /// Bidirectional associative memory (BAM) is a type of neural network 
    /// developed by Bart Kosko in 1988. The BAM is a recurrent neural network 
    /// that works similarly that allows patterns of different lengths to be 
    /// mapped bidirectionally to other patterns. This allows it to act as 
    /// almost a two-way hash map. During training the BAM is fed pattern pairs. 
    /// The two halves of each pattern do not have to be the to be of the 
    /// same length. However all patterns must be of the same overall structure. 
    /// The BAM can be fed a distorted pattern on either side and will attempt 
    /// to map to the correct value.
    /// </summary>
    ///
    [Serializable]
    public class BAMNetwork : BasicML
    {
        /// <summary>
        /// Neurons in the F1 layer.
        /// </summary>
        ///
        private int f1Count;

        /// <summary>
        /// Neurons in the F2 layer.
        /// </summary>
        ///
        private int f2Count;

        /// <summary>
        /// The weights between the F1 and F2 layers.
        /// </summary>
        ///
        private Matrix weightsF1toF2;

        /// <summary>
        /// The weights between the F1 and F2 layers.
        /// </summary>
        ///
        private Matrix weightsF2toF1;

        /// <summary>
        /// Default constructor, used mainly for persistence.
        /// </summary>
        ///
        public BAMNetwork()
        {
        }

        /// <summary>
        /// Construct the BAM network.
        /// </summary>
        ///
        /// <param name="theF1Count">The F1 count.</param>
        /// <param name="theF2Count">The F2 count.</param>
        public BAMNetwork(int theF1Count, int theF2Count)
        {
            f1Count = theF1Count;
            f2Count = theF2Count;

            weightsF1toF2 = new Matrix(f1Count, f2Count);
            weightsF2toF1 = new Matrix(f2Count, f1Count);
        }

        /// <summary>
        /// Set the F1 neuron count.
        /// </summary>
        public int F1Count
        {
            get { return f1Count; }
            set { f1Count = value; }
        }


        /// <summary>
        /// Set the F2 neuron count.
        /// </summary>
        public int F2Count
        {
            get { return f2Count; }
            set { f2Count = value; }
        }

        /// <summary>
        /// Set the weights for F1 to F2.
        /// </summary>
        public Matrix WeightsF1toF2
        {
            get { return weightsF1toF2; }
            set { weightsF1toF2 = value; }
        }


        /// <summary>
        /// Set the weights for F2 to F1.
        /// </summary>
        public Matrix WeightsF2toF1
        {
            get { return weightsF2toF1; }
            set { weightsF2toF1 = value; }
        }

        /// <summary>
        /// Add a pattern to the neural network.
        /// </summary>
        ///
        /// <param name="inputPattern">The input pattern.</param>
        /// <param name="outputPattern">The output pattern(for this input).</param>
        public void AddPattern(IMLData inputPattern,
                               IMLData outputPattern)
        {
            int weight;

            for (int i = 0; i < f1Count; i++)
            {
                for (int j = 0; j < f2Count; j++)
                {
                    weight = (int) (inputPattern[i]*outputPattern[j]);
                    weightsF1toF2.Add(i, j, weight);
                    weightsF2toF1.Add(j, i, weight);
                }
            }
        }

        /// <summary>
        /// Clear any connection weights.
        /// </summary>
        ///
        public void Clear()
        {
            weightsF1toF2.Clear();
            weightsF2toF1.Clear();
        }

        /// <summary>
        /// Setup the network logic, read parameters from the network. NOT USED, call
        /// compute(NeuralInputData).
        /// </summary>
        ///
        /// <param name="input">NOT USED</param>
        /// <returns>NOT USED</returns>
        public IMLData Compute(IMLData input)
        {
            throw new NeuralNetworkError(
                "Compute on BasicNetwork cannot be used, rather call"
                + " the compute(NeuralData) method on the BAMLogic.");
        }

        /// <summary>
        /// Compute the network for the specified input.
        /// </summary>
        ///
        /// <param name="input">The input to the network.</param>
        /// <returns>The output from the network.</returns>
        public NeuralDataMapping Compute(NeuralDataMapping input)
        {
            bool stable1 = true, stable2 = true;

            do
            {
                stable1 = PropagateLayer(weightsF1toF2, input.From,
                                         input.To);
                stable2 = PropagateLayer(weightsF2toF1, input.To,
                                         input.From);
            } while (!stable1 && !stable2);
            return null;
        }


        /// <summary>
        /// Get the specified weight.
        /// </summary>
        ///
        /// <param name="matrix">The matrix to use.</param>
        /// <param name="input">The input, to obtain the size from.</param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>The value from the matrix.</returns>
        private double GetWeight(Matrix matrix, IMLData input,
                                 int x, int y)
        {
            if (matrix.Rows != input.Count)
            {
                return matrix[x, y];
            }
            else
            {
                return matrix[y, x];
            }
        }


        /// <summary>
        /// Propagate the layer.
        /// </summary>
        ///
        /// <param name="matrix">The matrix for this layer.</param>
        /// <param name="input">The input pattern.</param>
        /// <param name="output">The output pattern.</param>
        /// <returns>True if the network has become stable.</returns>
        private bool PropagateLayer(Matrix matrix, IMLData input,
                                    IMLData output)
        {
            int i, j;
            double sum; // **FIX** ?? int ??
            int xout = 0;
            bool stable;

            stable = true;

            for (i = 0; i < output.Count; i++)
            {
                sum = 0;
                for (j = 0; j < input.Count; j++)
                {
                    sum += GetWeight(matrix, input, i, j)*input[j];
                }
                if (sum != 0)
                {
                    if (sum < 0)
                    {
                        xout = -1;
                    }
                    else
                    {
                        xout = 1;
                    }
                    if (xout != (int) output[i])
                    {
                        stable = false;
                        output[i] = xout;
                    }
                }
            }
            return stable;
        }

        /// <summary>
        /// 
        /// </summary>
        ///
        public override void UpdateProperties()
        {
            // TODO Auto-generated method stub
        }
    }
}

﻿using System.Collections.Generic;
using Encog.ML.Data.Basic;
using Encog.Util;

namespace Encog.ML.Data.Buffer.CODEC
{
    /// <summary>
    /// A CODEC that works with the NeuralDataSet class.
    /// </summary>
    public class NeuralDataSetCODEC : IDataSetCODEC
    {
        /// <summary>
        /// The dataset.
        /// </summary>
        private readonly MLDataSet dataset;

        /// <summary>
        /// The iterator used to read through the dataset.
        /// </summary>
        private IEnumerator<MLDataPair> enumerator;

        /// <summary>
        /// The number of ideal elements.
        /// </summary>
        private int idealSize;

        /// <summary>
        /// The number of input elements.
        /// </summary>
        private int inputSize;

        /// <summary>
        /// Construct a CODEC. 
        /// </summary>
        /// <param name="dataset">The dataset to use.</param>
        public NeuralDataSetCODEC(MLDataSet dataset)
        {
            this.dataset = dataset;
            inputSize = dataset.InputSize;
            idealSize = dataset.IdealSize;
        }

        #region IDataSetCODEC Members

        /// <inheritdoc/>
        public int InputSize
        {
            get { return inputSize; }
        }

        /// <inheritdoc/>
        public int IdealSize
        {
            get { return idealSize; }
        }

        /// <inheritdoc/>
        public bool Read(double[] input, double[] ideal)
        {
            if (!enumerator.MoveNext())
            {
                return false;
            }
            else
            {
                MLDataPair pair = enumerator.Current;
                EngineArray.ArrayCopy(pair.Input.Data, input);
                EngineArray.ArrayCopy(pair.Ideal.Data, ideal);
                return true;
            }
        }

        /// <inheritdoc/>
        public void Write(double[] input, double[] ideal)
        {
            MLDataPair pair = BasicMLDataPair.CreatePair(inputSize,
                                                         idealSize);
            EngineArray.ArrayCopy(input, pair.Input.Data);
            EngineArray.ArrayCopy(ideal, pair.Ideal.Data);
        }

        /// <inheritdoc/>
        public void PrepareWrite(int recordCount,
                                 int inputSize, int idealSize)
        {
            this.inputSize = inputSize;
            this.idealSize = idealSize;
        }

        /// <inheritdoc/>
        public void PrepareRead()
        {
            enumerator = dataset.GetEnumerator();
        }

        /// <inheritdoc/>
        public void Close()
        {
        }

        #endregion
    }
}
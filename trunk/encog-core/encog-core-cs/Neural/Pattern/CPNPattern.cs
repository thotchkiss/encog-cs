using System;
using Encog.Engine.Network.Activation;
using Encog.ML;
using Encog.Neural.CPN;

namespace Encog.Neural.Pattern
{
    /// <summary>
    /// Pattern that creates a CPN neural network.
    /// </summary>
    ///
    public class CPNPattern : NeuralNetworkPattern
    {
        /// <summary>
        /// The tag for the INSTAR layer.
        /// </summary>
        ///
        public const String TAG_INSTAR = "INSTAR";

        /// <summary>
        /// The tag for the OUTSTAR layer.
        /// </summary>
        ///
        public const String TAG_OUTSTAR = "OUTSTAR";

        /// <summary>
        /// The number of neurons in the hidden layer.
        /// </summary>
        ///
        private int inputCount;

        /// <summary>
        /// The number of neurons in the instar layer.
        /// </summary>
        ///
        private int instarCount;

        /// <summary>
        /// The number of neurons in the outstar layer.
        /// </summary>
        ///
        private int outstarCount;

        /// <summary>
        /// Set the number of neurons in the instar layer. This level is essentially
        /// a hidden layer.
        /// </summary>
        public int InstarCount
        {
            set { instarCount = value; }
        }

        /// <summary>
        /// Set the number of neurons in the outstar level, this level is mapped to
        /// the "output" level.
        /// </summary>
        public int OutstarCount
        {
            set { outstarCount = value; }
        }

        #region NeuralNetworkPattern Members

        /// <summary>
        /// Not used, will throw an error. CPN networks already have a predefined
        /// hidden layer called the instar layer.
        /// </summary>
        ///
        /// <param name="count">NOT USED</param>
        public void AddHiddenLayer(int count)
        {
            throw new PatternError(
                "A CPN already has a predefined hidden layer.  No additional"
                + "specification is needed.");
        }

        /// <summary>
        /// Clear any parameters that were set.
        /// </summary>
        ///
        public void Clear()
        {
            inputCount = 0;
            instarCount = 0;
            outstarCount = 0;
        }

        /// <summary>
        /// Generate the network.
        /// </summary>
        ///
        /// <returns>The generated network.</returns>
        public MLMethod Generate()
        {
            return new CPNNetwork(inputCount, instarCount, outstarCount, 1);
        }

        /// <summary>
        /// This method will throw an error. The CPN network uses predefined
        /// activation functions.
        /// </summary>
        public IActivationFunction ActivationFunction
        {
            set
            {
                throw new PatternError(
                    "A CPN network will use the BiPolar & competitive activation "
                    + "functions, no activation function needs to be specified.");
            }
        }


        /// <summary>
        /// Set the number of input neurons.
        /// </summary>
        public int InputNeurons
        {
            set { inputCount = value; }
        }


        /// <summary>
        /// Set the number of output neurons. Calling this method maps to setting the
        /// number of neurons in the outstar layer.
        /// </summary>
        public int OutputNeurons
        {
            set { outstarCount = value; }
        }

        #endregion
    }
}
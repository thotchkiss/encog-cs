﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using Encog.Neural.Activation;
using Encog.Neural.Networks.Layers;
using Encog.Neural.Networks.Synapse;

namespace Encog.Neural.Networks.Pattern
{
    /// <summary>
    /// This class is used to generate an Elman style recurrent neural network. This
    /// network type consists of three regular layers, an input output and hidden
    /// layer. There is also a context layer which accepts output from the hidden
    /// layer and outputs back to the hidden layer. This makes it a recurrent neural
    /// network.
    /// 
    /// The Elman neural network is useful for temporal input data. The specified
    /// activation function will be used on all layers.  The Elman neural 
    /// network is similar to the Jordan neural network.
    /// </summary>
    public class ElmanPattern : INeuralNetworkPattern
    {

        /// <summary>
        /// The number of input neurons.
        /// </summary>
        private int inputNeurons;

        /// <summary>
        /// The number of output neurons.
        /// </summary>
        private int outputNeurons;

        /// <summary>
        /// The number of hidden neurons.
        /// </summary>
        private int hiddenNeurons;

        /// <summary>
        /// The activation function.
        /// </summary>
        private IActivationFunction activation;

        /// <summary>
        /// The logging object.
        /// </summary>
        private readonly ILog logger = LogManager.GetLogger(typeof(ElmanPattern));

        /**
         * Create an object to generate Elman neural networks.
         */
        public ElmanPattern()
        {
            this.inputNeurons = -1;
            this.outputNeurons = -1;
            this.hiddenNeurons = -1;
        }

        /// <summary>
        /// Add a hidden layer with the specified number of neurons.
        /// </summary>
        /// <param name="count">The number of neurons in this hidden layer.</param>
        public void AddHiddenLayer(int count)
        {
            if (this.hiddenNeurons != -1)
            {
                String str =
                   "An Elman neural network should have only one hidden layer.";
                if (this.logger.IsErrorEnabled)
                {
                    this.logger.Error(str);
                }
                throw new PatternError(str);
            }

            this.hiddenNeurons = count;

        }

        /// <summary>
        /// Generate the Elman neural network.
        /// </summary>
        /// <returns>An Elman neural network.</returns>
        public BasicNetwork Generate()
        {
            int y = PatternConst.START_Y;
            ILayer context = new ContextLayer(this.hiddenNeurons);
            BasicNetwork network = new BasicNetwork();

            ILayer input = new BasicLayer(this.activation, true,
                   this.inputNeurons);
            network.AddLayer(input);
            input.X = PatternConst.START_X;
            input.Y = y;
            y += PatternConst.INC_Y;

            ILayer hidden = new BasicLayer(this.activation, true,
                   this.hiddenNeurons);
            network.AddLayer(hidden);
            hidden.X = PatternConst.START_X;
            hidden.Y = y;
            context.X = PatternConst.INDENT_X;
            context.Y = y;
            y += PatternConst.INC_Y;
            hidden.AddNext(context, SynapseType.OneToOne);
            context.AddNext(hidden);
            ILayer output = new BasicLayer(this.activation, true,
                    this.outputNeurons);
            network.AddLayer(output);
            output.X = PatternConst.START_X;
            output.Y = y;
            network.Structure.FinalizeStructure();
            network.Reset();
            return network;
        }

        /// <summary>
        /// Set the activation function to use on each of the layers.
        /// </summary>
        public IActivationFunction ActivationFunction
        {
            set
            {
                this.activation = value;
            }
            get
            {
                return this.activation;
            }
        }


        /// <summary>
        /// Set the number of output neurons.
        /// </summary>
        public int OutputNeurons
        {
            get
            {
                return this.outputNeurons;
            }
            set
            {
                this.outputNeurons = value;
            }

        }

        /// <summary>
        /// The number of input neurons.
        /// </summary>
        public int InputNeurons
        {
            get
            {
                return this.inputNeurons;
            }
            set
            {
                this.inputNeurons = value;
            }

        }

        /// <summary>
        /// Clear out any hidden neurons.
        /// </summary>
        public void Clear()
        {
            this.hiddenNeurons = 0;
        }

    }

}

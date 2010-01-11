﻿// Encog Artificial Intelligence Framework v2.x
// DotNet Version
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-cs/
// 
// Copyright 2009, Heaton Research Inc., and individual contributors.
// See the copyright.txt in the distribution for a full listing of 
// individual contributors.
//
// This is free software; you can redistribute it and/or modify it
// under the terms of the GNU Lesser General Public License as
// published by the Free Software Foundation; either version 2.1 of
// the License, or (at your option) any later version.
//
// This software is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this software; if not, write to the Free
// Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA
// 02110-1301 USA, or see the FSF site: http://www.fsf.org.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.Networks.Layers;
using Encog.Neural.Networks.Synapse;
#if logging
using log4net;
using Encog.Util;
#endif

namespace Encog.Neural.Networks
{
    /// <summary>
    /// Holds "cached" information about the structure of the neural network. This is
    /// a very good performance boost since the neural network does not need to
    /// traverse itself each time a complete collection of layers or synapses is needed.
    /// </summary>
#if !SILVERLIGHT
    [Serializable]
#endif
    public class NeuralStructure
    {
        /// <summary>
        /// The layers in this neural network.
        /// </summary>
        private IList<ILayer> layers = new List<ILayer>();

        /// <summary>
        /// The synapses in this neural network.
        /// </summary>
        private IList<ISynapse> synapses = new List<ISynapse>();

        /// <summary>
        /// The neural network this class belongs to.
        /// </summary>
        private BasicNetwork network;

#if logging
        /// <summary>
        /// The logging object.
        /// </summary>
        [NonSerialized]
        private readonly ILog logger = LogManager.GetLogger(typeof(NeuralStructure));
#endif

        /// <summary>
        /// Construct a structure object for the specified network.
        /// </summary>
        /// <param name="network">The network to construct a structure for.</param>
        public NeuralStructure(BasicNetwork network)
        {
            this.network = network;
        }

        /// <summary>
        /// Build the layer structure.
        /// </summary>
        private void FinalizeLayers()
        {
            IList<ILayer> result = new List<ILayer>();

            this.layers.Clear();

            foreach (ILayer layer in this.network.LayerTags.Values)
            {
                GetLayers(this.layers, layer);
            }

        }

        /// <summary>
        /// Build the synapse and layer structure.  This method should be called 
        /// after you are done adding layers to a network.
        /// </summary>
        public void FinalizeStructure()
        {
            FinalizeLayers();
            FinalizeSynapses();
            this.network.Logic.Init(this.network);
        }

        /// <summary>
        /// Build the synapse structure.
        /// </summary>
        private void FinalizeSynapses()
        {
            this.synapses.Clear();
            foreach (ILayer layer in this.Layers)
            {
                foreach (ISynapse synapse in layer.Next)
                {
                    if (!this.synapses.Contains(synapse))
                        this.synapses.Add(synapse);
                }
            }

        }

        /// <summary>
        /// The layers in this neural network.
        /// </summary>
        public IList<ILayer> Layers
        {
            get
            {
                return this.layers;
            }
        }

        /// <summary>
        /// Called to help build the layer structure.
        /// </summary>
        /// <param name="result">The layer list. </param>
        /// <param name="layer">The current layer being processed.</param>
        private void GetLayers(IList<ILayer> result,
                 ILayer layer)
        {

            if (!result.Contains(layer))
            {
                result.Add(layer);
            }

            foreach (ISynapse synapse in layer.Next)
            {
                ILayer nextLayer = synapse.ToLayer;

                if (!result.Contains(nextLayer))
                {
                    GetLayers(result, nextLayer);
                }
            }
        }

        /// <summary>
        /// The network this structure belongs to.
        /// </summary>
        public BasicNetwork Network
        {
            get
            {
                return this.network;
            }
        }

        /// <summary>
        /// Get the previous layers from the specified layer.
        /// </summary>
        /// <param name="targetLayer">The target layer.</param>
        /// <returns>The previous layers.</returns>
        public ICollection<ILayer> GetPreviousLayers(ILayer targetLayer)
        {
            ICollection<ILayer> result = new List<ILayer>();
            foreach (ILayer layer in this.Layers)
            {
                foreach (ISynapse synapse in layer.Next)
                {
                    if (synapse.ToLayer == targetLayer)
                    {
                        if (!result.Contains(synapse.FromLayer))
                        {
                            result.Add(synapse.FromLayer);
                        }
                    }
                }
            }
            return result;
        }


        /// <summary>
        /// Get the previous synapses.
        /// </summary>
        /// <param name="targetLayer">The layer to get the previous layers from.</param>
        /// <returns>A collection of synapses.</returns>
        public ICollection<ISynapse> GetPreviousSynapses(ILayer targetLayer)
        {

            ICollection<ISynapse> result = new List<ISynapse>();

            foreach (ISynapse synapse in this.synapses)
            {
                if (synapse.ToLayer == targetLayer)
                {
                    if( !result.Contains(synapse) )
                        result.Add(synapse);
                }
            }

            return result;

        }

        /// <summary>
        /// All synapses in the neural network.
        /// </summary>
        public IList<ISynapse> Synapses
        {
            get
            {
                return this.synapses;
            }
        }

        /// <summary>
        /// Get all of the names for a layer.
        /// </summary>
        /// <param name="layer">The layer to name.</param>
        /// <returns>A collection of the layer's names.</returns>
        public ICollection<String> NameLayer(ILayer layer)
        {
            ICollection<String> result = new List<String>();

            foreach (String key in this.network.LayerTags.Keys)
            {
                ILayer value = this.network.LayerTags[key];
                if (value == layer)
                {
                    result.Add(key);
                }
            }

            return result;
        }

        /// <summary>
        /// Find the specified synapse.
        /// </summary>
        /// <param name="fromLayer">From layer.</param>
        /// <param name="toLayer">To layer.</param>
        /// <param name="required">Throw an error if this synapse is not there.</param>
        /// <returns>The synapse, if found.</returns>
        public ISynapse FindSynapse(ILayer fromLayer, ILayer toLayer, bool required)
        {
            ISynapse result = null;
            foreach (ISynapse synapse in this.Synapses)
            {
                if ((synapse.FromLayer == fromLayer)
                    && (synapse.ToLayer == toLayer))
                {
                    result = synapse;
                    break;
                }
            }

            if (required && result == null)
            {
                String str = "This operation requires a network with a synapse between the "
                    + NameLayer(fromLayer) + " layer to the "
                    + NameLayer(toLayer) + " layer.";
#if logging
                if (logger.IsErrorEnabled)
                {
                    logger.Error(str);
                }
#endif
                throw new NeuralNetworkError(str);
            }

            return result;
        }

        /// <summary>
        /// Determine if the network contains a layer of the specified type.
        /// </summary>
        /// <param name="type">The layer type we are looking for.</param>
        /// <returns>True if this layer type is present.</returns>
        public bool ContainsLayerType(Type type)
        {
            foreach (ILayer layer in this.layers)
            {
                if (layer.GetType().IsInstanceOfType(type))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
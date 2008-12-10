﻿// Encog Neural Network and Bot Library v1.x (DotNet)
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-cs/
// 
// Copyright 2008, Heaton Research Inc., and individual contributors.
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
using Encog.Matrix;

namespace Encog.Neural.NeuralData.Bipolar
{
    public class BiPolarNeuralData : INeuralData
    {
        /// <summary>
        /// The data held by this object.
        /// </summary>
        private bool[] data;

        /// <summary>
        /// Construct this object with the specified data. 
        /// </summary>
        /// <param name="d">The data to create this object with.</param>
        public BiPolarNeuralData(bool[] d)
        {
            this.data = new bool[d.Length];
            for (int i = 0; i < d.Length; i++)
            {
                this.data[i] = d[i];
            }
        }

        /// <summary>
        /// Construct a data object with the specified size.
        /// </summary>
        /// <param name="size">The size of this data object.</param>
        public BiPolarNeuralData(int size)
        {
            this.data = new bool[size];
        }

        public double this[int x]
        {
            get
            {
                return BiPolarUtil.Bipolar2double(this.data[x]);
            }
            set
            {
                this.data[x] = BiPolarUtil.Double2bipolar(value);
            }
        }

        public double[] Data
        {
            get
            {
                return BiPolarUtil.Bipolar2double(this.data);
            }
            set
            {
                this.data = BiPolarUtil.Double2bipolar(value);
            }
        }

        public int Count
        {
            get
            {
                return this.data.Length;
            }
        }

        /// <summary>
        /// Get the specified data item as a boolean.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public bool GetBoolean(int i)
        {
            return this.data[i];
        }
    }
}

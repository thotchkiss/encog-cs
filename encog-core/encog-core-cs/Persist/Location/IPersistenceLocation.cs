﻿// Encog Artificial Intelligence Framework v2.x
// DotNet Version
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-cs/
// 
// Copyright 2009-2010, Heaton Research Inc., and individual contributors.
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
using System.IO;

namespace Encog.Persist.Location
{
    /// <summary>
    /// A persistence location specifies how the persistence collection is stored.
    /// </summary>
    public interface IPersistenceLocation
    {

        /// <summary>
        /// Create a new stream to read data from.
        /// </summary>
        /// <returns>A new stream to read data from.</returns>
        Stream CreateStream(FileMode mode);

        /// <summary>
        /// Determine if this location exists.
        /// </summary>
        /// <returns>True if this location exists.</returns>
        bool Exists();

        /// <summary>
        /// Delete the location, if possible.
        /// </summary>
        void Delete();

        /// <summary>
        /// Attempt to rename this location. Mainly for file locations.
        /// </summary>
        /// <param name="toLocation">The new name.</param>
        void RenameTo(IPersistenceLocation toLocation);
    }

}
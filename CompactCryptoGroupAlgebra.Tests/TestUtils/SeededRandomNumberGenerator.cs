// CompactCryptoGroupAlgebra - C# implementation of abelian group algebra for experimental cryptography

// SPDX-FileCopyrightText: 2020-2021 Lukas Prediger <lumip@lumip.de>
// SPDX-License-Identifier: GPL-3.0-or-later
// SPDX-FileType: SOURCE

// CompactCryptoGroupAlgebra is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// CompactCryptoGroupAlgebra is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Security.Cryptography;

namespace CompactCryptoGroupAlgebra.TestUtils
{
    public class SeededRandomNumberGenerator : RandomNumberGenerator
    {
        private readonly Random _random;

        public SeededRandomNumberGenerator(int seed)
        {
            _random = new Random(seed);
        }

        public SeededRandomNumberGenerator() : this(0) { }

        public override void GetBytes(byte[] data)
        {
            _random.NextBytes(data);
        }
    }
}

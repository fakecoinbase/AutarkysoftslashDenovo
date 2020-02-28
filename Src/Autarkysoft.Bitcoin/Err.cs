﻿// Autarkysoft.Bitcoin
// Copyright (c) 2020 Autarkysoft
// Distributed under the MIT software license, see the accompanying
// file LICENCE or http://www.opensource.org/licenses/mit-license.php.

namespace Autarkysoft.Bitcoin
{
    internal struct Err
    {
        internal const string EndOfStream = "Reached end of stream.";
        internal const string OpNotEnoughItems = "Not enough items left in the stack.";
        internal const string OpStackItemOverflow = "Stack item count limit exceeded.";
        internal const string BadRNG = "The provided RNG is broken.";
    }
}

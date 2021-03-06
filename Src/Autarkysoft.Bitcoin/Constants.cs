﻿// Autarkysoft.Bitcoin
// Copyright (c) 2020 Autarkysoft
// Distributed under the MIT software license, see the accompanying
// file LICENCE or http://www.opensource.org/licenses/mit-license.php.

namespace Autarkysoft.Bitcoin
{
    /// <summary>
    /// Constant values used globally in different places.
    /// </summary>
    public struct Constants
    {
        /// <summary>
        /// Smallest bitcoin unit (10^-8)
        /// </summary>
        public const decimal Satoshi = 0.0000_0001m;
        /// <summary>
        /// The bitcoin symbol
        /// </summary>
        public const string Symbol = "฿";
        /// <summary>
        /// Total bitcoin supply in satoshi, 21 million with 8 decimal places.
        /// </summary>
        public const ulong TotalSupply = 21_000_000_0000_0000;
        /// <summary>
        /// Number of times to retry setting random bytes using <see cref="Cryptography.IRandomNumberGenerator"/>.
        /// </summary>
        /// <remarks>
        /// 2 should be enough, we set it higher to be safe!
        /// </remarks>
        public const int RngRetryCount = 5;
        /// <summary>
        /// Byte length of a compressed public key
        /// </summary>
        public const int CompressedPubkeyLen = 33;
        /// <summary>
        /// Byte length of an uncompressed public key
        /// </summary>
        public const int UncompressedPubkeyLen = 65;
        /// <summary>
        /// Number of bytes in a checksum (first 4 bytes of double SHA256 of data)
        /// </summary>
        public const int CheckSumSize = 4;
        /// <summary>
        /// Byte length of <see cref="Cryptography.Hashing.Ripemd160Sha256"/> hash result
        /// </summary>
        public const int Hash160Length = 20;
        /// <summary>
        /// Byte length of <see cref="Cryptography.Hashing.Sha256"/> hash result
        /// </summary>
        public const int Sha256Length = 32;
        /// <summary>
        /// Byte length of the block headers
        /// </summary>
        public const int BlockHeaderSize = 80;
        /// <summary>
        /// Maximum allowed length of the scripts in bytes
        /// </summary>
        public const int MaxScriptLength = 10_000;
        /// <summary>
        /// Maximum allowed number of non-push OPs in a script
        /// </summary>
        public const int MaxScriptOpCount = 201;
        /// <summary>
        /// Maximum allowed length of items to be pushed to the stack 
        /// (the redeem scripts used in P2SH are also limited by this length)
        /// </summary>
        public const int MaxScriptItemLength = 520;
        /// <summary>
        /// Minimum allowed length of the coinbase signature script
        /// </summary>
        public const int MinCoinbaseScriptLength = 2;
        /// <summary>
        /// Maximum allowed length of the coinbase signature script
        /// </summary>
        public const int MaxCoinbaseScriptLength = 100;
        /// <summary>
        /// Maximum allowed number of signature check operations per block
        /// </summary>
        public const int MaxSigOpCount = 80_000;
        /// <summary>
        /// The factor by which values (eg. size, SigOpCount, ...) are multiplied 
        /// </summary>
        public const int WitnessScaleFactor = 4;
        /// <summary>
        /// Minimum byte length of the witness commitment in the coinbase output as defined by BIP-141.
        /// </summary>
        public const int MinWitnessCommitmentLen = 38;
        /// <summary>
        /// The constant string that is attached to the beginning of a message before it is signed
        /// </summary>
        public const string MsgSignConst = "Bitcoin Signed Message:\n";
        /// <summary>
        /// Default main network port
        /// </summary>
        public const ushort MainNetPort = 8333;
        /// <summary>
        /// Default test network port
        /// </summary>
        public const ushort TestNetPort = 18333;
        /// <summary>
        /// Default regtest network port
        /// </summary>
        public const ushort RegTestPort = 18444;
        /// <summary>
        /// 4 byte "magic" value used in P2P message headers for main-net
        /// </summary>
        public const string MainNetMagic = "f9beb4d9";
        /// <summary>
        /// 4 byte "magic" value used in P2P message headers for test-net
        /// </summary>
        public const string TestNetMagic = "0b110907";
        /// <summary>
        /// 4 byte "magic" value used in P2P message headers for reg-test
        /// </summary>
        public const string RegTestMagic = "fabfb5da";
        /// <summary>
        /// The latest P2P protocol version supported by this library
        /// </summary>
        public const int P2PProtocolVersion = 70015;
        /// <summary>
        /// Length of P2P message headers (4 magic + 12 command + 4 payloadSize + 4 checksum)
        /// </summary>
        public const int MessageHeaderSize = 24;
        /// <summary>
        /// Maximum allowed P2P message payload size (4 MB)
        /// </summary>
        /// <remarks>
        /// https://github.com/bitcoin/bitcoin/blob/5879bfa9a541576100d939d329a2639b79d9e4f9/src/net.h#L55-L56
        /// </remarks>
        public const int MaxPayloadSize = 4 * 1000 * 1000;
    }
}

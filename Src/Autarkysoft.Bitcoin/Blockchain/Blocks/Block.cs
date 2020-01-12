﻿// Autarkysoft.Bitcoin
// Copyright (c) 2020 Autarkysoft
// Distributed under the MIT software license, see the accompanying
// file LICENCE or http://www.opensource.org/licenses/mit-license.php.

using Autarkysoft.Bitcoin.Blockchain.Transactions;
using Autarkysoft.Bitcoin.Cryptography.Hashing;
using Autarkysoft.Bitcoin.Encoders;
using System;

namespace Autarkysoft.Bitcoin.Blockchain.Blocks
{
    // TODO: block validation reminder:
    //       a block with valid POW and valid merkle root can have its transactions modified (duplicate some txs)
    //       by a malicious node along the way to make it invalid. the node sometimes has to remember invalid block hashes
    //       that it received to avoid receiving the same thing again. if the reason for being invalid is only merkle root 
    //       and having those duplicate cases then the hash must not be stored or some workaround must be implemented.
    //       more info:
    // https://github.com/bitcoin/bitcoin/blob/1dbf3350c683f93d7fc9b861400724f6fd2b2f1d/src/consensus/merkle.cpp#L8-L42

    /// <summary>
    /// The main component of the blockchain that contains transactions and shapes up the chain by referencing
    /// the previous block and is secured using cryptography in form of the proof of work algorithm.
    /// Implements <see cref="IDeserializable"/>.
    /// </summary>
    public class Block : IDeserializable
    {
        /// <summary>
        /// Initializes a new empty instance of <see cref="Block"/>.
        /// </summary>
        public Block()
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="Block"/> using given parameters.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentOutOfRangeException"/>
        /// <param name="ver">Block version</param>
        /// <param name="prevHd">Hash of previous block header</param>
        /// <param name="merkle">Hash of merkle root</param>
        /// <param name="blockTime">Block time</param>
        /// <param name="nbits">Block target</param>
        /// <param name="nonce">Block nonce</param>
        /// <param name="txs">List of transactions</param>
        public Block(int ver, byte[] prevHd, byte[] merkle, uint blockTime, Target nbits, uint nonce, ITransaction[] txs)
        {
            Version = ver;
            PreviousBlockHeaderHash = prevHd;
            MerkleRootHash = merkle;
            BlockTime = blockTime;
            NBits = nbits;
            Nonce = nonce;
            TransactionList = txs;
        }



        private const int HeaderSize = 80;
        private readonly Sha256 hashFunc = new Sha256(true);

        private int _version;
        /// <summary>
        /// Block version. It must be a positive number.
        /// </summary>
        public int Version
        {
            get => _version;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(Version), "Version must be >= 0");

                _version = value;
            }
        }

        private byte[] _prvBlkHash = new byte[32];
        /// <summary>
        /// Hash of the previous block header
        /// </summary>
        public byte[] PreviousBlockHeaderHash
        {
            get => _prvBlkHash;
            set
            {
                if (value is null)
                    throw new ArgumentNullException(nameof(PreviousBlockHeaderHash), "Previous block Header hash can not be null.");
                if (value.Length != 32)
                {
                    throw new ArgumentOutOfRangeException(nameof(PreviousBlockHeaderHash),
                        "Previous block Header hash length is invalid.");
                }

                _prvBlkHash = value;
            }
        }

        private byte[] _merkle = new byte[32];
        /// <summary>
        /// Hash of the merkle root
        /// </summary>
        public byte[] MerkleRootHash
        {
            get => _merkle;
            set
            {
                if (value is null)
                    throw new ArgumentNullException(nameof(MerkleRootHash), "Merkle root hash can not be null.");
                if (value.Length != 32)
                    throw new ArgumentOutOfRangeException(nameof(MerkleRootHash), "Merkle root hash length is invalid.");

                _merkle = value;
            }
        }

        private uint _time;
        /// <summary>
        /// Block time
        /// </summary>
        public uint BlockTime
        {
            get => _time;
            set => _time = value;
        }

        private Target _nBits;
        /// <summary>
        /// Target of this block, used for defining difficulty
        /// </summary>
        public Target NBits
        {
            get => _nBits;
            set => _nBits = value;
        }

        private uint _nonce;
        /// <summary>
        /// Nonce
        /// </summary>
        public uint Nonce
        {
            get => _nonce;
            set => _nonce = value;
        }

        private ITransaction[] _txs = new ITransaction[0];
        /// <summary>
        /// List of transactions in this block
        /// </summary>
        public ITransaction[] TransactionList
        {
            get => _txs;
            set
            {
                if (value is null || value.Length == 0)
                    throw new ArgumentNullException(nameof(TransactionList), "Transaction list can not be null or empty.");

                _txs = value;
            }
        }



        /// <summary>
        /// Returns hash of this block using the defined <see cref="IHashFunction"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentOutOfRangeException"/>
        /// <returns>Block hash</returns>
        public byte[] GetBlockHash()
        {
            byte[] bytesToHash = SerializeHeader();
            return hashFunc.ComputeHash(bytesToHash);
        }

        /// <summary>
        /// Returns hash of this block as a base-16 encoded string.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentOutOfRangeException"/>
        /// <returns>Base-16 encoded block hash</returns>
        public string GetBlockID()
        {
            byte[] hashRes = GetBlockHash();
            Array.Reverse(hashRes);
            return Base16.Encode(hashRes);
        }


        /// <summary>
        /// Returns merkle root of this block using the list of transactions.
        /// </summary>
        /// <returns>Merkle root</returns>
        public byte[] GetMerkleRoot()
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Converts this block's header into its byte array representation and writes the result to the given stream.
        /// </summary>
        public void SerializeHeader(FastStream stream)
        {
            stream.Write(Version);
            stream.Write(PreviousBlockHeaderHash);
            stream.Write(MerkleRootHash);
            stream.Write(BlockTime);
            NBits.WriteToStream(stream);
            stream.Write(Nonce);
        }

        /// <summary>
        /// Converts this block's header into its byte array representation.
        /// </summary>
        /// <returns>An array of bytes</returns>
        public byte[] SerializeHeader()
        {
            FastStream stream = new FastStream(HeaderSize);
            SerializeHeader(stream);
            return stream.ToByteArray();
        }


        /// <inheritdoc/>
        public void Serialize(FastStream stream)
        {
            SerializeHeader(stream);

            CompactInt txCount = new CompactInt(TransactionList.Length);
            txCount.WriteToStream(stream);
            foreach (var tx in TransactionList)
            {
                tx.Serialize(stream);
            }
        }

        /// <summary>
        /// Converts this instance into its byte array representation.
        /// </summary>
        /// <returns>An array of bytes</returns>
        public byte[] Serialize()
        {
            FastStream stream = new FastStream();
            Serialize(stream);
            return stream.ToByteArray();
        }


        /// <summary>
        /// Deserializes the given byte array from the given stream. The return value indicates success.
        /// </summary>
        /// <param name="stream">Stream to use</param>
        /// <param name="error">Error message (null if sucessful, otherwise will contain information about the failure)</param>
        /// <returns>True if deserialization was successful, false if otherwise</returns>
        public bool TryDeserializeHeader(FastStreamReader stream, out string error)
        {
            if (stream is null)
            {
                error = "Stream can not be null.";
                return false;
            }

            if (!stream.TryReadInt32(out _version))
            {
                error = Err.EndOfStream;
                return false;
            }

            if (!stream.TryReadByteArray(32, out _prvBlkHash))
            {
                error = Err.EndOfStream;
                return false;
            }

            if (!stream.TryReadByteArray(32, out _merkle))
            {
                error = Err.EndOfStream;
                return false;
            }

            if (!stream.TryReadUInt32(out _time))
            {
                error = Err.EndOfStream;
                return false;
            }

            if (!Target.TryRead(stream, out _nBits, out error))
            {
                return false;
            }

            if (!stream.TryReadUInt32(out _nonce))
            {
                error = Err.EndOfStream;
                return false;
            }

            error = null;
            return true;
        }

        /// <inheritdoc/>
        public bool TryDeserialize(FastStreamReader stream, out string error)
        {
            if (stream is null)
            {
                error = "Stream can not be null.";
                return false;
            }

            // TODO: add block size check

            if (!TryDeserializeHeader(stream, out error))
            {
                return false;
            }

            if (!CompactInt.TryRead(stream, out CompactInt txCount, out error))
            {
                return false;
            }
            if (txCount > int.MaxValue) // TODO: set how many ~tx can be in a block instead of int.Max
            {
                error = "Number of transactions is too big.";
                return false;
            }

            TransactionList = new Transaction[(int)txCount];
            for (int i = 0; i < TransactionList.Length; i++)
            {
                Transaction temp = new Transaction();
                if (!temp.TryDeserialize(stream, out error))
                {
                    return false;
                }
                TransactionList[i] = temp;
            }

            error = null;
            return true;
        }
    }
}
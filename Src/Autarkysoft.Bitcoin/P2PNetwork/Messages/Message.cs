﻿// Autarkysoft.Bitcoin
// Copyright (c) 2020 Autarkysoft
// Distributed under the MIT software license, see the accompanying
// file LICENCE or http://www.opensource.org/licenses/mit-license.php.

using Autarkysoft.Bitcoin.Cryptography.Hashing;
using Autarkysoft.Bitcoin.P2PNetwork.Messages.MessagePayloads;
using System;
using System.Text;

namespace Autarkysoft.Bitcoin.P2PNetwork.Messages
{
    public class Message : IDeserializable
    {
        public Message(NetworkType netType)
        {
            networkMagic = netType switch
            {
                NetworkType.MainNet => new byte[] { 0xf9, 0xbe, 0xb4, 0xd9 },
                NetworkType.TestNet => new byte[] { 0x0b, 0x11, 0x09, 0x07 },
                NetworkType.RegTest => new byte[] { 0xfa, 0xbf, 0xb5, 0xda },
                _ => throw new ArgumentException("Invalid network type.")
            };
        }


        private readonly byte[] networkMagic;

        /// <summary>
        /// 4 magic + 12 command + 4 payloadSize + 4 checksum + 0 empty payload
        /// </summary>
        public const int MinSize = 24;
        private const uint MaxPayloadSize = 32 * 1024 * 1024; // 32 MiB
        private const int CheckSumSize = 4;
        private const int CommandNameSize = 12;

        private readonly Sha256 hash = new Sha256(true);

        private byte[] _magic;
        public byte[] Magic
        {
            get => _magic;
            set => _magic = value;
        }

        private uint payloadSize;
        private byte[] checkSum;

        private IMessagePayload _payload;
        public IMessagePayload Payload
        {
            get => _payload;
            set => _payload = value;
        }


        public void Serialize(FastStream stream)
        {
            byte[] commandName = Encoding.ASCII.GetBytes(Payload.PayloadType.ToString().ToLower());

            FastStream temp = new FastStream();
            Payload.Serialize(temp);
            byte[] plBa = temp.ToByteArray();

            byte[] checksum = CalculateChecksum(plBa);

            stream.Write(Magic);
            stream.Write(commandName, CommandNameSize);
            stream.Write(plBa.Length);
            stream.Write(checksum);
            stream.Write(plBa);
        }


        private byte[] CalculateChecksum(byte[] data)
        {
            return hash.ComputeHash(data).SubArray(0, CheckSumSize);
        }


        private bool TrySetPayload(PayloadType plt)
        {
            Payload = plt switch
            {
                PayloadType.Addr => new AddrPayload(),
                PayloadType.Alert => new AlertPayload(),
                PayloadType.Block => new BlockPayload(),
                PayloadType.BlockTxn => new BlockTxnPayload(),
                PayloadType.CmpctBlock => new CmpctBlockPayload(),
                PayloadType.FeeFilter => new FeeFilterPayload(),
                PayloadType.FilterAdd => new FilterAddPayload(),
                PayloadType.FilterClear => new FilterClearPayload(),
                PayloadType.FilterLoad => new FilterLoadPayload(),
                PayloadType.GetAddr => new GetAddrPayload(),
                PayloadType.GetBlocks => new GetBlocksPayload(),
                PayloadType.GetBlockTxn => new GetBlockTxnPayload(),
                PayloadType.GetData => new GetDataPayload(),
                PayloadType.GetHeaders => new GetHeadersPayload(),
                PayloadType.Headers => new HeadersPayload(),
                PayloadType.Inv => new InvPayload(),
                PayloadType.MemPool => new MemPoolPayload(),
                PayloadType.MerkleBlock => new MerkleBlockPayload(),
                PayloadType.NotFound => new NotFoundPayload(),
                PayloadType.Ping => new PingPayload(),
                PayloadType.Pong => new PongPayload(),
                PayloadType.Reject => new RejectPayload(),
                PayloadType.SendCmpct => new SendCmpctPayload(),
                PayloadType.SendHeaders => new SendHeadersPayload(),
                PayloadType.Tx => new TxPayload(),
                PayloadType.Verack => new VerackPayload(),
                PayloadType.Version => new VersionPayload(),
                _ => null,
            };

            return Payload != null;
        }


        public bool TryDeserializeHeader(FastStreamReader stream, out string error)
        {
            if (stream is null)
            {
                error = "Stream can not be null.";
                return false;
            }

            if (!stream.TryReadByteArray(4, out _magic))
            {
                error = Err.EndOfStream;
                return false;
            }

            if (!((Span<byte>)Magic).SequenceEqual(networkMagic))
            {
                error = "Invalid network magic.";
                return false;
            }

            if (!stream.TryReadByteArray(CommandNameSize, out byte[] cmd))
            {
                error = Err.EndOfStream;
                return false;
            }

            if (!Enum.TryParse(Encoding.ASCII.GetString(cmd.TrimEnd()), ignoreCase: true, out PayloadType plt))
            {
                error = "Invalid command name.";
                return false;
            }

            if (!TrySetPayload(plt))
            {
                error = "Undefined payload.";
                return false;
            }

            if (!stream.TryReadUInt32(out payloadSize))
            {
                error = Err.EndOfStream;
                return false;
            }

            if (payloadSize > MaxPayloadSize)
            {
                error = $"Payload size is bigger than allowed size ({MaxPayloadSize}).";
                return false;
            }

            if (!stream.TryReadByteArray(CheckSumSize, out checkSum))
            {
                error = Err.EndOfStream;
                return false;
            }

            error = null;
            return true;
        }

        public bool TryDeserialize(FastStreamReader stream, out string error)
        {
            if (!TryDeserializeHeader(stream, out error))
            {
                return false;
            }

            int startOfPayLoad = stream.GetCurrentIndex();
            int expectedEnd = startOfPayLoad + (int)payloadSize;

            if (!Payload.TryDeserialize(stream, out error))
            {
                return false;
            }

            byte[] actualChecksum = Payload.GetChecksum();

            if (!((Span<byte>)actualChecksum).SequenceEqual(checkSum))
            {
                error = "Invalid checksum.";
                return false;
            }

            if (stream.GetCurrentIndex() != expectedEnd)
            {
                error = "Invalid payload length in header.";
                return false;
            }

            error = null;
            return true;
        }
    }
}
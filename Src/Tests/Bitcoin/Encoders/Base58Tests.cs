﻿// Autarkysoft Tests
// Copyright (c) 2020 Autarkysoft
// Distributed under the MIT software license, see the accompanying
// file LICENCE or http://www.opensource.org/licenses/mit-license.php.

using Autarkysoft.Bitcoin.Encoders;
using System;
using System.Collections.Generic;
using Xunit;

namespace Tests.Bitcoin.Encoders
{
    public class Base58Tests
    {
        private readonly Base58 encoder = new Base58();
        // Encode with checksum of empty bytes (byte[0]) is the following:
        private const string Empty58 = "3QJmnh";
        private const string Empty16 = "5df6e0e2";

        [Theory]
        [InlineData(null, false)]
        [InlineData("", true)]
        [InlineData(" ", false)]
        [InlineData("O", false)]
        [InlineData("0", false)]
        [InlineData("I", false)]
        [InlineData("l", false)]
        [InlineData("abc%d", false)]
        public void HasValidCharsTest(string s, bool expected)
        {
            Assert.Equal(expected, encoder.HasValidChars(s));
        }

        [Theory]
        [InlineData(null, false)]
        // https://en.bitcoin.it/wiki/Address
        [InlineData("1BvBMSEYstWetqTFn5Au4m4GFg7xJaNVN2", true)]
        [InlineData("1BvBMSEYstWetqTFn5Au4m4GFg7xJaNVc2", false)] // Invalid checksum
        [InlineData("1BvBMSOYstWetqTFn5Au4m4GFg7xJaNVN2", false)] // Invalid char
        public void IsValidTest(string s, bool expected)
        {
            Assert.Equal(expected, encoder.IsValid(s));
        }

        // Test cases are from:
        // https://github.com/bitcoin/bitcoin/blob/e258ce792a4849927a6db51786732d71cbbb65fc/src/test/data/base58_encode_decode.json
        public static IEnumerable<object[]> GetDecodeCases()
        {
            yield return new object[] { "", new byte[0] };
            yield return new object[] { "2g", new byte[] { 0x61 } };
            yield return new object[] { "a3gV", Helper.HexToBytes("626262") };
            yield return new object[] { "aPEr", Helper.HexToBytes("636363") };
            yield return new object[]
            {
                "2cFupjhnEsSn59qHXstmK2ffpLv2",
                Helper.HexToBytes("73696d706c792061206c6f6e6720737472696e67")
            };
            yield return new object[]
            {
                "1NS17iag9jJgTHD1VXjvLCEnZuQ3rJDE9L",
                Helper.HexToBytes("00eb15231dfceb60925886b67d065299925915aeb172c06647")
            };
            yield return new object[] { "ABnLTmg", Helper.HexToBytes("516b6fcd0f") };
            yield return new object[] { "3SEo3LWLoPntC", Helper.HexToBytes("bf4f89001e670274dd") };
            yield return new object[] { "3EFU7m", Helper.HexToBytes("572e4794") };
            yield return new object[] { "EJDM8drfXA6uyA", Helper.HexToBytes("ecac89cad93923c02321") };
            yield return new object[] { "Rt5zm", Helper.HexToBytes("10c8511e") };
            yield return new object[] { "1111111111", Helper.HexToBytes("00000000000000000000") };
            yield return new object[]
            {
                "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz",
                Helper.HexToBytes("000111d38e5fc9071ffcd20b4a763cc9ae4f252bb4e48fd66a835e252ada93ff480d6dd43dc62a641155a5")
            };
            yield return new object[]
            {
                "1cWB5HCBdLjAuqGGReWE3R3CguuwSjw6RHn39s2yuDRTS5NsBgNiFpWgAnEx6VQi8csexkgYw3mdYrMHr8x9i7aEwP8kZ7vccXWqKDvGv3u1GxFKPuAkn8JCPPGDMf3vMMnbzm6Nh9zh1gcNsMvH3ZNLmP5fSG6DGbbi2tuwMWPthr4boWwCxf7ewSgNQeacyozhKDDQQ1qL5fQFUW52QKUZDZ5fw3KXNQJMcNTcaB723LchjeKun7MuGW5qyCBZYzA1KjofN1gYBV3NqyhQJ3Ns746GNuf9N2pQPmHz4xpnSrrfCvy6TVVz5d4PdrjeshsWQwpZsZGzvbdAdN8MKV5QsBDY",
                Helper.HexToBytes("000102030405060708090a0b0c0d0e0f101112131415161718191a1b1c1d1e1f202122232425262728292a2b2c2d2e2f303132333435363738393a3b3c3d3e3f404142434445464748494a4b4c4d4e4f505152535455565758595a5b5c5d5e5f606162636465666768696a6b6c6d6e6f707172737475767778797a7b7c7d7e7f808182838485868788898a8b8c8d8e8f909192939495969798999a9b9c9d9e9fa0a1a2a3a4a5a6a7a8a9aaabacadaeafb0b1b2b3b4b5b6b7b8b9babbbcbdbebfc0c1c2c3c4c5c6c7c8c9cacbcccdcecfd0d1d2d3d4d5d6d7d8d9dadbdcdddedfe0e1e2e3e4e5e6e7e8e9eaebecedeeeff0f1f2f3f4f5f6f7f8f9fafbfcfdfeff")
            };
        }


        public static IEnumerable<object[]> GetDecodeChecksumCases()
        {
            yield return new object[] { Empty58, new byte[0] };
            yield return new object[]
            {
                // source: https://en.bitcoin.it/wiki/Address
                "3J98t1WpEZ73CNmQviecrnyiWrnqRhWNLy",
                Helper.HexToBytes("05b472a266d0bd89c13706a4132ccfb16f7c3b9fcb")
            };
            yield return new object[]
            {
                // source: https://en.bitcoin.it/wiki/Technical_background_of_version_1_Bitcoin_addresses
                "1PMycacnJaSqwwJqjawXBErnLsZ7RkXUAs",
                Helper.HexToBytes("00f54a5851e9372b87810a8e60cdd2e7cfd80b6e31")
            };
            yield return new object[]
            {
                // source: https://en.bitcoin.it/wiki/Wallet_import_format
                "5HueCGU8rMjxEXxiPuD5BDku4MkFqeZyd4dZ1jvhTVqvbTLvyTJ",
                Helper.HexToBytes("800c28fca386c7a227600b2fe50b7cae11ec86d3bf1fbe471be89827e19d72aa1d")
            };
        }


        [Theory]
        [MemberData(nameof(GetDecodeCases))]
        public void DecodeTest(string s, byte[] expected)
        {
            byte[] actual = encoder.Decode(s);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Decode_EmptyBytesTest()
        {
            byte[] actual = encoder.Decode(Empty58);
            byte[] expected = Helper.HexToBytes(Empty16);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Decode_ExceptionTest()
        {
            Exception ex = Assert.Throws<FormatException>(() => encoder.Decode("I"));
            Assert.Contains("Input is not a valid Base-58 encoded string", ex.Message);
        }



        [Theory]
        [MemberData(nameof(GetDecodeChecksumCases))]
        public void DecodeWithCheckSumTest(string s, byte[] expected)
        {
            byte[] actual = encoder.DecodeWithCheckSum(s);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(null, "Input is not a valid base-58 encoded string.")]
        [InlineData("I", "Input is not a valid base-58 encoded string.")] // invalid char
        [InlineData("a", "Input is not a valid base-58 encoded string.")] // 1 byte data
        [InlineData("xyz1", "Input is not a valid base-58 encoded string.")] // 3 byte data
        [InlineData("1PMycacnJaSqwwJqjawXBErnLsZ7RkXUAa", "Invalid checksum.")]
        public void DecodeWithCheckSum_ExceptionTest(string s, string expErrMsg)
        {
            Exception ex = Assert.Throws<FormatException>(() => encoder.DecodeWithCheckSum(s));
            Assert.Contains(expErrMsg, ex.Message);
        }


        [Theory]
        [MemberData(nameof(GetDecodeCases))]
        public void EncodeTest(string expected, byte[] data)
        {
            string actual = encoder.Encode(data);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Encode_ExceptionTest()
        {
            Assert.Throws<ArgumentNullException>(() => encoder.Encode(null));
        }


        [Theory]
        [MemberData(nameof(GetDecodeChecksumCases))]
        public void EncodeWithCheckSumTest(string expected, byte[] data)
        {
            string actual = encoder.EncodeWithCheckSum(data);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void EncodeWithCheckSum_ExceptionTest()
        {
            Assert.Throws<ArgumentNullException>(() => encoder.EncodeWithCheckSum(null));
        }

    }
}

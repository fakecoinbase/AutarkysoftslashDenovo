﻿// Autarkysoft.Bitcoin
// Copyright (c) 2020 Autarkysoft
// Distributed under the MIT software license, see the accompanying
// file LICENCE or http://www.opensource.org/licenses/mit-license.php.

using Autarkysoft.Bitcoin.Cryptography.Asymmetric.EllipticCurve;
using Autarkysoft.Bitcoin.Cryptography.Asymmetric.KeyPairs;

namespace Autarkysoft.Bitcoin.Blockchain.Scripts.Operations
{
    // TODO: add a new method: Push(bool)

    /// <summary>
    /// Defines a last-in-first-out (LIFO) collection (similar to <see cref="System.Collections.Stack"/>)
    /// to be used with <see cref="IOperation"/>s as their data provider.
    /// <para/>All indexes are zero based meaning item at the end (real index = length-1) is index 0, 
    /// the item before last is 1,... the first item (real index=0) is length-1.
    /// </summary>
    public interface IOpData
    {
        /// <summary>
        /// Verifies correctness of the given signature with the given public key using
        /// the transaction and scripts set in constructor.
        /// </summary>
        /// <param name="sig">Signature</param>
        /// <param name="pubKey">Public key</param>
        /// <returns>True if verification succeeds, otherwise false.</returns>
        bool Verify(Signature sig, PublicKey pubKey);

        /// <summary>
        /// Verifies multiple signatures versus multiple public keys (for <see cref="OP.CheckMultiSig"/> operations).
        /// Assumes there are less signatures than public keys.
        /// </summary>
        /// <param name="sigs">Array of signatures</param>
        /// <param name="pubKeys">Array of public keys</param>
        /// <returns>True if all verifications succeed, otherwise false.</returns>
        bool Verify(Signature[] sigs, PublicKey[] pubKeys);

        /// <summary>
        /// Checks to see if the extra (last) item that a <see cref="OP.CheckMultiSig"/> operation pops is valid
        /// according to consensus rules.
        /// </summary>
        /// <param name="garbage">An arbitrary byte array</param>
        /// <returns>True if the data was valid, otherwise false.</returns>
        bool CheckMultiSigGarbage(byte[] garbage);

        /// <summary>
        /// Returns number of available items in the stack.
        /// </summary>
        int ItemCount { get; }

        /// <summary>
        /// Returns the item at the top of the stack without removing it.
        /// </summary>
        /// <returns>The byte array at the top of the stack</returns>
        byte[] Peek();

        /// <summary>
        /// Returns multiple items from the top of the stack without removing them.
        /// </summary>
        /// <param name="count">Number of items to return</param>
        /// <returns>An array of byte arrays from the top of the stack</returns>
        byte[][] Peek(int count);

        /// <summary>
        /// Returns the item at a specific index starting from the top of the stack without removing it.
        /// <para/>NOTE: Index starts from zero meaning the item at the end (length-1) is index 0, the item before end is 1 and so on.
        /// </summary>
        /// <param name="index">Index of item from end to return (starting from 0)</param>
        /// <returns>The byte array at the specified intex</returns>
        byte[] PeekAtIndex(int index);

        /// <summary>
        /// Removes and returns the item at the top of the stack.
        /// </summary>
        /// <returns>The removed byte array at the top of the stack</returns>
        byte[] Pop();

        /// <summary>
        /// Removes multiple items from the top of the stack and returns all of them without changing the order ([1234] -> [34]).
        /// </summary>
        /// <param name="count">Number of items to remove and return</param>
        /// <returns>An array of byte arrays removed from the top of the stack</returns>
        byte[][] Pop(int count);

        /// <summary>
        /// Removes and returns the item at the specified index (will shift the items in its place).
        /// <para/>NOTE: Index starts from zero meaning the item at the end (length-1) is index 0, the item before end is 1 and so on.
        /// </summary>
        /// <param name="index"></param>
        /// <returns>The byte array removed from the specified intex</returns>
        byte[] PopAtIndex(int index);

        /// <summary>
        /// Pushes (or inserts) an item at the top of the stack.
        /// </summary>
        /// <param name="data">Byte array to push onto the stack</param>
        void Push(byte[] data);

        /// <summary>
        /// Pushes (or inserts) multiple items at the top of the stack in the same order.
        /// </summary>
        /// <param name="data">Arrays of byte array to push</param>
        void Push(byte[][] data);

        /// <summary>
        /// Inserts an item at the specified index (from the top) of the stack.
        /// <para/>NOTE: Index starts from zero meaning the item at the end (length-1) is index 0, the item before end is 1 and so on.
        /// </summary>
        /// <param name="data">Byte array to insert in the stack</param>
        /// <param name="index">Index at which to insert the given <paramref name="data"/></param>
        void Insert(byte[] data, int index);

        /// <summary>
        /// Inserts multiple items at the specified index (from the top) of the stack.
        /// <para/>NOTE: Index starts from zero meaning the item at the end (length-1) is index 0, the item before end is 1 and so on.
        /// </summary>
        /// <param name="data">Array of Byte arrays to insert in the stack</param>
        /// <param name="index">Index at which to insert the given <paramref name="data"/></param>
        void Insert(byte[][] data, int index);


        /// <summary>
        /// Returns number of available items in the "alt-stack"
        /// </summary>
        int AltItemCount { get; }

        /// <summary>
        /// Removes and returns the item at the top of the "alt-stack".
        /// </summary>
        /// <returns>The removed byte array at the top of the "alt-stack"</returns>
        byte[] AltPop();

        /// <summary>
        /// Pushes (or inserts) an item at the top of the "alt-stack".
        /// </summary>
        /// <param name="data">Byte array to push onto the "alt-stack"</param>
        void AltPush(byte[] data);
    }
}

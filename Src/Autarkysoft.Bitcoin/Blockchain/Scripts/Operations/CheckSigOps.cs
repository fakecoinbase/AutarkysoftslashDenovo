﻿// Autarkysoft.Bitcoin
// Copyright (c) 2020 Autarkysoft
// Distributed under the MIT software license, see the accompanying
// file LICENCE or http://www.opensource.org/licenses/mit-license.php.

using Autarkysoft.Bitcoin.Cryptography.Asymmetric.EllipticCurve;
using Autarkysoft.Bitcoin.Cryptography.Asymmetric.KeyPairs;

namespace Autarkysoft.Bitcoin.Blockchain.Scripts.Operations
{
    /// <summary>
    /// Operation to check the transaction signature.
    /// </summary>
    public class CheckSigOp : BaseOperation
    {
        /// <inheritdoc cref="IOperation.OpValue"/>
        public override OP OpValue => OP.CheckSig;

        /// <summary>
        /// Removes top two stack items (signature and public key) and verifies the transaction signature.
        /// </summary>
        /// <param name="opData">Stack to use</param>
        /// <param name="error">Error message (null if sucessful, otherwise will contain information about the failure)</param>
        /// <returns>True if operation was successful, false if otherwise</returns>
        public override bool Run(IOpData opData, out string error)
        {
            if (opData.ItemCount < 2)
            {
                error = Err.OpNotEnoughItems;
                return false;
            }

            byte[] pubBa = opData.Pop();
            byte[] sigBa = opData.Pop();

            if (!Signature.TryRead(sigBa, out Signature sig, out error))
            {
                return false;
            }

            if (!PublicKey.TryRead(pubBa, out PublicKey pubK))
            {
                error = "Invalid public key format.";
                return false;
            }

            bool b = opData.Verify(sig, pubK);

            opData.Push(b ? new byte[] { 1 } : new byte[0]);

            error = null;
            return true;
        }
    }


    /// <summary>
    /// Operation to check the transaction signature.
    /// </summary>
    public class CheckSigVerifyOp : BaseOperation
    {
        /// <inheritdoc cref="IOperation.OpValue"/>
        public override OP OpValue => OP.CheckSigVerify;

        /// <summary>
        /// Same as <see cref="CheckSigOp"/> but runs <see cref="VerifyOp"/> afterwards.
        /// </summary>
        /// <inheritdoc/>
        public override bool Run(IOpData opData, out string error)
        {
            IOperation cs = new CheckSigOp();
            IOperation ver = new VerifyOp();

            if (!cs.Run(opData, out error))
            {
                return false;
            }

            return ver.Run(opData, out error);
        }
    }


    /// <summary>
    /// Operation to check multiple transaction signatures.
    /// </summary>
    public class CheckMultiSigOp : BaseOperation
    {
        /// <inheritdoc cref="IOperation.OpValue"/>
        public override OP OpValue => OP.CheckMultiSig;

        /// <summary>
        /// Evaluation starts by popping top stack item in the following pattern: 
        /// [garbage] [between 0 to m signatures] [OP_m between 0 to n] [n publickeys] [OP_n between 0 to ?]
        /// </summary>
        /// <param name="opData">Stack to use</param>
        /// <param name="error">Error message (null if sucessful, otherwise will contain information about the failure)</param>
        /// <returns>True if operation was successful, false if otherwise</returns>
        public override bool Run(IOpData opData, out string error)
        {
            // A multi-sig stack is (extra item, usually OP_0) + (m*sig) + (OP_m) + (n*pub) + (OP_n)
            // both m and n values are needed and the extra item is also mandatory. but since both m and n can be zero
            // the key[] and sig[] can be empty so the smallest stack item count should be 3 items [OP_0 (m=0) (n=0)]
            if (opData.ItemCount < 3)
            {
                error = Err.OpNotEnoughItems;
                return false;
            }

            byte[] nBa = opData.Pop();
            if (!TryConvertToLong(nBa, out long n, true, 1))
            {
                error = "Invalid number (n) format";
                return false;
            }
            if (n < 0 || n > 20)
            {
                error = "Invalid number of public keys in multi-sig.";
                return false;
            }

            // By knowing n we know the number of public keys and the "index" of m to be popped
            // eg. indes:item => n=3 => 3:m 2:pub1 1:pub2 0:pub3    n=2 => 2:m 1:pub1 0:pub2
            if (opData.ItemCount < n + 1)
            {
                error = Err.OpNotEnoughItems;
                return false;
            }

            byte[] mBa = opData.PopAtIndex((int)n);
            if (!TryConvertToLong(mBa, out long m, true, 1))
            {
                error = "Invalid number (m) format";
                return false;
            }
            if (m < 0 || m > n)
            {
                error = "Invalid number of signatures in multi-sig.";
                return false;
            }

            // Note that m and n are already popped (removed) from the stack so it looks like this:
            // (extra item, usually OP_0) + (m*sig) + (n*pub)
            if (opData.ItemCount < n + m + 1)
            {
                error = Err.OpNotEnoughItems;
                return false;
            }

            PublicKey[] pubs = new PublicKey[n];
            Signature[] sigs = new Signature[m];
            for (int i = 0; i < pubs.Length; i++)
            {
                // TODO: benchmark using PopMulti versus this
                byte[] temp = opData.Pop();
                if (!PublicKey.TryRead(temp, out pubs[i]))
                {
                    error = "Invalid public key.";
                    return false;
                }
            }
            for (int i = 0; i < sigs.Length; i++)
            {
                byte[] temp = opData.Pop();
                if (!Signature.TryRead(temp, out sigs[i], out string err))
                {
                    error = $"Invalid signature {err}";
                    return false;
                }
            }

            // Handle bitcoin-core bug (has to pop 1 extra item)
            byte[] garbage = opData.Pop();
            if (!opData.CheckMultiSigGarbage(garbage))
            {
                error = "The extra item must have been OP_0.";
                return false;
            }

            bool b = opData.Verify(sigs, pubs);
            if (b)
            {
                error = null;
                return true;
            }
            else
            {
                error = "Invalid signature.";
                return false;
            }
        }
    }


    /// <summary>
    /// Operation to check multiple transaction signatures.
    /// </summary>
    public class CheckMultiSigVerifyOp : BaseOperation
    {
        /// <inheritdoc cref="IOperation.OpValue"/>
        public override OP OpValue => OP.CheckMultiSigVerify;

        /// <summary>
        /// Same as <see cref="CheckMultiSigOp"/> but runs <see cref="VerifyOp"/> afterwards.
        /// </summary>
        /// <inheritdoc/>
        public override bool Run(IOpData opData, out string error)
        {
            IOperation cms = new CheckMultiSigOp();
            IOperation ver = new VerifyOp();

            if (!cms.Run(opData, out error))
            {
                return false;
            }

            return ver.Run(opData, out error);
        }
    }
}

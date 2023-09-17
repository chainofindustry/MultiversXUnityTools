using Mx.NET.SDK.Domain.Data.Common;
using Mx.NET.SDK.Domain.Helper;
using Mx.NET.SDK.Provider.Dtos.API.Blocks;
using System;

namespace Mx.NET.SDK.Domain.Data.Blocks
{
    public class Block
    {
        /// <summary>
        /// Block hash
        /// </summary>
        public string Hash { get; private set; }

        /// <summary>
        /// Block epoch
        /// </summary>
        public int Epoch { get; private set; }

        /// <summary>
        /// Block nonce
        /// </summary>
        public ulong Nonce { get; private set; }

        /// <summary>
        /// Block previous hash
        /// </summary>
        public string PrevHash { get; private set; }

        /// <summary>
        /// Block proposer
        /// </summary>
        public string Proposer { get; private set; }

        /// <summary>
        /// Block proposer identity
        /// </summary>
        public ProposerIdentity ProposerIdentity { get; private set; }

        /// <summary>
        /// Block public key bitmap
        /// </summary>
        public string PubKeyBitmap { get; private set; }

        /// <summary>
        /// Block round
        /// </summary>
        public ulong Round { get; private set; }

        /// <summary>
        /// Block shard
        /// </summary>
        public int Shard { get; private set; }

        /// <summary>
        /// Block size
        /// </summary>
        public int Size { get; private set; }

        /// <summary>
        /// Block size txs
        /// </summary>
        public int SizeTxs { get; private set; }

        /// <summary>
        /// Block state root hash
        /// </summary>
        public string StateRootHash { get; private set; }

        /// <summary>
        /// Block creation date
        /// </summary>
        public DateTime CreationDate { get; private set; }

        /// <summary>
        /// Block transactions count
        /// </summary>
        public long TxCount { get; private set; }

        /// <summary>
        /// Block gas consumed
        /// </summary>
        public ulong GasConsumed { get; private set; }

        /// <summary>
        /// Block gas refunded
        /// </summary>
        public ulong GasRefunded { get; private set; }

        /// <summary>
        /// Block gas penalized
        /// </summary>
        public ulong GasPenalized { get; private set; }

        /// <summary>
        /// Block max gas limit
        /// </summary>
        public ulong MaxGasLimit { get; private set; }

        /// <summary>
        /// Mini blocks hashes
        /// </summary>
        public string[] MiniBlocksHashes { get; private set; }

        /// <summary>
        /// Notarized blocks hashes
        /// </summary>
        public string[] NotarizedBlocksHashes { get; private set; }

        /// <summary>
        /// Block validators
        /// </summary>
        public string[] Validators { get; private set; }

        private Block() { }

        /// <summary>
        /// Creates a new Block from data
        /// </summary>
        /// <param name="block">Block Data Object from API</param>
        /// <returns></returns>
        public static Block From(BlockDto block)
        {
            return new Block()
            {
                Hash = block.Hash,
                Epoch = block.Epoch ?? 0,
                Nonce = block.Nonce ?? 0,
                PrevHash = block.PrevHash,
                Proposer = block.Proposer,
                ProposerIdentity = ProposerIdentity.From(block.ProposerIdentity),
                PubKeyBitmap = block.PubKeyBitmap,
                Round = block.Round ?? 0,
                Shard = block.Shard ?? 0,
                Size = block.Size ?? 0,
                SizeTxs = block.SizeTxs ?? 0,
                StateRootHash = block.StateRootHash,
                CreationDate = (block.Timestamp ?? 0).ToDateTime(),
                TxCount = block.TxCount ?? 0,
                GasConsumed = block.GasConsumed ?? 0,
                GasRefunded = block.GasRefunded ?? 0,
                GasPenalized = block.GasPenalized ?? 0,
                MaxGasLimit = block.MaxGasLimit ?? 0,
                MiniBlocksHashes = block.MiniBlocksHashes,
                NotarizedBlocksHashes = block.NotarizedBlocksHashes,
                Validators = block.Validators
            };
        }
    }
}

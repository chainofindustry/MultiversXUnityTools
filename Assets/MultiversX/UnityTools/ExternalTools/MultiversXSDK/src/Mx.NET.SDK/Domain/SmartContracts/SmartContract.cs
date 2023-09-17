using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mx.NET.SDK.Core.Cryptography;
using Mx.NET.SDK.Core.Domain.Abi;
using Mx.NET.SDK.Core.Domain.Codec;
using Mx.NET.SDK.Core.Domain.Helper;
using Mx.NET.SDK.Core.Domain.Values;
using Mx.NET.SDK.Provider.Dtos.Common.QueryVm;
using Org.BouncyCastle.Crypto.Digests;
using static Mx.NET.SDK.Core.Domain.Constants.Constants;
using Mx.NET.SDK.Domain.Exceptions;
using Mx.NET.SDK.Provider;

namespace Mx.NET.SDK.Domain.SmartContracts
{
    public class SmartContract
    {
        private static readonly BinaryCodec BinaryCoder = new BinaryCodec();

        /// <summary>
        /// Computes the address of a Smart Contract.
        /// The address is computed deterministically, from the address of the owner and the nonce of the deployment transaction.
        /// </summary>
        /// <param name="ownerAddress">The owner of the Smart Contract</param>
        /// <param name="nonce">The owner nonce used for the deployment transaction</param>
        /// <returns>The smart contract address</returns>
        public static Address ComputeAddress(Address ownerAddress, ulong nonce)
        {
            var ownerPubKey = Converter.FromHexString(ownerAddress.Hex);
            var initialPadding = new byte[8];
            var shardSelector = ownerPubKey.Skip(30).Take(2).ToArray();

            var bigNonceBuffer = BitConverter.GetBytes((long)nonce);

            var bytesToHash = ConcatByteArrays(ownerPubKey, bigNonceBuffer);
            var hash = CalculateHash(bytesToHash);

            var hashBytesToTake = hash.Skip(10).Take(20).ToArray();
            var vmTypeBytes = Converter.FromHexString(ArwenVirtualMachine);
            var addressBytes = ConcatByteArrays(
                                                initialPadding,
                                                vmTypeBytes,
                                                hashBytesToTake,
                                                shardSelector);

            var erdAddress = Bech32Engine.Encode(Hrp, addressBytes);
            return Address.FromBech32(erdAddress);
        }

        /// <summary>
        /// Computes the address of a Smart Contract.
        /// </summary>
        /// <param name="deployTransactionRequest">The deploy transaction request</param>
        /// <returns>Deployed smart contract address</returns>
        public static Address ComputeAddress(TransactionRequest deployTransactionRequest)
        {
            return ComputeAddress(deployTransactionRequest.Sender, deployTransactionRequest.Nonce);
        }

        /// <summary>
        /// Allows one to execute - with no side-effects - a pure function of a Smart Contract and retrieve the execution results (the Virtual Machine Output) from Gateway.
        /// </summary>
        /// <param name="provider">Gateway provider</param>
        /// <param name="address">The Addresses of the Smart Contract.</param>
        /// <param name="abiDefinition">The smart contract ABI Definition</param>
        /// <param name="endpoint">The name of the Pure Function to execute.</param>
        /// <param name="caller">Optional caller</param>
        /// <param name="args">The arguments of the Pure Function. Can be empty</param>
        /// <returns>The response</returns>
        public static Task<T> QuerySmartContractWithAbiDefinition<T>(
            IGatewayProvider provider,
            Address address,
            AbiDefinition abiDefinition,
            string endpoint,
            Address caller = null,
            params IBinaryType[] args) where T : IBinaryType
        {
            var endpointDefinition = abiDefinition.GetEndpointDefinition(endpoint);
            var outputs = endpointDefinition.Output.Select(o => o.Type).ToArray();

            return QuerySmartContract<T>(provider, address, outputs, endpoint, caller, args);
        }

        /// <summary>
        /// Allows one to execute - with no side-effects - a pure function of a Smart Contract and retrieve the execution results (the Virtual Machine Output) from API.
        /// </summary>
        /// <param name="provider">API provider</param>
        /// <param name="address">The Addresses of the Smart Contract.</param>
        /// <param name="abiDefinition">The smart contract ABI Definition</param>
        /// <param name="endpoint">The name of the Pure Function to execute.</param>
        /// <param name="caller">Optional caller</param>
        /// <param name="args">The arguments of the Pure Function. Can be empty</param>
        /// <returns>The response</returns>
        public static Task<T> QuerySmartContractWithAbiDefinition<T>(
            IApiProvider provider,
            Address address,
            AbiDefinition abiDefinition,
            string endpoint,
            Address caller = null,
            params IBinaryType[] args) where T : IBinaryType
        {
            var endpointDefinition = abiDefinition.GetEndpointDefinition(endpoint);
            var outputs = endpointDefinition.Output.Select(o => o.Type).ToArray();

            return QuerySmartContract<T>(provider, address, outputs, endpoint, caller, args);
        }

        /// <summary>
        /// Allows one to execute - with no side-effects - a pure function of a Smart Contract and retrieve the execution results (the Virtual Machine Output) from Gaetway.
        /// </summary>
        /// <param name="provider">MultiversX provider</param>
        /// <param name="address">The Address of the Smart Contract.</param>
        /// <param name="outputTypes">Output value types of the response</param>
        /// <param name="endpoint">The name of the Pure Function to execute.</param>
        /// <param name="caller">Optional caller</param>
        /// <param name="args">The arguments of the Pure Function. Can be empty</param>
        /// <returns>The response</returns>
        public static async Task<T> QuerySmartContract<T>(
            IGatewayProvider provider,
            Address address,
            TypeValue[] outputTypes,
            string endpoint,
            Address caller = null,
            params IBinaryType[] args) where T : IBinaryType
        {
            var arguments = args
                           .Select(typeValue => Converter.ToHexString(BinaryCoder.EncodeTopLevel(typeValue)))
                           .ToArray();

            var query = new QueryVmRequestDto { FuncName = endpoint, Args = arguments, ScAddress = address.Bech32, Caller = caller?.Bech32 };

            var response = await provider.QueryVm(query);
            var data = response.Data;

            if (data.ReturnData is null)
            {
                throw new APIException(data.ReturnMessage);
            }

            return GetResults<T>(outputTypes, data.ReturnData);
        }

        /// <summary>
        /// Allows one to execute - with no side-effects - a pure function of a Smart Contract and retrieve the execution results (the Virtual Machine Output) from API.
        /// </summary>
        /// <param name="provider">MultiversX provider</param>
        /// <param name="address">The Address of the Smart Contract.</param>
        /// <param name="outputTypes">Output value types of the response</param>
        /// <param name="endpoint">The name of the Pure Function to execute.</param>
        /// <param name="caller">Optional caller</param>
        /// <param name="args">The arguments of the Pure Function. Can be empty</param>
        /// <returns>The response</returns>
        public static async Task<T> QuerySmartContract<T>(
            IApiProvider provider,
            Address address,
            TypeValue[] outputTypes,
            string endpoint,
            Address caller = null,
            params IBinaryType[] args) where T : IBinaryType
        {
            var arguments = args
                           .Select(typeValue => Converter.ToHexString(BinaryCoder.EncodeTopLevel(typeValue)))
                           .ToArray();

            var query = new QueryVmRequestDto { FuncName = endpoint, Args = arguments, ScAddress = address.Bech32, Caller = caller?.Bech32 };

            var response = await provider.QueryVm(query);
            var data = response;

            return GetResults<T>(outputTypes, data.ReturnData ?? Array.Empty<string>());
        }

        public static T GetResults<T>(
            TypeValue[] outputTypes,
            string[] data)
        {
            var buffers = data.Select(d => Convert.FromBase64String(d)).ToArray();
            var outputValues = new List<IBinaryType>();
            var bufferIndex = 0;
            int numBuffers = buffers.Length;

            foreach (var outputType in outputTypes)
            {
                var value = ReadValue(outputType);
                outputValues.Add(value);
            }

            IBinaryType ReadValue(TypeValue typeValue)
            {
                if (typeValue.BinaryType == TypeValue.BinaryTypes.Optional)
                {
                    IBinaryType value = ReadValue(typeValue.InnerType);
                    return OptionalValue.NewProvided(value);
                }
                else if (typeValue.BinaryType == TypeValue.BinaryTypes.Variadic)
                {
                    return ReadVariadicValue(typeValue);
                }
                else if (typeValue.BinaryType == TypeValue.BinaryTypes.Multi)
                {
                    var values = new Dictionary<TypeValue, IBinaryType>();

                    foreach (var type in typeValue.MultiTypes)
                        values.Add(type, ReadValue(type));
                    return new MultiValue(typeValue, values);
                }
                else
                {
                    var value = DecodeNextBuffer(typeValue);
                    return value;
                }
            }

            IBinaryType ReadVariadicValue(TypeValue typeValue)
            {
                var values = new List<IBinaryType>();

                if (typeValue.IsCounted())
                {
                    var count = ReadValue(TypeValue.U32TypeValue).ToObject<uint>();
                    for (var i = 0; i < count; i++)
                        values.Add(ReadValue(typeValue.InnerType));
                }
                else
                {
                    while (!HasReachedTheEnd())
                        values.Add(ReadValue(typeValue.InnerType));
                }
                return new VariadicValue(typeValue, typeValue.InnerType, values.ToArray());
            }

            IBinaryType DecodeNextBuffer(TypeValue typeValue)
            {
                if (HasReachedTheEnd())
                {
                    return null;
                }

                var buffer = buffers[bufferIndex++];
                var decodedValue = BinaryCoder.DecodeTopLevel(buffer, typeValue);
                return decodedValue;
            }

            bool HasReachedTheEnd()
            {
                return bufferIndex >= numBuffers;
            }

            if (outputValues.Count == 1)
                return (T)outputValues[0];
            else
                return (T)(IBinaryType)MultiValue.From(outputValues.ToArray());
        }

        private static byte[] ConcatByteArrays(params byte[][] arrays)
        {
            return arrays.SelectMany(x => x).ToArray();
        }

        private static IEnumerable<byte> CalculateHash(byte[] value)
        {
            var digest = new KeccakDigest(256);
            var output = new byte[digest.GetDigestSize()];
            digest.BlockUpdate(value, 0, value.Length);
            digest.DoFinal(output, 0);
            return output;
        }
    }
}

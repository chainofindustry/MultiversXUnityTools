using System.Linq;
using System.Text;
using Mx.NET.SDK.Core.Domain.Values;
using Org.BouncyCastle.Crypto.Digests;

namespace Mx.NET.SDK.Core.Domain
{
    public class SignableMessage
    {
        const string MESSAGE_PREFIX = "\u0017Elrond Signed Message:\n";

        public string Message { get; set; }
        public string Signature { get; set; }
        public Address Address { get; set; }

        public SignableMessage() { }

        public SignableMessage(string message)
        {
            Message = message;
        }

        public SignableMessage(string message, string signature)
        {
            Message = message;
            Signature = signature;
        }

        public byte[] SerializeForSigning()
        {
            var messageSize = Encoding.UTF8.GetBytes($"{Message.Length}");
            var signableMessage = messageSize.Concat(Encoding.UTF8.GetBytes(Message)).ToArray();
            var messagePrefix = Encoding.UTF8.GetBytes(MESSAGE_PREFIX);
            var bytesToHash = messagePrefix.Concat(signableMessage).ToArray();

            var digest = new KeccakDigest(256);
            digest.BlockUpdate(bytesToHash, 0, bytesToHash.Length);
            var calculatedHash = new byte[digest.GetByteLength()];
            digest.DoFinal(calculatedHash, 0);
            var serializedMessage = calculatedHash.Take(32).ToArray();

            return serializedMessage;
        }

        public byte[] SerializeForSigningRaw()
        {
            return Encoding.UTF8.GetBytes(Message);
        }
    }
}

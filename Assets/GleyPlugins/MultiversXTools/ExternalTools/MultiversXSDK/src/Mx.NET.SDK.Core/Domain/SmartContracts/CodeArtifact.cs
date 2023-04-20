using System.IO;
using Mx.NET.SDK.Core.Domain.Helper;

namespace Mx.NET.SDK.Core.Domain.SmartContracts
{
    public class CodeArtifact
    {
        public string Value { get; }

        public CodeArtifact(byte[] bytes)
        {
            Value = Converter.ToHexString(bytes);
        }

        public static CodeArtifact FromFilePath(string filePath)
        {
            var fileBytes = File.ReadAllBytes(filePath);
            return new CodeArtifact(fileBytes);
        }
    }
}

using System;
using System.Security.Cryptography;
using System.Text;

namespace Dotcoin
{
    public class Block
    {
        public const int TRANSACTIONS_PER_BLOCK = 5;
        
        private SHA256 _hasher = SHA256.Create();

        public int Index = int.MinValue;
        public string PreviousHash = string.Empty;
        public string Data = string.Empty;
        public DateTime TimeStamp;
        public long? Alpha;

        public string Hash()
        {
            var blockBuilder = new StringBuilder();
            blockBuilder.Append(Index)
                .Append(TimeStamp.ToShortDateString())
                .Append(Data)
                .Append(PreviousHash)
                .Append(Alpha);
            
            var hashedBytes = _hasher.ComputeHash(blockBuilder.ToString().ToByteArray());
            return hashedBytes.ToNormString();
        }

        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }
}
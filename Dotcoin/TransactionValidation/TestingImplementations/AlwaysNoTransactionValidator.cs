using System.Collections.Generic;

namespace Dotcoin
{
    public class AlwaysNoTransactionValidator : ITransactionVerifier
    {
        public bool ValidTransaction(List<Block> blockChain, Transaction transaction)
        {
            return false;
        }

        public bool ValidTransaction(BlockChain blockChain, Transaction transaction)
        {
            return false;
        }
    }
}
using System.Collections.Generic;

namespace Dotcoin
{
    //Allways returns true, used for testing
    public class AlwaysYesTransactionValidator : ITransactionVerifier
    {
        public bool ValidTransaction(List<Block> blockChain, Transaction transaction)
        {
            return true;
        }

        public bool ValidTransaction(BlockChain blockChain, Transaction transaction)
        {
            return true;
        }
    }
}
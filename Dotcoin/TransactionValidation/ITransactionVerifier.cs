using System.Collections.Generic;

namespace Dotcoin
{
    public interface ITransactionVerifier
    {
        bool ValidTransaction(List<Block> blockChain, Transaction transaction);
        bool ValidTransaction(BlockChain blockChain, Transaction transaction);

    }
}
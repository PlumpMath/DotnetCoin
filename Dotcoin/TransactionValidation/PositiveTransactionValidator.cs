using System;
using System.Collections.Generic;

using static Newtonsoft.Json.JsonConvert;

namespace Dotcoin
{
    public class PositiveTransactionValidator : ITransactionVerifier
    {
        public bool ValidTransaction(List<Block> blockChain, Transaction transaction)
        {
            var to = new Entity
            {
                Identity = transaction.To,
                Balance = 0
            };
            
            var from = new Entity
            {
                Identity = transaction.From,
                Balance = 0
            };
            
            //dont allow people to send to themselves
            if (transaction.To == transaction.From)
            {
                Console.WriteLine("Cannot send to yourself");
                return false;
            }

            bool fromUserExists = false;
            
            //build of the balance of each entity from
            //the ledger
            foreach (var thing in blockChain)
            {
                var transactions = DeserializeObject<List<Transaction>>(thing.Data);
                
                if (transactions == null || transactions.Count == 0)
                {
                    continue;
                }
                
                foreach (var oldTransaction in transactions)
                {
                    if (oldTransaction.To == to.Identity)
                    {
                        to.Balance += oldTransaction.Amount;
                    }
                    if (oldTransaction.To == from.Identity)
                    {
                        fromUserExists = true;
                        from.Balance += oldTransaction.Amount;
                    }
                }
            }

            //trying to send money from a user that does not exist
            if (!fromUserExists)
            {
                return false;
            }
            
            if (from.Balance - transaction.Amount < 0)
            {
                Console.WriteLine("User does not have enough funds");
                return false;
            }

            to.Balance += transaction.Amount;
            from.Balance -= transaction.Amount;

            return true;
        }

        public bool ValidTransaction(BlockChain blockChain, Transaction transaction)
        {
            return ValidTransaction(blockChain.GetBlockChain(), transaction);
        }

        private class Entity
        {
            public string Identity;
            public long Balance;
        }
    }
}
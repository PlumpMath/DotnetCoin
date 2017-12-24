using System;
using System.Collections.Generic;
using Dotcoin;
using Xunit;
using static Newtonsoft.Json.JsonConvert;

namespace DotcoinTests
{
    public class PositiveTransactionValidatorTests
    {
        
        [Fact]
        public void Test_UserDoesNotExist()
        {
            var blockChain = CreateChainWithTransactions();
            var transactionValidator = new PositiveTransactionValidator();

            var badTransaction = new Transaction
            {
                To = "dillon",
                From = "doesnotexists",
                Amount = 21
            };
            
            Assert.False(transactionValidator.ValidTransaction(blockChain, badTransaction));
        }

        [Fact]
        public void Test_UserDoesNotHaveEnoughMoney()
        {
            var blockChain = CreateChainWithTransactions();
            var transactionValidator = new PositiveTransactionValidator();

            var badTransaction = new Transaction
            {
                To= "bob",
                From = "dillon",
                Amount = 10000
            };
            
            Assert.False(transactionValidator.ValidTransaction(blockChain, badTransaction));

        }

        [Fact]
        public void Test_GoodTransaction()
        {
            var blockChain = CreateChainWithTransactions();
            var transactionValidator = new PositiveTransactionValidator();

            var goodTransaction = new Transaction
            {
                To = "bob",
                From = "dillon",
                Amount = 2
            };
            
            Assert.True(transactionValidator.ValidTransaction(blockChain, goodTransaction));
            
        }
        private BlockChain CreateChainWithTransactions()
        {
            var blockChain = new BlockChain("transactionTest.chain");

            if (blockChain.Size() == 1)
            {
                var block1 = blockChain.GetNextBlock();
                var transaction1 = new Transaction
                {
                    To = "dillon",
                    From = "",
                    Amount = 23
                };
                block1.Alpha = 0;
                block1.TimeStamp = DateTime.Now;
                block1.Data = SerializeObject(new List<Transaction>
                {
                    transaction1
                });
            
                blockChain.AddNextBlock(block1);
            }

            return blockChain;
        }
        
    }
}
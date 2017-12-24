using System;
using System.Collections.Generic;

using Dotcoin;
using Dotcoin.Network.Server;
using Xunit;

using static Newtonsoft.Json.JsonConvert;

namespace DotcoinTests
{
    public class NodeTests
    {
        
        [Fact]
        public void TestNode_Creation()
        {
            var transactionValidator = new AlwaysYesTransactionValidator();
            var node = new Node(transactionValidator, dotcoinServer:GetTestServer());
            
            Assert.True(node.ToString() != "");
        }

        [Fact]
        public void TestNode_LoadAndSize()
        {
            var node = new Node(new AlwaysYesTransactionValidator(), "load_test.chain",dotcoinServer:GetTestServer());
            
            Assert.True(node.BlockChainSize() == 1);
        }

        [Fact]
        public void TestNode_AddingGoodTransaction()
        {
            var node = CreateYesTestNode();
            
            var transaction = new Transaction
            {
                To = "adam",
                From = "eli",
                Amount = 23
            };
            
            node.AddTransaction(transaction);

            var verified = node.VerifyPendingTransactions();
            
            Assert.True(node.VerifyedBlocksSize() == 1 && verified);
        }

        [Fact]
        public void TestNode_RejectingBadTransaction()
        {
            var node = CreateNoTestNode();
                
            var transaction = new Transaction
            {
                To = "adam",
                From = "eli",
                Amount = 23
            };
            
            node.AddTransaction(transaction);

            var bad = node.VerifyPendingTransactions();
            
            Assert.True(!bad && node.VerifyedBlocksSize() == 0);
        }

        [Fact]
        public void TestNode_Verifying5Transaction()
        {
            var node = CreateYesTestNode();


            for (int i = 0; i < 6; i++)
            {
                var transaction = new Transaction
                {
                    To = "bob",
                    From = "billy",
                    Amount = 23
                };

                node.AddTransaction(transaction);
            }

            var good = node.VerifyPendingTransactions();
            
            Assert.True(good 
                        && node.VerifyedBlocksSize() == 1
                        && node.PendingTransactionsSize() == 1);
        }

        [Fact]
        public void TestNode_BadPreviousHashAdd()
        {
            var node = CreateYesTestNode();

            var badBlock = new Block
            {
                TimeStamp = DateTime.MinValue,
                Data = "bad data",
                PreviousHash = "21",
                Alpha = 1,
                Index = 1
            };

            var result = node.Add(new List<Block>
            {
                badBlock
            });
            
            Assert.False(result);
        }

        [Fact]
        public void TestNode_BadIndex()
        {
            var node = CreateYesTestNode();
    
            
            var badBlock = new Block
            {
                TimeStamp = DateTime.MinValue,
                Data = "bad data",
                PreviousHash = "21",
                Alpha = 1,
                Index = 3
            };

            var result = node.Add(new List<Block>{ 
                badBlock
            });
            
            Assert.False(result);
        }
        
        [Fact]
        public void TestNode_MiningBlock()
        {
            var node = CreateYesTestNode();
            
            var transaction = new Transaction
            {
                To = "dillon",
                From = "bob",
                Amount = 2
            };
            
            for (int i = 0; i < 2; i++)
            {
                node.AddTransaction(transaction);
            }

            var r1 = node.VerifyPendingTransactions();
            
            Assert.True(r1);

            var minedBlock = node.Mine();
            
            Assert.True(minedBlock != null);
            
        }
        
        [Fact]
        public void TestNode_MultipleTransactionsAddedToBlock()
        {
            var node = CreateYesTestNode();


            var transaction = new Transaction
            {
                To = "dillon",
                From = "bob",
                Amount = 2
            };
            
            for (int i = 0; i < 2; i++)
            {
                node.AddTransaction(transaction);
            }

            var r1 = node.VerifyPendingTransactions();
            
            Assert.True(r1);

            var minedBlock = node.Mine();

            var transactions = DeserializeObject<List<Transaction>>(minedBlock.Data);
            
            Assert.True(transactions.Count == 2);
        }
        
        private static Node CreateYesTestNode()
        {
            return new Node(new AlwaysYesTransactionValidator(), "dotcoin.chain", true, dotcoinServer:GetTestServer());
        }
        
        private static Node CreateNoTestNode()
        {
            return new Node(new AlwaysNoTransactionValidator(), "dotcoin.chain", true, dotcoinServer:GetTestServer());
        }

        private static IDotcoinServer GetTestServer()
        {
            return new DotcoinTestServer();
        }
    }
}
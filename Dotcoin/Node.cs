using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Dotcoin.Network;
using Newtonsoft.Json;
using static Newtonsoft.Json.JsonConvert;

namespace Dotcoin
{
    public class Node : IDisposable
    {
        private readonly BlockChain _blockChain;
        private readonly Queue<Transaction> _incomingTransactions = new Queue<Transaction>();
        private readonly Queue<Block> _verifyedBlocks = new Queue<Block>();
        private readonly ITransactionVerifier _transactionVerifier;
        private readonly IProofOfWork _proofOfWork = new SimpleProofOfWork();
        private readonly bool _overideBlockchainSave; //used if when we modify the chain in a test we don't save that change
        private readonly DotcoinNetwork _network;
        
        private IPAddress _masterAddress = null;
        
        public Node(ITransactionVerifier transactionVerifier, string overrideFilelocation = "", bool overideBlockchainSave = false, IPAddress master = null)
        {
            //if you dont pass in a master address it assumes
            //there is no master and takes over that roll
            IPAddress myIp = null;
            
            var hostname = Dns.GetHostName();
                
            Console.WriteLine(string.Format("Nodes hostname is: {0}", hostname));

            var ips = Dns.GetHostAddresses(hostname);

            foreach (var ip in ips)
            {
                //TODO make this a correct public ip
                myIp = ip;
                _masterAddress = ip;
                Console.WriteLine(ip.ToString());
            }
            
            if (myIp == null)
            {
                throw new Exception("Error getting ip");
            }
            
            if (master == null)
            {
                _masterAddress = myIp;
            }
            
            _network = new DotcoinNetwork(myIp, _masterAddress);
            
            _transactionVerifier = transactionVerifier;
            _overideBlockchainSave = overideBlockchainSave;
            
            if (overrideFilelocation == "")
            {
                overrideFilelocation = "dotcoin.chain";
            }
            
            _blockChain = new BlockChain(overrideFilelocation);
        }

        #region Public Methods
        
        /// <summary>
        /// This method is mean to add a mined block to bring the block chain
        /// in line with the rest of the network
        /// </summary>
        /// <param name="blocks">A completly filled out block</param>
        /// <returns>
        /// If the block's mine is incorect or the blocks prevhash is incorrect returns false, 
        /// otherwise the block is added and returns true
        /// </returns>
        public bool Add(List<Block> blocks)
        {
            foreach (var block in blocks)
            {
                if (!_proofOfWork.VerifyBlock(block))
                {
                    return false;
                }

                if (!_blockChain.AddNextBlock(block))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// This method checks the consistency of the block chain
        /// </summary>
        /// <returns>
        /// True chain is consistent, 
        /// false chain is not consistent
        /// </returns>
        public bool VerifyChain()
        {
            return _blockChain.VerifyChain();
        }


        /// <summary>
        /// Adds a new transaction to the incoming transaction queue
        /// </summary>
        /// <param name="transaction"></param>
        public void AddTransaction(Transaction transaction)
        {
            _incomingTransactions.Enqueue(transaction);
        }

        /// <summary>
        /// This method grabs n=TRANSACTIONS_PER_BLOCK pending transactions and checks if they
        /// are allowed.  If a Transaction is not allowed it is removed from
        /// the queue..
        /// </summary>
        /// <returns></returns>
        public bool VerifyPendingTransactions()
        {
            //shoudl go threw the chain and make sure the person
            //has enough balance to send value a to persone b
            //right now we will just create a block and allow negative numbers
            var block = new Block
            {
                TimeStamp = DateTime.Now,
            };

            var verifiedTransactions = new List<Transaction>();
            var numVerified = 0;
            
            for (int i = 0; i < Block.TRANSACTIONS_PER_BLOCK && _incomingTransactions.Count > 0; i++)
            {
                var transactionToVerify = _incomingTransactions.Dequeue();
                if (VerifyTransactionValidity(transactionToVerify))
                {
                    verifiedTransactions.Add(transactionToVerify);
                    numVerified++;
                }
                else
                {
                    Console.WriteLine(string.Format("The following transaction was invalid {0}",
                        SerializeObject(transactionToVerify)));
                }
            }

            if (numVerified > 0)
            {
                block.Data = SerializeObject(verifiedTransactions);

                _verifyedBlocks.Enqueue(block);

                return true;
            }
                
            return false;
            
        }

        /// <summary>
        /// Mines the next verified block with no alpha and returns the "Mined" block
        /// </summary>
        /// <returns>A block ready to be added to the block chain</returns>
        public Block Mine()
        {
            if (!VerifyPendingTransactions() && _verifyedBlocks.Count == 0)
            {
                Console.WriteLine("No new verified blocks to mine");
                return null;
            }

            var blockToAdd = _blockChain.GetNextBlock();

            Console.WriteLine(string.Format("Mining the next verified block #{0}", blockToAdd.Index));    
                
            blockToAdd.Data = _verifyedBlocks.Dequeue().Data;
                
            var result = _proofOfWork.MineBlock(blockToAdd);
                
            if (result != null)
            {
                Console.WriteLine("Blocked Mined");
                Console.WriteLine(SerializeObject(result));
                
                _blockChain.AddNextBlock(result);
                    
                Console.WriteLine("Block Added");

                if (!_overideBlockchainSave)
                {
                    _blockChain.Save();
                    Console.WriteLine("Block chain saved to file");
                }
                else
                {
                    Console.WriteLine("Block chain not saved, overide active");
                }
                
                return result;
            }
                
            Console.WriteLine("Block not verified");
            
            return null;
        }

        /// <summary>
        /// Returns a string representation of a wallet address
        /// and the current balacne
        /// </summary>
        /// <param name="walletAddress">The wallet address</param>
        /// <returns>
        /// Json representation of wallet address and the blance
        /// of the wallet
        /// </returns>
        public string GetWalletInformation(string walletAddress)
        {
            return SerializeObject(new
            {
                Address = walletAddress,
                Balance = _blockChain.GetBalance(walletAddress)
            });
        }

        public List<Transaction> GetPendingTransactions()
        {
            return _incomingTransactions.Take(5).ToList();
        }

        public int BlockChainSize()
        {
            return _blockChain.Size();
        }

        public int PendingTransactionsSize()
        {
            return _incomingTransactions.Count;
        }

        public int VerifyedBlocksSize()
        {
            return _verifyedBlocks.Count;
        }
                
        /// <summary>
        /// Returns a pretty formated string of all the blocks
        /// </summary>
        /// <returns>Pretty string</returns>
        public override string ToString()
        {
            return _blockChain.ToString();
        }
        
        /// <summary>
        /// Returns a json representaion of the block chain
        /// </summary>
        /// <returns>Json string</returns>
        public string ToJsonString()
        {
            return SerializeObject(_blockChain.GetBlockChain());
        }
        
        public void Dispose()
        {
            if (!_overideBlockchainSave)
            {
                _blockChain?.Dispose();        
            }
        }
        #endregion
        
        #region Private Methods
        
        private bool VerifyTransactionValidity(Transaction transaction)
        {
            return _transactionVerifier.ValidTransaction(_blockChain, transaction);
        }
        
        #endregion
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using static Newtonsoft.Json.JsonConvert;

namespace Dotcoin
{
    //Wraper class for containing and adding blocks
    public class BlockChain : IDisposable
    {   
        private readonly List<Block> _blockChain = new List<Block>();
        private readonly string _fileLocation;
        
        public BlockChain()
        {
            _fileLocation = "dotcoin.chain";
            PrimeChain();
        }

        public BlockChain(string filelocation)
        {
            _fileLocation = filelocation;
            
            //load the node stuff from a file
            if (File.Exists(_fileLocation))
            {
                var file = File.ReadAllText(_fileLocation);
                var lines = file.Split('\n');
                foreach (var line in lines)
                {
                    var block = DeserializeObject<Block>(line);
                    if (block != null)
                    {
                        _blockChain.Add(block);
                    }
                }
                if (_blockChain.Count == 0)
                {
                    PrimeChain();
                }
                else
                {
                    Console.WriteLine(string.Format("Loaded {0} blocks", _blockChain.Count));
                }
            }
            else
            {
                File.Create(_fileLocation);
                
                PrimeChain();
            }
        }

        public List<Block> GetBlockChain()
        {
            return _blockChain;
        }
        
        public bool VerifyChain()
        {
            for (int i = 0; i < _blockChain.Count - 1; i++)
            {
                if (_blockChain[i].Hash() != _blockChain[i + 1].PreviousHash)
                {
                    return false;
                }
            }
            
            return true;
        }

        //Gets the next block that can be added to the chain
        public Block GetNextBlock()
        {
            return new Block
            {
                PreviousHash = _blockChain.Last().Hash(),
                Index = _blockChain.Count
            };
        }

        public bool AddNextBlock(Block block)
        {
            if (block.Index != _blockChain.Count)
            {
                throw new ArgumentException("Please make sure that you use the get next block method");
            }
            if (block.Alpha == null)
            {
                throw new ArgumentException("Make sure the block has been mined");
            }
            
            if (block.PreviousHash != _blockChain.Last().Hash())
            {
                return false;
            }
            
            _blockChain.Add(block);
            return true;
        }

        public long GetBalance(string walletAddress)
        {
            long balance = 0;
            
            foreach (var block in _blockChain)
            {
                var transactions = DeserializeObject<List<Transaction>>(block.Data);

                //first primed transaction will be null
                if (transactions == null)
                {
                    continue;
                }
                
                foreach (var transaction in transactions)
                {
                    if (transaction.To == walletAddress)
                    {
                        balance += transaction.Amount;
                    }
                    else if (transaction.From == walletAddress)
                    {
                        balance -= transaction.Amount;
                    }
                }
            }
            return balance;
        }
        public int Size()
        {
            return _blockChain.Count;
        }

        public void Save()
        {
            Console.WriteLine("Saving BlockChain to: " + _fileLocation);
            using (var streamWriter = File.CreateText(_fileLocation))
            {
                //this method with handle saving to persistent memory
                foreach (var block in _blockChain)
                {
                    streamWriter.WriteLine(block);
                }
            }    
        }
        public void Dispose()
        {
            //only want to save on disposle if the
            //chain is in a good place
            if (VerifyChain())
            {
                Save();
            }
        }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();

            for (int i = 0; i < _blockChain.Count; i++)
            {
                stringBuilder.AppendFormat("Block #{0}\n", i);
                stringBuilder.AppendFormat("Hash #{0}\n", _blockChain[i].Hash());
                stringBuilder.AppendLine();
            }

            return stringBuilder.ToString();
        }
        
        private void PrimeChain()
        {
            Console.WriteLine("Priming block chain");
            var omega = new Block
            {
                Data = "",
                TimeStamp = DateTime.Now,
                PreviousHash = "",
                Index = 0
            };
            _blockChain.Add(omega);
        }
    }
}
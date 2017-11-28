using System;
using System.Security.Cryptography.X509Certificates;
using Dotcoin;
using Xunit;

namespace DotcoinTests
{
    public class BlockChainTests
    {
        [Fact]
        public void TestGetNextBlock()
        {
            var blockChain = new BlockChain();
            var block2 = blockChain.GetNextBlock();
            
            Assert.True(block2.Index == 1);
        }

        [Fact]
        public void TestVerifyChainSuccess()
        {
            var blockChain = new BlockChain();
            
            var block1 = blockChain.GetNextBlock();
            
            block1.Data = "eli snores";
            block1.Alpha = 21;
            block1.TimeStamp = DateTime.Now;
            
            blockChain.AddNextBlock(block1);

            var block2 = blockChain.GetNextBlock();
            
            block2.Data = "eli sleep talks";
            block2.Alpha = 2;
            block2.TimeStamp = DateTime.Now;
                        
            blockChain.AddNextBlock(block2);
            
            Assert.True(blockChain.VerifyChain());
        }

        [Fact]
        public void TestVerifyChainFailure_BlockPrevHashModified()
        {
            var blockChain = new BlockChain();
            
            var block1 = blockChain.GetNextBlock();
            
            block1.Data = "eli snores";
            block1.Alpha = 21;
            block1.TimeStamp = DateTime.Now;
            block1.PreviousHash = "ppop";
            
            Assert.False(blockChain.AddNextBlock(block1));
        }
        
        [Fact]
        public void TestVerifyChainFailure_BlockIndexModified()
        {
            var blockChain = new BlockChain();
            
            var block1 = blockChain.GetNextBlock();
            
            block1.Data = "eli snores";
            block1.Alpha = 21;
            block1.TimeStamp = DateTime.Now;
            block1.Index = 0;
            
            Assert.Throws<ArgumentException>(() => blockChain.AddNextBlock(block1));
        }

        [Fact]
        public void TestVerifyChain_UnloadinLoading()
        {
            var blockChain = new BlockChain();

            var block = blockChain.GetNextBlock();

            block.Data = "hi";
            block.Alpha = 1;
            block.TimeStamp = DateTime.Now;
            
            blockChain.AddNextBlock(block);
            
            blockChain.Dispose();
            
            var loadedChain = new BlockChain("dotcoin.chain");
            
            Assert.True(loadedChain.Size() == 2);

        }
    }
}
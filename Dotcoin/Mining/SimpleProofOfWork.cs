using System;
using System.Numerics;

namespace Dotcoin
{
    public class SimpleProofOfWork : IProofOfWork
    {

        private const int DIFFICULTY = 3;
        
        //Verifyies a block and returns a block with the correct alpha
        //to be added to the chain
        public Block MineBlock(Block block)
        {
            if (block.Alpha != null)
            {
                throw new Exception("This alpha should not be set" + block);
            }

            for (long i = 0; i < long.MaxValue; i++)
            {
                block.Alpha = i;
                var sub = block.Hash().Substring(1, DIFFICULTY);
                //Console.WriteLine(sub);
                if (sub == new string('0', DIFFICULTY))
                {
                    return block;
                }
            }
            return null;
        }

        /// <summary>
        /// Verifies a mined block
        /// </summary>
        /// <param name="block">A complete block</param>
        /// <returns>Returns true if the block is good, false if the block's alpha is incorect</returns>
        public bool VerifyBlock(Block block)
        {
            if (block.Alpha == null)
            {
                return false;
            }
            return block.Hash().Substring(1, DIFFICULTY) == new string('0', DIFFICULTY);
        }
    }
}
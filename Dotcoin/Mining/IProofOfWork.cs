namespace Dotcoin
{
    public interface IProofOfWork
    {
        Block MineBlock(Block block);
        bool VerifyBlock(Block block);
    }
}
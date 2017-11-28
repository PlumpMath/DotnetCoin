using System;
using System.Net;
using System.Threading.Tasks;
using Dotcoin;
using DotcoinApi.Settings;

using Microsoft.AspNetCore.Mvc;

namespace DotcoinApi.Controllers
{
    [Route("api/[controller]")]
    public class CoinController : Controller
    {

        private readonly Node _node;
        private readonly SettingsManager _settingsManager;
        private SettingsModel _settingsModel;
        
        public CoinController(Node node, SettingsManager settingsManager)
        {
            _node = node;
            _settingsManager = settingsManager;
        }
        
        //Adds a transaction to the chain
        [HttpPost("Transaction")]
        public async Task<IActionResult> AddTransaction([FromBody] Transaction transaction)
        {
            _node.AddTransaction(transaction);
            return new CreatedResult("Transaction added", "Transaction Added");
        }
        
        //Gets the first 5 unverified transactions
        [HttpGet("Transaction")]
        public async Task<IActionResult> GetPendingTransactions()
        {
            return new OkObjectResult(_node.GetPendingTransactions());
        }

        //Mines the current transactions and returns the resulting block
        [HttpGet("Mine")]
        public async Task<IActionResult> MineChain()
        {
            var block = _node.Mine();
            if (block == null && !_node.VerifyChain())
            {
                return new NoContentResult();
            }
            return new OkObjectResult(block); 
        }

        //Verifies a block if added is allowed
        [HttpPost("Block")]
        public async Task<IActionResult> VerifyBlock([FromBody] Block block)
        {
            throw new NotImplementedException();
            if (_node.Add(null))
            {
                return new CreatedResult("Block added", "Block added");
            }
            return new BadRequestObjectResult("Block rejected");
        }
        
        //Returns a pretty string representation of a block chain
        [HttpGet("BlockChain/Pretty")]
        public async Task<IActionResult> GetBlockChain()
        {
            return new OkObjectResult(_node.ToString());
        }

        //Returns a Json of representation of the actual block chain
        [HttpGet("BlockChain")]
        public async Task<IActionResult> GetJsonBlockChain()
        {
            return new OkObjectResult(_node.ToJsonString());
        }

        //Verifyies the chains consistensy
        [HttpGet("BlockChain/Verify")]
        public async Task<IActionResult> VerifyChain()
        {
            return new OkObjectResult(_node.VerifyChain());
        }
        
        //Gets all the nodes on the network
        [HttpGet("BlockChain/Network")]
        public async Task<IActionResult> GetComputersOnNetwork()
        {
            if (_settingsModel == null)
            {
                _settingsModel = await _settingsManager.GetSettings();
            }
            return new OkObjectResult(_settingsModel.IpAddresses);

        }
        
        //Adds a node to the network
        [HttpPost("BlockChain/Network")]
        public async Task<IActionResult> AddComputerToNetwork([FromQuery] string ipAddress)
        {
            if (_settingsModel == null)
            {
                _settingsModel = await _settingsManager.GetSettings();
            }
            
            int loc = _settingsModel.IpAddresses.Count;
 
            _settingsModel.IpAddresses.Add(new IPAddress(ipAddress.ToByteArray()));
            _settingsManager.Updatesettings(_settingsModel);
            
            return new CreatedResult("BlockChain/Network/" + loc, ipAddress);
        }
        
        //Gets the wallet of a user based on the address
        [HttpGet("Wallet")]
        public async Task<IActionResult> GetWalletAmount([FromQuery] string walletAddress)
        {
            return new OkObjectResult(_node.GetWalletInformation(walletAddress));
        }
    }
}
using System;
using System.Net.Http;
using Dotcoin;

namespace SimpleBlockChain
{
    class Program
    {
        private static ConsoleSettings _settings = new ConsoleSettings();
        private const string DEFAULT_CONSOLE_MODE = "Node";
        
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                args = new string[1];
                args[0] = DEFAULT_CONSOLE_MODE;
            }

            _settings.LoadSettings().Wait();
            
            switch (args[0])
            {
                 case "Node":
                     NodeMode();
                     break;
                 case "Client":
                     HttpClientMode();
                     break;
                 case "Help":
                     PrintHelpMessage();
                     break;
                 default:
                     HttpClientMode();
                     break;
            }
        }
        static void PrintHelpMessage()
        {
            string helpMessage = "args0: Mode (Client, Node, Help)\n" +
                                 "args1: Settings File\n";
            
            Console.WriteLine(helpMessage);
        }
        static void NodeMode()
        {
            Console.WriteLine("Welcome to node mode");

            var node = new Node(new PositiveTransactionValidator());

        }
        static void HttpClientMode()
        {
            Console.WriteLine("Welcome to client mode");

            var client = new HttpClient();
            client.BaseAddress = new Uri(_settings.GetConnectionUrl());
            
            while (true)
            {
                Console.WriteLine("Enter -1 to quite, 0 to add a transaction, 1 to print the chain, 2 to verify the chain, 3 mine the node, 4 to get the chain in json");
                
                int command = int.Parse(Console.ReadLine());
                
                if (command == -1)
                {
                    return;
                }
                if (command == 0)
                {
                    Console.WriteLine("Creating new transaction");
                    Console.WriteLine("enter the too address: " );
                    
                    string to = Console.ReadLine();
                    
                    Console.WriteLine("Enter the from address: ");
                    string from = Console.ReadLine();
                    
                    Console.WriteLine("Enter the value: ");
                    int value = int.Parse(Console.ReadLine());
                    
                    var trans = new Transaction
                    {
                        To = to,
                        From = from,
                        Amount = value
                    };

                    var result = client.PostAsJsonAsync("Transaction", trans);
                    
                    Console.WriteLine(result.Result.Content.ReadAsStringAsync().Result);
                }
                if (command == 1) //print chain
                {
                    var result = client.GetAsync("BlockChain/Pretty");
                    
                    Console.WriteLine(result.Result.Content.ReadAsStringAsync().Result);
                }
                if (command == 2) //verify chain
                {
                    var result = client.GetAsync("BlockChain/Verify");
                    
                    Console.WriteLine(result.Result.Content.ReadAsStringAsync().Result);
                }
                if (command == 3)
                {
                    var result = client.GetAsync("Mine");
                    
                    Console.WriteLine(result.Result.Content.ReadAsStringAsync().Result);
                }
                if (command == 4)
                {
                    var result = client.GetAsync("BlockChain");
                    
                    Console.WriteLine(result.Result.Content.ReadAsStringAsync().Result);
                }
            }
        }
    }
}
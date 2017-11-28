using System;
using System.Net.Http;
using Dotcoin;

namespace SimpleBlockChain
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to this simple blockchain");

            var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:5000/api/Coin/");
            
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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using VirtualCashCard.Interface;

namespace VirtualCashCard
{
    class Program
    {
        private static readonly AutoResetEvent waitHandle = new AutoResetEvent(false);

        static void Main(string[] args)
        {
            IServiceCollection services = new ServiceCollection();
            Startup startup = new Startup();
            startup.ConfigureServices(services);
            IServiceProvider serviceProvider = services.BuildServiceProvider();

            //configure console logging
            serviceProvider
                .GetService<ILoggerFactory>()
                .AddConsole(LogLevel.Debug);

            var logger = serviceProvider.GetService<ILoggerFactory>()
                .CreateLogger<Program>();

            logger.LogDebug("Starting Virtual Cash Card system...");

            Task.Run(() =>
            {
                var service = serviceProvider.GetService<ICardService>();
                service.Initialize();

                while (true)
                {
                    int userInput = MainMenu();

                    switch (userInput)
                    {
                        case 1:
                            {
                                Console.WriteLine("Enter Account Number - ");
                                long accountId = long.Parse(Console.ReadLine());
                                Console.WriteLine("Enter Account Pin - ");
                                int accountPin = int.Parse(Console.ReadLine());
                                if (!service.VerifyAccount(accountId, accountPin))
                                {
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.WriteLine("Invalid Account & Pin combination. Retry again");
                                    Console.ResetColor();
                                }
                                else
                                {
                                    Console.ForegroundColor = ConsoleColor.Green;
                                    Console.WriteLine($"Logged in to Account {accountId}");
                                    Console.ResetColor();
                                }
                                break;
                            }
                        case 2:
                            {
                                if (!service.IsCurrentUserLoggedIn())
                                {
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.WriteLine("Not logged into Account. Please try option 1 to login with valid credentials.");
                                    Console.ResetColor();
                                }
                                else
                                {
                                    DisplayCurrentAccountBalance(service);
                                }
                                break;
                            }
                        case 3:
                            {
                                if (!service.IsCurrentUserLoggedIn())
                                {
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.WriteLine("Not logged into Account. Please try option 1 to login with valid credentials.");
                                    Console.ResetColor();
                                }
                                else
                                {
                                    Console.WriteLine("Enter Account Pin - ");
                                    int accountPin = int.Parse(Console.ReadLine());
                                    if(!service.ValidatePin(accountPin))
                                    {
                                        Console.ForegroundColor = ConsoleColor.Red;
                                        Console.WriteLine("Invalid pin entered for logged in account. Can't withdraw.");
                                        Console.ResetColor();
                                        break;
                                    }

                                    Console.WriteLine("Enter amount to withdraw - ");
                                    double amount = double.Parse(Console.ReadLine());

                                    if(!service.WithdrawCashFromAccount(amount))
                                    {
                                        Console.ForegroundColor = ConsoleColor.Red;
                                        Console.WriteLine("Not enough balance in account to withdraw.");
                                        Console.ResetColor();
                                        break;
                                    }
                                    DisplayCurrentAccountBalance(service);
                                }
                                break;
                            }
                        case 4:
                            {
                                if (!service.IsCurrentUserLoggedIn())
                                {
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.WriteLine("Not logged into Account. Please try option 1 to login with valid credentials.");
                                    Console.ResetColor();
                                }
                                else
                                {
                                    Console.WriteLine("Enter Account Pin - ");
                                    int accountPin = int.Parse(Console.ReadLine());
                                    if (!service.ValidatePin(accountPin))
                                    {
                                        Console.ForegroundColor = ConsoleColor.Red;
                                        Console.WriteLine("Invalid pin entered for logged in account. Can't withdraw.");
                                        Console.ResetColor();
                                        break;
                                    }

                                    Console.WriteLine("Enter amount to deposit - ");
                                    double amount = double.Parse(Console.ReadLine());

                                    service.DepositCashToAccount(amount);
                                    DisplayCurrentAccountBalance(service);
                                }
                                break;
                            }
                        case 5:
                            {
                                Console.WriteLine("Exit");
                                waitHandle.Set();
                                Environment.Exit(0);
                                break;
                            }

                    }
                }
            });

            // Handle Control+C or Control+Break
            Console.CancelKeyPress += (o, e) =>
            {
                Console.WriteLine("Exit");

                // Allow the manin thread to continue and exit...
                waitHandle.Set();
            };

            // Wait
            waitHandle.WaitOne();
        }

        private static void DisplayCurrentAccountBalance(ICardService service)
        {
            (long Account, double Balance) accountBalance = service.GetAccountBalance();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Account : {accountBalance.Account} | Account Balance Amount : {accountBalance.Balance}");
            Console.ResetColor();
        }

        private static int MainMenu()
        {
            Console.WriteLine("Main menu:");
            Console.WriteLine("(1) - Log in to Account");
            Console.WriteLine("(2) - View my balance");
            Console.WriteLine("(3) - Withdraw cash");
            Console.WriteLine("(4) - Deposit cash");
            Console.WriteLine("(5) - Exit");
            Console.WriteLine("\nEnter an option: ");

            return int.Parse(Console.ReadLine());
        }
    }
}

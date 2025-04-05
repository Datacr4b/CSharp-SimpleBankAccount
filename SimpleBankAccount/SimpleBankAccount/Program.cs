using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBankAccount
{
    class Program
    {
        private static List<Account> accounts = new List<Account>();
        private static string input;
        private static string accNumber;
        private static string owner;
        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("Create Account (c)");
                Console.WriteLine("Deposit into Account (d)");
                Console.WriteLine("Withdraw from Account (w)");
                Console.WriteLine("Transfer to Account (t)");
                Console.WriteLine("Quit (q)");
                Console.WriteLine("Choose option: ");
                input = Console.ReadLine();
                if (!string.IsNullOrEmpty(input))
                {
                    input = input.Trim().ToLower();
                    if (input == "c" || input == "d" || input == "w")
                    {
                        AccountOperations(input);
                    }

                }
                else
                {
                    Console.WriteLine("Invalid input");
                    Console.ReadLine();
                }
            }
        }

        static void AccountOperations(string entry)
        {
            while (true) // Name Loop
            {
                Console.WriteLine("Enter your name: ");
                input= Console.ReadLine();
                if (!string.IsNullOrEmpty(input))
                {
                    owner = input;
                    while(true) // Account Number Loop
                    {
                        Console.WriteLine("Enter your account number: ");
                        accNumber = Console.ReadLine();
                        if (!string.IsNullOrEmpty(accNumber))
                        {
                            Predicate<Account> check = acc => acc.AccountNumber == accNumber && acc.Owner == owner;
                            if (entry=="c") // Creating Account
                            {
                                Account existingAcc = accounts.Find(check);
                                if (existingAcc == null)
                                    accounts.Add(new Account(input, accNumber));
                                else
                                    Console.WriteLine("Account already exists");
                                break;
                            }
                            else // Deposit and Withdraw
                            {
                                while (true) // Amount Loop
                                {
                                    Console.WriteLine("Enter the amount: ");
                                    input=Console.ReadLine();
                                    if (WithdrawDeposit(entry, input, check))
                                        break;
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("Invalid Account Number");
                            Console.ReadLine();
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Invalid name");
                    Console.ReadLine();
                }
            }
        }

        static bool WithdrawDeposit(string entry, string input, Predicate<Account> check)
        {
            Account existingAcc = accounts.Find(check);
            decimal amount;

            if (Decimal.TryParse(input, out amount))
            {
                if (entry == "d")
                    existingAcc.Deposit(amount);
                else if (entry == "w")
                    existingAcc.Withdraw(amount);
                return true;
            }
            else
            {
                Console.WriteLine("Invalid amount");
                Console.ReadLine();
                return false;
            }
        }


    }

    class Account
    {
        private string owner;
        private string accountNumber;
        private decimal balance;

        public string Owner
        {
            get { return owner; }
            set { owner = value; }
        }
        public string AccountNumber
        {
            get { return accountNumber; }
            set { accountNumber = value; }
        }
        public decimal Balance
        {
            get { return balance; }
            set { balance = value; }
        }

        public Account(string owner, string accountNumber)
        {
            Owner = owner;
            AccountNumber = accountNumber;
            Balance = 0;
        }

        public void Deposit(decimal amount)
        {
            Balance += amount;
            Console.WriteLine($"Deposited {amount:C}");
        }

        public decimal Withdraw(decimal amount)
        {
            Balance -= amount;
            return amount;
        }

        public void Transfer(decimal amount, string accnumber, string owner, List<Account> accounts, Predicate<Account> check)
        {
            Account transferAcc = accounts.Find(check);
            if (transferAcc != null)
            {
                transferAcc.Balance += amount;
                this.Balance -= amount;
            }
        }
    }
}

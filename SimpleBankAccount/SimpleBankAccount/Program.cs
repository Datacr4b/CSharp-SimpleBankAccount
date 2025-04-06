using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
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
        static void Main(string[] args)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Create Account (c)");
                Console.WriteLine("Deposit into Account (d)");
                Console.WriteLine("Withdraw from Account (w)");
                Console.WriteLine("Transfer to Account (t)");
                Console.WriteLine("View the Accounts (v)");
                Console.WriteLine("Quit (q)");
                input = GetValidInput("Choose option: ");
                input = input.Trim().ToLower();
                if (input == "c" || input == "d" || input == "w" || input=="t")
                {
                    AccountOperations(input);
                }
                else if (input == "v")
                {
                    DisplayAccounts();
                }
                else if (input == "q")
                {
                    break;
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

            input = GetValidInput("Enter your name: ");
            accNumber = GetValidInput("Enter your account number: ");

            Predicate<Account> check = acc => acc.AccountNumber == accNumber && acc.Owner == input;
            Account existingAcc = accounts.Find(check);
            if (entry=="c") // Creating Account
            {
                string accountEntry = GetValidInput("Enter which account to create, Checking (c) or Savings (s): ");
                accountEntry = accountEntry.Trim().ToLower();
                if (existingAcc == null)
                {
                    if (accountEntry == "c" || accountEntry == "checking")
                    {
                        accounts.Add(new CheckingAccount(input, accNumber, 5m));
                        Console.WriteLine("Checking Account successfully created!");
                    }
                    else if (accountEntry == "s" || accountEntry == "savings")
                    {
                        accounts.Add(new SavingsAccount(input, accNumber, 5m));
                        Console.WriteLine("Savings Account successfully created!");
                    }
                    else
                    {
                        Console.WriteLine("Invalid Account type");
                    }
                }
                else
                    Console.WriteLine("Account already exists");
            }
            else if (entry=="d" || entry=="w" || entry=="t" ) // Deposit, Withdraw and Transfer
            {
                decimal amount;
                amount = GetValidDecimal("Enter your amount: ");
                Operations(entry, amount, existingAcc);
            }
            Console.ReadLine();
        }

        static void Operations(string entry, decimal amount, Account existingAcc)
        {
            if (existingAcc != null)
            {
                if (entry == "d")
                    existingAcc.Deposit(amount);
                else if (entry == "w")
                    existingAcc.Withdraw(amount);
                else if (entry == "t")
                {
                    input = GetValidInput("Enter the name of the account to Transfer to: ");
                    accNumber = GetValidInput("Enter the account number: ");
                    existingAcc.Transfer(amount, accNumber, input, accounts);
                    
                }
            }
            else
            {
                Console.WriteLine("This account doesn't exist");
            }
        }

        static void DisplayAccounts()
        {
            foreach(Account account in accounts)
            {
                Console.WriteLine($"Owner: {account.Owner}, Account Number: {account.AccountNumber}, Balance: {account.Balance}");
            }
            Console.ReadLine();
        }

        static string GetValidInput(string localisation)
        {
            string owner; 
            while (true) // Input Loop
            {
                Console.WriteLine(localisation);
                owner = Console.ReadLine();
                if (!string.IsNullOrEmpty(owner))
                {
                    return owner;
                }
                else
                {
                    Console.WriteLine("Invalid Input");
                    Console.ReadLine();
                }
            }
        }

        static decimal GetValidDecimal(string localisation)
        {
            decimal amount;
            string stringAmount;

            while (true) // Input Loop
            {
                Console.WriteLine(localisation);
                stringAmount = Console.ReadLine();
                if (Decimal.TryParse(stringAmount, out amount) && amount > 0)
                {
                    return amount;
                }
                else
                {
                    Console.WriteLine("Invalid Input");
                    Console.ReadLine();
                }
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
            protected set { balance = value; }
        }

        public Account(string owner, string accountNumber)
        {
            Owner = owner;
            AccountNumber = accountNumber;
            Balance = 0;
        }

        public virtual void Deposit(decimal amount)
        {
            Balance += amount;
            Console.WriteLine($"Deposited {amount:C}");
        }

        public virtual decimal Withdraw(decimal amount)
        {
            if (Balance >= amount)
            {
                Balance -= amount;
                Console.WriteLine($"Withdrew {amount:C}");
                return amount;
            }
            else
            {
                Console.WriteLine("Insufficient funds on your account");
                return 0;
            }
        }

        public virtual void Transfer(decimal amount, string accnumber, string owner, List<Account> accounts)
        {
            Predicate<Account> check = acc => acc.AccountNumber == accnumber && acc.Owner == owner;
            Account transferAcc = accounts.Find(check);
            if (transferAcc != null)
            {
                if (Balance >= amount)
                {
                    transferAcc.Balance += amount;
                    Balance -= amount;
                    Console.WriteLine("Successfully transfered the amount on the account");
                }
                else
                {
                    Console.WriteLine("Insufficient Balance on your account");
                }
            }
            else
            {
                Console.WriteLine("Account doesn't exist");
            }
        }
    }

    class SavingsAccount : Account
    {
        private decimal interestRate;
        public SavingsAccount(string owner, string accountNumber, decimal interestRate)  : base (owner, accountNumber) 
        { 
            this.interestRate = interestRate;
        }

        public void ApplyInterest()
        {
            decimal interest = Balance * interestRate;
            Balance += interest;
            Console.WriteLine($"Applied interest: {interest:C}");
        }


        public override decimal Withdraw(decimal amount)
        {
            if (Balance >= amount)
            {
                Balance -= amount + 5m;
                Console.WriteLine($"Withdrew {amount:C} and {5m:C} in fees.");
                return amount;
            }
            else
            {
                Console.WriteLine("Insufficient funds on your account");
                return 0;
            }
        }
    }

    class CheckingAccount : Account
    {
        private decimal overdraftFee;

        public CheckingAccount(string owner, string accountNumber, decimal overdraftFee) : base(owner, accountNumber)
        {
            this.overdraftFee = overdraftFee;
        }

        public override decimal Withdraw(decimal amount)
        {
            if (Balance < amount)
            {
                Balance -= amount + overdraftFee;
                Console.WriteLine($"Withdrew {amount:C} and {overdraftFee:C} in overdraft fees.");
                return amount;
            }
            else
            {
                Balance -= amount;
                Console.WriteLine($"Withdrew {amount:C}");
                return amount;
            }
        }
    }
}

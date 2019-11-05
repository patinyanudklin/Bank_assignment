using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandNewDay_Assignment
{
    enum MENU
    {
        CREATE_ACCOUNT = 1,
        DEPOSIT_MONEY = 2,
        TRANSFER_MONEY = 3,
        QUIT_PROGRAM = 777
    }
    enum STATUS_CODE
    {
        OK = 0,
        INSUFFICIENT_AMOUNT = 1,
        TRANSFER_FAIL = 2
    }

    class Constants
    {
        public const double feeCharge = 0.1;
    }
    class Program
    {
        static void Main(string[] args)
        {
            string iban = GetIban();
            DebugDB();
            return;
        }

        static string GetIban()
        {
            string webDestination = @"http:\\randomiban.com\?country=Netherlands";
            string xPath = "//*[@id='demo']";

            var chromeDriver = new ChromeDriver();

            chromeDriver.Navigate().GoToUrl(webDestination);
            var number = chromeDriver.FindElementByXPath(xPath);
            var output = number.Text;
            chromeDriver.Close();

            return output;
        }
        #region functions
        static int MainMenu()
        {
            int option = 0;
            string temp;
            while (true)
            {
                Console.WriteLine("What would you like to do?");
                Console.WriteLine("1. Create new account\n" +
                                  "2. Deposit money\n" +
                                  "3. Transfer money");
                Console.Write("Enter choice: ");
                temp = Console.ReadLine();
                if (!Int32.TryParse(temp, out option) || (option > 3 || option < 1))
                {
                    Console.WriteLine("Invalid choice");
                    // Delete this one out!
                    if (option == (int) MENU.QUIT_PROGRAM)
                    {
                        Console.WriteLine("Quit the program");
                        return option;
                    }
                    continue;
                }
                break;
            }
            return option;
        }

        static int CreateNewAcct()
        {
            Console.Write("Citizen Id: ");
            Console.ReadLine();

            // if new customer ask name and surname, else create the bank account

            Console.Write("Account name: ");
            Console.ReadLine();

            Console.Write("Initial Deposit: ");
            Console.ReadLine();
            return 0;
        }
        #endregion

        #region DB functions
        static int CreateNewCustomer(string citizenId)
        {
            Console.WriteLine("First Name:");
            string firstName = Console.ReadLine();
            Console.WriteLine("Last Name:");
            string lastName = Console.ReadLine();

            using (var bankcontext = new BankContext())
            {
                Customer cus = new Customer
                {
                    CitizenID = citizenId,
                    FirstName = firstName,
                    LastName = lastName,
                    Accounts = null
                };
                bankcontext.Customers.Add(cus);
                bankcontext.SaveChanges();
            }
            return 0;
        }
        static int CreateNewCustomer()
        {
            Console.WriteLine("Citizen ID:");
            string citizenId = Console.ReadLine();
            CreateNewCustomer(citizenId);
            return 0;
        }
        static int DepositMoney()
        {
            string iban;
            double amount;

            Console.Write("iban: ");
            iban = Console.ReadLine();

            // validation

            //
            Console.Write("Amount: ");
            amount = Double.Parse( Console.ReadLine() );
            DepositMoney(iban, amount);
            return 0;
        }

        static bool DepositValidation(string citizenID, string iban)
        {
            using(var context = new BankContext())
            {
                var acct = context.Customers.SingleOrDefault(c => c.CitizenID == citizenID)
                    .Accounts.SingleOrDefault(a => a.IBAN == iban);
                if (acct != null)
                    return true;
                else
                    return false;
            }
        }

        static int TransferMoney(string senderCitizenID, string iban1, string iban2, double amount)
        {
            using (var context = new BankContext())
            {
                var acct1 = context.Customers.Single(c => c.CitizenID == senderCitizenID)
                .Accounts.SingleOrDefault(a => a.IBAN == iban1);
                var acct2 = context.Accounts.Single(a => a.IBAN == iban2);

                if (acct1.Balance < amount) return (int) STATUS_CODE.INSUFFICIENT_AMOUNT;

                acct1.Balance -= amount;
                acct2.Balance += amount;
            }
            return (int) STATUS_CODE.OK;
        }

        static int DepositMoney( string iban, double amount)
        {
            Console.WriteLine("Received {0}, {1}% fee charged.", amount, Constants.feeCharge);
            double realDeposit = amount - amount * Constants.feeCharge / 100;
            Console.WriteLine("Deposit amount: {0}", realDeposit);
            using (var context = new BankContext())
            {
                var account = context.Accounts.SingleOrDefault(a => a.IBAN == iban);
                if(account != null)
                {
                    account.Balance += realDeposit;
                }
                context.SaveChanges();
            }
            return 0;
        }

        static void DebugDB()
        {
            using (var context = new BankContext())
            {
                var cusQuery = from cus in context.Customers
                               select cus;
                var allCustomers = cusQuery.ToList();
                var accountQuery = from account in context.Accounts
                                   select account;
                var allAccounts = accountQuery.ToList();
            }
        }
        #endregion


        public class Customer
        {
            public int ID { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string CitizenID { get; set; }
            public virtual ICollection<Account> Accounts { get; set; }
        }

        public class Account
        {
            public int ID { get; set; }
            public string IBAN { get; set; }
            public virtual Customer Owner { get; set; }
            public string AccountName { get; set; }
            public double Balance { get; set; }
        }

        public class BankContext : DbContext
        {
            public virtual DbSet<Customer> Customers { get; set; }
            public virtual DbSet<Account> Accounts { get; set; }

        }
    }
}

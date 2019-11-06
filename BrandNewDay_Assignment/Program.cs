using BrandNewDay_Assignment.Models;
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
        QUIT_PROGRAM = 777,
        DEBUG_DB = 888,
        TRUNCATE_DB = 999
    }
    enum STATUS_CODE
    {
        OK = 0,
        INSUFFICIENT_AMOUNT = 1,
        TRANSFER_FAIL = 2
    }

    class Constants
    {
        // 0.1%
        public const double feeCharge = 0.001;
    }
    class Program
    {
        static void Main(string[] args)
        {
            int choice = 0;

            do
            {
                choice = MainMenu();
                switch(choice)
                {
                    case (int)MENU.CREATE_ACCOUNT:
                        CreateNewAcct();
                        break;
                    case (int)MENU.DEPOSIT_MONEY:
                        DepositMoney();
                        break;
                    case (int)MENU.TRANSFER_MONEY:
                        TransferMoney();
                        break;
                    case (int)MENU.DEBUG_DB:
                        DebugDB();
                        break;
                    case (int)MENU.TRUNCATE_DB:
                        DeleteDB();
                        break;
                    case (int)MENU.QUIT_PROGRAM:
                        break;
                    default:
                        Console.WriteLine("Invalid choice");
                        break;
                }
            } while (choice != (int)MENU.QUIT_PROGRAM);

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
                if (!Int32.TryParse(temp, out option))
                {
                    Console.WriteLine("Invalid choice");
                    continue;
                }
                return option;
            }
        }

        static int CreateNewAcct()
        {
            string citizenID, acctName;
            double initAmount;
            Console.Write("Citizen Id: ");
            citizenID = Console.ReadLine();

            // if new customer ask name and surname, else create the bank account
            if (IsNewCustomer(citizenID))
                CreateNewCustomer(citizenID);

            Console.Write("Account name: ");
            acctName = Console.ReadLine();

            Console.Write("Initial Deposit: ");
            initAmount = Double.Parse(Console.ReadLine());

            CreateNewAcct(citizenID, acctName, initAmount);
            return 0;
        }
        #endregion

        #region DB functions
        static int CreateNewAcct(string citizenID, string acctName, double initAmount)
        {
            using (var context = new BankContext())
            {
                var cus = context.Customers.Single(c => c.CitizenID == citizenID);
                Account newAcct = new Account()
                {
                    IBAN = GetIban(),
                    Owner = cus,
                    AccountName = acctName,
                    Balance = initAmount
                };
                
                context.Accounts.Add(newAcct);
                context.SaveChanges();
                cus.Accounts.Add(newAcct);
                context.SaveChanges();
            }
            return (int)STATUS_CODE.OK;
        }
        static bool IsNewCustomer(string citizenId)
        {
            using (var context = new BankContext())
            {
                var cus = context.Customers.SingleOrDefault(c=>c.CitizenID == citizenId);
                if (cus != null)
                    return false;
                else
                    return true;
            }
        }
        static int CreateNewCustomer(string citizenId)
        {
            Console.Write("First Name:");
            string firstName = Console.ReadLine();
            Console.Write("Last Name:");
            string lastName = Console.ReadLine();

            using (var bankcontext = new BankContext())
            {
                Customer cus = new Customer
                {
                    CitizenID = citizenId,
                    FirstName = firstName,
                    LastName = lastName
                };
                bankcontext.Customers.Add(cus);
                bankcontext.SaveChanges();
            }
            return (int)STATUS_CODE.OK;
        }
        static int CreateNewCustomer()
        {
            string citizenId;
            Console.WriteLine("Citizen ID:");
            citizenId = Console.ReadLine();
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

        static int TransferMoney()
        {
            string senderCitizenID, iban1, iban2;
            double amount;
            Console.Write("Citizen ID: ");
            senderCitizenID = Console.ReadLine();
            Console.Write("Sender account iban: ");
            iban1 = Console.ReadLine();
            Console.Write("Receiver account iban: ");
            iban2 = Console.ReadLine();
            Console.Write("Amount: ");
            amount = Double.Parse(Console.ReadLine());
            TransferMoney(senderCitizenID, iban1, iban2, amount);

            return (int) STATUS_CODE.OK;
        }

        static int TransferMoney(string senderCitizenID, string iban1, string iban2, double amount)
        {
            using (var context = new BankContext())
            {
                var cus1 = context.Customers.Single(c => c.CitizenID == senderCitizenID);
                var senderAcct = context.Accounts.Single(a => a.IBAN == iban1);
                var receiverAcct = context.Accounts.Single(a => a.IBAN == iban2);

                if (senderAcct.Balance < amount) return (int) STATUS_CODE.INSUFFICIENT_AMOUNT;

                senderAcct.Balance -= amount;
                receiverAcct.Balance += amount;
                context.SaveChanges();
            }
            return (int) STATUS_CODE.OK;
        }

        static int DepositMoney( string iban, double amount)
        {
            Console.WriteLine("Received {0}, {1}% fee charged.", amount, Constants.feeCharge);
            double realDeposit = amount *(1 - Constants.feeCharge);
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
            return (int)STATUS_CODE.OK;
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
                foreach(Customer c in allCustomers)
                {
                    Console.WriteLine("CitizenID: {0}, {1} {2}", c.CitizenID, c.FirstName, c.LastName);
                    Console.WriteLine("----- Accounts -----");
                    if (c.Accounts!=null)
                        foreach (Account a in c.Accounts)
                            Console.WriteLine("iban: {0} {1}, Remain: {2}", a.IBAN, a.AccountName, a.Balance);
                    Console.WriteLine("--------------------");
                }
                foreach(Account a in allAccounts)
                {
                    Console.WriteLine("Owner: {0} iban: {1} {2}, Balance: {3}", a.Owner.CitizenID, a.IBAN, a.AccountName, a.Balance);
                }
            }
        }
        #endregion

        static void DeleteDB()
        {
            Console.Write("Pass: ");
            if (Console.ReadLine() != "confirm")
                return;
            using (var context = new BankContext())
            {
                context.Database.ExecuteSqlCommand("DELETE FROM Accounts");
                context.Database.ExecuteSqlCommand("DELETE FROM Customers");
            }
        }
    }
}

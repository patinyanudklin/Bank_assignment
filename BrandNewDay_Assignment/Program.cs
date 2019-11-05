using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandNewDay_Assignment
{
    class Program
    {
        static void Main(string[] args)
        {
        }

        static string getIban()
        {
            string webDestination = @"http:\\randomiban.com\?country=Netherlands";
            string xPath = "//*[@id='demo']";
            string chromeDriverAbsolutePath = @"C:\Users\patin\source\repos\core_learning\core_learning\bin\Debug\netcoreapp2.1";
            var chromeDriver = new ChromeDriver(chromeDriverAbsolutePath);

            chromeDriver.Navigate().GoToUrl(webDestination);
            var number = chromeDriver.FindElementByXPath(xPath);
            var output = number.Text;
            chromeDriver.Close();

            return output;
        }

        static int mainMenu()
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
                    if (option == 777)
                    {
                        Console.WriteLine("Shortcut");
                        break;
                    }
                    continue;
                }
                break;
            }
            return option;
        }

        static int createNewAcct()
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

        static int depositMoney()
        {
            Console.Write("iban: ");
            Console.WriteLine();

            // validation

            //
            return 0;
        }

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
            public virtual Customer Owner { get; set; }
            public string AccountName { get; set; }
            public long Balance { get; set; }
        }

        public class BankContext : DbContext
        {
            public virtual DbSet<Customer> Customers { get; set; }
            public virtual DbSet<Account> Accounts { get; set; }

        }
    }
}

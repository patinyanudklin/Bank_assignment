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
    public enum MENU
    {
        CREATE_ACCOUNT = 1,
        DEPOSIT_MONEY = 2,
        TRANSFER_MONEY = 3,
        QUIT_PROGRAM = 777,
        DEBUG_DB = 888,
        TRUNCATE_DB = 999
    }
    public enum STATUS_CODE
    {
        OK = 0,
        INSUFFICIENT_AMOUNT = 1,
        TRANSFER_FAIL = 2
    }

    class Program : IProgram
    {
        public IDataProcessor DB;
        public IConsoleReadLine MyConsole;

        public Program(IDataProcessor db, IConsoleReadLine console)
        {
            DB = db;
            MyConsole = console;
        }

        public static void Main(string[] args)
        {
            Program p = new Program( new DataProcessor(), new ConsoleReadLine());
            int choice = 0;
            do
            {
                choice = p.MainMenu();
                switch (choice)
                {
                    case (int)MENU.CREATE_ACCOUNT:
                        p.CreateNewAcct();
                        break;
                    case (int)MENU.DEPOSIT_MONEY:
                        p.DepositMoney();
                        break;
                    case (int)MENU.TRANSFER_MONEY:
                        p.TransferMoney();
                        break;
                    case (int)MENU.DEBUG_DB:
                        DBDebugger.DebugDB();
                        break;
                    case (int)MENU.TRUNCATE_DB:
                        DBDebugger.DeleteDB();
                        break;
                    case (int)MENU.QUIT_PROGRAM:
                        break;
                    default:
                        Console.WriteLine("Invalid choice");
                        break;
                }
            } while (choice != (int)MENU.QUIT_PROGRAM);
            return;
        }

        public int MainMenu()
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

        public void CreateNewAcct()
        {
            string citizenID, acctName;
            double initAmount;
            Console.Write("Citizen Id: ");
            citizenID = MyConsole.Input();//Console.ReadLine();

            // if new customer ask name and surname, else create the bank account
            if (DB.IsNewCustomer(citizenID))
                CreateNewCustomer(citizenID);

            Console.Write("Account name: ");
            acctName = MyConsole.Input(); //Console.ReadLine();

            Console.Write("Initial Deposit: ");
            initAmount = Double.Parse(MyConsole.Input()); // Console.ReadLine());

            DB.CreateNewAcct(citizenID, acctName, initAmount);
        }

        public void CreateNewCustomer()
        {
            string citizenId;
            Console.WriteLine("Citizen ID:");
            citizenId = Console.ReadLine();
            CreateNewCustomer(citizenId);
        }

        public void CreateNewCustomer(string citizenId)
        {
            Console.Write("First Name:");
            string firstName = Console.ReadLine();
            Console.Write("Last Name:");
            string lastName = Console.ReadLine();
            DB.CreateNewCustomer(citizenId, firstName, lastName);
        }

        public void DepositMoney()
        {
            string iban;
            double amount;

            Console.Write("iban: ");
            iban = Console.ReadLine();

            // validation
            if (!DB.DepositValidation(iban))
            {
                Console.WriteLine("This account does not exist!");
                return;
            }
            //
            Console.Write("Amount: ");
            amount = Double.Parse(Console.ReadLine());
            DB.DepositMoney(iban, amount);
        }

        public void TransferMoney()
        {
            string senderCitizenID, iban1, iban2;
            bool validated = false;
            double amount;
            do
            {
                Console.Write("Citizen ID: ");
                senderCitizenID = Console.ReadLine();
                Console.Write("Sender account iban: ");
                iban1 = Console.ReadLine();
                validated = DB.IsSenderOwnThisAcct(senderCitizenID, iban1);
            }
            while (!validated) ;
            Console.Write("Receiver account iban: ");
            iban2 = Console.ReadLine();
            Console.Write("Amount: ");
            amount = Double.Parse(Console.ReadLine());
            if(DB.TransferMoney(senderCitizenID, iban1, iban2, amount) == (int)STATUS_CODE.INSUFFICIENT_AMOUNT)
            {
                Console.WriteLine("Transfer not success!");
                Console.WriteLine("Insufficient amount!");
            }
        }
    }

    public interface IProgram
    {
        void CreateNewAcct();
        void CreateNewCustomer();
        void CreateNewCustomer(string citizenId);
        void DepositMoney();
        void TransferMoney();
    }

    public interface IConsoleReadLine
    {
        string Input();
    }

    public class ConsoleReadLine: IConsoleReadLine
    {
        public string Input() { return Console.ReadLine(); }
    }
}

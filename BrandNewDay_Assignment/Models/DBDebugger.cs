using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandNewDay_Assignment.Models
{
    class DBDebugger
    {
        public static void DebugDB()
        {
            using (var context = new BankContext())
            {
                var cusQuery = from cus in context.Customers
                                select cus;
                var allCustomers = cusQuery.ToList();
                var accountQuery = from account in context.Accounts
                                    select account;
                var allAccounts = accountQuery.ToList();
                foreach (Customer c in allCustomers)
                {
                    Console.WriteLine("CitizenID: {0}, {1} {2}", c.CitizenID, c.FirstName, c.LastName);
                    Console.WriteLine("----- Accounts -----");
                    if (c.Accounts != null)
                        foreach (Account a in c.Accounts)
                            Console.WriteLine("iban: {0} {1}, Remain: {2}", a.IBAN, a.AccountName, a.Balance);
                    Console.WriteLine("--------------------\n");
                }
                foreach (Account a in allAccounts)
                {
                    Console.WriteLine("Owner: {0} iban: {1} {2}, Balance: {3}", a.Owner.CitizenID, a.IBAN, a.AccountName, a.Balance);
                }
            }
        }

        public static void DeleteDB()
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

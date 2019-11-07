using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandNewDay_Assignment.Models
{
    class DataProcessor : IDataProcessor
    {

        public bool IsNewCustomer(string citizenId)
        {
            using (var context = new BankContext())
            {
                var cus = context.Customers.SingleOrDefault(c => c.CitizenID == citizenId);
                if (cus != null)
                    return false;
                else
                    return true;
            }
        }

        public bool DepositValidation(string iban)
        {
            using(var context = new BankContext())
            {
                var acct = context.Accounts.SingleOrDefault(a => a.IBAN == iban);
                if (acct != null)
                    return true;
                else
                    return false;
            }
        }

        public void CreateNewCustomer(string citizenId, string firstName, string lastName)
        {
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
        }

        public void CreateNewAcct(string citizenID, string acctName, double initAmount)
        {
            IIbanRetriver ibanR = new IbanRetriever();
            using (var context = new BankContext())
            {
                var cus = context.Customers.Single(c => c.CitizenID == citizenID);
                Account newAcct = new Account()
                {
                    IBAN = ibanR.GetIban(),
                    Owner = cus,
                    AccountName = acctName,
                    Balance = initAmount
                };

                context.Accounts.Add(newAcct);
                cus.Accounts.Add(newAcct);
                context.SaveChanges();
            }
        }

        public bool IsSenderOwnThisAcct(string senderCitizenID, string iban)
        {
            using (var context = new BankContext())
            {
                var cus1 = context.Customers.Single(c => c.CitizenID == senderCitizenID);
                var senderAcct = context.Accounts.Single(a => a.IBAN == iban);

                var acct1 = cus1.Accounts.SingleOrDefault(a => a.IBAN == iban);
                if (acct1 == null)
                    return false;
                else
                    return true;
            }
        }
        public int TransferMoney(string senderCitizenID, string iban1, string iban2, double amount)
        {
            using (var context = new BankContext())
            {
                var cus1 = context.Customers.Single(c => c.CitizenID == senderCitizenID);
                var senderAcct = context.Accounts.Single(a => a.IBAN == iban1);
                var receiverAcct = context.Accounts.Single(a => a.IBAN == iban2);

                if (senderAcct.Balance < amount) return (int)STATUS_CODE.INSUFFICIENT_AMOUNT;

                senderAcct.Balance -= amount;
                receiverAcct.Balance += amount;
                context.SaveChanges();
            }
            return (int)STATUS_CODE.OK;
        }

        public void DepositMoney(string iban, double amount)
        {
            Console.WriteLine("Received {0}, {1}% fee charged.", amount, Constants.feeCharge);
            double realDeposit = amount * (1 - Constants.feeCharge);
            Console.WriteLine("Deposit amount: {0}", realDeposit);
            using (var context = new BankContext())
            {
                var account = context.Accounts.SingleOrDefault(a => a.IBAN == iban);
                if (account != null)
                {
                    account.Balance += realDeposit;
                }
                context.SaveChanges();
            }
        }
    }

    public interface IDataProcessor
    {
        bool IsNewCustomer(string citizenId);
        bool DepositValidation(string iban);
        void CreateNewCustomer(string citizenId, string firstName, string lastName);
        void CreateNewAcct(string citizenID, string acctName, double initAmount);
        bool IsSenderOwnThisAcct(string senderCitizenID, string iban);
        int TransferMoney(string senderCitizenID, string iban1, string iban2, double amount);
        void DepositMoney(string iban, double amount);


    }

}

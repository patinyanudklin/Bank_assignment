using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandNewDay_Assignment.Models
{
    public class Customer
    {
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CitizenID { get; set; }
        public virtual ICollection<Account> Accounts { get; set; }

        public Customer() { Accounts = new List<Account>(); }
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

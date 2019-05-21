using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Contracts.Models
{
    public class BankAccount
    {
        public Guid BankAccountId { get; set; }
        public decimal Balance { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Text;
using VirtualCashCard.Interface;

namespace VirtualCashCard.Model
{
    public class Customer : ICustomer
    {
        public string Name { get; set; }
        public long CustomerId { get; set; }
        public List<IAccount> Accounts { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace VirtualCashCard.Interface
{
    public interface ICustomer
    {
        string Name { get; set; }
        long CustomerId { get; set; }
        List<IAccount> Accounts { get; set; }
    }
}

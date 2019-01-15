using System;
using System.Collections.Generic;
using System.Text;
using VirtualCashCard.Model;

namespace VirtualCashCard.Interface
{
    public interface ICardService
    {
        void Initialize();
        bool IsCurrentUserLoggedIn();
        bool VerifyAccount(long accountId, int pin);
        (long Account, double Balance) GetAccountBalance();
        bool WithdrawCashFromAccount(double amount);
        void DepositCashToAccount(double amount);
        bool ValidatePin(int pin);
    }
}

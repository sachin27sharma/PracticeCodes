using System.Linq;
using VirtualCashCard.Interface;
using VirtualCashCard.Model;

namespace VirtualCashCard
{
    public class CardService: ICardService
    {
        private ICustomerAccountRepository _customerAccountRepository;
        private VirtualBank _virtualBank = null;
        private IAccount _loggedInAccount = null;

        public CardService(ICustomerAccountRepository customerAccountRepository)
        {
            _customerAccountRepository = customerAccountRepository;
        }

        public bool IsCurrentUserLoggedIn()
        {
            return _loggedInAccount != null;
        }

        public bool VerifyAccount(long accountId, int pin)
        {
            IAccount account = null;

            var customer = _virtualBank.Customers.Where(c => IsCustomerAccount(c, accountId, out account)).FirstOrDefault();

            if (account == null || !account.ValidatePin(pin))
                return false;
    
            _loggedInAccount = account;
            return true;
        }

        private bool IsCustomerAccount(Customer customer, long accountId, out IAccount account)
        {
            account = null;
            IAccount userAccount = customer.Accounts.Where(acc => acc.Id.Equals(accountId)).FirstOrDefault();
            if (userAccount == null)
                return false;
            account = userAccount;
            return true;
        }

        public bool ValidatePin(int pin)
        {
            return _loggedInAccount.AccountPin.Equals(pin);
        }

        public void Initialize()
        {
            _virtualBank  = _customerAccountRepository.Load().Result;
        }

        public (long Account, double Balance) GetAccountBalance()
        {
            return (Account: _loggedInAccount.Id, Balance: _loggedInAccount.AvailableBalance);
        }
        
        public bool WithdrawCashFromAccount(double amount)
        {
            if (_loggedInAccount.AvailableBalance <= 0)
                return false;
            _loggedInAccount.Debit(amount);
            _customerAccountRepository.UpdateAccountBalance(_virtualBank);
            return true;
        }

        public void DepositCashToAccount(double amount)
        {
            _loggedInAccount.Credit(amount);
            _customerAccountRepository.UpdateAccountBalance(_virtualBank);
        }
   }
}

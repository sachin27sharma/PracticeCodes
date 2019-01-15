using VirtualCashCard.Interface;

namespace VirtualCashCard.Model
{
    public class Account : IAccount
    {
        public long Id { get; set; }
        public long CustomerId { get; set; }
        public int AccountPin { get; set; }
        public double AvailableBalance { get; set; }

        public bool ValidatePin(int pin)
        {
            return AccountPin.Equals(pin);
        }

        public void Credit(double amount)
        {
            AvailableBalance += amount;
        }

        public void Debit(double amount)
        {
            if (amount < 1) return;

            if ((AvailableBalance - amount) <= 0)
            {
                AvailableBalance = 0;
                return;
            }

            AvailableBalance -= amount;
        }
    }
}

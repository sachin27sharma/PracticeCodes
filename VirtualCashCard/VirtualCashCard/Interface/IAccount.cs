namespace VirtualCashCard.Interface
{
    public interface IAccount
    {
        long Id { get; set; }
        long CustomerId { get; set; }
        int AccountPin { get; set; }
        double AvailableBalance { get; set; }
        bool ValidatePin(int pin);
        void Credit(double amount);
        void Debit(double amount);

    }
}
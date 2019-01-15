using System.Threading.Tasks;
using VirtualCashCard.Model;

namespace VirtualCashCard.Interface
{
    public interface ICustomerAccountRepository
    {
        Task<VirtualBank> Load();
        bool UpdateAccountBalance(VirtualBank virtualBank);
    }
}

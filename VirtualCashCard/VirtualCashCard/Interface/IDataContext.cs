using System.Threading.Tasks;

namespace VirtualCashCard.Interface
{
    public interface IDataContext
    {
        Task<string> ProcessRead(string fileName);
        void ProcessWrite(string content, string filePath);
    }
}

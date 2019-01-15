using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Threading.Tasks;
using VirtualCashCard.Interface;
using VirtualCashCard.Model;

namespace VirtualCashCard.Core
{
    public class CustomerAccountRepository : ICustomerAccountRepository
    {
        private IDataContext _dataContext = null;
        private string _fileName = string.Empty;

        public CustomerAccountRepository(IConfigurationRoot configuration, IDataContext dataContext)
        {
            _fileName = configuration["Settings:AccountFileName"];
            _dataContext = dataContext;
        }

        public async Task<VirtualBank> Load()
        {
            var content =  await _dataContext.ProcessRead(_fileName);
            VirtualBank virtualBank = JsonConvert.DeserializeObject<VirtualBank>(content, new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
            return virtualBank;
        }

        public bool UpdateAccountBalance(VirtualBank vb)
        {
            string content = JsonConvert.SerializeObject(vb, new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Auto
            });

            _dataContext.ProcessWrite(content, _fileName);
            return true;
        }
    }
}

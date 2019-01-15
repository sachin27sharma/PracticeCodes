using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using System.Threading.Tasks;
using VirtualCashCard.Core;
using VirtualCashCard.Interface;
using VirtualCashCard.Model;

namespace VirtualCashCard.Tests.Core
{
    public class CustomerAccountRepositoryTest
    {
        [Test]
        public void When_RepositoryInitialized_ReturnJsonString()
        {
            Mock<IDataContext> mockDataContext = new Mock<IDataContext>();
            mockDataContext.Setup(d => d.ProcessRead(It.IsAny<string>()))
                .Returns(Task.FromResult(GetData()));


            Mock<IConfigurationRoot> mockConfiguration = new Mock<IConfigurationRoot>();
            mockConfiguration.SetupGet(x => x[It.IsAny<string>()]).Returns("Account.json");

            ICustomerAccountRepository customerAccountRepository = new CustomerAccountRepository(mockConfiguration.Object, mockDataContext.Object);
            var contentTask = customerAccountRepository.Load();
            var virtualBank = contentTask.Result;

            Assert.IsNotNull(virtualBank);
            Assert.AreEqual(virtualBank.Customers.Count, 2);
            mockDataContext.Verify(d => d.ProcessRead(It.IsAny<string>()));
        }

        [Test]
        public void When_AccountBalanceUpdated_ReturnUpdateAccount()
        {
            Mock<IDataContext> mockDataContext = new Mock<IDataContext>();
            mockDataContext.Setup(d => d.ProcessRead(It.IsAny<string>()))
                .Returns(Task.FromResult(GetData()));

            VirtualBank virtualBank = null;
            bool isProcessWriteCalled = false;

            mockDataContext.Setup(d => d.ProcessWrite(It.IsAny<string>(), It.IsAny<string>()))
                .Callback(()=> { isProcessWriteCalled = true;
                    virtualBank.Customers[0].Accounts[0].AvailableBalance = 50;
                                });
                
           
            Mock<IConfigurationRoot> mockConfiguration = new Mock<IConfigurationRoot>();
            mockConfiguration.SetupGet(x => x[It.IsAny<string>()]).Returns("Account.json");

            ICustomerAccountRepository customerAccountRepository = new CustomerAccountRepository(mockConfiguration.Object, mockDataContext.Object);
            var contentTask = customerAccountRepository.Load();
            virtualBank = contentTask.Result;
            Assert.IsNotNull(virtualBank);

            customerAccountRepository.UpdateAccountBalance(virtualBank);

            Assert.AreEqual(isProcessWriteCalled, true);
            Assert.AreEqual(virtualBank.Customers[0].Accounts[0].AvailableBalance, 50);
            mockDataContext.Verify(d => d.ProcessWrite(It.IsAny<string>(), It.IsAny<string>()));
        }

        private string GetData()
        {
            string content = @"{
                      ""Customers"": [
                        {
                          ""Name"": ""Richard"",
                          ""CustomerId"": 10001,
                          ""Accounts"": [
                            {
                              ""$type"": ""VirtualCashCard.Model.Account, VirtualCashCard"",
                              ""Id"": 120001,
                              ""CustomerId"": 10001,
                              ""AccountPin"": 1121,
                              ""AvailableBalance"": 100.0
                            }
                          ]
                        },
                        {
                          ""Name"": ""Mark"",
                          ""CustomerId"": 10002,
                          ""Accounts"": [
                            {
                              ""$type"": ""VirtualCashCard.Model.Account, VirtualCashCard"",
                              ""Id"": 120002,
                              ""CustomerId"": 10002,
                              ""AccountPin"": 1115,
                              ""AvailableBalance"": 100.0
                            }
                          ]
                        }
                      ]
                    }";

            return content;
        }

    }
}

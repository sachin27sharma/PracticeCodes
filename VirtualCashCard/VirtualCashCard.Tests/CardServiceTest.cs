using NUnit.Framework;
using Moq;
using VirtualCashCard.Interface;
using System.Threading.Tasks;
using VirtualCashCard.Core;
using Microsoft.Extensions.Configuration;
using VirtualCashCard.Model;
using VirtualCashCard;

namespace Tests
{
    public class CardServiceTest
    {
        Mock<ICustomerAccountRepository> _mockCustomerAccountRepository = null;
            
        [SetUp]
        public void Initialise()
        {
            _mockCustomerAccountRepository = new Mock<ICustomerAccountRepository>();
            _mockCustomerAccountRepository.Setup(d => d.Load())
                .Returns(GetRepositoryData());
        }

        [Test]
        public void When_Initialise_ReturnsValidVirtualBankInstance()
        {
            CardService cardService = new CardService(_mockCustomerAccountRepository.Object);
            VirtualBank virtualBank = null;
            _mockCustomerAccountRepository.Setup(d => d.Load())
                                          .Returns(() =>
                                          {
                                              var result = GetRepositoryData();
                                              virtualBank = result.Result;
                                              return result;
                                          });

            cardService.Initialize();

            _mockCustomerAccountRepository.Verify(repo => repo.Load());
            Assert.IsNotNull(virtualBank);
        }

        [Test]
        public void When_ValidAccountIdAndPinEntered_ReturnTrue()
        {
            CardService cardService = new CardService(_mockCustomerAccountRepository.Object);
            cardService.Initialize();

            bool isValid = cardService.VerifyAccount(120001, 1121);

            Assert.AreEqual(isValid, true);
        }

        [Test]
        public void When_InValidAccountIdOrPinEntered_ReturnFalse()
        {
            CardService cardService = new CardService(_mockCustomerAccountRepository.Object);
            cardService.Initialize();

            bool isValid = cardService.VerifyAccount(120001, 333333);
            Assert.AreEqual(isValid, false);

            isValid = cardService.VerifyAccount(33434343, 1121);
            Assert.AreEqual(isValid, false);
        }

        [Test]
        public void When_WithdrawValidCashAmount_ReturnAvailableBalances()
        {
            CardService cardService = new CardService(_mockCustomerAccountRepository.Object);
            cardService.Initialize();
            cardService.VerifyAccount(120002, 1115);

            bool isValid = cardService.WithdrawCashFromAccount(60);
            Assert.AreEqual(isValid, true);

            (long Account, double Balance) accountBalance = cardService.GetAccountBalance();
            Assert.AreEqual(accountBalance.Account, 120002);
            Assert.AreEqual(accountBalance.Balance, 40);

            isValid = cardService.WithdrawCashFromAccount(40);
            Assert.AreEqual(isValid, true);
            (long Account, double Balance) newAccountBalance = cardService.GetAccountBalance();
            Assert.AreEqual(newAccountBalance.Account, 120002);
            Assert.AreEqual(newAccountBalance.Balance, 0);
        }

        [Test]
        public void When_DepositValidCashAmount_ReturnAvailableBalances()
        {
            CardService cardService = new CardService(_mockCustomerAccountRepository.Object);
            cardService.Initialize();
            cardService.VerifyAccount(120002, 1115);

            cardService.DepositCashToAccount(150);
            
            (long Account, double Balance) accountBalance = cardService.GetAccountBalance();
            Assert.AreEqual(accountBalance.Account, 120002);
            Assert.AreEqual(accountBalance.Balance, 250);
        }

        [Test]
        public void When_VerifyPinForLoggedInAccount_ReturnTrue()
        {
            CardService cardService = new CardService(_mockCustomerAccountRepository.Object);
            cardService.Initialize();
            cardService.VerifyAccount(120002, 1115);

            bool isValid = cardService.ValidatePin(1115);
            Assert.AreEqual(isValid, true);
        }

        [Test]
        public void When_VerifyInvalidPinForLoggedInAccount_ReturnFalse()
        {
            CardService cardService = new CardService(_mockCustomerAccountRepository.Object);
            cardService.Initialize();
            cardService.VerifyAccount(120002, 1115);

            bool isValid = cardService.ValidatePin(66666);
            Assert.AreEqual(isValid, false);
        }

        private Task<VirtualBank> GetRepositoryData()
        {
            Mock<IDataContext> dataContext = new Mock<IDataContext>();
            dataContext.Setup(d => d.ProcessRead(It.IsAny<string>()))
                .Returns(Task.FromResult(GetData()));

            Mock<IConfigurationRoot> configuration = new Mock<IConfigurationRoot>();
            ICustomerAccountRepository customerAccountRepository = new CustomerAccountRepository(configuration.Object, dataContext.Object);
            return customerAccountRepository.Load();
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
using System;
using System.Collections.Generic;
using System.Configuration;
using FizzWare.NBuilder;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mozu.Api.Contracts.AppDev;
using Mozu.Api.Contracts.Customer;
using Mozu.Api.Resources.Commerce.Customer;
using Mozu.Api.Security;
using Mozu.Api.ToolKit.Handlers;
using Mozu.Api.ToolKit.Models;

namespace Mozu.Api.ToolKit.Test
{
    [TestClass]
    public class CustomerSegmentTest : BaseTest
    {
        private const string Password = "p@$$w0rd1";
        private ICustomerSegmentHandler _customerSegmentHandler;
        private ICustomerHandler _customerHandler;

        [TestInitialize]
        public void Initialize()
        {
            var tenantId = Convert.ToInt16(ConfigurationManager.AppSettings["TenantId"]);
            var applicationId = ConfigurationManager.AppSettings["ApplicationId"];
            var sharedSecret = ConfigurationManager.AppSettings["SharedSecret"];
            var masterCatalogId = Convert.ToInt16(ConfigurationManager.AppSettings["MasterCatalogId"]);
            var catalogId = Convert.ToInt16(ConfigurationManager.AppSettings["CatalogId"]);
            var siteId = Convert.ToInt16(ConfigurationManager.AppSettings["SiteId"]);            

            var appAuthInfo = new AppAuthInfo
            {
                ApplicationId = applicationId,
                SharedSecret = sharedSecret
            };

            var refreshInterval = new RefreshInterval(10000, 36000);

            var appAuthenticator = AppAuthenticator.Initialize(appAuthInfo, refreshInterval);

            var appAuthClaim = appAuthenticator.AppAuthTicket.AccessToken;

            var apiContext = new ApiContext { TenantId = tenantId, SiteId = siteId, CatalogId = catalogId, MasterCatalogId = masterCatalogId, AppAuthClaim = appAuthClaim };

            var customerSegmentResource = new CustomerSegmentResource(apiContext);

            _customerSegmentHandler = new CustomerSegmentHandler(customerSegmentResource);

            var customerAccountResource = new CustomerAccountResource(apiContext);

            _customerHandler = new CustomerHandler(customerAccountResource);
        }

        [TestMethod]
        public void GetSegmentsTest()
        {
            var customerSegmentCollection = _customerSegmentHandler.GetCustomerSegments().Result;
            Assert.IsNotNull(customerSegmentCollection.Items);
            Assert.IsTrue(customerSegmentCollection.Items.Count > 0);
        }

        [TestMethod]
        public void AddSegmentTest()
        {            
            // Add customer segment.            
            var randomCustomerSegment = CreateRandomSegment();
            var customerSegment = _customerSegmentHandler.AddCustomerSegment(randomCustomerSegment).Result;

            Assert.IsNotNull(customerSegment);
            Assert.IsTrue(customerSegment.Id > 0);            
            Assert.AreEqual(randomCustomerSegment.Code, customerSegment.Code);
            Assert.AreEqual(randomCustomerSegment.Name, customerSegment.Name);
            Assert.AreEqual(randomCustomerSegment.Description, customerSegment.Description);
        }

        [TestMethod]
        public void AddAndUpdateSegmentTest()
        {
            // Add customer segment.            
            var randomCustomerSegment = CreateRandomSegment();
            var customerSegment = _customerSegmentHandler.AddCustomerSegment(randomCustomerSegment).Result;

            Assert.IsNotNull(customerSegment);
            Assert.IsTrue(customerSegment.Id > 0);
            
            Assert.AreEqual(randomCustomerSegment.Code, customerSegment.Code);
            Assert.AreEqual(randomCustomerSegment.Name, customerSegment.Name);
            Assert.AreEqual(randomCustomerSegment.Description, customerSegment.Description);

            var dateTime = DateTime.Now;

            var newCustomerSegmentDescription = string.Format("I have changed on '{0}_{1}_{2}_{3}_{4}_{5}_{6}'",
                dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second,
                dateTime.Millisecond);

            customerSegment.Description = newCustomerSegmentDescription;

            var updatedCustomerSegment = _customerSegmentHandler.UpdateCustomerSegment(customerSegment, customerSegment.Id).Result;

            Assert.IsNotNull(updatedCustomerSegment);
            Assert.IsTrue(updatedCustomerSegment.Id == customerSegment.Id);
            Assert.AreEqual(updatedCustomerSegment.Code, customerSegment.Code);
            Assert.AreEqual(updatedCustomerSegment.Name, customerSegment.Name);
            Assert.AreEqual(updatedCustomerSegment.Description, newCustomerSegmentDescription);
        }

        [TestMethod]
        public void AddCustomerAccountToSegment()
        {
            // Add customer segment.            
            var randomCustomerSegment = CreateRandomSegment();
            var customerSegment = _customerSegmentHandler.AddCustomerSegment(randomCustomerSegment).Result;

            Assert.IsNotNull(customerSegment);
            Assert.IsTrue(customerSegment.Id > 0);

            var customerAccounts = CreateRandomCustomerList(1);

            // Add Customer(s).
            var customerAccountCollection = _customerHandler.AddCustomerAccount(customerAccounts).Result;

            Assert.IsNotNull(customerAccountCollection);
            Assert.IsTrue(customerAccountCollection.TotalCount == 1);

            var customerAccountToAdd = customerAccountCollection.Items[0];

            var accountIdList = new List<int> {customerAccountToAdd.Id};

            _customerSegmentHandler.AddSegmentAccounts(accountIdList, customerSegment.Id);
        }

        private List<CustomerAccountAndAuthInfo> CreateRandomCustomerList(int size)
        {
            var customerAccountInfoList = new List<CustomerAccountAndAuthInfo>();
            var customerAccounts = CreateCustomerAccounts(size);

            foreach (var customerAccount in customerAccounts)
            {
                var customerAccountAndAuthInfo = new CustomerAccountAndAuthInfo
                {
                    Account = customerAccount,
                    Password = Password
                };

                customerAccountInfoList.Add(customerAccountAndAuthInfo);
            }

            return customerAccountInfoList;
        }

        private static List<CustomerAccount> CreateCustomerAccounts(int size)
        {
            var customers = Builder<Customer>
                .CreateListOfSize(size)
                .All()
                .With(c => c.FirstName = Faker.Name.First())
                .With(c => c.LastName = Faker.Name.Last())
                .With(c => c.Email = Faker.Internet.FreeEmail())
                .With(c => c.Password = Password)
                .With(c => c.UserName = string.Format(Faker.Lorem.GetFirstWord() + "_" + GetRandomNumber()))
                .Build();

            var customerAccounts = new List<CustomerAccount>();

            foreach (var customer in customers)
            {
                var customerAccount = new CustomerAccount
                {
                    FirstName = customer.FirstName,
                    LastName = customer.LastName,
                    EmailAddress = customer.Email,
                    UserName = customer.UserName
                };

                customerAccounts.Add(customerAccount);
            }

            return customerAccounts;

        }

        private CustomerSegment CreateRandomSegment()
        {
            int randomSegmentNumber;
            var rand = GetRandom(out randomSegmentNumber);

            var randomCharacter = GetRandomCharacter(rand);

            var suffix = randomCharacter + randomSegmentNumber;

            var customerSegment = new CustomerSegment
            {
                Code = string.Format("Code_{0}", suffix),
                Name = string.Format("Name _{0}", suffix),
                Description = string.Format("Some description for suffix {0}", suffix)
            };

            return customerSegment;
        }

        private static int GetRandomNumber()
        {
            var rand = new Random((int)DateTime.Now.Ticks);
            var randomSegmentNumber = rand.Next(100000, 999999);
            return randomSegmentNumber;            
        }

        private static Random GetRandom(out int randomSegmentNumber)
        {
            var rand = new Random((int) DateTime.Now.Ticks);
            randomSegmentNumber = rand.Next(100000, 999999);
            return rand;
        }

        private static char GetRandomCharacter(Random rand)
        {
            var charCode = rand.Next(Convert.ToInt32('a'), Convert.ToInt32('z'));
            var ranfomSegmentCharacter = Convert.ToChar(charCode);
            return ranfomSegmentCharacter;
        }

        [TestCleanup]
        public void Cleanup()
        {

        }
    }
}

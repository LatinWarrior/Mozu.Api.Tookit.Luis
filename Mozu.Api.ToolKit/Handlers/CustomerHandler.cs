using System.Collections.Generic;
using System.Threading.Tasks;
using Mozu.Api.Contracts.Customer;
using Mozu.Api.Resources.Commerce.Customer;

namespace Mozu.Api.ToolKit.Handlers
{
    public interface ICustomerHandler
    {
        Task<CustomerAccountCollection> AddCustomerAccount(List<CustomerAccountAndAuthInfo> customers);
    }

    public class CustomerHandler : ICustomerHandler
    {
        private readonly CustomerAccountResource _customerAccountResource;

        public CustomerHandler(CustomerAccountResource customerAccountResource)
        {
            _customerAccountResource = customerAccountResource;
        }

        public Task<CustomerAccountCollection> AddCustomerAccount(List<CustomerAccountAndAuthInfo> customers)
        {
            var newCustomers = _customerAccountResource.AddAccountsAsync(customers);
            return newCustomers;
        }
    }
}

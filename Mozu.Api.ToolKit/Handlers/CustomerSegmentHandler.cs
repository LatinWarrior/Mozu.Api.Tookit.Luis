using System.Collections.Generic;
using System.Threading.Tasks;
using Mozu.Api.Contracts.Customer;
using Mozu.Api.Resources.Commerce.Customer;

namespace Mozu.Api.ToolKit.Handlers
{
    public interface ICustomerSegmentHandler
    {
        Task<CustomerSegment> AddCustomerSegment(CustomerSegment customerSegment);
        void DeleteCustomerSegment(int customerSegmentId);
        Task<CustomerSegment> GetCustomerSegment(int customerSegmentId);
        Task<CustomerSegmentCollection> GetCustomerSegments();
        Task<CustomerSegment> UpdateCustomerSegment(CustomerSegment customerSegment, int customerSegmentId);
        void RemoveSegmentAccount(int segmentId, int accountId);
        void AddSegmentAccounts(List< int> accountIds, int segmentId);
    }

    public class CustomerSegmentHandler : ICustomerSegmentHandler 
    {        
        private readonly CustomerSegmentResource _customerSegmentResource;        

        public CustomerSegmentHandler(CustomerSegmentResource customerSegmentResource)
        {            
            _customerSegmentResource = customerSegmentResource;                      
        }

        public async void DeleteCustomerSegment(int customerSegmentId)
       {           
            await _customerSegmentResource.DeleteSegmentAsync(customerSegmentId);
        }

        public async Task<CustomerSegment> AddCustomerSegment(CustomerSegment customerSegment)
        {
            var result = await _customerSegmentResource.AddSegmentAsync(customerSegment);
            return result;
        }

        public async Task<CustomerSegment> GetCustomerSegment(int customerSegmentId)
        {
            var result = await _customerSegmentResource.GetSegmentAsync(customerSegmentId);
            return result;
        }

        public async Task<CustomerSegmentCollection> GetCustomerSegments()
        {
            var result = await _customerSegmentResource.GetSegmentsAsync();
            return result;
        }

        public async Task<CustomerSegment> UpdateCustomerSegment(CustomerSegment customerSegment, int customerSegmentId)
        {
            var result = await _customerSegmentResource.UpdateSegmentAsync(customerSegment, customerSegmentId);
            return result;
        }

        public async void RemoveSegmentAccount(int segmentId, int accountId)
        {
            await _customerSegmentResource.RemoveSegmentAccountAsync(segmentId, accountId);
        }

        public async void AddSegmentAccounts(List<int> accountIds, int segmentId)
        {
            await _customerSegmentResource.AddSegmentAccountsAsync(accountIds, segmentId);
        }
    }
}

using SalesWebAPI.Models;

namespace SalesWebAPI.Interfaces
{
    public interface ICustomerRepository
    {
        IEnumerable<Customer> GetCustomers();
        List<Customer> GetCustomers(string firstName, string lastName);
        Customer Add(Customer newItem);
        Customer GetById(int id);
        void Remove(int id);
    }
}

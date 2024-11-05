using SalesWebAPI.Interfaces;
using SalesWebAPI.Models;

namespace SalesWebAPI.Data
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly List<Customer> _customers;
       
        public CustomerRepository()
        {
            _customers = new List<Customer>()
            {
                new Customer
                {
                     CustomerId = 1,
                     Name = "John Robin"
                },
                new  Customer
                {
                     CustomerId = 2,
                    Name = "Jane Smith"
                },

                new  Customer
                {
                     CustomerId = 3,
                    Name = "Hanna Nielsen"
                },
                new  Customer
                {
                     CustomerId = 4,
                    Name = "Silan Hansen"
                }
            };

        }
        public IEnumerable<Customer> GetCustomers() => _customers;


        public List<Customer> GetCustomers(string firstName, string lastName)
        {
            throw new NotImplementedException();
        }

        public Customer Add(Customer newCustomer)
        {
            _customers.Add(newCustomer);
            return newCustomer;
        }

        public Customer GetById(int id)
        {
            if (id > 0)
                return _customers.Where(a => a.CustomerId == id).FirstOrDefault();
            else return null;
        }

        public void Remove(int id)
        {
            var existing = _customers.First(a => a.CustomerId == id);
            if(existing != null)
            {
                _customers.Remove(existing);
            }         
        }
    }
}

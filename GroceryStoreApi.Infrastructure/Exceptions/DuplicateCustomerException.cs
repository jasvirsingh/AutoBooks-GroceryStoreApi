using System;

namespace GroceryStoreApi.Infrastructure.Exceptions
{
    public class DuplicateCustomerException : System.Exception
    {
        public DuplicateCustomerException()
        {
        }

        public DuplicateCustomerException(string message): base(message)
        { }
    }
}

namespace GroceryStoreApi.Infrastructure.Exceptions
{
    public class CustomerNotFoundException : System.Exception
    {
        public CustomerNotFoundException()
        {
        }

        public CustomerNotFoundException(string message) : base(message)
        { }
    }
}

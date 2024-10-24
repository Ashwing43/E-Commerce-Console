namespace CustomExceptions
{
    public class OrderNotFoundException : Exception
    {
        public OrderNotFoundException() : base("Order cannot be found") { }

        public OrderNotFoundException(string message) : base(message) { }

        public OrderNotFoundException(string message, Exception innerException) : base(message, innerException) { }
    }
}

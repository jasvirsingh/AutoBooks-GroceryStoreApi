namespace GroceryStoreApi.Infrastructure
{
    /// <summary>
    /// 
    /// </summary>
    public class ValidationResult
    {
        /// <summary>
        /// The unique Billhighway code for the response message.
        /// Example: 400.0001
        /// </summary>
        /// <value>The code.</value>
        public string ErrorCode { get; set; }

        /// <summary>
        /// A brief description of the message.
        /// </summary>
        /// <value>The long description.</value>
        public string Message { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ValidationResult()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public ValidationResult(string message)
        {
            this.Message = message;
            this.ErrorCode = "400";
        }
    }
}

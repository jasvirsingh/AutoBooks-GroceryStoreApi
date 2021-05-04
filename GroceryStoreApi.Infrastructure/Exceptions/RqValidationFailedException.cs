using System;
using System.Collections.Generic;
using System.Text;

namespace GroceryStoreApi.Infrastructure.Exceptions
{
    /// <summary>
    /// Validation exception class
    /// </summary>
    public class RqValidationFailedException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        public List<ValidationResult> Messages { get; set; }

        /// <summary>
        /// empty constructor
        /// </summary>
        public RqValidationFailedException()
        {
        }

        /// <summary>
        /// constructor with message
        /// </summary>
        /// <param name="message"></param>
        public RqValidationFailedException(string message) : base(message)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="messages"></param>
        public RqValidationFailedException(List<ValidationResult> messages)
        {
            this.Messages = new List<ValidationResult>();
            this.Messages.AddRange(messages);
        }
    }
}

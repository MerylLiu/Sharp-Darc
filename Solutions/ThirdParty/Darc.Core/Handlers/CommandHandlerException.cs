namespace Darc.Core
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class CommandHandlerException : Exception
    {
        private readonly List<string> _errorMessage = new List<string>();

        public CommandHandlerException(string message)
            : base(message)
        {
            _errorMessage.Add(message);
        }

        public CommandHandlerException(IEnumerable<string> messages)
        {
            _errorMessage.AddRange(messages);
        }

        public CommandHandlerException(IEnumerable<ValidationResult> results)
        {
            foreach (ValidationResult item in results)
            {
                _errorMessage.Add(item.ErrorMessage);
            }
        }

        public List<string> ErrorMessages
        {
            get { return _errorMessage; }
        }
    }
}
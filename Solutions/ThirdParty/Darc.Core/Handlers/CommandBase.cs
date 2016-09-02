namespace Darc.Core
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class CommandBase : ICommand
    {
        public virtual bool IsValid()
        {
            return ValidationResults().Count == 0;
        }

        public virtual ICollection<ValidationResult> ValidationResults()
        {
            var validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(this, new ValidationContext(this, null, null), validationResults, true);
            return validationResults;
        }

        public virtual void Handle()
        {
            throw new CommandHandlerNotFoundException(typeof(ICommand));
        }

        public virtual TResult Handle<TResult>()
        {
            throw new CommandHandlerNotFoundException(typeof(ICommand));
        }
    }
}
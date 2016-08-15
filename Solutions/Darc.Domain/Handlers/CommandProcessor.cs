namespace Darc.Domain
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    public class CommandProcessor : ICommandProcessor
    {
        public void Process<TCommand>(TCommand command) where TCommand : ICommand
        {
            var validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(command, new ValidationContext(command, null, null), validationResults, true);

            if (validationResults.Any()) throw new CommandHandlerException(validationResults);

            command.Handle();
        }

        public TResult Process<TResult>(ICommand command)
        {
            var validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(command, new ValidationContext(command, null, null), validationResults, true);

            if (validationResults.Any()) throw new CommandHandlerException(validationResults);

            var res = command.Handle<TResult>();

            return res;
        }

        public void Process<TResult>(ICommand command, Action<TResult> resultHandler)
        {
            resultHandler(Process<TResult>(command));
        }
    }
}
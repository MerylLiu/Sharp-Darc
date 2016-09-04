namespace Darc.Core
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using Entities;

    public class CommandProcessor : ICommandProcessor
    {
        public void Process<TCommand>(TCommand command) where TCommand : ICommand
        {
            var validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(command, new ValidationContext(command, null, null), validationResults, true);

            var properties = command.GetType().GetProperties()
                .Where(p => typeof (EntityBase).IsAssignableFrom(p.PropertyType));

            foreach (var item in properties)
            {
                var target = item.GetValue(command);

                var itemValidationResults = new List<ValidationResult>();
                Validator.TryValidateObject(target, new ValidationContext(target, null, null),
                    itemValidationResults, true);

                validationResults.AddRange(itemValidationResults);
            }

            if (validationResults.Any()) throw new CommandHandlerException(validationResults);

            command.Handle();
        }

        public TResult Process<TResult>(ICommand command)
        {
            var validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(command, new ValidationContext(command, null, null), validationResults, true);

            var properties = command.GetType().GetProperties()
                .Where(p => typeof (EntityBase).IsAssignableFrom(p.PropertyType));

            foreach (var item in properties)
            {
                var target = item.GetValue(command);

                var itemValidationResults = new List<ValidationResult>();
                Validator.TryValidateObject(target, new ValidationContext(target, null, null),
                    itemValidationResults, true);

                validationResults.AddRange(itemValidationResults);
            }

            if (validationResults.Any()) throw new CommandHandlerException(validationResults);

            var res = command.Handle<object>();

            return (TResult) res;
        }

        public void Process<T>(ICommand command, Action<T> resultHandler)
        {
            resultHandler(Process<T>(command));
        }
    }
}
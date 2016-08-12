namespace Darc.Domain
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using Microsoft.Practices.ServiceLocation;

    public class CommandProcessor : ICommandProcessor
    {
        public void Process<TCommand>(TCommand command) where TCommand : ICommand
        {
            var validationResults = new List<ValidationResult>();

            Validator.TryValidateObject(command, new ValidationContext(command, null, null), validationResults, true);

            if (validationResults.Any())
            {
                throw new CommandHandlerException(validationResults);
            }

            IEnumerable<ICommandHandler<TCommand>> handlers =
                ServiceLocator.Current.GetAllInstances<ICommandHandler<TCommand>>();
            if (handlers == null || !handlers.Any())
            {
                throw new CommandHandlerException(typeof (TCommand).Name + "is null");
            }

            foreach (var handler in handlers)
            {
                handler.Handle(command);
            }
        }

        public IEnumerable<TResult> Process<TCommand, TResult>(TCommand command) where TCommand : ICommand
        {
            var validationResults = new List<ValidationResult>();

            Validator.TryValidateObject(command, new ValidationContext(command, null, null), validationResults, true);

            if (validationResults.Any())
            {
                throw new CommandHandlerException(validationResults);
            }

            IEnumerable<ICommandHandler<TCommand, TResult>> handlers =
                ServiceLocator.Current.GetAllInstances<ICommandHandler<TCommand, TResult>>();
            if (handlers == null || !handlers.Any())
            {
                throw new CommandHandlerNotFoundException(typeof (TCommand), typeof (TResult));
            }

            foreach (var handler in handlers)
            {
                yield return handler.Handle(command);
            }
        }

        public void Process<TCommand, TResult>(TCommand command, Action<TResult> resultHandler)
            where TCommand : ICommand
        {
            foreach (TResult result in Process<TCommand, TResult>(command))
            {
                resultHandler(result);
            }
        }
    }
}
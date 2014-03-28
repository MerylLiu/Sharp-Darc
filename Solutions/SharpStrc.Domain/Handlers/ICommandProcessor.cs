namespace SharpStrc.Domain
{
    using System;
    using System.Collections.Generic;

    public interface ICommandProcessor
    {
        void Process<TCommand>(TCommand command) where TCommand : ICommand;

        IEnumerable<TResult> Process<TCommand, TResult>(TCommand command) where TCommand : ICommand;

        void Process<TCommand, TResult>(TCommand command, Action<TResult> resultHandler) where TCommand : ICommand;
    }
}
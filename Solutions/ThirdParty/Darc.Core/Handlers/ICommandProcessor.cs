namespace Darc.Core
{
    using System;
    using Entities;

    public interface ICommandProcessor
    {
        void Process<TCommand>(TCommand command) where TCommand : ICommand;

        TResult Process<TResult>(ICommand command);

        void Process<TResult>(ICommand command, Action<TResult> resultHandler);
    }
}
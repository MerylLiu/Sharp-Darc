namespace Darc.Core
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Runtime.Remoting.Messaging;
    using Attributes;
    using Contracts;
    using Entities;
    using global::Castle.Core.Internal;
    using Microsoft.Practices.ServiceLocation;

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

            var dataSourceAttr = command.GetType().GetAttributes<DataSourceAttribute>();
            dataSourceAttr = dataSourceAttr.Any()
                ? dataSourceAttr
                : command.GetType().BaseType.GetAttributes<DataSourceAttribute>();
            var dataSource = dataSourceAttr.Select(p => p.DataSource).FirstOrDefault();
            CallContext.LogicalSetData("$DataSource", dataSource);

            command.Handler();

            AspectHandler(command, command.Handler);
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

            return AspectHandler(command, () => (TResult)command.Handler<object>());
        }

        public void Process<T>(ICommand command, Action<T> resultHandler)
        {
            resultHandler(Process<T>(command));
        }

        private TResult AspectHandler<TResult>(ICommand command,Func<TResult> func)
        {
            var dataSourceAttr = command.GetType().GetAttributes<DataSourceAttribute>();
            dataSourceAttr = dataSourceAttr.Any()
                ? dataSourceAttr
                : command.GetType().BaseType.GetAttributes<DataSourceAttribute>();
            var dataSource = dataSourceAttr.Select(p => p.DataSource).FirstOrDefault();
            CallContext.LogicalSetData("$DataSource", dataSource);

            var attributes = command.GetType().GetAttributes<TransAttribute>();
            var invocationAttr = command.GetType().GetMethod("Handler").GetAttributes<TransAttribute>();

            if (attributes.Any() || invocationAttr.Any())
            {
                var session = ServiceLocator.Current.GetInstance<IDataContext>();

                using (var conn = session.GetConnection(dataSource))
                {
                    var trans = conn.BeginTransaction();

                    CallContext.LogicalSetData("$DataSession", new DataSessionItems
                    {
                        Connection = conn,
                        Transaction = trans
                    });

                    try
                    {
                        var res = func();
                        trans.Commit();
                        return res;
                    }
                    catch (Exception)
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            return func();
        }

        private void AspectHandler(ICommand command, Action action)
        {
            var dataSourceAttr = command.GetType().GetAttributes<DataSourceAttribute>();
            dataSourceAttr = dataSourceAttr.Any()
                ? dataSourceAttr
                : command.GetType().BaseType.GetAttributes<DataSourceAttribute>();
            var dataSource = dataSourceAttr.Select(p => p.DataSource).FirstOrDefault();
            CallContext.LogicalSetData("$DataSource", dataSource);

            var attributes = command.GetType().GetAttributes<TransAttribute>();
            var invocationAttr = command.GetType().GetMethod("Handler").GetAttributes<TransAttribute>();

            if (attributes.Any() || invocationAttr.Any())
            {
                var session = ServiceLocator.Current.GetInstance<IDataContext>();

                using (var conn = session.GetConnection(dataSource))
                {
                    var trans = conn.BeginTransaction();

                    CallContext.LogicalSetData("$DataSession", new DataSessionItems
                    {
                        Connection = conn,
                        Transaction = trans
                    });

                    try
                    {
                        action();
                        trans.Commit();
                    }
                    catch (Exception)
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            action();
        }
    }
}
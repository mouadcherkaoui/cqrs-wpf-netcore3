using CQRSExample.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;

namespace CQRSExample.Commands
{
    public class ValidateCommandDecorator<TCommand, TResult> : IQueryHandler<TCommand, object>
        where TCommand: IQuery<object>
    {
        IQueryHandler<TCommand, object> _innerHandler;
        public ValidateCommandDecorator(IQueryHandler<TCommand, object> innerHandler)
        {
            _innerHandler = innerHandler;
        }
        public List<ValidationResult> ValidateModel(TCommand command)
        {
            var context = new ValidationContext(command, null, null);
            var validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(command, context, validationResults, true);

            var properties = command.GetType().GetProperties();
            foreach (var property in properties)
            {
                property.GetCustomAttributes(false);
                var value = property.GetValue(command);
                context.MemberName = property.Name;
                Validator.TryValidateProperty(property.GetValue(command), context, validationResults);
            }
            return validationResults;
        }
        public object Handle(TCommand command)
        {
            var validationResults = ValidateModel(command);
            dynamic resultToReturn = validationResults.Count > 0 
                ? (dynamic)new { validationResults }
                : (dynamic)new {  result = _innerHandler.Handle(command) };
            return resultToReturn;
        }

        public async Task<object> HandleAsync(TCommand query)
        {
            return HandleAsync(query, new CancellationToken()) ;
        }

        public async Task<object> HandleAsync(TCommand query, CancellationToken token)
        {
            return await Task.Run<object>(() => Handle(query), token);
        }
    }
}

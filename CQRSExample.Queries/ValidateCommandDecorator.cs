using CQRSExample.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;

namespace CQRSExample.Commands
{
    public class ValidateQueryDecorator<TQuery, TResult> : IQueryHandler<TQuery, object>
        where TQuery: IQuery<object>
    {
        IQueryHandler<TQuery, object> _innerHandler;
        public ValidateQueryDecorator(IQueryHandler<TQuery, object> innerHandler)
        {
            _innerHandler = innerHandler;
        }
        public List<ValidationResult> ValidateModel(TQuery command)
        {
            var context = new ValidationContext(command, null, null);
            var validationResults = new List<ValidationResult>();
            
            //Validator.TryValidateObject(command, context, validationResults, true);

            var properties = command.GetType().GetProperties();
            foreach (var property in properties)
            {
                var value = property.GetValue(command);
                context.MemberName = property.Name;
                Validator.TryValidateProperty(property.GetValue(command), context, validationResults);
            }
            return validationResults;
        }
        public object Handle(TQuery command)
        {
            var validationResults = ValidateModel(command);
            dynamic resultToReturn = validationResults.Count > 0 
                ? (dynamic)new { validationResults }
                : (dynamic)new {  result = _innerHandler.Handle(command) };
            return resultToReturn;
        }

        public async Task<object> HandleAsync(TQuery query)
        {
            return await HandleAsync(query, new CancellationToken()) ;
        }

        public async Task<object> HandleAsync(TQuery query, CancellationToken token)
        {
            return await Task.Run<object>(() => Handle(query), token);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Reflection;
using System.ComponentModel.DataAnnotations.Schema;

namespace CQRSExqmple.DataAccessLayer
{
    public static class QueriesResolver
    {
        static QueriesResolver()
        {

        }

        public static string GetSelectFor<TType>()
        {
            StringBuilder selectQuery = new StringBuilder("SELECT ");

            var propDict = 
                typeof(TType).GetProperties().ToDictionary(p => p.GetCustomAttribute<ColumnAttribute>(true)?.Name ?? p.Name, p => p.PropertyType);
            var count = propDict.Count;
            foreach (var columnName in propDict.Keys)
            {
                var separator = (--count == 0) ? " " : ","; 
                selectQuery.Append($"{columnName}{separator}");
            }
            // selectQuery.Remove(selectQuery.Length - 1, 1);
            selectQuery.Append($" FROM {typeof(TType).Name}");

            return selectQuery.ToString();
        }
    }
}

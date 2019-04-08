using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Reflection;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlTypes;
using System.Data.SqlClient;

namespace CQRSExqmple.DataAccessLayer
{
    public static class QueriesResolver
    {
        static Dictionary<Type, DbType> typeMap;
        static QueriesResolver()
        {
            typeMap = new Dictionary<Type, DbType>();
            typeMap[typeof(byte)] = DbType.Byte;
            typeMap[typeof(sbyte)] = DbType.SByte;
            typeMap[typeof(short)] = DbType.Int16;
            typeMap[typeof(ushort)] = DbType.UInt16;
            typeMap[typeof(int)] = DbType.Int32;
            typeMap[typeof(uint)] = DbType.UInt32;
            typeMap[typeof(long)] = DbType.Int64;
            typeMap[typeof(ulong)] = DbType.UInt64;
            typeMap[typeof(float)] = DbType.Single;
            typeMap[typeof(double)] = DbType.Double;
            typeMap[typeof(decimal)] = DbType.Decimal;
            typeMap[typeof(bool)] = DbType.Boolean;
            typeMap[typeof(string)] = DbType.String;
            typeMap[typeof(char)] = DbType.StringFixedLength;
            typeMap[typeof(Guid)] = DbType.Guid;
            typeMap[typeof(DateTime)] = DbType.DateTime;
            typeMap[typeof(DateTimeOffset)] = DbType.DateTimeOffset;
            typeMap[typeof(byte[])] = DbType.Binary;
            typeMap[typeof(byte?)] = DbType.Byte;
            typeMap[typeof(sbyte?)] = DbType.SByte;
            typeMap[typeof(short?)] = DbType.Int16;
            typeMap[typeof(ushort?)] = DbType.UInt16;
            typeMap[typeof(int?)] = DbType.Int32;
            typeMap[typeof(uint?)] = DbType.UInt32;
            typeMap[typeof(long?)] = DbType.Int64;
            typeMap[typeof(ulong?)] = DbType.UInt64;
            typeMap[typeof(float?)] = DbType.Single;
            typeMap[typeof(double?)] = DbType.Double;
            typeMap[typeof(decimal?)] = DbType.Decimal;
            typeMap[typeof(bool?)] = DbType.Boolean;
            typeMap[typeof(char?)] = DbType.StringFixedLength;
            typeMap[typeof(Guid?)] = DbType.Guid;
            typeMap[typeof(DateTime?)] = DbType.DateTime;
            typeMap[typeof(DateTimeOffset?)] = DbType.DateTimeOffset;
            typeMap[typeof(SqlBinary)] = DbType.Binary;
        }

        public static string SelectFor<TType>()
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

        public static string CreateTableFor<TType>(string schema = "[dbo]")
        {
            StringBuilder createTableQuery = 
                new StringBuilder($"CREATE TABLE [{schema}].[{typeof(TType).Name}] (");

            var keyProps = typeof(TType).GetProperties().Where(p => p.GetCustomAttributes<KeyAttribute>(true).Any());

            foreach (var keyProp in keyProps)
            {
                
            }

            var propDict = 
                typeof(TType).GetProperties().ToDictionary(p => p.GetCustomAttribute<ColumnAttribute>(true)?.Name ?? p.Name, p => p.PropertyType);
            var count = propDict.Count;
            foreach (var columnName in propDict.Keys)
            {
                var separator = (--count == 0) ? ")" : ","; 
                createTableQuery.Append($"{columnName}{separator}");
            }
            // selectQuery.Remove(selectQuery.Length - 1, 1);
            createTableQuery.Append($" WITH ({typeof(TType).Name})");

            return createTableQuery.ToString();            
        }
    }
}

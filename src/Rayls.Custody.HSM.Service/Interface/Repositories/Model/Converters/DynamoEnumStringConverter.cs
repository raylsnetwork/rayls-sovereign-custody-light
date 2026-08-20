using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using System;

namespace Rayls.Custody.HSM.Service.Interface.Repositories.Model.Converters
{
    public class DynamoEnumStringConverter<TEnum> : IPropertyConverter
    {
        public object FromEntry(DynamoDBEntry entry)
        {
            return (TEnum)Enum.Parse(typeof(TEnum), entry.AsString());
        }

        public DynamoDBEntry ToEntry(object value)
        {
            return new Primitive(value.ToString());
        }
    }
}

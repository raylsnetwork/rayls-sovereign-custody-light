using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace Rayls.Custody.HSM.Service.Interface.Repositories.Model.Converters
{
    public class DynamoIEnumerableConverter<T> : IPropertyConverter where T : class
    {
        private readonly StringEnumConverter _enumConverter;
        public DynamoIEnumerableConverter()
        {
            _enumConverter = new StringEnumConverter();
        }

        public object FromEntry(DynamoDBEntry entry)
        {
            var primitive = entry as Primitive;
            if (primitive == null || !(primitive.Value is string) || string.IsNullOrEmpty((string)primitive.Value))
                throw new ArgumentOutOfRangeException();
            var deserialized = JsonConvert.DeserializeObject<IEnumerable<T>>(primitive.Value as string);
            return deserialized;
        }

        public DynamoDBEntry ToEntry(object value)
        {
            var serialized = JsonConvert.SerializeObject(value, _enumConverter);
            return new Primitive(serialized);
        }
    }
}

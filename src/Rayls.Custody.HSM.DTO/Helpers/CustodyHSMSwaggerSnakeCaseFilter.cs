using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;

namespace Rayls.Custody.HSM.DTO.Helpers
{
    public class CustodyHSMSwaggerSnakeCaseFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (schema?.Properties == null)
            {
                return;
            }

            IDictionary<string, OpenApiSchema> properties = schema.Properties;
            Dictionary<string, OpenApiSchema> dictionary = new Dictionary<string, OpenApiSchema>();
            foreach (KeyValuePair<string, OpenApiSchema> item in properties)
            {
                dictionary.Add(ToSnakeCase(item.Key), item.Value);
            }

            schema.Properties = dictionary;
        }

        private string ToSnakeCase(string? str)
        {
            return str is null ? null : new DefaultContractResolver()
            {
                NamingStrategy = new SnakeCaseNamingStrategy()
            }.GetResolvedPropertyName(str);
        }
    }
}

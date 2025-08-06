#region
using AutoFixture;
using NJsonSchema;
using Vonage.Common.Monads;
#endregion

namespace DockerApiSandbox.Api.OutputGeneration;

internal static class DataGenerator
{
    internal static Maybe<object> GenerateData(JsonSchema schema)
    {
        if (schema.OneOf.Count > 0)
        {
            return GenerateData(schema.OneOf.First().ActualSchema);
        }

        if (schema.Enumeration?.Count > 0 &&
            schema.Enumeration.ToList()[new Random().Next(schema.Enumeration.Count)] is string enumValue)
        {
            return enumValue;
        }
        
        object result =  schema.Type switch
        {
            JsonObjectType.String => schema.Example as string ?? new Fixture().Create<string>(),
            JsonObjectType.Number => schema.Example != null ? Convert.ToDecimal(schema.Example) : new Fixture().Create<decimal>(),
            JsonObjectType.Integer => schema.Example != null ? Convert.ToInt32(schema.Example) : new Fixture().Create<int>(),
            JsonObjectType.Boolean => new Fixture().Create<bool>(),
            JsonObjectType.Object => GenerateObject(schema),
            JsonObjectType.Array => GenerateArray(schema),
            _ => GenerateObject(schema),
        };
        return result;
    }

    private static Dictionary<string, object> GenerateObject(JsonSchema schema)
    {
        var fakeObject = new Dictionary<string, object>();
        schema.ActualProperties.ToList().ForEach(property => GenerateData(property.Value.ActualSchema).IfSome(some =>  fakeObject[property.Key] = some));
        schema.AllOf.SelectMany(allOf => allOf.ActualProperties).ToList().ForEach(property => GenerateData(property.Value.ActualSchema).IfSome(some =>  fakeObject[property.Key] = some));
        return fakeObject;
    }

    private static IEnumerable<object> GenerateArray(JsonSchema schema)
    {
        var list = new List<object>();
        GenerateData(schema.Item!.ActualSchema).IfSome(some => list.Add(some));
        return list;
    }
}
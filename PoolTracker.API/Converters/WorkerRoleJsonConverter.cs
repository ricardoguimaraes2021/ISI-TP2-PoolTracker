using System.Text.Json;
using System.Text.Json.Serialization;
using PoolTracker.Core.Entities;

namespace PoolTracker.API.Converters;

public class WorkerRoleJsonConverter : JsonConverter<WorkerRole>
{
    private static readonly Dictionary<string, WorkerRole> StringToEnum = new()
    {
        { "nadador_salvador", WorkerRole.NadadorSalvador },
        { "NadadorSalvador", WorkerRole.NadadorSalvador },
        { "bar", WorkerRole.Bar },
        { "Bar", WorkerRole.Bar },
        { "vigilante", WorkerRole.Vigilante },
        { "Vigilante", WorkerRole.Vigilante },
        { "bilheteira", WorkerRole.Bilheteira },
        { "Bilheteira", WorkerRole.Bilheteira }
    };

    public override WorkerRole Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            var stringValue = reader.GetString();
            if (stringValue != null && StringToEnum.TryGetValue(stringValue, out var role))
            {
                return role;
            }
        }
        else if (reader.TokenType == JsonTokenType.Number)
        {
            var intValue = reader.GetInt32();
            if (Enum.IsDefined(typeof(WorkerRole), intValue))
            {
                return (WorkerRole)intValue;
            }
        }

        throw new JsonException($"Unable to convert '{reader.GetString()}' to {nameof(WorkerRole)}");
    }

    public override void Write(Utf8JsonWriter writer, WorkerRole value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}


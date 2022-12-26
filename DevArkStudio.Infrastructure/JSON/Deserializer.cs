using System.Text.Json;
using DevArkStudio.Persistence.DTOModels;

namespace DevArkStudio.Infrastructure.JSON;

public static class Deserializer
{
    public static JsonSerializerOptions Options = Serializer.Options;

    public static PageDTO DeserializePageDTO(string pageDTOJson)
    {
        return JsonSerializer.Deserialize<PageDTO>(pageDTOJson, Options);
    }

    public static NodeDTO DeserializeNodeDTO(string nodeDTOJson)
    {
        return JsonSerializer.Deserialize<NodeDTO>(nodeDTOJson, Options);
    }
}
using BasicCAM.Core.Segments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BasicCAM.WASM.Serialization
{
    public class SegmentSerializerConverter : JsonConverter<Segment>
    {
        public override Segment Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotSupportedException($"Deserializing not supported. Type={typeToConvert}.");
        }

        public override void Write(Utf8JsonWriter writer, Segment value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, value.GetType(), options);
        }
    }
}

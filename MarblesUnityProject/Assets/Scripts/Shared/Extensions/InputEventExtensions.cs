using System.Collections.Generic;
using MemoryPack;

namespace GameCoreLib
{
    public static class InputEventSerialization
    {
        //IMPORTANT: The serialization fails if I don't include the <InputEvent> in the serialize type!!!
        public static byte[] ToBinary(this InputEvent inputEvent)
        {
            return MemoryPackSerializer.Serialize<InputEvent>(inputEvent);
        }

        public static InputEvent? FromBinary(byte[] data)
        {
            return MemoryPackSerializer.Deserialize<InputEvent>(data);
        }

        public static byte[] SerializeList(List<InputEvent> events)
        {
            return MemoryPackSerializer.Serialize<List<InputEvent>>(events);
        }

        public static List<InputEvent> DeserializeList(byte[] data)
        {
            return MemoryPackSerializer.Deserialize<List<InputEvent>>(data)
                ?? new List<InputEvent>();
        }
    }
}

using MemoryPack;

[MemoryPackable(SerializeLayout.Explicit)]
public partial class LevelFile
{
    [MemoryPackOrder(0)]
    public string UnityPrefabGUID;

    [MemoryPackOrder(1)]
    public string LevelName;

    [MemoryPackOrder(2)]
    public string ObjHierarchyJson;

    public LevelFile(string unityPrefabGUID, string levelName, string objHierarchyJson)
    {
        UnityPrefabGUID = unityPrefabGUID;
        LevelName = levelName;
        ObjHierarchyJson = objHierarchyJson;
    }

    public byte[] ToBinary()
    {
        return MemoryPackSerializer.Serialize<LevelFile>(this);
    }
}

using UnityEditor;

public class GameTileExporter : EditorWindow
{
    //1. Iterate through all the prefabs in the Assets/Prefabs folder

    //2. If a prefab has a GameTileAuth as the root, then Convert the entire prefab to one JSON file and save the file to Temp/LevelJSONs/

    //3. Next we need to upload these LevelJSONs to spacetimedb

    //4. Connect to spacetimedb server with admin credentials

    //5. Upsert each LevelJSON to the spacetimedb database
}

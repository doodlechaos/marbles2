using GameCoreLib;
using MemoryPack;
using SpacetimeDB;

public static partial class Module
{
    [Reducer]
    public static void CloseAndCycleGameTile(ReducerContext ctx, byte worldId)
    {
        Log.Info($"Closing and cycling game tile for world {worldId}");
        try
        {
            byte[] eventData = new InputEvent.StartCloseDoorAnimation(worldId).ToBinary();
            Log.Info($"Serialized event data: {eventData.Length} bytes");
            //1. Start door close animation
            ctx.Db.InputCollector.Insert(
                new InputCollector { delaySeqs = 0, inputEventData = eventData }
            );

            LevelFile levelFile = GetRandomLevelFile(ctx);

            //2. Schedule a load level file input event
            byte[] eventData2 = new InputEvent.LoadLevelFile(worldId, levelFile).ToBinary();

            Log.Info($"Serialized event data 2: {eventData2.Length} bytes");
            ctx.Db.InputCollector.Insert(
                new InputCollector { delaySeqs = 120, inputEventData = eventData2 }
            );
        }
        catch (Exception e)
        {
            Log.Error($"Error closing and cycling game tile: {e.Message}");
            return;
        }
    }

    public static LevelFile GetRandomLevelFile(ReducerContext ctx)
    {
        //If we have no level file datas stored, create a default one
        if (ctx.Db.LevelFileData.Iter().Count() == 0)
        {
            LevelFile defaultLevelFile = new LevelFile(
                "default",
                "Default Level",
                GetDefaultGameTileJSON(ctx)
            );
            return defaultLevelFile;
        }

        //Get a random level file data
        int randomIndex = ctx.Rng.Next(0, ctx.Db.LevelFileData.Iter().Count());
        LevelFileData levelFileData = ctx.Db.LevelFileData.Iter().ToArray()[randomIndex];

        //Deserialize the level file data into a level file
        LevelFile levelFile =
            MemoryPackSerializer.Deserialize<LevelFile>(levelFileData.LevelFileBinary)
            ?? new LevelFile("default", "Default Level", GetDefaultGameTileJSON(ctx));
        return levelFile;
    }

    public static string GetDefaultGameTileJSON(ReducerContext ctx)
    {
        return """
            {
            "RuntimeId": 0,
            "Name": "TestTile1 Variant",
            "Children": [
                {
                "RuntimeId": 0,
                "Name": "Cube",
                "Children": [],
                "Transform": {
                    "localPosition": {
                    "x": {
                        "RawValue": 0
                    },
                    "y": {
                        "RawValue": 0
                    },
                    "z": {
                        "RawValue": 13107
                    }
                    },
                    "localRotation": {
                    "x": {
                        "RawValue": 0
                    },
                    "y": {
                        "RawValue": 0
                    },
                    "z": {
                        "RawValue": 0
                    },
                    "w": {
                        "RawValue": 65536
                    }
                    },
                    "localScale": {
                    "x": {
                        "RawValue": 655360
                    },
                    "y": {
                        "RawValue": 1310720
                    },
                    "z": {
                        "RawValue": 6553
                    }
                    }
                },
                "Components": [
                    {
                    "type": "UnityEngine.MeshFilter",
                    "enabled": true,
                    "data": "{}"
                    },
                    {
                    "type": "UnityEngine.MeshRenderer",
                    "enabled": true,
                    "data": "{}"
                    }
                ],
                "RenderPrefabID": -1
                },
                {
                "RuntimeId": 0,
                "Name": "Cube (1)",
                "Children": [],
                "Transform": {
                    "localPosition": {
                    "x": {
                        "RawValue": -327680
                    },
                    "y": {
                        "RawValue": 0
                    },
                    "z": {
                        "RawValue": 0
                    }
                    },
                    "localRotation": {
                    "x": {
                        "RawValue": 0
                    },
                    "y": {
                        "RawValue": 0
                    },
                    "z": {
                        "RawValue": 0
                    },
                    "w": {
                        "RawValue": 65536
                    }
                    },
                    "localScale": {
                    "x": {
                        "RawValue": 13107
                    },
                    "y": {
                        "RawValue": 1310720
                    },
                    "z": {
                        "RawValue": 65536
                    }
                    }
                },
                "Components": [
                    {
                    "type": "UnityEngine.MeshFilter",
                    "enabled": true,
                    "data": "{}"
                    },
                    {
                    "type": "UnityEngine.MeshRenderer",
                    "enabled": true,
                    "data": "{}"
                    },
                    {
                    "type": "UnityEngine.BoxCollider",
                    "enabled": true,
                    "data": "{}"
                    }
                ],
                "RenderPrefabID": -1
                },
                {
                "RuntimeId": 0,
                "Name": "Cube (2)",
                "Children": [],
                "Transform": {
                    "localPosition": {
                    "x": {
                        "RawValue": 0
                    },
                    "y": {
                        "RawValue": 655360
                    },
                    "z": {
                        "RawValue": -38010
                    }
                    },
                    "localRotation": {
                    "x": {
                        "RawValue": 32768
                    },
                    "y": {
                        "RawValue": -32768
                    },
                    "z": {
                        "RawValue": 32768
                    },
                    "w": {
                        "RawValue": 32768
                    }
                    },
                    "localScale": {
                    "x": {
                        "RawValue": 13107
                    },
                    "y": {
                        "RawValue": 655360
                    },
                    "z": {
                        "RawValue": 65536
                    }
                    }
                },
                "Components": [
                    {
                    "type": "UnityEngine.MeshFilter",
                    "enabled": true,
                    "data": "{}"
                    },
                    {
                    "type": "UnityEngine.MeshRenderer",
                    "enabled": true,
                    "data": "{}"
                    }
                ],
                "RenderPrefabID": -1
                },
                {
                "RuntimeId": 0,
                "Name": "Cube (3)",
                "Children": [],
                "Transform": {
                    "localPosition": {
                    "x": {
                        "RawValue": 0
                    },
                    "y": {
                        "RawValue": -655360
                    },
                    "z": {
                        "RawValue": -38010
                    }
                    },
                    "localRotation": {
                    "x": {
                        "RawValue": 32768
                    },
                    "y": {
                        "RawValue": -32768
                    },
                    "z": {
                        "RawValue": 32768
                    },
                    "w": {
                        "RawValue": 32768
                    }
                    },
                    "localScale": {
                    "x": {
                        "RawValue": 13107
                    },
                    "y": {
                        "RawValue": 655360
                    },
                    "z": {
                        "RawValue": 65536
                    }
                    }
                },
                "Components": [
                    {
                    "type": "UnityEngine.MeshFilter",
                    "enabled": true,
                    "data": "{}"
                    },
                    {
                    "type": "UnityEngine.MeshRenderer",
                    "enabled": true,
                    "data": "{}"
                    }
                ],
                "RenderPrefabID": -1
                },
                {
                "RuntimeId": 0,
                "Name": "Cube (4)",
                "Children": [],
                "Transform": {
                    "localPosition": {
                    "x": {
                        "RawValue": 327680
                    },
                    "y": {
                        "RawValue": 0
                    },
                    "z": {
                        "RawValue": 0
                    }
                    },
                    "localRotation": {
                    "x": {
                        "RawValue": 0
                    },
                    "y": {
                        "RawValue": 0
                    },
                    "z": {
                        "RawValue": 0
                    },
                    "w": {
                        "RawValue": 65536
                    }
                    },
                    "localScale": {
                    "x": {
                        "RawValue": 13107
                    },
                    "y": {
                        "RawValue": 1310720
                    },
                    "z": {
                        "RawValue": 65536
                    }
                    }
                },
                "Components": [
                    {
                    "type": "UnityEngine.MeshFilter",
                    "enabled": true,
                    "data": "{}"
                    },
                    {
                    "type": "UnityEngine.MeshRenderer",
                    "enabled": true,
                    "data": "{}"
                    },
                    {
                    "type": "UnityEngine.BoxCollider",
                    "enabled": true,
                    "data": "{}"
                    }
                ],
                "RenderPrefabID": -1
                },
                {
                "RuntimeId": 0,
                "Name": "Cylinder",
                "Children": [],
                "Transform": {
                    "localPosition": {
                    "x": {
                        "RawValue": -165150
                    },
                    "y": {
                        "RawValue": 157286
                    },
                    "z": {
                        "RawValue": 0
                    }
                    },
                    "localRotation": {
                    "x": {
                        "RawValue": 46340
                    },
                    "y": {
                        "RawValue": 0
                    },
                    "z": {
                        "RawValue": 0
                    },
                    "w": {
                        "RawValue": 46340
                    }
                    },
                    "localScale": {
                    "x": {
                        "RawValue": 65536
                    },
                    "y": {
                        "RawValue": 65536
                    },
                    "z": {
                        "RawValue": 65536
                    }
                    }
                },
                "Components": [
                    {
                    "type": "UnityEngine.MeshFilter",
                    "enabled": true,
                    "data": "{}"
                    },
                    {
                    "type": "UnityEngine.MeshRenderer",
                    "enabled": true,
                    "data": "{}"
                    },
                    {
                    "type": "UnityEngine.CircleCollider2D",
                    "enabled": true,
                    "data": "{}"
                    }
                ],
                "RenderPrefabID": 2
                },
                {
                "RuntimeId": 0,
                "Name": "Cylinder (1)",
                "Children": [],
                "Transform": {
                    "localPosition": {
                    "x": {
                        "RawValue": -88473
                    },
                    "y": {
                        "RawValue": -22282
                    },
                    "z": {
                        "RawValue": 0
                    }
                    },
                    "localRotation": {
                    "x": {
                        "RawValue": 46340
                    },
                    "y": {
                        "RawValue": 0
                    },
                    "z": {
                        "RawValue": 0
                    },
                    "w": {
                        "RawValue": 46340
                    }
                    },
                    "localScale": {
                    "x": {
                        "RawValue": 65536
                    },
                    "y": {
                        "RawValue": 65536
                    },
                    "z": {
                        "RawValue": 65536
                    }
                    }
                },
                "Components": [
                    {
                    "type": "UnityEngine.MeshFilter",
                    "enabled": true,
                    "data": "{}"
                    },
                    {
                    "type": "UnityEngine.MeshRenderer",
                    "enabled": true,
                    "data": "{}"
                    },
                    {
                    "type": "UnityEngine.CircleCollider2D",
                    "enabled": true,
                    "data": "{}"
                    }
                ],
                "RenderPrefabID": 2
                },
                {
                "RuntimeId": 0,
                "Name": "Cylinder (2)",
                "Children": [],
                "Transform": {
                    "localPosition": {
                    "x": {
                        "RawValue": -151388
                    },
                    "y": {
                        "RawValue": -263454
                    },
                    "z": {
                        "RawValue": 0
                    }
                    },
                    "localRotation": {
                    "x": {
                        "RawValue": 46340
                    },
                    "y": {
                        "RawValue": 0
                    },
                    "z": {
                        "RawValue": 0
                    },
                    "w": {
                        "RawValue": 46340
                    }
                    },
                    "localScale": {
                    "x": {
                        "RawValue": 65536
                    },
                    "y": {
                        "RawValue": 65536
                    },
                    "z": {
                        "RawValue": 65536
                    }
                    }
                },
                "Components": [
                    {
                    "type": "UnityEngine.MeshFilter",
                    "enabled": true,
                    "data": "{}"
                    },
                    {
                    "type": "UnityEngine.MeshRenderer",
                    "enabled": true,
                    "data": "{}"
                    },
                    {
                    "type": "UnityEngine.CircleCollider2D",
                    "enabled": true,
                    "data": "{}"
                    }
                ],
                "RenderPrefabID": 2
                }
            ],
            "Transform": {
                "localPosition": {
                "x": {
                    "RawValue": 0
                },
                "y": {
                    "RawValue": 0
                },
                "z": {
                    "RawValue": 0
                }
                },
                "localRotation": {
                "x": {
                    "RawValue": 0
                },
                "y": {
                    "RawValue": 0
                },
                "z": {
                    "RawValue": 0
                },
                "w": {
                    "RawValue": 65536
                }
                },
                "localScale": {
                "x": {
                    "RawValue": 65536
                },
                "y": {
                    "RawValue": 65536
                },
                "z": {
                    "RawValue": 65536
                }
                }
            },
            "Components": [
                {
                "type": "GameTileAuth",
                "enabled": true,
                "data": "{\"MinutesUntilCycle\":1.0}"
                }
            ],
            "RenderPrefabID": -1
            }
            """;
    }
}

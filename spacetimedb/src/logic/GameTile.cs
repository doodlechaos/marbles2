using GameCoreLib;
using MemoryPack;
using SpacetimeDB;

public static partial class Module
{
    [Reducer]
    public static void TestSerialization(ReducerContext ctx)
    {
        GameCore gameCore = new GameCore();
        byte[] serialized = MemoryPackSerializer.Serialize(gameCore);
        GameCore deserialized = MemoryPackSerializer.Deserialize<GameCore>(serialized);
        Log.Info($"Serialized 1: {serialized.Length}");
        Log.Info($"Deserialized 1: {deserialized.GetDeterministicHashHex()}");

        InputEvent inputEvent = new InputEvent.StartCloseDoorAnimation(1);
        serialized = inputEvent.ToBinary();
        InputEvent d2 = MemoryPackSerializer.Deserialize<InputEvent>(serialized);
        Log.Info($"Serialized 2: {serialized.Length}");

        InputEvent inputEvent2 = new InputEvent.LoadTileFile(2, 0, "bazinga");
        serialized = inputEvent2.ToBinary();
        InputEvent d3 = MemoryPackSerializer.Deserialize<InputEvent>(serialized);
        Log.Info($"Serialized 3: {serialized.Length}");
    }

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
            byte[] eventData2 = new InputEvent.LoadTileFile(
                worldId,
                levelFile.Id,
                levelFile.Json
            ).ToBinary();

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
        int randomIndex = ctx.Rng.Next(0, ctx.Db.LevelFile.Iter().Count());
        return ctx.Db.LevelFile.Iter().ToArray()[randomIndex];
    }

    public static void InitDefaultGameTiles(ReducerContext ctx)
    {
        const string json1 = """
                {
                "RuntimeId": 0,
                "Name": "TestAuthoringWorld",
                "Children": [
                    {
                    "RuntimeId": 0,
                    "Name": "Cube",
                    "Children": [],
                    "Transform": {
                        "localPosition": {
                        "x": {
                            "RawValue": -149422
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
                            "RawValue": 1251737
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
                        "type": "UnityEngine.BoxCollider2D",
                        "enabled": true,
                        "data": "{}"
                        }
                    ],
                    "RenderPrefabID": 0
                    },
                    {
                    "RuntimeId": 0,
                    "Name": "Cube (1)",
                    "Children": [],
                    "Transform": {
                        "localPosition": {
                        "x": {
                            "RawValue": -28835
                        },
                        "y": {
                            "RawValue": 168427
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
                        "type": "UnityEngine.BoxCollider2D",
                        "enabled": true,
                        "data": "{}"
                        },
                        {
                        "type": "UnityEngine.Rigidbody2D",
                        "enabled": true,
                        "data": "{}"
                        }
                    ],
                    "RenderPrefabID": 0
                    },
                    {
                    "RuntimeId": 0,
                    "Name": "Cube (2)",
                    "Children": [
                        {
                        "RuntimeId": 0,
                        "Name": "GameObject",
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
                        "Components": [],
                        "RenderPrefabID": -1
                        }
                    ],
                    "Transform": {
                        "localPosition": {
                        "x": {
                            "RawValue": -62259
                        },
                        "y": {
                            "RawValue": 74711
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
                        "type": "UnityEngine.BoxCollider2D",
                        "enabled": true,
                        "data": "{}"
                        },
                        {
                        "type": "UnityEngine.Rigidbody2D",
                        "enabled": true,
                        "data": "{}"
                        }
                    ],
                    "RenderPrefabID": 0
                    },
                    {
                    "RuntimeId": 0,
                    "Name": "Cube (3)",
                    "Children": [
                        {
                        "RuntimeId": 0,
                        "Name": "Sphere",
                        "Children": [],
                        "Transform": {
                            "localPosition": {
                            "x": {
                                "RawValue": -224788
                            },
                            "y": {
                                "RawValue": -105512
                            },
                            "z": {
                                "RawValue": -24733
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
                            "type": "UnityEngine.SphereCollider",
                            "enabled": true,
                            "data": "{}"
                            }
                        ],
                        "RenderPrefabID": 1
                        }
                    ],
                    "Transform": {
                        "localPosition": {
                        "x": {
                            "RawValue": -98959
                        },
                        "y": {
                            "RawValue": 422707
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
                            "RawValue": -25079
                        },
                        "w": {
                            "RawValue": 60547
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
                        "type": "UnityEngine.BoxCollider2D",
                        "enabled": true,
                        "data": "{}"
                        },
                        {
                        "type": "UnityEngine.Rigidbody2D",
                        "enabled": true,
                        "data": "{}"
                        }
                    ],
                    "RenderPrefabID": 0
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
                    "data": "{}"
                    }
                ],
                "RenderPrefabID": -1
                }
            """;

        const string json2 = """
                {
                "RuntimeId": 0,
                "Name": "DefaultWorld2",
                "Children": [
                    {
                    "RuntimeId": 0,
                    "Name": "Cube",
                    "Children": [],
                    "Transform": {
                        "localPosition": {
                        "x": {
                            "RawValue": -149422
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
                            "RawValue": -4166
                        },
                        "w": {
                            "RawValue": 65403
                        }
                        },
                        "localScale": {
                        "x": {
                            "RawValue": 1251737
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
                        "type": "UnityEngine.BoxCollider2D",
                        "enabled": true,
                        "data": "{}"
                        }
                    ],
                    "RenderPrefabID": 0
                    },
                    {
                    "RuntimeId": 0,
                    "Name": "Cube (1)",
                    "Children": [],
                    "Transform": {
                        "localPosition": {
                        "x": {
                            "RawValue": -300810
                        },
                        "y": {
                            "RawValue": 230686
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
                        "type": "UnityEngine.BoxCollider2D",
                        "enabled": true,
                        "data": "{}"
                        },
                        {
                        "type": "UnityEngine.Rigidbody2D",
                        "enabled": true,
                        "data": "{}"
                        }
                    ],
                    "RenderPrefabID": 0
                    },
                    {
                    "RuntimeId": 0,
                    "Name": "Cube (2)",
                    "Children": [
                        {
                        "RuntimeId": 0,
                        "Name": "GameObject",
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
                        "Components": [],
                        "RenderPrefabID": -1
                        }
                    ],
                    "Transform": {
                        "localPosition": {
                        "x": {
                            "RawValue": -494796
                        },
                        "y": {
                            "RawValue": 136970
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
                        "type": "UnityEngine.BoxCollider2D",
                        "enabled": true,
                        "data": "{}"
                        },
                        {
                        "type": "UnityEngine.Rigidbody2D",
                        "enabled": true,
                        "data": "{}"
                        }
                    ],
                    "RenderPrefabID": 0
                    },
                    {
                    "RuntimeId": 0,
                    "Name": "Cube (4)",
                    "Children": [
                        {
                        "RuntimeId": 0,
                        "Name": "GameObject",
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
                        "Components": [],
                        "RenderPrefabID": -1
                        }
                    ],
                    "Transform": {
                        "localPosition": {
                        "x": {
                            "RawValue": -390594
                        },
                        "y": {
                            "RawValue": 477757
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
                        "type": "UnityEngine.BoxCollider2D",
                        "enabled": true,
                        "data": "{}"
                        },
                        {
                        "type": "UnityEngine.Rigidbody2D",
                        "enabled": true,
                        "data": "{}"
                        }
                    ],
                    "RenderPrefabID": 0
                    },
                    {
                    "RuntimeId": 0,
                    "Name": "Cube (5)",
                    "Children": [
                        {
                        "RuntimeId": 0,
                        "Name": "GameObject",
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
                        "Components": [],
                        "RenderPrefabID": -1
                        }
                    ],
                    "Transform": {
                        "localPosition": {
                        "x": {
                            "RawValue": -186122
                        },
                        "y": {
                            "RawValue": 563609
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
                        "type": "UnityEngine.BoxCollider2D",
                        "enabled": true,
                        "data": "{}"
                        },
                        {
                        "type": "UnityEngine.Rigidbody2D",
                        "enabled": true,
                        "data": "{}"
                        }
                    ],
                    "RenderPrefabID": 0
                    },
                    {
                    "RuntimeId": 0,
                    "Name": "Cube (3)",
                    "Children": [
                        {
                        "RuntimeId": 0,
                        "Name": "Sphere",
                        "Children": [],
                        "Transform": {
                            "localPosition": {
                            "x": {
                                "RawValue": -224788
                            },
                            "y": {
                                "RawValue": -105512
                            },
                            "z": {
                                "RawValue": -24733
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
                            "type": "UnityEngine.SphereCollider",
                            "enabled": true,
                            "data": "{}"
                            }
                        ],
                        "RenderPrefabID": 1
                        }
                    ],
                    "Transform": {
                        "localPosition": {
                        "x": {
                            "RawValue": 154009
                        },
                        "y": {
                            "RawValue": 334888
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
                            "RawValue": -25079
                        },
                        "w": {
                            "RawValue": 60547
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
                        "type": "UnityEngine.BoxCollider2D",
                        "enabled": true,
                        "data": "{}"
                        },
                        {
                        "type": "UnityEngine.Rigidbody2D",
                        "enabled": true,
                        "data": "{}"
                        }
                    ],
                    "RenderPrefabID": 0
                    }
                ],
                "Transform": {
                    "localPosition": {
                    "x": {
                        "RawValue": 4066
                    },
                    "y": {
                        "RawValue": 0
                    },
                    "z": {
                        "RawValue": 784802
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
                    "data": "{}"
                    }
                ],
                "RenderPrefabID": -1
                }
            """;

        ctx.Db.LevelFile.Insert(new LevelFile { Id = 0, Json = json1 });
        ctx.Db.LevelFile.Insert(new LevelFile { Id = 1, Json = json2 });
        Log.Info("Initialized default game tiles");
    }
}

using System.Security.Cryptography.X509Certificates;
using GameCoreLib;
using MemoryPack;
using SpacetimeDB;

public static partial class Module
{
    [Reducer]
    public static void CloseAndCycleGameTile(ReducerContext ctx, byte worldId)
    {
        //1. Start door close animation
        ctx.Db.InputCollector.Insert(
            new InputCollector
            {
                delaySeqs = 0,
                inputEventData =
                    MemoryPackSerializer.Serialize(new InputEvent.StartCloseDoorAnimation(worldId))
                    ?? throw new Exception("Failed to serialize input event"),
            }
        );

        int randomIndex = ctx.Rng.Next(0, ctx.Db.LevelFile.Iter().Count());
        LevelFile levelFile = ctx.Db.LevelFile.Iter().ToArray()[randomIndex];

        //2. Schedule a load level file input event
        ctx.Db.InputCollector.Insert(
            new InputCollector
            {
                delaySeqs = 120,
                inputEventData =
                    MemoryPackSerializer.Serialize(
                        new InputEvent.LoadTileFile(worldId, levelFile.Id, levelFile.Json)
                    ) ?? throw new Exception("Failed to serialize input event"),
            }
        );
    }

    public static void InitDefaultGameTiles(ReducerContext ctx)
    {
        const string json = """
            {
            "Name": "TestAuthoringWorld",
            "Children": [
                {
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
                "Name": "Cube (2)",
                "Children": [
                    {
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
                "Name": "Cube (3)",
                "Children": [
                    {
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

        ctx.Db.LevelFile.Insert(new LevelFile { Id = 0, Json = json });
    }
}

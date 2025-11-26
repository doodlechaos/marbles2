using MemoryPack;

namespace GameCoreLib
{
    [MemoryPackable(SerializeLayout.Explicit)]
    public partial class LevelFile
    {
        [MemoryPackOrder(0)]
        public string UnityPrefabGUID;

        [MemoryPackOrder(1)]
        public LevelMetadata LevelMetaData;

        [MemoryPackOrder(2)]
        public string ObjHierarchyJson;

        public LevelFile(
            string unityPrefabGUID,
            LevelMetadata levelMetaData,
            string objHierarchyJson
        )
        {
            UnityPrefabGUID = unityPrefabGUID;
            LevelMetaData = levelMetaData;
            ObjHierarchyJson = objHierarchyJson;
        }

        public byte[] ToBinary()
        {
            return MemoryPackSerializer.Serialize<LevelFile>(this);
        }

        public static LevelFile Default =>
            new LevelFile("default", LevelMetadata.Default, DefaultObjHierarchyJson);

        public const string DefaultObjHierarchyJson =
            @"{
            ""RuntimeId"": 0,
            ""Name"": ""TestTile1 Variant"",
            ""Children"": [
                {
                ""RuntimeId"": 0,
                ""Name"": ""Cube"",
                ""Children"": [],
                ""Transform"": {
                    ""localPosition"": {
                    ""x"": {
                        ""RawValue"": 0
                    },
                    ""y"": {
                        ""RawValue"": 0
                    },
                    ""z"": {
                        ""RawValue"": 13107
                    }
                    },
                    ""localRotation"": {
                    ""x"": {
                        ""RawValue"": 0
                    },
                    ""y"": {
                        ""RawValue"": 0
                    },
                    ""z"": {
                        ""RawValue"": 0
                    },
                    ""w"": {
                        ""RawValue"": 65536
                    }
                    },
                    ""localScale"": {
                    ""x"": {
                        ""RawValue"": 655360
                    },
                    ""y"": {
                        ""RawValue"": 1310720
                    },
                    ""z"": {
                        ""RawValue"": 6553
                    }
                    }
                },
                ""Components"": [
                    {
                    ""type"": ""UnityEngine.MeshFilter"",
                    ""enabled"": true,
                    ""data"": ""{}""
                    },
                    {
                    ""type"": ""UnityEngine.MeshRenderer"",
                    ""enabled"": true,
                    ""data"": ""{}""
                    }
                ],
                ""RenderPrefabID"": -1
                },
                {
                ""RuntimeId"": 0,
                ""Name"": ""Cube (1)"",
                ""Children"": [],
                ""Transform"": {
                    ""localPosition"": {
                    ""x"": {
                        ""RawValue"": -327680
                    },
                    ""y"": {
                        ""RawValue"": 0
                    },
                    ""z"": {
                        ""RawValue"": 0
                    }
                    },
                    ""localRotation"": {
                    ""x"": {
                        ""RawValue"": 0
                    },
                    ""y"": {
                        ""RawValue"": 0
                    },
                    ""z"": {
                        ""RawValue"": 0
                    },
                    ""w"": {
                        ""RawValue"": 65536
                    }
                    },
                    ""localScale"": {
                    ""x"": {
                        ""RawValue"": 13107
                    },
                    ""y"": {
                        ""RawValue"": 1310720
                    },
                    ""z"": {
                        ""RawValue"": 65536
                    }
                    }
                },
                ""Components"": [
                    {
                    ""type"": ""UnityEngine.MeshFilter"",
                    ""enabled"": true,
                    ""data"": ""{}""
                    },
                    {
                    ""type"": ""UnityEngine.MeshRenderer"",
                    ""enabled"": true,
                    ""data"": ""{}""
                    },
                    {
                    ""type"": ""UnityEngine.BoxCollider"",
                    ""enabled"": true,
                    ""data"": ""{}""
                    }
                ],
                ""RenderPrefabID"": -1
                },
                {
                ""RuntimeId"": 0,
                ""Name"": ""Cube (2)"",
                ""Children"": [],
                ""Transform"": {
                    ""localPosition"": {
                    ""x"": {
                        ""RawValue"": 0
                    },
                    ""y"": {
                        ""RawValue"": 655360
                    },
                    ""z"": {
                        ""RawValue"": -38010
                    }
                    },
                    ""localRotation"": {
                    ""x"": {
                        ""RawValue"": 32768
                    },
                    ""y"": {
                        ""RawValue"": -32768
                    },
                    ""z"": {
                        ""RawValue"": 32768
                    },
                    ""w"": {
                        ""RawValue"": 32768
                    }
                    },
                    ""localScale"": {
                    ""x"": {
                        ""RawValue"": 13107
                    },
                    ""y"": {
                        ""RawValue"": 655360
                    },
                    ""z"": {
                        ""RawValue"": 65536
                    }
                    }
                },
                ""Components"": [
                    {
                    ""type"": ""UnityEngine.MeshFilter"",
                    ""enabled"": true,
                    ""data"": ""{}""
                    },
                    {
                    ""type"": ""UnityEngine.MeshRenderer"",
                    ""enabled"": true,
                    ""data"": ""{}""
                    }
                ],
                ""RenderPrefabID"": -1
                },
                {
                ""RuntimeId"": 0,
                ""Name"": ""Cube (3)"",
                ""Children"": [],
                ""Transform"": {
                    ""localPosition"": {
                    ""x"": {
                        ""RawValue"": 0
                    },
                    ""y"": {
                        ""RawValue"": -655360
                    },
                    ""z"": {
                        ""RawValue"": -38010
                    }
                    },
                    ""localRotation"": {
                    ""x"": {
                        ""RawValue"": 32768
                    },
                    ""y"": {
                        ""RawValue"": -32768
                    },
                    ""z"": {
                        ""RawValue"": 32768
                    },
                    ""w"": {
                        ""RawValue"": 32768
                    }
                    },
                    ""localScale"": {
                    ""x"": {
                        ""RawValue"": 13107
                    },
                    ""y"": {
                        ""RawValue"": 655360
                    },
                    ""z"": {
                        ""RawValue"": 65536
                    }
                    }
                },
                ""Components"": [
                    {
                    ""type"": ""UnityEngine.MeshFilter"",
                    ""enabled"": true,
                    ""data"": ""{}""
                    },
                    {
                    ""type"": ""UnityEngine.MeshRenderer"",
                    ""enabled"": true,
                    ""data"": ""{}""
                    }
                ],
                ""RenderPrefabID"": -1
                },
                {
                ""RuntimeId"": 0,
                ""Name"": ""Cube (4)"",
                ""Children"": [],
                ""Transform"": {
                    ""localPosition"": {
                    ""x"": {
                        ""RawValue"": 327680
                    },
                    ""y"": {
                        ""RawValue"": 0
                    },
                    ""z"": {
                        ""RawValue"": 0
                    }
                    },
                    ""localRotation"": {
                    ""x"": {
                        ""RawValue"": 0
                    },
                    ""y"": {
                        ""RawValue"": 0
                    },
                    ""z"": {
                        ""RawValue"": 0
                    },
                    ""w"": {
                        ""RawValue"": 65536
                    }
                    },
                    ""localScale"": {
                    ""x"": {
                        ""RawValue"": 13107
                    },
                    ""y"": {
                        ""RawValue"": 1310720
                    },
                    ""z"": {
                        ""RawValue"": 65536
                    }
                    }
                },
                ""Components"": [
                    {
                    ""type"": ""UnityEngine.MeshFilter"",
                    ""enabled"": true,
                    ""data"": ""{}""
                    },
                    {
                    ""type"": ""UnityEngine.MeshRenderer"",
                    ""enabled"": true,
                    ""data"": ""{}""
                    },
                    {
                    ""type"": ""UnityEngine.BoxCollider"",
                    ""enabled"": true,
                    ""data"": ""{}""
                    }
                ],
                ""RenderPrefabID"": -1
                },
                {
                ""RuntimeId"": 0,
                ""Name"": ""Cylinder"",
                ""Children"": [],
                ""Transform"": {
                    ""localPosition"": {
                    ""x"": {
                        ""RawValue"": -165150
                    },
                    ""y"": {
                        ""RawValue"": 157286
                    },
                    ""z"": {
                        ""RawValue"": 0
                    }
                    },
                    ""localRotation"": {
                    ""x"": {
                        ""RawValue"": 46340
                    },
                    ""y"": {
                        ""RawValue"": 0
                    },
                    ""z"": {
                        ""RawValue"": 0
                    },
                    ""w"": {
                        ""RawValue"": 46340
                    }
                    },
                    ""localScale"": {
                    ""x"": {
                        ""RawValue"": 65536
                    },
                    ""y"": {
                        ""RawValue"": 65536
                    },
                    ""z"": {
                        ""RawValue"": 65536
                    }
                    }
                },
                ""Components"": [
                    {
                    ""type"": ""UnityEngine.MeshFilter"",
                    ""enabled"": true,
                    ""data"": ""{}""
                    },
                    {
                    ""type"": ""UnityEngine.MeshRenderer"",
                    ""enabled"": true,
                    ""data"": ""{}""
                    },
                    {
                    ""type"": ""UnityEngine.CircleCollider2D"",
                    ""enabled"": true,
                    ""data"": ""{}""
                    }
                ],
                ""RenderPrefabID"": 2
                },
                {
                ""RuntimeId"": 0,
                ""Name"": ""Cylinder (1)"",
                ""Children"": [],
                ""Transform"": {
                    ""localPosition"": {
                    ""x"": {
                        ""RawValue"": -88473
                    },
                    ""y"": {
                        ""RawValue"": -22282
                    },
                    ""z"": {
                        ""RawValue"": 0
                    }
                    },
                    ""localRotation"": {
                    ""x"": {
                        ""RawValue"": 46340
                    },
                    ""y"": {
                        ""RawValue"": 0
                    },
                    ""z"": {
                        ""RawValue"": 0
                    },
                    ""w"": {
                        ""RawValue"": 46340
                    }
                    },
                    ""localScale"": {
                    ""x"": {
                        ""RawValue"": 65536
                    },
                    ""y"": {
                        ""RawValue"": 65536
                    },
                    ""z"": {
                        ""RawValue"": 65536
                    }
                    }
                },
                ""Components"": [
                    {
                    ""type"": ""UnityEngine.MeshFilter"",
                    ""enabled"": true,
                    ""data"": ""{}""
                    },
                    {
                    ""type"": ""UnityEngine.MeshRenderer"",
                    ""enabled"": true,
                    ""data"": ""{}""
                    },
                    {
                    ""type"": ""UnityEngine.CircleCollider2D"",
                    ""enabled"": true,
                    ""data"": ""{}""
                    }
                ],
                ""RenderPrefabID"": 2
                },
                {
                ""RuntimeId"": 0,
                ""Name"": ""Cylinder (2)"",
                ""Children"": [],
                ""Transform"": {
                    ""localPosition"": {
                    ""x"": {
                        ""RawValue"": -151388
                    },
                    ""y"": {
                        ""RawValue"": -263454
                    },
                    ""z"": {
                        ""RawValue"": 0
                    }
                    },
                    ""localRotation"": {
                    ""x"": {
                        ""RawValue"": 46340
                    },
                    ""y"": {
                        ""RawValue"": 0
                    },
                    ""z"": {
                        ""RawValue"": 0
                    },
                    ""w"": {
                        ""RawValue"": 46340
                    }
                    },
                    ""localScale"": {
                    ""x"": {
                        ""RawValue"": 65536
                    },
                    ""y"": {
                        ""RawValue"": 65536
                    },
                    ""z"": {
                        ""RawValue"": 65536
                    }
                    }
                },
                ""Components"": [
                    {
                    ""type"": ""UnityEngine.MeshFilter"",
                    ""enabled"": true,
                    ""data"": ""{}""
                    },
                    {
                    ""type"": ""UnityEngine.MeshRenderer"",
                    ""enabled"": true,
                    ""data"": ""{}""
                    },
                    {
                    ""type"": ""UnityEngine.CircleCollider2D"",
                    ""enabled"": true,
                    ""data"": ""{}""
                    }
                ],
                ""RenderPrefabID"": 2
                }
            ],
            ""Transform"": {
                ""localPosition"": {
                ""x"": {
                    ""RawValue"": 0
                },
                ""y"": {
                    ""RawValue"": 0
                },
                ""z"": {
                    ""RawValue"": 0
                }
                },
                ""localRotation"": {
                ""x"": {
                    ""RawValue"": 0
                },
                ""y"": {
                    ""RawValue"": 0
                },
                ""z"": {
                    ""RawValue"": 0
                },
                ""w"": {
                    ""RawValue"": 65536
                }
                },
                ""localScale"": {
                ""x"": {
                    ""RawValue"": 65536
                },
                ""y"": {
                    ""RawValue"": 65536
                },
                ""z"": {
                    ""RawValue"": 65536
                }
                }
            },
            ""Components"": [
                {
                ""type"": ""GameTileAuth"",
                ""enabled"": true,
                ""data"": ""{\""MinutesUntilCycle\"":1.0}""
                }
            ],
            ""RenderPrefabID"": -1
            }";
    }

    [MemoryPackable(SerializeLayout.Explicit)]
    public partial class LevelMetadata
    {
        [MemoryPackOrder(0)]
        public string LevelName;

        [MemoryPackOrder(1)]
        public Rarity Rarity;

        [MemoryPackOrder(2)]
        public int MinAuctionSpots;

        [MemoryPackOrder(3)]
        public int MaxAuctionSpots;

        [MemoryPackOrder(4)]
        public int MaxRaffleDraws;

        public static LevelMetadata Default =>
            new LevelMetadata
            {
                LevelName = "Default Level",
                Rarity = Rarity.Common,
                MinAuctionSpots = 0,
                MaxAuctionSpots = 10,
                MaxRaffleDraws = 10,
            };
    }
}

using SpacetimeDB;
using LockSim;

/// <summary>
/// Example integration of LockSim physics with SpacetimeDB
/// This shows how to use the deterministic physics engine on the server
/// </summary>
public static partial class Module
{
        // Table to store serialized world state
        [SpacetimeDB.Table(Public = true)]
        public partial struct PhysicsWorldState
        {
            [SpacetimeDB.Column(ColumnAttrs.PrimaryKeyAuto)]
            public uint Id;
            
            public byte[] SerializedWorld;
            public long LastUpdateTick;
        }

        // Table to store individual bodies for querying
        [SpacetimeDB.Table(Public = true)]
        public partial struct PhysicsBody
        {
            [SpacetimeDB.Column(ColumnAttrs.PrimaryKey)]
            public int BodyId;
            
            public long PositionX;  // FP.RawValue
            public long PositionY;  // FP.RawValue
            public long Rotation;   // FP.RawValue
            public long VelocityX;  // FP.RawValue
            public long VelocityY;  // FP.RawValue
            public byte BodyType;   // 0 = Static, 1 = Dynamic
            public byte ShapeType;  // 0 = Box, 1 = Circle
        }

        // Initialize physics world
        [SpacetimeDB.Reducer]
        public static void InitPhysicsWorld(ReducerContext ctx)
        {
            // Create new world
            World world = new World();
            world.Gravity = FPVector2.FromFloats(0f, -9.81f);
            world.VelocityIterations = 8;
            world.PositionIterations = 3;

            // Create ground
            RigidBody ground = RigidBody.CreateStatic(
                0,
                FPVector2.FromFloats(0f, -5f),
                FP.Zero
            );
            ground.SetBoxShape(FP.FromFloat(20f), FP.FromFloat(1f));
            world.AddBody(ground);

            // Save to database
            SaveWorldToDatabase(world, 0);
            
            ctx.Log($"Physics world initialized with {world.Bodies.Count} bodies");
        }

        // Add a dynamic box to the world
        [SpacetimeDB.Reducer]
        public static void AddDynamicBox(ReducerContext ctx, float x, float y, float width, float height)
        {
            World world = LoadWorldFromDatabase();
            
            int nextId = world.Bodies.Count;
            RigidBody box = RigidBody.CreateDynamic(
                nextId,
                FPVector2.FromFloats(x, y),
                FP.Zero,
                FP.One // mass
            );
            box.SetBoxShape(FP.FromFloat(width), FP.FromFloat(height));
            box.Friction = FP.FromFloat(0.5f);
            box.Restitution = FP.FromFloat(0.2f);
            
            world.AddBody(box);
            SaveWorldToDatabase(world, 0);
            
            ctx.Log($"Added dynamic box at ({x}, {y})");
        }

        // Step the physics simulation forward
        [SpacetimeDB.Reducer]
        public static void StepPhysics(ReducerContext ctx)
        {
            World world = LoadWorldFromDatabase();
            
            // Fixed timestep: 20 ticks per second
            FP deltaTime = FP.FromFloat(1f / 20f);
            
            // Step simulation
            PhysicsEngine.Step(world, deltaTime);
            
            // Increment tick counter
            var worldState = ctx.Db.PhysicsWorldState.Iter().First();
            long newTick = worldState.LastUpdateTick + 1;
            
            // Save back to database
            SaveWorldToDatabase(world, newTick);
            
            ctx.Log($"Physics stepped to tick {newTick}");
        }

        // Query current bodies (for debugging/visualization)
        [SpacetimeDB.Reducer]
        public static void SyncBodiesToTable(ReducerContext ctx)
        {
            World world = LoadWorldFromDatabase();
            
            // Clear existing body records
            foreach (var body in ctx.Db.PhysicsBody.Iter())
            {
                ctx.Db.PhysicsBody.Delete(body);
            }
            
            // Insert current bodies
            foreach (var body in world.Bodies)
            {
                ctx.Db.PhysicsBody.Insert(new PhysicsBody
                {
                    BodyId = body.Id,
                    PositionX = body.Position.X.RawValue,
                    PositionY = body.Position.Y.RawValue,
                    Rotation = body.Rotation.RawValue,
                    VelocityX = body.LinearVelocity.X.RawValue,
                    VelocityY = body.LinearVelocity.Y.RawValue,
                    BodyType = (byte)body.BodyType,
                    ShapeType = (byte)body.ShapeType
                });
            }
            
            ctx.Log($"Synced {world.Bodies.Count} bodies to table");
        }

        // Helper: Save world to database
        private static void SaveWorldToDatabase(World world, long tick)
        {
            // Take snapshot
            World.Snapshot snapshot = world.TakeSnapshot();
            
            // Serialize snapshot (simplified - you'd want proper serialization)
            byte[] data = SerializeSnapshot(snapshot);
            
            // Delete existing world state
            var existing = SpacetimeDB.Runtime.Db.PhysicsWorldState.Iter().FirstOrDefault();
            if (existing != null)
            {
                SpacetimeDB.Runtime.Db.PhysicsWorldState.Delete(existing);
            }
            
            // Insert new state
            SpacetimeDB.Runtime.Db.PhysicsWorldState.Insert(new PhysicsWorldState
            {
                SerializedWorld = data,
                LastUpdateTick = tick
            });
        }

        // Helper: Load world from database
        private static World LoadWorldFromDatabase()
        {
            var worldState = SpacetimeDB.Runtime.Db.PhysicsWorldState.Iter().FirstOrDefault();
            
            if (worldState == null)
            {
                // Return empty world if none exists
                return new World();
            }
            
            // Deserialize snapshot
            World.Snapshot snapshot = DeserializeSnapshot(worldState.SerializedWorld);
            
            // Restore world from snapshot
            World world = new World();
            world.RestoreSnapshot(snapshot);
            
            return world;
        }

        // Simplified serialization (you'd want to use a proper serialization library)
        private static byte[] SerializeSnapshot(World.Snapshot snapshot)
        {
            // This is a placeholder - implement proper binary serialization
            // For production, use MessagePack, Protocol Buffers, or similar
            using (var ms = new System.IO.MemoryStream())
            using (var writer = new System.IO.BinaryWriter(ms))
            {
                // Write body count
                writer.Write(snapshot.Bodies.Length);
                
                // Write each body
                foreach (var body in snapshot.Bodies)
                {
                    writer.Write(body.Id);
                    writer.Write((byte)body.BodyType);
                    writer.Write((byte)body.ShapeType);
                    writer.Write(body.Position.X.RawValue);
                    writer.Write(body.Position.Y.RawValue);
                    writer.Write(body.Rotation.RawValue);
                    writer.Write(body.LinearVelocity.X.RawValue);
                    writer.Write(body.LinearVelocity.Y.RawValue);
                    writer.Write(body.AngularVelocity.RawValue);
                    writer.Write(body.Mass.RawValue);
                    writer.Write(body.Inertia.RawValue);
                    writer.Write(body.Friction.RawValue);
                    writer.Write(body.Restitution.RawValue);
                    
                    // Write shape data
                    if (body.ShapeType == ShapeType.Box)
                    {
                        writer.Write(body.BoxShape.HalfWidth.RawValue);
                        writer.Write(body.BoxShape.HalfHeight.RawValue);
                    }
                    else if (body.ShapeType == ShapeType.Circle)
                    {
                        writer.Write(body.CircleShape.Radius.RawValue);
                    }
                }
                
                // Write gravity and other world properties
                writer.Write(snapshot.Gravity.X.RawValue);
                writer.Write(snapshot.Gravity.Y.RawValue);
                writer.Write(snapshot.NextBodyId);
                
                return ms.ToArray();
            }
        }

        private static World.Snapshot DeserializeSnapshot(byte[] data)
        {
            // This is a placeholder - implement proper binary deserialization
            using (var ms = new System.IO.MemoryStream(data))
            using (var reader = new System.IO.BinaryReader(ms))
            {
                var snapshot = new World.Snapshot(new World());
                
                // Read body count
                int bodyCount = reader.ReadInt32();
                snapshot.Bodies = new RigidBody[bodyCount];
                
                // Read each body
                for (int i = 0; i < bodyCount; i++)
                {
                    RigidBody body = new RigidBody();
                    body.Id = reader.ReadInt32();
                    body.BodyType = (BodyType)reader.ReadByte();
                    body.ShapeType = (ShapeType)reader.ReadByte();
                    body.Position = new FPVector2(FP.FromRaw(reader.ReadInt64()), FP.FromRaw(reader.ReadInt64()));
                    body.Rotation = FP.FromRaw(reader.ReadInt64());
                    body.LinearVelocity = new FPVector2(FP.FromRaw(reader.ReadInt64()), FP.FromRaw(reader.ReadInt64()));
                    body.AngularVelocity = FP.FromRaw(reader.ReadInt64());
                    body.Mass = FP.FromRaw(reader.ReadInt64());
                    body.Inertia = FP.FromRaw(reader.ReadInt64());
                    
                    // Calculate inverse mass/inertia
                    body.InverseMass = body.Mass > FP.Zero ? FP.One / body.Mass : FP.Zero;
                    body.InverseInertia = body.Inertia > FP.Zero ? FP.One / body.Inertia : FP.Zero;
                    
                    body.Friction = FP.FromRaw(reader.ReadInt64());
                    body.Restitution = FP.FromRaw(reader.ReadInt64());
                    
                    // Read shape data
                    if (body.ShapeType == ShapeType.Box)
                    {
                        body.BoxShape = new BoxShape(
                            FP.FromRaw(reader.ReadInt64()),
                            FP.FromRaw(reader.ReadInt64())
                        );
                    }
                    else if (body.ShapeType == ShapeType.Circle)
                    {
                        body.CircleShape = new CircleShape(FP.FromRaw(reader.ReadInt64()));
                    }
                    
                    snapshot.Bodies[i] = body;
                }
                
                // Read gravity and other world properties
                snapshot.Gravity = new FPVector2(FP.FromRaw(reader.ReadInt64()), FP.FromRaw(reader.ReadInt64()));
                snapshot.NextBodyId = reader.ReadInt32();
                
                return snapshot;
            }
        }
    }
}


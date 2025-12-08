using System;
using System.Collections.Generic;
using FPMathLib;
using LockSim;
using MemoryPack;

namespace GameCoreLib
{
    [MemoryPackable(SerializeLayout.Explicit)]
    public partial class GameCoreObj
    {
        /// <summary>
        /// Stable, unique ID that persists through serialization.
        /// Used for tracking RuntimeObj-to-GameObject mappings across save/load cycles.
        /// </summary>
        [MemoryPackOrder(0)]
        public ulong RuntimeId;

        [MemoryPackOrder(1)]
        public string Name = "";

        [MemoryPackOrder(2)]
        [NonSerialized]
        public List<GameCoreObj> Children = new List<GameCoreObj>();

        [MemoryPackOrder(3)]
        public FPTransform3D Transform = new FPTransform3D();

        /// <summary>
        /// Strongly-typed components for game logic.
        /// These are the components that GameCore actually uses.
        /// </summary>
        [MemoryPackOrder(4)]
        public List<GCComponent> GameComponents = new List<GCComponent>();

        /// <summary>
        /// ID referencing which prefab to use for rendering.
        /// Only assigned to prefab roots (not children within prefabs).
        /// This allows distinguishing "this is a prefab I should instantiate" from
        /// "this is a child that's part of another prefab's definition."
        /// -1 = not a prefab root (either an empty container or part of a parent prefab)
        /// 0+ = 0-based index into RenderPrefabRegistry.Prefabs list
        /// </summary>
        [MemoryPackOrder(5)]
        public int RenderPrefabID = -1;

        /// <summary>
        /// Physics body ID in the LockSim world. -1 means no physics body.
        /// Set by RuntimePhysicsBuilder when a physics body is created for this object.
        /// Use SetWorldPos() to move objects with physics bodies to ensure synchronization.
        /// </summary>
        [MemoryPackOrder(6)]
        public int PhysicsBodyId = -1;

        /// <summary>
        /// True if this RuntimeObj has an associated physics body.
        /// </summary>
        [MemoryPackIgnore]
        public bool HasPhysicsBody => PhysicsBodyId >= 0;

        /// <summary>
        /// True if this RuntimeObj is the root of a prefab and should be instantiated.
        /// When true, the renderer instantiates this prefab and finds/links its children
        /// from the instantiated hierarchy rather than creating new GameObjects.
        /// </summary>
        [MemoryPackIgnore]
        public bool IsPrefabRoot => RenderPrefabID >= 0;

        #region Component Query Helpers

        /// <summary>
        /// Get the first component of type T on this RuntimeObj
        /// </summary>
        public T GetComponent<T>()
            where T : class
        {
            if (GameComponents == null)
                return null;
            foreach (var comp in GameComponents)
            {
                if (comp is T typed)
                    return typed;
            }
            return null;
        }

        /// <summary>
        /// Check if this RuntimeObj has a component of type T
        /// </summary>
        public bool HasComponent<T>()
            where T : class
        {
            return GetComponent<T>() != null;
        }

        /// <summary>
        /// Get all components of type T on this RuntimeObj
        /// </summary>
        public IEnumerable<T> GetComponents<T>()
            where T : class
        {
            if (GameComponents == null)
                yield break;
            foreach (var comp in GameComponents)
            {
                if (comp is T typed)
                    yield return typed;
            }
        }

        /// <summary>
        /// Add a component to this RuntimeObj.
        /// Sets the component's RuntimeObj reference automatically.
        /// </summary>
        public T AddComponent<T>(T component)
            where T : GCComponent
        {
            GameComponents ??= new List<GCComponent>();
            component.GCObj = this;
            GameComponents.Add(component);
            return component;
        }

        /// <summary>
        /// Find the first component of type T in this object or any descendant.
        /// Returns the component directly (use component.RuntimeObj to get the owning object).
        /// </summary>
        public T FindComponentInChildren<T>()
            where T : class
        {
            var comp = GetComponent<T>();
            if (comp != null)
                return comp;

            if (Children != null)
            {
                foreach (var child in Children)
                {
                    var found = child.FindComponentInChildren<T>();
                    if (found != null)
                        return found;
                }
            }
            return null;
        }

        /// <summary>
        /// Find all components of type T in this object and all descendants.
        /// </summary>
        public void FindAllComponentsInChildren<T>(List<T> results)
            where T : class
        {
            var comp = GetComponent<T>();
            if (comp != null)
                results.Add(comp);

            if (Children != null)
            {
                foreach (var child in Children)
                {
                    child.FindAllComponentsInChildren(results);
                }
            }
        }

        /// <summary>
        /// Find the first child (recursively) that has a component of type T.
        /// Returns the RuntimeObj, not the component.
        /// </summary>
        public GameCoreObj FindChildWithComponent<T>()
            where T : class
        {
            if (HasComponent<T>())
                return this;

            if (Children != null)
            {
                foreach (var child in Children)
                {
                    var found = child.FindChildWithComponent<T>();
                    if (found != null)
                        return found;
                }
            }
            return null;
        }

        /// <summary>
        /// Rebuild component RuntimeObj references after deserialization.
        /// Call this on the root after MemoryPack deserialization to restore
        /// the component.RuntimeObj back-references and component-specific references.
        /// </summary>
        public void RebuildComponentReferences()
        {
            var componentMap = new Dictionary<ulong, GCComponent>();
            BuildComponentLookup(componentMap);
            ResolveComponentCrossReferences(componentMap);
        }

        private void BuildComponentLookup(Dictionary<ulong, GCComponent> componentMap)
        {
            if (GameComponents != null)
            {
                foreach (var comp in GameComponents)
                {
                    comp.GCObj = this;

                    if (comp.ComponentId != 0)
                    {
                        if (!componentMap.TryAdd(comp.ComponentId, comp))
                        {
                            Logger.Error(
                                $"Duplicate ComponentId {comp.ComponentId} detected on '{Name}'. "
                                    + "Cross-component references may be unreliable."
                            );
                        }
                    }
                }
            }

            if (Children != null)
            {
                foreach (var child in Children)
                {
                    child.BuildComponentLookup(componentMap);
                }
            }
        }

        /// <summary>
        /// Second pass of reference rebuilding: resolve cross-references between components.
        /// Called after all RuntimeObj back-references are set.
        /// </summary>
        private void ResolveComponentCrossReferences(Dictionary<ulong, GCComponent> componentMap)
        {
            if (GameComponents != null)
            {
                foreach (var comp in GameComponents)
                {
                    if (comp is IGCComponentReferenceResolver resolver)
                    {
                        resolver.ResolveReferences(componentMap);
                    }
                }
            }

            // Recursively resolve for children
            if (Children != null)
            {
                foreach (var child in Children)
                {
                    child.ResolveComponentCrossReferences(componentMap);
                }
            }
        }

        /// <summary>
        /// Find a RuntimeObj by its RuntimeId in this hierarchy.
        /// </summary>
        public GameCoreObj FindByRuntimeId(ulong id)
        {
            if (RuntimeId == id)
                return this;

            if (Children != null)
            {
                foreach (var child in Children)
                {
                    var found = child.FindByRuntimeId(id);
                    if (found != null)
                        return found;
                }
            }
            return null;
        }

        #endregion

        #region Position Management

        /// <summary>
        /// Set this RuntimeObj's world position, synchronizing its physics body if it has one.
        /// This is the safe way to move RuntimeObjs - do not set Transform.LocalPosition directly
        /// on objects with physics bodies, as this will cause position desync.
        /// </summary>
        /// <param name="newPosition">The new position in world coordinates</param>
        /// <param name="sim">The physics world (required if this object has a physics body)</param>
        /// <param name="resetVelocity">If true, resets linear and angular velocity on teleport</param>
        public void SetWorldPos(FPVector3 newPosition, World sim = null, bool resetVelocity = true)
        {
            // Update transform position
            Transform.Position = newPosition;

            // Sync physics body if present
            if (HasPhysicsBody)
            {
                if (sim == null)
                {
                    Logger.Error(
                        $"RuntimeObj.SetWorldPos: Object '{Name}' has physics body but no World provided. "
                            + "Physics body position not updated - this will cause desync!"
                    );
                    return;
                }

                try
                {
                    var body = sim.GetBody(PhysicsBodyId);
                    body.Position = new FPVector2(newPosition.X, newPosition.Y);

                    if (resetVelocity)
                    {
                        body.LinearVelocity = FPVector2.Zero;
                        body.AngularVelocity = FP.Zero;
                    }

                    sim.SetBody(PhysicsBodyId, body);
                }
                catch (Exception e)
                {
                    Logger.Error(
                        $"RuntimeObj.SetWorldPos: Failed to update physics body: {e.Message}"
                    );
                }
            }
        }

        /// <summary>
        /// Set this RuntimeObj's world position and sync all physics bodies in the hierarchy.
        /// Only the root's Position is changed - children maintain their relative positions.
        /// Physics bodies are updated to match the computed world positions.
        /// </summary>
        /// <param name="newRootPosition">The new world position for the root object</param>
        /// <param name="sim">The physics world</param>
        /// <param name="resetVelocity">If true, resets velocity on all physics bodies</param>
        public void SetHierarchyWorldPos(
            FPVector3 newRootPosition,
            World sim,
            bool resetVelocity = true
        )
        {
            // Set this object's position
            Transform.Position = newRootPosition;

            // Sync physics for entire hierarchy, computing world positions
            SyncPhysicsPositionsRecursive(newRootPosition, sim, resetVelocity);
        }

        private void SyncPhysicsPositionsRecursive(
            FPVector3 worldPosition,
            World sim,
            bool resetVelocity
        )
        {
            // Sync physics body if present
            if (HasPhysicsBody && sim != null)
            {
                try
                {
                    var body = sim.GetBody(PhysicsBodyId);
                    body.Position = new FPVector2(worldPosition.X, worldPosition.Y);

                    if (resetVelocity)
                    {
                        body.LinearVelocity = FPVector2.Zero;
                        body.AngularVelocity = FP.Zero;
                    }

                    sim.SetBody(PhysicsBodyId, body);
                }
                catch (Exception e)
                {
                    Logger.Error(
                        $"RuntimeObj.SetHierarchyWorldPos: Failed to update physics for '{Name}': {e.Message}"
                    );
                }
            }

            // Recursively sync children's physics bodies
            // Children keep their LocalPosition (relative offset), but we compute their world position
            // for physics by adding their LocalPosition to our world position
            if (Children != null)
            {
                foreach (var child in Children)
                {
                    FPVector3 childWorldPos = worldPosition + child.Transform.LocalPosition;
                    child.SyncPhysicsPositionsRecursive(childWorldPos, sim, resetVelocity);
                }
            }
        }

        #endregion
    }
}

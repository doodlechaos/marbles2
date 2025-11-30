using System;
using System.Collections.Generic;
using FPMathLib;
using MemoryPack;

namespace GameCoreLib
{
    [MemoryPackable(SerializeLayout.Explicit)]
    public partial class RuntimeObj
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
        public List<RuntimeObj> Children = new List<RuntimeObj>();

        [MemoryPackOrder(3)]
        public FPTransform3D Transform = new FPTransform3D();

        /// <summary>
        /// Strongly-typed components for game logic.
        /// These are the components that GameCore actually uses.
        /// </summary>
        [MemoryPackOrder(4)]
        public List<GameComponent> GameComponents = new List<GameComponent>();

        /// <summary>
        /// ID referencing which prefab to use for rendering.
        /// This is automatically assigned during export based on the RuntimeRenderer's renderPrefabs list.
        /// -1 = no prefab (empty GameObject)
        /// 0+ = 0-based index into RuntimeRenderer.renderPrefabs list
        /// </summary>
        [MemoryPackOrder(5)]
        public int RenderPrefabID = -1;

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
            where T : GameComponent
        {
            GameComponents ??= new List<GameComponent>();
            component.RuntimeObj = this;
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
        public RuntimeObj FindChildWithComponent<T>()
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
        /// the component.RuntimeObj back-references.
        /// </summary>
        public void RebuildComponentReferences()
        {
            // Set RuntimeObj reference for all components on this object
            if (GameComponents != null)
            {
                foreach (var comp in GameComponents)
                {
                    comp.RuntimeObj = this;
                }
            }

            // Recursively rebuild for children
            if (Children != null)
            {
                foreach (var child in Children)
                {
                    child.RebuildComponentReferences();
                }
            }
        }

        /// <summary>
        /// Find a RuntimeObj by its RuntimeId in this hierarchy.
        /// </summary>
        public RuntimeObj FindByRuntimeId(ulong id)
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
    }
}


namespace LockSim
{
    using System.Collections.Generic;
    using FPMathLib;

    /// <summary>
    /// Candidate pair from broad phase, with canonical ordering (IdA &lt; IdB).
    /// </summary>
    public readonly struct BroadPhasePair
    {
        public readonly int ColliderIndexA;
        public readonly int ColliderIndexB;

        public BroadPhasePair(int indexA, int indexB)
        {
            // Canonical ordering ensures deterministic pair generation
            if (indexA < indexB)
            {
                ColliderIndexA = indexA;
                ColliderIndexB = indexB;
            }
            else
            {
                ColliderIndexA = indexB;
                ColliderIndexB = indexA;
            }
        }

        public override int GetHashCode() => ColliderIndexA * 73856093 ^ ColliderIndexB * 19349663;

        public override bool Equals(object obj) =>
            obj is BroadPhasePair other
            && ColliderIndexA == other.ColliderIndexA
            && ColliderIndexB == other.ColliderIndexB;
    }

    /// <summary>
    /// Deterministic spatial hash broad phase for collision detection.
    /// Reduces O(n²) to ~O(n) by only testing pairs that share grid cells.
    ///
    /// Determinism rules:
    /// - Cell coordinates derived from fixed-point positions with deterministic floor
    /// - Pairs generated in canonical order (minIndex, maxIndex)
    /// - Uses sorted cell keys to ensure iteration order is deterministic
    ///
    /// Snapshot/restore: This is scratch data stored in WorldSimulationContext.
    /// On restore, rebuild from world state - results match because rebuild is pure.
    /// </summary>
    public class BroadPhase
    {
        // Cell size in world units (fixed-point)
        private readonly FP cellSize;
        private readonly FP inverseCellSize;

        // Grid storage: cellKey -> list of collider indices
        // We use a sorted list of keys to ensure deterministic iteration
        private readonly Dictionary<long, List<int>> cells = new Dictionary<long, List<int>>();
        private readonly List<long> sortedCellKeys = new List<long>();

        // Reusable collections to avoid allocations
        private readonly HashSet<BroadPhasePair> pairSet = new HashSet<BroadPhasePair>();
        private readonly List<BroadPhasePair> pairs = new List<BroadPhasePair>();
        private readonly List<int> cellIndices = new List<int>(4);

        // Pool of index lists for cells
        private readonly Stack<List<int>> listPool = new Stack<List<int>>();

        public BroadPhase(FP cellSize)
        {
            this.cellSize = cellSize;
            this.inverseCellSize = FP.One / cellSize;
        }

        /// <summary>
        /// Clears all cells and returns lists to pool.
        /// </summary>
        public void Clear()
        {
            foreach (var kvp in cells)
            {
                kvp.Value.Clear();
                listPool.Push(kvp.Value);
            }
            cells.Clear();
            sortedCellKeys.Clear();
            pairSet.Clear();
            pairs.Clear();
        }

        /// <summary>
        /// Computes deterministic cell coordinate from world position.
        /// Uses floor division to get consistent cell assignment.
        /// </summary>
        private int GetCellCoord(FP value)
        {
            FP scaled = value * inverseCellSize;
            return FPMath.Floor(scaled).ToInt();
        }

        /// <summary>
        /// Creates a deterministic cell key from (x, y) coordinates.
        /// Uses a space-filling approach to ensure unique keys.
        /// </summary>
        private static long MakeCellKey(int x, int y)
        {
            // Pack two 32-bit ints into a 64-bit key
            return ((long)x << 32) | (uint)y;
        }

        /// <summary>
        /// Gets or creates a list for the given cell key.
        /// </summary>
        private List<int> GetOrCreateCell(long key)
        {
            if (!cells.TryGetValue(key, out List<int> list))
            {
                list = listPool.Count > 0 ? listPool.Pop() : new List<int>(8);
                cells[key] = list;
                sortedCellKeys.Add(key);
            }
            return list;
        }

        /// <summary>
        /// Inserts a collider's AABB into the spatial hash.
        /// The collider may span multiple cells.
        /// </summary>
        public void Insert(int colliderIndex, AABB aabb)
        {
            int minX = GetCellCoord(aabb.Min.X);
            int minY = GetCellCoord(aabb.Min.Y);
            int maxX = GetCellCoord(aabb.Max.X);
            int maxY = GetCellCoord(aabb.Max.Y);

            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    long key = MakeCellKey(x, y);
                    List<int> cell = GetOrCreateCell(key);
                    cell.Add(colliderIndex);
                }
            }
        }

        /// <summary>
        /// Generates candidate collision pairs from the spatial hash.
        /// Pairs are canonical (IndexA &lt; IndexB) and deduplicated.
        /// Returns pairs sorted by (IndexA, IndexB) for determinism.
        /// </summary>
        public List<BroadPhasePair> GeneratePairs()
        {
            pairSet.Clear();
            pairs.Clear();

            // Sort cell keys for deterministic iteration
            sortedCellKeys.Sort();

            // Generate pairs from each cell
            foreach (long key in sortedCellKeys)
            {
                List<int> cell = cells[key];
                int count = cell.Count;

                // Generate all unique pairs within this cell
                for (int i = 0; i < count; i++)
                {
                    for (int j = i + 1; j < count; j++)
                    {
                        var pair = new BroadPhasePair(cell[i], cell[j]);
                        if (pairSet.Add(pair))
                        {
                            pairs.Add(pair);
                        }
                    }
                }
            }

            // Sort pairs for deterministic order
            pairs.Sort(
                (a, b) =>
                {
                    int cmp = a.ColliderIndexA.CompareTo(b.ColliderIndexA);
                    return cmp != 0 ? cmp : a.ColliderIndexB.CompareTo(b.ColliderIndexB);
                }
            );

            return pairs;
        }
    }
}

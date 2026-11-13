using System;
using System.Collections.Generic;

namespace MunicipalityApp.Structures
{
    // Basic n-ary tree: municipality -> category -> status -> requests.
    internal sealed class TreeNode<T>
    {
        internal T Value;
        internal readonly List<TreeNode<T>> Children = new List<TreeNode<T>>();
        internal TreeNode(T value) { Value = value; }
        internal TreeNode<T> Add(T value) { var node = new TreeNode<T>(value); Children.Add(node); return node; }
    }

    // Shared binary node representation; BST and AVL are explicit binary trees.
    internal class BinaryNode<K, V>
    {
        internal K Key;
        internal V Value;
        internal BinaryNode<K, V> Left, Right;
        internal int Height = 1;
        internal bool Red = true;
        internal BinaryNode(K key, V value) { Key = key; Value = value; }
    }

    internal class SearchTree<K, V>
    {
        internal BinaryNode<K, V> Root;
        protected readonly IComparer<K> Comparer;
        internal SearchTree(IComparer<K> comparer = null) { Comparer = comparer ?? Comparer<K>.Default; }
        internal virtual void Put(K key, V value) { Root = Insert(Root, key, value); }
        private BinaryNode<K, V> Insert(BinaryNode<K, V> node, K key, V value)
        {
            if (node == null) return new BinaryNode<K, V>(key, value);
            int c = Comparer.Compare(key, node.Key);
            if (c < 0) node.Left = Insert(node.Left, key, value);
            else if (c > 0) node.Right = Insert(node.Right, key, value);
            else node.Value = value;
            return node;
        }
        internal bool TryGet(K key, out V value)
        {
            var node = Root;
            while (node != null)
            {
                int c = Comparer.Compare(key, node.Key);
                if (c == 0) { value = node.Value; return true; }
                node = c < 0 ? node.Left : node.Right;
            }
            value = default(V); return false;
        }
        internal IEnumerable<V> Ordered() { return Walk(Root); }
        private IEnumerable<V> Walk(BinaryNode<K, V> node)
        {
            if (node == null) yield break;
            foreach (V value in Walk(node.Left)) yield return value;
            yield return node.Value;
            foreach (V value in Walk(node.Right)) yield return value;
        }
        protected static int Height(BinaryNode<K, V> n) { return n == null ? 0 : n.Height; }
        protected static void Fix(BinaryNode<K, V> n) { n.Height = 1 + Math.Max(Height(n.Left), Height(n.Right)); }
        protected static BinaryNode<K, V> RotateLeft(BinaryNode<K, V> n)
        { var top = n.Right; n.Right = top.Left; top.Left = n; Fix(n); Fix(top); return top; }
        protected static BinaryNode<K, V> RotateRight(BinaryNode<K, V> n)
        { var top = n.Left; n.Left = top.Right; top.Right = n; Fix(n); Fix(top); return top; }
    }

    internal sealed class AvlTree<K, V> : SearchTree<K, V>
    {
        internal AvlTree(IComparer<K> comparer = null) : base(comparer) { }
        internal override void Put(K key, V value) { Root = InsertBalanced(Root, key, value); }
        private BinaryNode<K, V> InsertBalanced(BinaryNode<K, V> n, K key, V value)
        {
            if (n == null) return new BinaryNode<K, V>(key, value);
            int c = Comparer.Compare(key, n.Key);
            if (c < 0) n.Left = InsertBalanced(n.Left, key, value);
            else if (c > 0) n.Right = InsertBalanced(n.Right, key, value);
            else { n.Value = value; return n; }
            Fix(n);
            int balance = Height(n.Left) - Height(n.Right);
            if (balance > 1) { if (Comparer.Compare(key, n.Left.Key) > 0) n.Left = RotateLeft(n.Left); return RotateRight(n); }
            if (balance < -1) { if (Comparer.Compare(key, n.Right.Key) < 0) n.Right = RotateRight(n.Right); return RotateLeft(n); }
            return n;
        }
    }

    // Left-leaning red-black insertion: rotations and colour flips preserve black height.
    internal sealed class RedBlackTree<K, V> : SearchTree<K, V>
    {
        internal RedBlackTree(IComparer<K> comparer = null) : base(comparer) { }
        private static bool IsRed(BinaryNode<K, V> n) { return n != null && n.Red; }
        internal override void Put(K key, V value) { Root = InsertBalanced(Root, key, value); Root.Red = false; }
        private BinaryNode<K, V> InsertBalanced(BinaryNode<K, V> n, K key, V value)
        {
            if (n == null) return new BinaryNode<K, V>(key, value);
            int c = Comparer.Compare(key, n.Key);
            if (c < 0) n.Left = InsertBalanced(n.Left, key, value);
            else if (c > 0) n.Right = InsertBalanced(n.Right, key, value);
            else n.Value = value;
            if (IsRed(n.Right) && !IsRed(n.Left)) { bool colour = n.Red; n = RotateLeft(n); n.Red = colour; n.Left.Red = true; }
            if (IsRed(n.Left) && IsRed(n.Left.Left)) { bool colour = n.Red; n = RotateRight(n); n.Red = colour; n.Right.Red = true; }
            if (IsRed(n.Left) && IsRed(n.Right)) { n.Red = !n.Red; n.Left.Red = !n.Left.Red; n.Right.Red = !n.Right.Red; }
            return n;
        }
    }

    internal sealed class MinHeap<T>
    {
        private readonly List<T> items = new List<T>();
        private readonly Comparison<T> compare;
        internal int Count { get { return items.Count; } }
        internal MinHeap(Comparison<T> comparison) { compare = comparison; }
        internal void Push(T item)
        {
            items.Add(item); int i = items.Count - 1;
            while (i > 0) { int p = (i - 1) / 2; if (compare(items[p], items[i]) <= 0) break; Swap(i, p); i = p; }
        }
        internal T Pop()
        {
            if (Count == 0) throw new InvalidOperationException("Heap is empty.");
            T result = items[0]; items[0] = items[Count - 1]; items.RemoveAt(Count - 1);
            int i = 0;
            while (2 * i + 1 < Count)
            { int child = 2 * i + 1; if (child + 1 < Count && compare(items[child + 1], items[child]) < 0) child++;
              if (compare(items[i], items[child]) <= 0) break; Swap(i, child); i = child; }
            return result;
        }
        private void Swap(int a, int b) { T temp = items[a]; items[a] = items[b]; items[b] = temp; }
    }

    internal sealed class RequestGraph
    {
        internal sealed class Edge
        {
            internal string From, To;
            internal int Weight;
            internal Edge(string from, string to, int weight) { From = from; To = to; Weight = weight; }
        }
        private readonly Dictionary<string, List<Edge>> adjacency = new Dictionary<string, List<Edge>>(StringComparer.OrdinalIgnoreCase);
        internal void AddVertex(string id) { if (!adjacency.ContainsKey(id)) adjacency.Add(id, new List<Edge>()); }
        internal void Connect(string a, string b, int weight)
        { AddVertex(a); AddVertex(b); adjacency[a].Add(new Edge(a, b, weight)); adjacency[b].Add(new Edge(b, a, weight)); }
        internal List<string> BreadthFirst(string start)
        {
            var result = new List<string>(); if (!adjacency.ContainsKey(start)) return result;
            var visited = new HashSet<string>(StringComparer.OrdinalIgnoreCase); var queue = new Queue<string>();
            visited.Add(start); queue.Enqueue(start);
            while (queue.Count > 0) { string id = queue.Dequeue(); result.Add(id);
                foreach (var e in adjacency[id]) if (visited.Add(e.To)) queue.Enqueue(e.To); }
            return result;
        }
        // Prim from each component returns a minimum spanning forest, including isolated vertices.
        internal List<Edge> MinimumSpanningForest()
        {
            var result = new List<Edge>(); var visited = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (string start in adjacency.Keys)
            {
                if (!visited.Add(start)) continue;
                var heap = new MinHeap<Edge>((a, b) => { int c = a.Weight.CompareTo(b.Weight); return c != 0 ? c : StringComparer.Ordinal.Compare(a.From + a.To, b.From + b.To); });
                foreach (var edge in adjacency[start]) heap.Push(edge);
                while (heap.Count > 0) { var e = heap.Pop(); if (!visited.Add(e.To)) continue; result.Add(e);
                    foreach (var next in adjacency[e.To]) if (!visited.Contains(next.To)) heap.Push(next); }
            }
            return result;
        }
    }
}

using System;
using System.Collections.Generic;

namespace Slowcat.Data
{
    public class DataTree
    {
        public string Name;
        public string Description;
        public int Count
        {
            get
            {
                return Nodes.Count;
            }
        }
        private List<TreeNode> Nodes = new List<TreeNode>();

        /// <summary>
        /// Add a new root node to the DataTree
        /// </summary>
        /// <param name="newNode">New root TreeNode to be added at the end of the collection</param>
        public void Add(TreeNode newNode)
        {
            newNode.Parent = null;
            foreach(TreeNode root in Nodes)
            {
                root.Siblings.Add(newNode);
                newNode.Siblings.Add(root);
            }
            newNode.SetDepth(1);
            Nodes.Add(newNode);
        }

        public TreeNode this[int index]
        {
            get => index < Nodes.Count ? Nodes[index] : null;
        }

        /// <summary>
        /// Remove all root nodes and children from DataTree
        /// </summary>
        public void RemoveAll()
        {
            Nodes = new List<TreeNode>();
        }

        /// <summary>
        /// Remove a specific root node and all children from DataTree
        /// </summary>
        /// <param name="index">Index of root node to remove</param>
        public void RemoveAt(int index)
        {
            if (index < Nodes.Count)
                Remove(Nodes[index]);
        }

        /// <summary>
        /// Remove a specific root node and all children from DataTree
        /// </summary>
        /// <param name="deadNode">Available root node to remove</param>
        public void Remove(TreeNode deadNode)
        {
            foreach (TreeNode child in Nodes)
                child.Siblings.Remove(deadNode);
            Nodes.Remove(deadNode);
        }

        /// <summary>
        /// Calculate the width of the tree at the requested depth. Returned value is the number of nodes with the same depth
        /// </summary>
        public int Width(int depth)
        {
            foreach(TreeNode child in Nodes)
            {

            }
            return 0;
        }

        /// <summary>
        /// Calculate the depth of the tree. Returned value is the node with the most parents.
        /// </summary>
        public int Depth()
        {
            int depth = 0;
            foreach (TreeNode child in Nodes)
                depth = Math.Max(depth, Depth(child, depth));
            return depth;
        }

        private int Depth(TreeNode parent, int initialDepth)
        {
            int depth = initialDepth;
            foreach (TreeNode child in parent.Children)
                depth = Math.Max(depth, Math.Max(child.Depth, Depth(child, depth)));
            return depth;
        }
    }
}

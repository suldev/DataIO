using System;
using System.Collections.Generic;

namespace DataTree
{
    public class DataTree
    {
        public string Name;
        public string Description;
        private List<TreeNode> Nodes;
        private int _depth;

        public void Add(TreeNode newNode)
        {
            newNode.Parent = null;
            foreach(TreeNode root in Nodes)
            {
                root.Siblings.Add(newNode);
                newNode.Siblings.Add(root);
            }
            Nodes.Add(newNode);
        }

        public TreeNode this[int index]
        {
            get => index < Nodes.Count ? Nodes[index] : null;
        }

        public void RemoveAll()
        {
            Nodes = new List<TreeNode>();
        }

        public void RemoveAt(int index)
        {
            if (index < Nodes.Count)
                Remove(Nodes[index]);
        }

        public void Remove(TreeNode deadNode)
        {
            foreach (TreeNode child in Nodes)
                child.Siblings.Remove(deadNode);
            Nodes.Remove(deadNode);
        }

        public int Depth()
        {
            _depth = 0;
            int ddepth = 1;
            foreach (TreeNode node in Nodes)
                Depth(node, ddepth);
            return _depth;
        }

        private void Depth(TreeNode parent, int initialDepth)
        {
            _depth = Math.Max(_depth, initialDepth++);
            foreach (TreeNode child in parent.Children)
                Depth(child, initialDepth);
        }
    }
}

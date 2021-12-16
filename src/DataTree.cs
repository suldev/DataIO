using System;
using System.Collections.Generic;

namespace DataTree
{
    public class DataTree
    {
        public string Name;
        public string Description;
        private List<TreeNode> Nodes;

        void Add(TreeNode newNode)
        {
            newNode.Parent = null;
            newNode.Siblings = new List<TreeNode>();
            foreach(TreeNode child in Nodes)
            {
                child.Siblings.Add(newNode);
                newNode.Siblings.Add(child);
            }
            Nodes.Add(newNode);
        }

        public TreeNode this[uint index]
        {
            get => index < Nodes.Count ? Nodes[index] : null;
        }

        public void RemoveAll()
        {
            Nodes = new List<TreeNode>();
        }

        public void RemoveAt(uint index)
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
    }
}

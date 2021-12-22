using System;
using System.Collections.Generic;

namespace Slowcat.Data
{
    public class TreeNode
    {
        internal TreeNode Parent = null;
        internal List<TreeNode> Children = new List<TreeNode>();
        internal List<TreeNode> Siblings = new List<TreeNode>();
        internal int Depth = -1;
        public int Count
        {
            get
            {
                return Children.Count;
            }
        }
        public object Value;
        
        public void Add(TreeNode newNode)
        {
            newNode.Parent = this;
            newNode.Siblings = new List<TreeNode>();
            foreach (TreeNode child in Children)
            {
                child.Siblings.Add(newNode);
                newNode.Siblings.Add(child);
            }
            newNode.Depth = Depth + 1;
            Children.Add(newNode);
        }

        public void Insert(TreeNode newNode, int index)
        {
            foreach (TreeNode child in Children)
                child.Siblings.Insert(index, newNode);
            Children.Insert(index, newNode);
        }

        public TreeNode this[int index]
        {
            get => index < Children.Count ? Children[index] : null;
        }

        public TreeNode Sibling(int index)
        {
            if (index > 0 || index < Siblings.Count)
                return Siblings[index];
            return null;
        }

        public void RemoveAll()
        {
            Children = new List<TreeNode>();
        }

        public void RemoveAt(int index)
        {
            if (index < Children.Count)
                Remove(Children[index]);
        }

        public void Remove(TreeNode deadNode)
        {
            foreach (TreeNode child in Children)
                child.Siblings.Remove(deadNode);
            Children.Remove(deadNode);
        }

        internal void SetDepth(int depth)
        {
            Depth = depth;
            foreach (TreeNode child in Children)
                SetDepth(Depth + 1);
        }
    }
}
using System;
using System.Collections.Generic;

namespace DataTree
{
    public class TreeNode
    {
        internal TreeNode Parent = null;
        private List<TreeNode> Children = new List<TreeNode>();
        internal List<TreeNode> Siblings = new List<TreeNode>();
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
    }
}
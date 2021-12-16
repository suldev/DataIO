using System;
using System.Collections.Generic;

namespace DataTree
{
    public class TreeNode
    {
        private TreeNode Parent;
        private List<TreeNode> Children;
        private List<TreeNode> Siblings;
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

        public void Insert(TreeNode newNode, uint index)
        {
            foreach (TreeNode child in Children)
                child.Siblings.Insert(index, newNode);
            Children.Insert(index, newNode);
        }

        public TreeNode this[uint index]
        {
            get => index < Children.Count ? Children[index] : null;
        }

        public void RemoveAll()
        {
            Children = new List<TreeNode>();
        }

        public void RemoveAt(uint index)
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
using Slowcat.Data;
using System;

namespace DataIOTest
{
    class DataTree_Test : Base_Test
    {
        DataTree tree;
        public void TestAll()
        {
            TestClass = "DataTree";
            CreateTree();
            CreateNode();
            RemoveNode();
            AddSibling();
            RemoveSibling();
            AddChild();
            RemoveChild();
            DepthCheckZero();
            DepthCheckShallow();
            DepthCheckDeep();
            AddValue();
            ChangeValue();
        }

        private void CreateTree()
        {
            InitializeTest("Create DataTree");
            tree = new DataTree();
            try
            {
                ConditionTest<int>(tree.Count, Condition.EQ, 0);
            }
            catch(Exception e)
            {
                ConsoleMessage(State.FAILED, null, null);
                DisplayException(e);
            }
        }

        private void CreateNode()
        {
            InitializeTest("Create node on DataTree");
            tree = new DataTree();
            try
            {
                tree.Add(new TreeNode());
                ConditionTest<int>(tree.Count, Condition.EQ, 1);
            }
            catch(Exception e)
            {
                ConsoleMessage(State.FAILED, null, null);
                DisplayException(e);
            }
        }

        private void RemoveNode()
        {
            InitializeTest("Remove node from DataTree");
            tree = new DataTree();
            try
            {
                tree.Add(new TreeNode());
                tree.RemoveAt(0);
                ConditionTest<int>(tree.Count, Condition.EQ, 0);
            }
            catch(Exception e)
            {
                ConsoleMessage(State.FAILED, null, null);
                DisplayException(e);
            }
        }

        private void AddSibling()
        {
            InitializeTest("Check if two nodes are siblings");
            tree = new DataTree();
            try
            {
                tree.Add(new TreeNode());
                tree.Add(new TreeNode());
                ConditionTest<TreeNode>(tree[0].Sibling(0), Condition.EQ, tree[1]);
            }
            catch(Exception e)
            {
                ConsoleMessage(State.FAILED, null, null);
                DisplayException(e);
            }
        }

        private void RemoveSibling()
        {
            InitializeTest("Check if removing a node removes sibling");
            tree = new DataTree();
            try
            {
                tree.Add(new TreeNode());
                tree.Add(new TreeNode());
                tree.RemoveAt(1);
                ConditionTest<TreeNode>(tree[0].Sibling(0), Condition.EQ, null);
            }
            catch(Exception e)
            {
                ConsoleMessage(State.FAILED, null, null);
                DisplayException(e);
            }
        }

        private void AddChild()
        {
            InitializeTest("Add a child to an existing node");
            tree = new DataTree();
            try
            {
                tree.Add(new TreeNode());
                tree[0].Add(new TreeNode());
                ConditionTest<TreeNode>(tree[0][0], Condition.NE, null);
            }
            catch(Exception e)
            {
                ConsoleMessage(State.FAILED, null, null);
                DisplayException(e);
            }
        }

        private void RemoveChild()
        {
            InitializeTest("Check if a child is properly removed");
            tree = new DataTree();
            try
            {
                tree.Add(new TreeNode());
                tree[0].Add(new TreeNode());
                tree[0].RemoveAt(0);
                ConditionTest<int>(tree[0].Count, Condition.EQ, 0);
            }
            catch(Exception e)
            {
                ConsoleMessage(State.FAILED, null, null);
                DisplayException(e);
            }
        }


        private void DepthCheckZero()
        {
            InitializeTest("Check zero depth");
            tree = new DataTree();
            try
            {
                ConditionTest<int>(tree.Depth(), Condition.EQ, 0);
            }
            catch(Exception e)
            {
                ConsoleMessage(State.FAILED, null, null);
                DisplayException(e);
            }
        }

        private void DepthCheckShallow()
        {
            InitializeTest("Check shallow depth");
            tree = new DataTree();
            try
            {
                tree.Add(new TreeNode());
                tree.Add(new TreeNode());
                tree[1].Add(new TreeNode());
                ConditionTest<int>(tree.Depth(), Condition.EQ, 2);
            }
            catch(Exception e)
            {
                ConsoleMessage(State.FAILED, null, null);
                DisplayException(e);
            }
        }

        private void DepthCheckDeep()
        {
            InitializeTest("Check deep depth");
            tree = new DataTree();
            try
            {
                tree.Add(new TreeNode());
                tree.Add(new TreeNode());
                tree.Add(new TreeNode());
                tree.Add(new TreeNode());
                tree[1].Add(new TreeNode());
                tree[2].Add(new TreeNode());
                tree[2].Add(new TreeNode());
                tree[3].Add(new TreeNode());
                tree[3][0].Add(new TreeNode());
                tree[3][0][0].Add(new TreeNode());
                tree[3].Add(new TreeNode());
                tree[3][1].Add(new TreeNode());
                tree[3][1][0].Add(new TreeNode());
                tree[3][1][0][0].Add(new TreeNode());
                tree.Add(new TreeNode());
                ConditionTest<int>(tree.Depth(), Condition.EQ, 5);
            }
            catch(Exception e)
            {
                ConsoleMessage(State.FAILED, null, null);
                DisplayException(e);
            }
        }

        private void AddValue()
        {
            InitializeTest("Add value to node");
            tree = new DataTree();
            try
            {
                tree.Add(new TreeNode());
                tree[0].Value = 5;
                ConditionTest<int>(Convert.ToInt32(tree[0].Value), Condition.EQ, 5);
            }
            catch(Exception e)
            {
                ConsoleMessage(State.FAILED, null, null);
                DisplayException(e);
            }
        }

        private void ChangeValue()
        {
            InitializeTest("Change value to node");
            tree = new DataTree();
            try
            {
                tree.Add(new TreeNode());
                tree[0].Value = 5;
                int oldValue = Convert.ToInt32(tree[0].Value);
                tree[0].Value = "String";
                ConditionTest<string>(tree[0].Value.ToString(), Condition.EQ, "String");
            }
            catch(Exception e)
            {
                ConsoleMessage(State.FAILED, null, null);
                DisplayException(e);
            }
        }
    }
}

# DataTree
Abstract tree of object data, with relationships from each node. Nodes contain object data independent of other node data, whose data types are also independent

# How to Use
1. A `DataTree` contains a series of `TreeNodes` at the root. You may have as many root nodes as necessary.
2. A `TreeNode` contains relational pointers, such as its parent node, children, and sibling nodes
3. `TreeNode`s also contain an object Value, where data can be stored in the tree
4. Data from one `TreeNode` can be different from the next. For example, a `TreeNode` may contain an int, while its children could contain strings
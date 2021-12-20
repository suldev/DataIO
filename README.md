# DataIO Description
This .NET 5.0 project aims to simplify data management, either via in-memory data tree collections, or SQLite read/write and manipulation.

# How to Use
This project uses .NET 5.0

## DataTree
- A `DataTree` is a collection of `TreeNodes`
- Each `TreeNode` contains three parts: a parent node, children nodes, and object data
- There is between 0 and 1 parent node for each `TreeNode`. Root nodes do not have parents, and as such the pointer is null
- `TreeNode` data is assigned to the `object Value` property

## SQLite
- Each instance of `SQLite` contains a single database connection (optional), and an in-memory database via a `DataTable` collection
- Assign a database file when constructing an instance of this class `SQLite dataBase = new SQLite("databaseFile.db")`
- By default, the database is opened in ReadOnly mode. Use `SQLite dataBase = new SQLite("databaseFile.db", FileMode.ReadWrite)` to write to the database as well
- Create new DataTables using `dataBase.AddTable(new DataTable("NewTableName"))`
- Create new DataColumns, e.g. an integer column, using `dataBase.AddColumn("NewTableName", "NewColumnName", DataType.Int, true)`
- Create new DataRows using `dataBase.AddRow("NewTableName")`
- Set values in each cell using `dataBase.SetData("NewTableName", "NewColumnName", 0, 4, 100)` where 0 is the row, 4 is the column and 100 is the integer value
- Commit changes to memory using the `dataBase.Commit()` command. Be sure to write changes to disk using `dataBase.Write()`
- Rollback any changes made since the last Commit using `dataBase.Rollback()`
- Data can be read from cells as well by using the associated column datatype `int newValue = dataBase.GetData<int>("NewTableName", 0, 4)`

#Testing
Currently, testing is limited to local. Run the DataIO_Test project (exe) to run the tests

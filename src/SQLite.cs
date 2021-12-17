using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace Slowcat.Data
{
    public class SQLite
    {
        public enum FileMode
        {
            ReadOnly,
            ReadWrite,
            CreateWrite
        }

        public enum DataType
        {
            Int,
            Float,
            Double,
            String
        }
        
        private readonly string FilePath;
        private List<DataTable> SQLiteDataTables = new List<DataTable>();
        private List<DataTable> MemoryDataTables = new List<DataTable>();
        private SqliteConnection SqliteFile;
        private readonly FileMode Mode;

        /// <summary>
        /// SQLite constructor.
        /// Set the SQLite file to read from (read only mode)
        /// </summary>
        /// <param name="path">Absolute path to the SQLite database to be manipulated</param>
        public SQLite(string path)
        {
            FilePath = path;
            Mode = FileMode.ReadOnly;
        }

        /// <summary>
        /// SQLite constructor.
        /// Set the SQLite file and access mode
        /// </summary>
        /// <param name="path">Absolute path to the SQLite database to be manipulated</param>
        public SQLite(string path, FileMode mode)
        {
            FilePath = path;
            Mode = mode;
        }

        public bool Open()
        {
            SqliteConnectionStringBuilder connection = new SqliteConnectionStringBuilder();
            connection.DataSource = FilePath;
            switch (Mode)
            {
                case FileMode.CreateWrite:
                    connection.Mode = SqliteOpenMode.ReadWriteCreate;
                    break;
                case FileMode.ReadWrite:
                    connection.Mode =  SqliteOpenMode.ReadWrite;
                    break;
                default:
                    connection.Mode = SqliteOpenMode.ReadOnly;
                    break;
            }
            SqliteFile = new SqliteConnection(connection.ConnectionString);
            SqliteFile.Open();
            if (SqliteFile.State == ConnectionState.Closed)
                return false;
            List<DataTable> tables = new List<DataTable>();
            DataTable metaTable = new DataTable();
            SqliteCommand cmd = new SqliteCommand("SELECT name FROM sqlite_master WHERE type='table' and name NOT LIKE 'sqlite_%'", SqliteFile);
            metaTable.Load(cmd.ExecuteReader(CommandBehavior.KeyInfo));
            foreach (DataRow row in metaTable.Rows)
            {
                string tableName = row[0].ToString();
                DataTable dataTable = new DataTable(tableName);
                cmd = new SqliteCommand("SELECT * FROM " + tableName, SqliteFile);
                SqliteDataReader dbReader = cmd.ExecuteReader();
                dataTable.Load(dbReader);
                SQLiteDataTables.Add(dataTable);
                dbReader.Close();
            }
            MemoryDataTables = SQLiteDataTables;
            return true;
        }

        /// <summary>
        /// Save all changes to disk
        /// </summary>
        public bool Write()
        {
            if (Mode == FileMode.ReadOnly)
                return false;
            if (SqliteFile.State == ConnectionState.Closed)
                return false;
            if (SQLiteDataTables.Count < 1)
                return false;
            foreach(DataTable dataTable in SQLiteDataTables)
            {
                // Skip over this table if the name is empty
                if (string.IsNullOrEmpty(dataTable.TableName))
                    continue;

                SqliteCommand cmd;
                string command;
                string[] colName = new string[dataTable.Columns.Count];

                // Remove table if it already exists
                command = "DROP TABLE IF EXISTS " + dataTable.TableName;
                cmd = new SqliteCommand(command, SqliteFile);
                cmd.ExecuteNonQuery();

                // Create new table
                command = "CREATE TABLE ";
                command += dataTable.TableName + "(";
                foreach (DataColumn col in dataTable.Columns)
                {
                    if (col.Ordinal > 0)
                        command += ", ";
                    colName[col.Ordinal] += col.ColumnName;
                    command += col.ColumnName;
                    Type ty = col.DataType;
                    if (ty == Type.GetType("System.String"))
                        command += " TEXT";
                    else if (ty == Type.GetType("System.Int64"))
                        command += " INT";
                    else if (ty == Type.GetType("System.Single") || ty == Type.GetType("System.Double"))
                        command += " REAL";
                    else
                        throw new SqliteException("Invalid data type given during WriteOut", -1);
                    if (col.AllowDBNull == false)
                        command += " NOT NULL";
                }
                command += ")";
                cmd = new SqliteCommand(command, SqliteFile);
                cmd.ExecuteNonQuery();

                foreach (DataRow row in dataTable.Rows)
                {
                    command = "INSERT INTO " + dataTable.TableName + "(";
                    foreach (DataColumn col in dataTable.Columns)
                    {
                        if (col.Ordinal > 0)
                            command += ", ";
                        command += col.ColumnName;
                    }
                    command += ") VALUES(";
                    foreach (DataColumn col in dataTable.Columns)
                    {
                        if (col.Ordinal > 0)
                            command += ", ";
                        Type ty = col.DataType;
                        if (ty == Type.GetType("System.String"))
                            command += "'" + row[col.Ordinal] + "'";
                        else if (ty == Type.GetType("System.Int64"))
                            command += row[col.Ordinal];
                        else if (ty == Type.GetType("System.Single") || ty == Type.GetType("System.Double"))
                            command += row[col.Ordinal];
                        else
                            throw new SqliteException("Invalid data type given during WriteOut", -1);
                    }
                    command += ")";
                    cmd = new SqliteCommand(command, SqliteFile);
                    cmd.ExecuteNonQuery();
                }
            }
            return true;
        }

        /// <summary>
        /// Save all changes into DataTable Memory.
        /// This command does not write to the SQLiteDatabase
        /// Use Write() after commit to save changes to disk
        /// </summary>
        public void Commit()
        {
            SQLiteDataTables = MemoryDataTables;
        }

        /// <summary>
        /// Toss all unsaved changes
        /// </summary>
        public void RollBack()
        {
            MemoryDataTables = SQLiteDataTables;
        }

        /// <summary>
        /// Move existing DataTable into SQLite class memory
        /// </summary>
        /// <param name="newTable">Existing DataTable</param>
        public bool AddTable(DataTable newTable)
        {
            if (newTable == null || MemoryDataTables.Exists(x => x.TableName == newTable.TableName))
                return false;
            MemoryDataTables.Add(newTable);
            return true;
        }

        /// <summary>
        /// Create a blank in-memory table
        /// </summary>
        /// <param name="tableName">Name for the new table</param>
        public bool NewTable(string tableName)
        {
            if (MemoryDataTables.Exists(x => x.TableName == tableName))
                return false;
            MemoryDataTables.Add(new DataTable(tableName));
            return true;
        }

        /// <summary>
        /// Add new column to specified in-memory DataTable
        /// </summary>
        /// <param name="tableName">Name of existing in-memory DataTable to add to</param>
        /// <param name="columnName">Name of new column</param>
        /// <param name="type">Type of new column</param>
        /// <param name="setNull">SQLite null state - true to allow null, false to prohibit null</param>
        public void AddColumn(string tableName, string columnName, DataType type, bool setNull)
        {
            int tableIndex = MemoryDataTables.FindIndex(x => x.TableName == tableName);
            if (tableIndex < 0)
                throw new IndexOutOfRangeException("Selected table does not exist");
            DataColumn col = new DataColumn(columnName);
            switch (type)
            {
                case DataType.Int:
                    col.DataType = System.Type.GetType("System.Int64");
                    if (!setNull)
                        col.DefaultValue = 0;
                    break;
                case DataType.Float:
                    col.DataType = System.Type.GetType("System.Single");
                    if (!setNull)
                        col.DefaultValue = 0f;
                    break;
                case DataType.Double:
                    col.DataType = System.Type.GetType("System.Double");
                    if (!setNull)
                        col.DefaultValue = 0.0;
                    break;
                case DataType.String:
                    col.DataType = System.Type.GetType("System.String");
                    if (!setNull)
                        col.DefaultValue = "";
                    break;
                default:
                    throw new Exception("Invalid data type for column definition");
            }
            col.AllowDBNull = setNull;
            MemoryDataTables[tableIndex].Columns.Add(col);
        }

        /// <summary>
        /// Add row to existing table using the associated column template
        /// </summary>
        /// <param name="tableName">Name of existing in-memory DataTable to add to</param>
        public void AddRow(string tableName)
        {
            int tableIndex = MemoryDataTables.FindIndex(x => x.TableName == tableName);
            if (tableIndex < 0)
                throw new IndexOutOfRangeException("Selected table does not exist");
            if (MemoryDataTables[tableIndex].Columns.Count < 1)
                return;
            MemoryDataTables[tableIndex].Rows.Add();
        }

        /// <summary>
        /// Update String value of specified cell in specified DataTable
        /// </summary>
        /// <param name="tableName">Name of existing in-memory DataTable to add to</param>
        /// <param name="row">Row number of specified DataTable to add to</param>
        /// <param name="col">Column number of specified DataTable to add to</param>
        /// <param name="data">The value to add to cell</param>
        public void SetData(string tableName, int row, int col, string data)
        {
            int tableIndex = MemoryDataTables.FindIndex(x => x.TableName == tableName);
            if (tableIndex < 0)
                throw new IndexOutOfRangeException("Selected table does not exist");
            if (col >= MemoryDataTables[tableIndex].Columns.Count)
                throw new IndexOutOfRangeException("Column index is out of range");
            if (row >= MemoryDataTables[tableIndex].Rows.Count)
                throw new IndexOutOfRangeException("Row index is out of range");
            if (MemoryDataTables[tableIndex].Columns[col].DataType != Type.GetType("System.String"))
                throw new ArgumentException(tableName + ": Cell [" + row.ToString() + "," + col.ToString() + "] is not defined as a string");
            MemoryDataTables[tableIndex].Rows[row][col] = data;
        }

        /// <summary>
        /// Update integer value of specified cell in specified DataTable
        /// </summary>
        /// <param name="tableName">Name of existing in-memory DataTable to add to</param>
        /// <param name="row">Row number of specified DataTable to add to</param>
        /// <param name="col">Column number of specified DataTable to add to</param>
        /// <param name="data">The value to add to cell</param>
        public void SetData(String tableName, int row, int col, int data)
        {
            int tableIndex = MemoryDataTables.FindIndex(x => x.TableName == tableName);
            if (tableIndex < 0)
                throw new IndexOutOfRangeException("Selected table does not exist");
            if (col >= MemoryDataTables[tableIndex].Columns.Count)
                throw new IndexOutOfRangeException("Column index is out of range");
            if (row >= MemoryDataTables[tableIndex].Rows.Count)
                throw new IndexOutOfRangeException("Row index is out of range");
            if (MemoryDataTables[tableIndex].Columns[col].DataType != Type.GetType("System.Int32") && MemoryDataTables[tableIndex].Columns[col].DataType != Type.GetType("System.Int64"))
                throw new ArgumentException(tableName + ": Cell [" + row.ToString() + "," + col.ToString() + "] is not defined as a int nor int64");
            MemoryDataTables[tableIndex].Rows[row][col] = data;
        }

        /// <summary>
        /// Update float value of specified cell in specified DataTable
        /// </summary>
        /// <param name="tableName">Name of existing in-memory DataTable to add to</param>
        /// <param name="row">Row number of specified DataTable to add to</param>
        /// <param name="col">Column number of specified DataTable to add to</param>
        /// <param name="data">The value to add to cell</param>
        public void SetData(String tableName, int row, int col, float data)
        {
            int tableIndex = MemoryDataTables.FindIndex(x => x.TableName == tableName);
            if (tableIndex < 0)
                throw new IndexOutOfRangeException("Selected table does not exist");
            if (col >= MemoryDataTables[tableIndex].Columns.Count)
                throw new IndexOutOfRangeException("Column index is out of range");
            if (row >= MemoryDataTables[tableIndex].Rows.Count)
                throw new IndexOutOfRangeException("Row index is out of range");
            if (MemoryDataTables[tableIndex].Columns[col].DataType != Type.GetType("System.Single"))
                throw new ArgumentException(tableName + ": Cell [" + row.ToString() + "," + col.ToString() + "] is not defined as a single precision");
            MemoryDataTables[tableIndex].Rows[row][col] = data;
        }

        /// <summary>
        /// Update double value of specified cell in specified DataTable
        /// </summary>
        /// <param name="tableName">Name of existing in-memory DataTable to add to</param>
        /// <param name="row">Row number of specified DataTable to add to</param>
        /// <param name="col">Column number of specified DataTable to add to</param>
        /// <param name="data">The value to add to cell</param>
        public void SetData(String tableName, int row, int col, double data)
        {
            int tableIndex = MemoryDataTables.FindIndex(x => x.TableName == tableName);
            if (tableIndex < 0)
                throw new IndexOutOfRangeException("Selected table does not exist");
            if (col >= MemoryDataTables[tableIndex].Columns.Count)
                throw new IndexOutOfRangeException("Column index is out of range");
            if (row >= MemoryDataTables[tableIndex].Rows.Count)
                throw new IndexOutOfRangeException("Row index is out of range");
            if (MemoryDataTables[tableIndex].Columns[col].DataType != Type.GetType("System.Double"))
                throw new ArgumentException(tableName + ": Cell [" + row.ToString() + "," + col.ToString() + "] is not defined as a double precision");
            MemoryDataTables[tableIndex].Rows[row][col] = data;
        }

        /// <summary>
        /// Return in-memory DataTable to local DataTable
        /// </summary>
        /// <param name="tableName">Name of existing in-memory DataTable</param>
        /// <returns></returns>
        public DataTable GetDataTable(string tableName)
        {
            int tableIndex = MemoryDataTables.FindIndex(x => x.TableName == tableName);
            if (tableIndex < 0)
                throw new IndexOutOfRangeException("Selected table does not exist");
            return MemoryDataTables[tableIndex];
        }

        /// <summary>
        /// Return all in-memory DataTables to local DataTable list
        /// </summary>
        /// <returns></returns>
        public List<DataTable> GetDataTables()
        {
            if (MemoryDataTables.Count < 0)
                throw new IndexOutOfRangeException("No tables exist");
            return MemoryDataTables;
        }

        /// <summary>
        /// Get value from specified cell of in-memory DataTable
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public object GetData(string tableName, int row, int col)
        {
            int tableIndex = MemoryDataTables.FindIndex(x => x.TableName == tableName);
            if (tableIndex < 0)
                throw new IndexOutOfRangeException("Selected table does not exist");
            if (col >= MemoryDataTables[tableIndex].Columns.Count)
                throw new IndexOutOfRangeException("Column index is out of range");
            if (row >= MemoryDataTables[tableIndex].Rows.Count)
                throw new IndexOutOfRangeException("Row index is out of range");
            return MemoryDataTables[tableIndex].Rows[row][col];
        }

        /// <summary>
        /// Return name of column from specified in-memory DataTable
        /// </summary>
        /// <param name="tableName">Name of in-memory DataTable</param>
        /// <param name="col">Column index</param>
        /// <returns></returns>
        public string GetColumnName(string tableName, int col)
        {
            int tableIndex = MemoryDataTables.FindIndex(x => x.TableName == tableName);
            if (tableIndex < 0)
                throw new IndexOutOfRangeException("Selected table does not exist");
            if (col >= MemoryDataTables[tableIndex].Columns.Count)
                throw new IndexOutOfRangeException("Column index is out of range");
            return MemoryDataTables[tableIndex].Columns[col].ColumnName;
        }

        /// <summary>
        /// Return datatype of column from specified in-memory DataTable
        /// </summary>
        /// <param name="tableName">Name of in-memory DataTable</param>
        /// <param name="col">Column index</param>
        /// <returns></returns>
        public Type GetColumnType(string tableName, int col)
        {
            int tableIndex = MemoryDataTables.FindIndex(x => x.TableName == tableName);
            if (tableIndex < 0)
                throw new IndexOutOfRangeException("Selected table does not exist");
            if (col >= MemoryDataTables[tableIndex].Columns.Count)
                throw new IndexOutOfRangeException("Column index is out of range");
            return MemoryDataTables[tableIndex].Columns[col].DataType;
        }

        /// <summary>
        /// Retrieves the total number of columns found in the specified in-memory DataTable
        /// </summary>
        /// <param name="tableName">Name of in-memory DataTable</param>
        /// <returns></returns>
        public int GetColumnCount(string tableName)
        {
            int tableIndex = MemoryDataTables.FindIndex(x => x.TableName == tableName);
            if (tableIndex < 0)
                throw new IndexOutOfRangeException("Selected table does not exist");
            return MemoryDataTables[tableIndex].Columns.Count;
        }

        /// <summary>
        /// Retrieves the total number of rows found in the specified in-memory DataTable
        /// </summary>
        /// <param name="tableName">Name of in-memory DataTable</param>
        /// <returns></returns>
        public int GetRowCount(string tableName)
        {
            int tableIndex = MemoryDataTables.FindIndex(x => x.TableName == tableName);
            if (tableIndex < 0)
                throw new IndexOutOfRangeException("Selected table does not exist");
            return MemoryDataTables[tableIndex].Rows.Count;
        }

        /// <summary>
        /// Deletes an in-memory DataTable
        /// Don't forget to Commit() changes
        /// </summary>
        /// <param name="tableName">Name of the DataTable to be removed</param>
        public void RemoveTable(string tableName)
        {
            int tableIndex = MemoryDataTables.FindIndex(x => x.TableName == tableName);
            if (tableIndex < 0)
                return;
            MemoryDataTables.RemoveAt(tableIndex);
        }

        /// <summary>
        /// Deletes all in-memory DataTables.
        /// Don't forget to Commit() changes
        /// </summary>
        public void RemoveAll()
        {
            MemoryDataTables.Clear();
        }
    }
}

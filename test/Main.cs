using System;

namespace DataIOTest
{
    class DataIOTestExe
    {
        static void Main(string[] args)
        {
            DataTree_Test dataTreeTest = new DataTree_Test();
            SQLite_Test sqliteTest = new SQLite_Test();
            Console.WriteLine("DataIO Test Suite");
            Console.WriteLine("Slowcat Labs\n\r");
            dataTreeTest.TestAll();
            sqliteTest.TestAll();
        }
    }
}

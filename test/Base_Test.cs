using System;

namespace DataIOTest
{
    public class Base_Test
    {
        protected const ConsoleColor defaultColor = ConsoleColor.White;
        private string _testClass;
        private int _index;
        protected string TestClass
        {
            get
            {
                return _testClass;
            }
            set
            {
                _testClass = value;
                _index = -1;
            }
        }
        protected string TestDescription;

        public enum State
        {
            WAITING,
            RUNNING,
            PASSED,
            FAILED
        }
        protected void ConsoleMessage(State state)
        {
            string sState;
            bool newLine;
            Console.Write("[" + _testClass + "]");
            Console.Write("[" + _index.ToString("D3") + "][");
            switch (state)
            {
                case State.RUNNING:
                    sState = "RUNNING";
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    newLine = false;
                    break;
                case State.PASSED:
                    sState = "PASSED ";
                    Console.ForegroundColor = ConsoleColor.Green;
                    newLine = true;
                    break;
                case State.FAILED:
                    sState = "FAILED ";
                    Console.ForegroundColor = ConsoleColor.Red;
                    newLine = true;
                    break;
                default:
                    sState = "WAITING";
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    newLine = false;
                    break;
            }
            
            Console.Write(sState);
            Console.ForegroundColor = defaultColor;
            Console.Write("] " + TestDescription);
            if (newLine)
                Console.Write("\n\r");
            else
                Console.Write("\r");
        }

        protected void InitializeTest(string testDescription)
        {
            TestDescription = testDescription;
            _index++;
            ConsoleMessage(State.RUNNING);
        }

        protected void ConditionTest(bool result)
        {
            ConsoleMessage(result ? State.PASSED : State.FAILED);
        }

        protected void DisplayException(Exception e)
        {
            Console.WriteLine("\t" + e.Message);
        }
    }
}

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
        public enum Condition
        {
            EQ,
            NE,
            LT,
            LE,
            GT,
            GE
        }

        protected void ConsoleMessage(State state, string observed, string actual)
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
            if(!string.IsNullOrEmpty(actual) && !string.IsNullOrEmpty(observed))
                Console.Write(". Expected " + actual + " but received " + observed);
            
            if (newLine)
                Console.Write("\n\r");
            else
                Console.Write("\r");
        }

        protected void InitializeTest(string testDescription)
        {
            TestDescription = testDescription;
            _index++;
            ConsoleMessage(State.RUNNING, null, null);
        }

        protected void ConditionTest<T>(T observed, Condition condition, T actual)
        {
            bool result = false;
            if(!(observed is IComparable))
            {
                if (condition != Condition.EQ && condition != Condition.NE)
                    throw new ArgumentException("Condition Test Failure: Received Non IComparable when comparison was requested");
            }
            switch(condition)
            {
                case Condition.EQ:
                    result = object.Equals(actual, observed);
                    break;
                case Condition.NE:
                    result = !object.Equals(actual, observed);
                    break;
                case Condition.LT:
                    result = ((IComparable)observed).CompareTo(actual) < 0;
                    break;
                case Condition.LE:
                    result = ((IComparable)observed).CompareTo(actual) <= 0;
                    break;
                case Condition.GT:
                    result = ((IComparable)observed).CompareTo(actual) > 0;
                    break;
                case Condition.GE:
                    result = ((IComparable)observed).CompareTo(actual) >= 0;
                    break;
            }
            if (result)
                ConsoleMessage(State.PASSED, null, null);
            else
                ConsoleMessage(State.FAILED, observed.ToString(), actual.ToString());
        }

        protected void DisplayException(Exception e)
        {
            Console.WriteLine("\t" + e.Message);
        }
    }
}

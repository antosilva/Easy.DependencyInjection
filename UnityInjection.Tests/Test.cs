namespace UnityInjection.Tests
{
    internal interface ITest
    {
        string GetValue();
    }

    internal class Test1 : ITest
    {
        private readonly string _value;

        public Test1(string value)
        {
            _value = value;
        }

        public string GetValue()
        {
            return nameof(Test1) + "=" + _value;
        }
    }

    internal class Test2 : ITest
    {
        private readonly string _value1;
        private readonly string _value2;

        public Test2(string value1, string value2)
        {
            _value1 = value1;
            _value2 = value2;
        }

        public string GetValue()
        {
            return nameof(Test2) + "=" + _value1 + "/" + _value2;
        }
    }
}

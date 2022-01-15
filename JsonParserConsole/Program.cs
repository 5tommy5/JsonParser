using JsonParser;
using System;

namespace JsonParserConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            IJsonParser<TestClassArray> parser = new JsonParser<TestClassArray>();
            string case1 = "{ 'Var1':'hi, man', 'Var2':2, 'Var7':[4, 5, 7], 'Var3':{'Var1':'hi, man again', 'Var2':3, 'Var3':2.9, 'TEST':{'Hello':'Hi, man!', 'Date':5/1/2008 8:30:52 AM, 'Value16':16}}}";
            string case2 = "{ 'Met':'Hi, homie!', 'Array':[{'Entity':7},{'Entity':6},{'Entity':5},{'Entity':444}]}";
            var res = parser.Deserialize(case2);
            Console.ReadKey();
        }
    }
}

using JsonParser;
using System;

namespace JsonParserConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            IJsonParser<Test2Class> parser = new JsonParser<Test2Class>();
            var obj = "{ 'Var1':'hi, man', 'Var2':2, 'Var3':{'Var1':'hi, man again', 'Var2':3, 'Var3':2.9, 'TEST':{'Hello':'Hi, man!', 'Date':5/1/2008 8:30:52 AM, 'Value16':16}}}";
            var res = parser.Deserialize(obj);



            HiddenParser parser1 = new HiddenParser(typeof(Test2Class));
            Span<char> var1 = new Span<char>("{ 'Var1':'hi, man', 'Var2':2, 'Var3':{'Var1':'hi, man again', 'Var2':3, 'Var3':2.9, 'TEST':{'Hello':'Hi, man!', 'Date':5/1/2008 8:30:52 AM, 'Value16':16}}}".ToCharArray());
            var newres =parser1.GetBetween(var1, typeof(TestClass), "'Var3'");
            Console.WriteLine(newres);



            Console.ReadKey();
        }
    }
}

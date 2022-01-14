using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace JsonParser
{
    public class JsonParser<T> : IJsonParser<T>
        where T: new()
    {

        public T Deserialize(string obj)
        {
            HiddenParser parser = new HiddenParser(typeof(T));
            return (T)parser.Parse(obj) ;
        }

        public string Serialize(T obj)
        {
            throw new NotImplementedException();
        }
    }
}

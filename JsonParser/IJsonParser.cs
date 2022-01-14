using System;
using System.Collections.Generic;
using System.Text;

namespace JsonParser
{
    public interface IJsonParser<T>
    {
        T Deserialize(string obj);
        string Serialize(T obj);
    }
}

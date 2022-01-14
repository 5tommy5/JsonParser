using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JsonParserConsole
{
    public class HiddenParser
    {
        private Type _type;
        public HiddenParser(Type type)
        {
            _type = type;
        }
        public object Parse(string obj)
        {
            List<FieldInfo> fields = new List<FieldInfo>();
            var fields2 = this._type.GetFields();
            fields.AddRange(fields2);
            object result = Activator.CreateInstance(this._type);
            foreach (var field in fields)
            {
                bool isPrimitive = field.FieldType.IsPrimitive || field.FieldType == typeof(String) || field.FieldType.IsValueType;
                bool isString = field.FieldType == typeof(String);
                var find = getBetween(ref obj, "'" + field.Name + "':", ",", isPrimitive, isString);
                if (!isPrimitive)
                {
                    var newDES = new HiddenParser(field.FieldType);
                    var convertedValue = newDES.Parse(find+"}");
                    field.SetValue(result, convertedValue);
                }
                else
                {
                    try
                    {
                        var convertedValue = Convert.ChangeType(find, field.FieldType);
                        field.SetValue(result, convertedValue);
                    }
                    catch
                    {
                        field.SetValue(result, default);
                    }

                }

            }
            return result;
        }
        private string getBetween(ref string strSource, string strStart, string strEnd, bool isPrimitive, bool isString)
        {

            int StartIndex=0, EndIndex=0;
            string result = "";
            bool toEnd = false;

            if (!isPrimitive)
            {
                strEnd = "},";
                strStart += "{";
            }
            if (isString)
            {
                strEnd = "',";
                strStart += "'";
            }
            if (strSource.Contains(strStart))
            {
                StartIndex = strSource.IndexOf(strStart);
                try
                {
                    EndIndex = strSource.IndexOf(strEnd, StartIndex);
                }
                catch(Exception ex)
                {
                    throw;
                }
                if(EndIndex == -1)
                {
                    strEnd = strEnd.Replace(",", "");
                    if (String.IsNullOrEmpty(strEnd))
                    {
                        strEnd = "}";
                    }
                }
                try
                {
                    EndIndex = strSource.IndexOf(strEnd, StartIndex);
                }
                catch (Exception ex)
                {
                    throw;
                }
                if (EndIndex == -1)
                {
                    return null;
                }
            }
            ReadOnlySpan<char> sp = strSource.AsSpan();
            if (toEnd)
            {
                result = sp.Slice(StartIndex).ToString();
                strSource = strSource.Replace(result, "");
                result = result.Replace(strStart, "");
            }
            else
            {
                result = sp.Slice(StartIndex, EndIndex - StartIndex + 1).ToString();
                strSource = strSource.Replace(result, "");
                result = result.Replace(strStart, "");
                result = result.Replace(strEnd, "");
                if (isString)
                {
                    result = result.Replace("'", "");
                }
            }

            
            //var toRemove = strSource.Substring(StartIndex, EndIndex + strEnd.Length);
            
            return result;
            




            //string res = "";
            //int Start, End;
            //if (!isPrimitive)
            //{
            //    strEnd = "},";
            //}
            //if (isString)
            //{
            //    strStart += "'";
            //    strEnd = "',";
            //}
            //if (!strSource.Contains(strEnd))
            //{
            //    strEnd = "}";
            //}
            //if (strSource.Contains(strStart) && strSource.Contains(strEnd))
            //{

            //    Start = strSource.IndexOf(strStart, 0)-1 + strStart.Length;
            //    End = strSource.IndexOf(strEnd, Start);
            //    if (End == -1)
            //    {
            //        strEnd = "}";
            //        End = strSource.IndexOf(strEnd, Start);
            //    }
            //    if (!isPrimitive)
            //    {
            //        res = strSource.Substring(Start, End - Start+1);
            //    }
            //    else
            //    {
            //        res = strSource.Substring(Start, End - Start+1);
            //    }
                
            //    strSource = strSource.Remove(Start - strStart.Length, End);
            //}
            //return res;
        }
    }
}

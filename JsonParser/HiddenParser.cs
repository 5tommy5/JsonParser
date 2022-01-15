﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace JsonParser
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
            
            return result;
        }
        public string GetBetween(Span<char> obj, Type typeOfParsing, string fieldName)
        {
            string res = "";
            (string strStart, string strEnd ) = GetParamsByType(typeOfParsing);
            strStart = "'"+fieldName +"'"+ strStart;
            (int? startIndex, int? endIndex) = GetIndexes(obj, strStart, strEnd+",");
            if(startIndex == null)
            {
                return null;
            }
            if(endIndex == null)
            {
                (startIndex, endIndex) = GetIndexes(obj, strStart, strEnd+"}");
                if(endIndex == null)
                {
                    return null;
                }
                strEnd += "}";
            }
            else
            {
                strEnd += ",";
            }

            if(startIndex != null && endIndex != null)
            {
                var resStart = startIndex.Value + strStart.Length;
                res = obj.Slice(resStart, endIndex.Value - resStart).ToString();
            }
            return res;
        }
        public (int?,int?) GetIndexes(Span<char> obj, string strStart, string strEnd)
        {
            int? startIndex = null;
            int? endIndex = null;
            for (var index = 0; index < obj.Length; index++)
            {
                if (startIndex == null && obj.Slice(index, strStart.Length).ToString() == strStart)
                {
                    startIndex = index;
                    index += strStart.Length - 1;
                }
                try
                {
                    if(startIndex != null && strEnd == "}}")
                    {
                        if(obj.Slice(obj.Length-strEnd.Length, strEnd.Length).ToString() == strEnd)
                        {
                            return (startIndex, obj.Length - strEnd.Length);
                        }
                    }
                    if (startIndex != null && obj.Slice(index, strEnd.Length).ToString() == strEnd)
                    {
                        endIndex = index;
                        break;
                    }
                }
                catch
                {
                    return (startIndex, null);
                }

            }
            return (startIndex, endIndex);
        }
        public (string, string) GetParamsByType(Type typeOfParsing)
        {
            if(typeOfParsing == typeof(String))
            {
                return (":'", "'");
            }
            if (typeOfParsing.IsArray)
            {
                return (":[", "]");
            }
            if (typeOfParsing.IsPrimitive)
            {
                return (":", "");
            }
            if (!typeOfParsing.IsPrimitive)
            {
                return (":{", "}");
            }
            return ("","");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Golf.Domain.Shared
{
   public static class StringHelper
    {
        //public string StringValues { get; set; }
        public static List<string> GetListStrings(string StringValues)
        {
            List<string> result = new List<string>();
            if (StringValues != "" && StringValues != null)
            {
                var tmp = StringValues.Split(",");
                foreach (var i in tmp)
                {
                    if (i != "")
                    {
                        result.Add(i);
                    }

                }
            }
            return result;
        }
        public static List<Guid> GetListGuids(string StringValues)
        {
            List<Guid> result = new List<Guid>();
            if (StringValues != "" && StringValues != null)
            {
                var tmp = StringValues.Split(",");
                foreach (var i in tmp)
                {
                    if (i != "")
                    {
                        result.Add(new Guid(i));
                    }

                }
            }
            return result;
        }
        /// <summary>
        /// Thêm chuỗi vào chuỗi ban đầu ngăn cách bởi dấu ","
        /// </summary>
        /// <param name="StringValues">Chuỗi ban đầu</param>
        /// <param name="stringValue">Chuối cộng thêm</param>
        public static void AddString(string StringValues, string stringValue)
        {
            if (StringValues == "" || StringValues == null)
            {
                StringValues = stringValue.ToString();
            }
            else
            {
                StringValues = StringValues + "," + stringValue.ToString();
            }
        }
        public static void AddGuid(string StringValues, Guid stringValue)
        {
            if (StringValues == "" || StringValues == null)
            {
                StringValues = stringValue.ToString();
            }
            else
            {
                StringValues = StringValues + "," + stringValue.ToString();
            }
        }
        public static void RemoveString(string StringValues, string stringValue)
        {
            if (StringValues == "" || StringValues == null)
            {
                return;
            }
            else
            {
                StringValues = StringValues.Replace("," + stringValue.ToString(), "");
                StringValues = StringValues.Replace(stringValue.ToString() + ",", "");
            }
        } 
        public static void RemoveGuid(string StringValues, Guid stringValue)
        {
            if (StringValues == "" || StringValues == null)
            {
                return;
            }
            else
            {
                StringValues = StringValues.Replace("," + stringValue.ToString(), "");
                StringValues = StringValues.Replace(stringValue.ToString() + ",", "");
            }
        }
        public static void RemoveStrings(string StringValues,List<string> stringValues)
        {
            foreach (var i in stringValues)
            {
                RemoveString(StringValues, i);
            }
        }
        public static void RemoveGuids(string StringValues,List<Guid> stringValues)
        {
            foreach (var i in stringValues)
            {
                RemoveGuid(StringValues, i);
            }
        }
        public static void AddStrings(string StringValues, List<string> stringValues)
        {
            foreach (var i in stringValues)
            {
                AddString(StringValues, i);
            }
        }
        public static void SetStrings(string StringValues, List<string> stringValues)
        {
            var tmp = "";
            foreach (var i in stringValues)
            {
                if (tmp == "")
                {
                    tmp = tmp + i;
                }
                else
                {
                    tmp = tmp + "," + i;
                }
            }
           StringValues = tmp;
        }
        public static void SetGuids(string StringValues, List<Guid> stringValues)
        {
            var tmp = "";
            foreach (var i in stringValues)
            {
                if (tmp == "")
                {
                    tmp = tmp + i;
                }
                else
                {
                    tmp = tmp + "," + i.ToString();
                }
            }
           StringValues = tmp;
        }
        //Odoo StringHelper
        public static string OdooGetIntByString(string s)
        {
            int a = s.IndexOf("(");
            int b = s.IndexOf(",)");
            return s.Substring(a + 1, b - a-1);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VaptchaSdk
{
    public class JsonHelper
    {
        public static T ToObj<T>(string data)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(data);
        }

        public static string ToJson<T>(T data)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(data);
        }
    }
}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wechat.Utils
{
    public class JsonHelper
    {
        public static string Serialize(object value)
        {
            return JsonConvert.SerializeObject(value);
        }

        public static T DeSerialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}

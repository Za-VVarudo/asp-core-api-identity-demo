using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityDemo.Helpers
{
    public static class SessionHelper
    {
        public static void SetObject(this ISession session, string key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T GetObject<T> (this ISession session, string key)
        {
            string jString = session.GetString(key);
            return jString == null ? default : JsonConvert.DeserializeObject<T>(jString);
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FirewoodEngine.Componenents;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FirewoodEngine.Core
{
    static class Editor
    {
        public static GameObject selectedObject = null;



        public static void LoadScene(String path)
        {
            using (StreamReader file = File.OpenText(path))
            using (JsonTextReader reader = new JsonTextReader(file))
            {
                JObject jsonObject = (JObject)JToken.ReadFrom(reader);

                Console.WriteLine(jsonObject.ToString());
            }
        }

        
    }
}

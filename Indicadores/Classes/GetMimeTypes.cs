using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiIndicadores.Classes
{
    public class GetMimeTypes
    {
        public Dictionary<string,string> Types()
        {
            return new Dictionary<string, string>
            {
                {".pdf","application/pdf" }
            };
        }
    }
}

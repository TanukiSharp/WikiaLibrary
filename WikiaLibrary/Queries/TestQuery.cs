using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace WikiaLibrary.Queries
{
    public class TestQuery : QueryBase
    {
        public TestQuery(HttpClient client, string action, params Param[] parameters)
            : base(client, action, parameters)
        {
        }

        protected override void OnResponse(XElement element)
        {
            Console.WriteLine(element);
        }
    }
}

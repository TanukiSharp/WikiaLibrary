using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace WikiaLibrary.Queries
{
    public class GetPageContent : QueryBase
    {
        public GetPageContent(HttpClient client, string pageName, bool escapepageName)
            : base(client, "query",
            new Param("prop", "revisions"),
            new Param("rvprop", "content"),
            new Param("titles", escapepageName ? Uri.EscapeDataString(pageName) : pageName))
        {
        }

        protected override void OnResponse(XElement element)
        {
            throw new NotImplementedException();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace WikiaLibrary.Queries
{
    public class EditToken : QueryBase
    {
        public EditToken(HttpClient client)
            : base(client, "query",
            new Param("prop", "info"),
            new Param("intoken", "edit"),
            new Param("titles", "Upload"))
        {
        }

        public int Ns { get; private set; }
        public string Title { get; private set; }
        public string Missing { get; private set; }
        public DateTime StartTimeStamp { get; private set; }
        public string Token { get; private set; }

        protected override void OnResponse(XElement element)
        {
            /*
            <api>
              <query>
                <pages>
                  <page ns="0" title="Upload" missing="" starttimestamp="2013-03-21T17:01:13Z" edittoken="80b22ce067abecedb752572683bc5105+\" />
                </pages>
              </query>
            </api>
            */

            var page = element.Element("query").Element("pages").Element("page");

            Ns = (int)page.Attribute("ns");
            Title = (string)page.Attribute("title");
            Missing = (string)page.Attribute("missing");
            StartTimeStamp = (DateTime)page.Attribute("starttimestamp");
            Token = (string)page.Attribute("edittoken");
        }
    }
}

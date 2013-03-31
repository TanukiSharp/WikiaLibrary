using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace WikiaLibrary.Queries
{
    public class GetImageInfo : QueryBase
    {
        public GetImageInfo(HttpClient client, string imageName)
            : base(client, "query",
            new Param("titles", Uri.EscapeDataString(imageName)),
            new Param("prop", "imageinfo"),
            new Param("iiprop", "url"))
        {
        }

        protected override void OnResponse(XElement element)
        {
            throw new NotImplementedException();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace WikiaLibrary.Queries
{
    public class GetAllImages : QueryBase
    {
        private List<ImageInfo> images;

        public GetAllImages(HttpClient client, List<ImageInfo> images, string continueFrom = null)
            : base(client,
            "query",
            new Param("list", "allimages"),
            new Param("aiprop", "url"),
            new Param("ailimit", "500"),
            new Param("aifrom", continueFrom))
        {
            this.images = images;
        }

        public string ContinueFrom { get; private set; }

        protected override void OnResponse(XElement element)
        {
            var imageElements = element.Element("query").Element("allimages").Elements("img");

            foreach (var imageElement in imageElements)
            {
                var name = (string)imageElement.Attribute("name");
                var url = (string)imageElement.Attribute("url");

                images.Add(new ImageInfo(name, url));
            }

            var queryContrinue = element.Element("query-continue");
            if (queryContrinue != null)
            {
                var node = queryContrinue.Element("allimages");
                if (node != null)
                {
                    var attr = node.Attribute("aifrom");
                    if (attr != null)
                        ContinueFrom = attr.Value;
                }
            }
        }
    }
}

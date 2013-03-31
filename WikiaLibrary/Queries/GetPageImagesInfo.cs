using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace WikiaLibrary.Queries
{
    public class GetPageImagesInfo : QueryBase
    {
        public GetPageImagesInfo(HttpClient client, string pageName)
            : base(client, "query",
            new Param("titles", pageName),
            new Param("prop", "images"))
        {
        }

        protected override void OnResponse(XElement element)
        {
            /*
            <api>
              <query>
                <pages>
                  <page pageid="1531" ns="0" title="MH3U - Grande Épée">
                    <images>
                      <im ns="6" title="Fichier:Alags-sharp1.png" />
                      <im ns="6" title="Fichier:Alags-sharp2.png" />
                      <im ns="6" title="Fichier:Alags-sharp3.png" />
                      <im ns="6" title="Fichier:Amygs-sharp1.png" />
                      <im ns="6" title="Fichier:Amygs-sharp2.png" />
                      <im ns="6" title="Fichier:Ancientgs-sharp1.png" />
                      <im ns="6" title="Fichier:Ancientgs-sharp2.png" />
                      <im ns="6" title="Fichier:Ancientgs-sharp3.png" />
                      <im ns="6" title="Fichier:Ancientgs-sharp4.png" />
                      <im ns="6" title="Fichier:Berserkgs-sharp1.png" />
                    </images>
                  </page>
                </pages>
              </query>
              <query-continue>
                <images imcontinue="1531|Berserkgs-sharp2.png" />
              </query-continue>
            </api>
            */
        }
    }
}

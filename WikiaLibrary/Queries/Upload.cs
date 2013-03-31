using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace WikiaLibrary.Queries
{
    public class Upload : QueryBase
    {
        public Upload(HttpClient client, params Param[] parameters)
            : base(client, "upload", parameters)
        {
        }

        public bool IsSuccess { get; private set; }
        public bool IsWarning { get; private set; }
        public bool IsError { get; private set; }

        public string ErrorCode { get; private set; }
        public string ErrorInfo { get; private set; }

        public bool IsDuplicate { get; private set; }
        public string DuplicatingFile { get; private set; }

        public string RawResult { get; private set; }

        protected override void OnResponse(XElement element)
        {
            /*
            <api servedby="ap-s33">
              <error code="notoken" info="The token parameter must be set" /> 
            </api>
            */

            RawResult = element.ToString();

            var error = element.Element("error");
            if (error != null)
            {
                IsError = true;
                ErrorCode = (string)error.Attribute("code");
                ErrorInfo = (string)error.Attribute("info");
                return;
            }

            var upload = element.Element("upload");
            if (upload != null)
            {
                var result = (string)upload.Attribute("result");

                switch (result.ToLowerInvariant())
                {
                    case "success":
                        {
                            IsSuccess = true;
                            break;
                        }
                    case "warning":
                        {
                            IsWarning = true;
                            var duplicates = upload.Elements("warnings").Elements("duplicate").Elements("duplicate");
                            var first = duplicates.FirstOrDefault();
                            if (first != null)
                            {
                                IsDuplicate = true;
                                DuplicatingFile = first.Value;
                            }
                            break;
                        }
                }
            }


            /*
            <api>
              <upload result="Warning" filekey="11biqdzr55x0.61segz.7331534.png" sessionkey="11biqdzr55x0.61segz.7331534.png">
                <warnings>
                  <duplicate>
                    <duplicate>Ironhammer-sharp45.png</duplicate>
                  </duplicate>
                </warnings>
              </upload>
            </api>
            */

            /*
            <api>
              <upload result="Success" filename="Test_derp.jpg">
                <imageinfo timestamp="2013-03-22T15:26:09Z" user="TanukiSharp" userid="7331534" size="20954" width="338" height="383" parsedcomment="" comment="" url="http://images.wikia.com/mogapedia/fr/images/8/88/Test_derp.jpg" descriptionurl="http://fr.mogapedia.wikia.com/wiki/Fichier:Test_derp.jpg" sha1="f80995975090a4702bc21e502757c63600f4f899" mime="image/jpeg" mediatype="BITMAP" bitdepth="8">
                  <metadata>
                    <metadata name="Orientation" value="1" />
                    <metadata name="YResolution" value="72/1" />
                    <metadata name="ResolutionUnit" value="2" />
                    <metadata name="Software" value="Adobe Photoshop CS2 Windows" />
                    <metadata name="DateTime" value="2009:06:10 12:37:31" />
                    <metadata name="ColorSpace" value="65535" />
                    <metadata name="MEDIAWIKI_EXIF_VERSION" value="2" />
                  </metadata>
                </imageinfo>
              </upload>
            </api>
            */
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace WikiaLibrary.Queries
{
    public class Login : QueryBase
    {
        public Login(HttpClient client, params Param[] parameters)
            : base(client, "login", parameters)
        {
        }

        /*
        <api>
          <login result="NeedToken" token="b3e3ad4e57bb696fb9a547d8054feeed" cookieprefix="wikicities" /> 
        </api>
        */

        /*
        <api>
          <login result="Success" lguserid="7331534" lgusername="TanukiSharp" lgtoken="6bec32cc27894cb6c8a802e795932ca5" cookieprefix="wikicities" sessionid="033860a8dc658e4719742fbbb28bf031" />
        </api>
        */

        public string Result { get; private set; }
        public string Token { get; private set; }
        public string UserID { get; private set; }
        public string SessionID { get; private set; }
        public int WaitTime { get; private set; }

        protected override void OnResponse(XElement element)
        {
            var login = element.Element("login");

            Result = (string)login.Attribute("result");
            Token = (string)login.Attribute("token");
            if (Token == null)
                Token = (string)login.Attribute("lgtoken");
            UserID = (string)login.Attribute("lguserid");
            SessionID = (string)login.Attribute("sessionid");

            var waitAttr = login.Attribute("wait");
            if (waitAttr != null)
                WaitTime = (int)waitAttr;
        }
    }
}

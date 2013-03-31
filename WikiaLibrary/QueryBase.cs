using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace WikiaLibrary
{
    public struct Param
    {
        public string Name { get; private set; }
        public string Value { get; private set; }

        public Param(string name, string value)
            : this()
        {
            Name = name;
            Value = value;
        }
    }

    public abstract class QueryBase
    {
        public string Action { get; private set; }

        private HttpClient client;
        private Param[] parameters;

        public QueryBase(HttpClient client, string action, params Param[] parameters)
        {
            if (client == null)
                throw new ArgumentNullException("client");
            if (string.IsNullOrWhiteSpace(action))
                throw new ArgumentException("Invalid 'action' argument.", "action");

            Action = action;
            this.client = client;
            this.parameters = parameters;
        }

        public Cookie[] SetCookie { get; private set; }

        private const string BaseUrl = "api.php";

        public Task Run()
        {
            return Run(null);
        }

        public async Task Run(HttpContent httpContent)
        {
            var inlinedParameters = string.Empty;
            if (parameters.Length > 0)
            {
                var sb = new StringBuilder();

                foreach (var kv in parameters)
                {
                    sb.AppendFormat("&{0}", Uri.EscapeDataString(kv.Name));
                    if (string.IsNullOrWhiteSpace(kv.Value) == false)
                        sb.AppendFormat("={0}", Uri.EscapeDataString(kv.Value));
                }

                inlinedParameters = sb.ToString();
            }

            var url = string.Format("{0}?action={1}{2}&format=xml", BaseUrl, Action, inlinedParameters);

            using (var response = await client.PostAsync(url, httpContent))
            {
                response.EnsureSuccessStatusCode();

                IEnumerable<string> values;
                if (response.Headers.TryGetValues("Set-Cookie", out values))
                    SetCookie = ParseCookies(values);

                var content = await response.Content.ReadAsStringAsync();
                var xml = XElement.Parse(content);

                if (xml != null)
                    OnResponse(xml);
            }
        }

        protected abstract void OnResponse(XElement element);

        private Cookie[] ParseCookies(IEnumerable<string> values)
        {
            return values
                .Select(ParseCookie)
                .Where(c => c != null)
                .ToArray();
        }

        private Cookie ParseCookie(string value)
        {
            var query = from elements in value.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                        select elements.Split(new[] { '=' }, 2)
                            into kv
                            where kv.Length > 0
                            select CreateKeyValuePair(kv);

            var list = query.ToList();

            var pathIndex = list.FindIndex(kv => kv.Key == "path");
            if (pathIndex == -1)
                return null;

            var path = list[pathIndex].Value.Trim();
            list.RemoveAt(pathIndex);

            var domainIndex = list.FindIndex(kv => kv.Key == "domain");
            if (domainIndex == -1)
                return null;

            var domain = list[domainIndex].Value.Trim();
            list.RemoveAt(domainIndex);

            if (list.Count == 0)
                return null;

            return new Cookie(list[0].Key.Trim(), list[0].Value.Trim(), path, domain);
        }

        private KeyValuePair<string, string> CreateKeyValuePair(string[] kv)
        {
            if (kv == null || kv.Length == 0)
                throw new ArgumentException("kv");

            if (string.IsNullOrWhiteSpace(kv[0]))
                throw new ArgumentException("kv");

            string value = null;
            if (kv.Length > 1 && string.IsNullOrWhiteSpace(kv[1]) == false)
                value = kv[1].Trim();

            return new KeyValuePair<string, string>(kv[0].Trim(), value);
        }
    }
}

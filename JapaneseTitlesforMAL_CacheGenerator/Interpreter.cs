using System;
using System.IO;
using System.Linq;
using System.Net;
using HtmlAgilityPack;

namespace JapaneseTitlesforMAL_CacheGenerator
{
    public class Interpreter
    {
        private readonly int _id;
        private readonly string _prefix;

        public Interpreter(Type type, int id)
        {
            _id = id;
            switch (type)
            {
                case Type.Anime:
                    _prefix = @"http://myanimelist.net/anime/";
                    break;
                case Type.Manga:
                    _prefix = @"http://myanimelist.net/manga/";
                    break;
            }
        }

        protected string GetHtml()
        {
            var html = "";
            var request = WebRequest.Create(_prefix + _id) as HttpWebRequest;
            try
            {
                var response = request?.GetResponse() as HttpWebResponse;
                if (response == null || response.StatusCode != HttpStatusCode.OK)
                    return html;
                var stream = response.GetResponseStream();
                if (stream != null)
                {
                    var reader = new StreamReader(stream);
                    html = reader.ReadToEnd();
                }
                response.Close();
            }
            catch (Exception)
            {
            }
            return html;
        }

        public string GetJapaneseTitle()
        {
            var htmlDoc = new HtmlDocument();
            var html = GetHtml();
            if (html == "")
                return "";
            htmlDoc.LoadHtml(html);
            if (htmlDoc.DocumentNode == null)
                return "";
            var nodes = htmlDoc.DocumentNode.SelectNodes("//div[@class='spaceit_pad']");
            foreach (var text in nodes.Select(node => node.InnerText).Where(text => text.Contains("Japanese:")))
                return text.Replace("Japanese:", "").Trim();
            return "";
        }
    }
}
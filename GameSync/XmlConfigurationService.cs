using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace GameSync
{
    public class XmlConfigurationService : IConfigurationService
    {
        private readonly XDocument _document;

        private readonly string _steamPath;

        public XmlConfigurationService(string xmlFilename)
        {
            this._document = XDocument.Load(xmlFilename);

            _steamPath = GetSteamPath();
        }

        private IEnumerable<XElement> GetCustomAndDefault(params string[] nodes)
        {
            Func<XElement, IEnumerable<XElement>> filter = x =>
            {
                IEnumerable<XElement> filtered = new [] { x };
                foreach (var node in nodes)
                    filtered = filtered.Elements<XElement>(node);

                return filtered;
            };

            var defaultElements = _document.Element("config").Element("default");
            var customElements = _document.Element("config").Element("custom");

            return filter(defaultElements).Union(filter(customElements));
        }

        private string GetSteamPath()
        {
            var steamPaths = from node in GetCustomAndDefault("paths", "path")
                             where node.Attribute("name").Value == "Steam"
                             let path = node.Attribute("value").Value
                             where Directory.Exists(path)
                             select path;

            return steamPaths.First();
        }

        public Folder[] GetFolders()
        {
            return (from element in GetCustomAndDefault("games", "game")
                    let path = element.Attribute("path").Value
                    let realPath = ExpandVariables(path)
                    select new Folder(
                        element.Attribute("name").Value,
                        realPath)
                   ).ToArray();
        }

        private string ExpandVariables(string rawFolder)
        {
            return Environment.ExpandEnvironmentVariables(rawFolder)
                .Replace("$MyDocuments$", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments))
                .Replace("$Steam$", this._steamPath);
        }
    }
}

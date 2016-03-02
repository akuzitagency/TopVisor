using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using TopVisor.Core.Model;

namespace TopVisor.Core.Services.DataLoader
{
    public class XMLDataLoader : DataLoaderBase
    {
        private readonly string _dataFileName;
        private XDocument _xDoc;

        public XDocument XDoc
        {
            get { return _xDoc =_xDoc ?? (_xDoc= XDocument.Load(_dataFileName)); }
        }

        public XMLDataLoader(String dataFileName)
        {
            _dataFileName = dataFileName;
        }

        public override async Task<List<Project>> LoadProjects()
        {
            return XDoc
                .Root?
                .Elements("Project")
                .Select(project => new Project(UInt32.Parse(project.Attribute("id").Value), project.Attribute("name").Value, project.Attribute("site").Value))
                .ToList()
                   ?? new List<Project>();
        }

        public override async Task<List<Phrase>> LoadPhrases(uint projectId)
        {
            return XDoc
                .Root?
                .Elements("Project")
                .FirstOrDefault(e => e.Attribute("id").Value == projectId.ToString())?
                .Element("Phrases")?
                .Elements("Phrase")
                .Select(phrase => new Phrase(UInt32.Parse(phrase.Attribute("id").Value), phrase.Attribute("phrase").Value))
                .ToList()
                   ?? new List<Phrase>();
        }

        public override async Task<List<Phrase>> LoadPhrases(string projectName)
        {
            return XDoc
                .Root?
                .Elements("Project")
                .FirstOrDefault(e => e.Attribute("name").Value == projectName)?
                .Element("Phrases")?
                .Elements("Phrase")
                .Select(phrase => new Phrase(UInt32.Parse(phrase.Attribute("id").Value), phrase.Attribute("phrase").Value))
                .ToList()
                   ?? new List<Phrase>();
        }
    }
}
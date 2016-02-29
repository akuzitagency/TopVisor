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
        private readonly XDocument _xDoc;
        private const String DataFilePath = "LocalData.xml";
        public XMLDataLoader()
        {
            _xDoc = XDocument.Load(DataFilePath);
        }

        public async override Task<List<Project>> LoadProjects()
        {
            return _xDoc
                .Root?
                .Elements("Project")
                .Select(project => new Project(UInt32.Parse(project.Attribute("id").Value), project.Attribute("name").Value, project.Attribute("site").Value))
                .ToList()
                   ?? new List<Project>();
        }

        public async override Task<List<Phrase>> LoadPhrases(uint projectId)
        {
            return _xDoc
                .Root?
                .Elements("Project")
                .FirstOrDefault(e => e.Attribute("id").Value == projectId.ToString())?
                .Element("Phrases")?
                .Elements("Phrase")
                .Select(phrase => new Phrase(UInt32.Parse(phrase.Attribute("id").Value), phrase.Attribute("phrase").Value))
                .ToList()
                   ?? new List<Phrase>();
        }

        public async override Task<List<Phrase>> LoadPhrases(string projectName)
        {
            return _xDoc
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
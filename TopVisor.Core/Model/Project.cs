using System;
using System.Collections.Generic;

namespace TopVisor.Core.Model
{
    public class Project
    {
        public UInt32 Id { get; private set; }
        public String Name { get; set; }
        public String Site { get; set; }
        public List<Phrase> Phrases { get; private set; }

        public Project(UInt32 id, String name, String site)
        {
            Id = id;
            Name = name;
            Site = site;
            Phrases=new List<Phrase>();
        }
    }
}
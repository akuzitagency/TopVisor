using System.Collections.Generic;

namespace TopVisor.Core.Model
{
    public class TopVisorData
    {
        public List<Project> Projects { get; private set; }

        public TopVisorData()
        {
            Projects = new List<Project>();
        }
    }
}
using System;

namespace TopVisor.Core.Model
{
    public class Phrase
    {
        public UInt32 Id { get; private set; }
        public String Text { get; set; }

        public Phrase(uint id, string text)
        {
            Id = id;
            Text = text;
        }
    }
}
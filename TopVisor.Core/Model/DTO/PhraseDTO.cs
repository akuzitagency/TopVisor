using System;
// ReSharper disable InconsistentNaming

namespace TopVisor.Core.Model.DTO
{
    public class PhraseDTO
    {
        public UInt32 id { get; set; }
        public UInt32 phrase_id { get; set; }
        public UInt32 project_id { get; set; }
        public UInt32 group_id { get; set; }
        public UInt32 ord { get; set; }
        public String target { get; set; }
        public String tag { get; set; }
        public String group_name { get; set; }
        public String phrase { get; set; }
        public String target_status { get; set; }
        public String frequency1 { get; set; }
        public String frequency2 { get; set; }
        public String frequency3 { get; set; }
        public String price1 { get; set; }
        public String price2 { get; set; }
        public String price3 { get; set; }
    }
}
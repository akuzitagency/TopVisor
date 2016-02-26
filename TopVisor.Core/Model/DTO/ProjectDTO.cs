using System;
// ReSharper disable InconsistentNaming

namespace TopVisor.Core.Model.DTO
{
    public class ProjectDTO
    {
        public UInt32 id { get; set; }
        public UInt32 user { get; set; }
        public String name { get; set; }
        public String site { get; set; }
        public UInt32 competitor { get; set; }
        public UInt32 competition_ord { get; set; }
        public String comment { get; set; }
        public DateTime date { get; set; }
        public String update { get; set; }
        public DateTime last_view { get; set; }
        public UInt32 status { get; set; }
        public Boolean status_frequency { get; set; }
        public UInt32 status_claster { get; set; }
        public UInt32 subdomains { get; set; }
        public UInt32 filter { get; set; }
        public UInt32 auto_correct { get; set; }
        public UInt32 common_trafic { get; set; }
        public UInt32 on { get; set; }
        public UInt32 auto_cond { get; set; }
        public UInt32 wait_after_updates { get; set; }
        public String time_for_update { get; set; }
        public UInt32 with_snippets { get; set; }
        public UInt32 count_keywords { get; set; }
        public UInt32 count_tasks { get; set; }
        public UInt32 price { get; set; }
        public UInt32 report_on { get; set; }
        public String report_time { get; set; }
        public UInt32 report_format { get; set; }
        public UInt32 guest_link_right { get; set; }
        public String cat_y { get; set; }
        public String cat_dm { get; set; }
        public String cat_m { get; set; }
        public String right { get; set; }
        public String email { get; set; }
        public UInt32 favorite { get; set; }
        public String tag { get; set; }
        public Object[] history{ get; set; }
    }
}
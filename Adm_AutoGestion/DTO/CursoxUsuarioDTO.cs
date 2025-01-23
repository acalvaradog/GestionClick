using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.DTO
{
    public class CursoxUsuarioDTO
    {
        public int id { get; set; }
        public string shortname { get; set; }
        public string fullname { get; set; }
        public string displayname { get; set; }
        public int enrolledusercount { get; set; }
        public string idnumber { get; set; }
        public int visible { get; set; }
        public string summary { get; set; }
        public int summaryformat { get; set; }
        public string format { get; set; }
        public bool showgrades { get; set; }
        public string lang { get; set; }
        public bool enablecompletion { get; set; }
        public bool completionhascriteria { get; set; }
        public bool completionusertracked { get; set; }
        public int category { get; set; }
        public decimal? progress { get; set; }
        public bool completed { get; set; }
        public int startdate { get; set; }
        public int enddate { get; set; }
        public int marker { get; set; }
        public int? lastaccess { get; set; }
        public bool isfavourite { get; set; }
        public bool hidden { get; set; }
        public List<Overviewfile> overviewfiles { get; set; }
        public bool showactivitydates { get; set; }
        public bool showcompletionconditions { get; set; }
    }

    public class Overviewfile
    {
        public string filename { get; set; }
        public string filepath { get; set; }
        public int filesize { get; set; }
        public string fileurl { get; set; }
        public int timemodified { get; set; }
        public string mimetype { get; set; }
    }
}
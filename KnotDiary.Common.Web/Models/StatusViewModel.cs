using System.Collections.Generic;

namespace KnotDiary.Common.Web.Models
{
    public class StatusConfiguration
    {
        public List<StatusItem> StatusItems { get; set; }
    }

    public class StatusItem
    {
        public string Key { get; set; }

        public string Value { get; set; }
    }
}

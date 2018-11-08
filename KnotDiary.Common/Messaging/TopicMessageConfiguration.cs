using System;
using System.Collections.Generic;
using System.Text;

namespace KnotDiary.Common.Messaging
{
    public class TopicMessageConfiguration
    {
        public string ExchangeName { get; set; }

        public List<string> Topics { get; set; }
    }
}

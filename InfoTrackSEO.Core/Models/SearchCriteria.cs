using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace InfoTrackSEO.Core.Models
{
    public class SearchCriteria
    {
        private const string urlPattern = "((http|https)://)(www.)?[a-zA-Z0-9@:%._\\+~#?&//=]{2,256}\\.[a-z]{2,6}\\b([-a-zA-Z0-9@:%._\\+~#?&//=]*)";

        public string Keywords { get; set; }
        public string Url { get; set; }

        public bool IsUrlValid()
        {
            return !string.IsNullOrEmpty(Url) && Regex.IsMatch(Url, urlPattern);
        }
    }
}

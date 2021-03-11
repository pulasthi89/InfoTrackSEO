using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InfoTrackSEO.Core.Models
{
    public class SearchResult
    {
        public SearchResult()
        {
            Occurrences = new List<Occurrence>();
        }

        public string OccurrencesDescription { get { return Occurrences.Any() ? Occurrences.Select(x => x.ResultSetNumber.ToString()).Aggregate((x, y) => x + "," + y) : ""; } }
        public int TotalOccurrences { get; set; }
        public List<Occurrence> Occurrences { get; set; }
    }

    public class Occurrence
    {
        public Occurrence(int pageNo, int resultSetNo)
        {
            PageNumber = pageNo;
            ResultSetNumber = resultSetNo;
        }

        public int PageNumber { get; set; }
        public int ResultSetNumber { get; set; }
        public string ResultUrl { get; set; }
    }
}

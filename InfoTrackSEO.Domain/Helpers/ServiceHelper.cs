using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InfoTrackSEO.Core.Models;
using InfoTrackSEO.Domain.Exceptions;

namespace InfoTrackSEO.Domain.Helpers
{
    public static class ServiceHelper
    {
        public static bool ValidateCriteria(SearchCriteria criteria)
        {
            if (string.IsNullOrEmpty(criteria.Keywords))
                throw new DomainServiceException("Keywords for searching is missing in the search criteria.");
            if (string.IsNullOrEmpty(criteria.Url))
                throw new DomainServiceException("URL for monitoring is missing in the search criteria.");
            if (!criteria.IsUrlValid())
                throw new DomainServiceException("Given URL is not valid.");

            return true;
        }

        public static Section ExtractSection(string htmlBody, string startingHtml, string finishingHtml, int scanStartingIdx = 0, bool includeFinishHtml = false)
        {
            int resultsBodyStartIdx = htmlBody.IndexOf(startingHtml, scanStartingIdx);
            if (resultsBodyStartIdx == -1)
            {
                return null; //did not find the section start
            }

            int resultsBodyFinishIdx = htmlBody.IndexOf(finishingHtml, resultsBodyStartIdx + startingHtml.Length + 1);
            if (resultsBodyFinishIdx == -1)
            {

                return null; //did not find the section finish
            }

            var finishingIdx = includeFinishHtml ? resultsBodyFinishIdx + finishingHtml.Length : resultsBodyFinishIdx;

            var section = new Section()
            {
                StartingIdx = resultsBodyStartIdx,
                FinishingIdx = finishingIdx,
                SectionBody = htmlBody.Substring(resultsBodyStartIdx, finishingIdx - resultsBodyStartIdx)
            };

            return section;
        }

        public static List<Occurrence> GetUrlOccurrences(Section mainBody, string setPatternStart, string setPatternFinish, string url, int pageNo, int totalResultSets, out int sectionResultSets, string urlAttribute = "href")
        {
            var occurrences = new List<Occurrence>();
            sectionResultSets = 0;

            int scanIdx = 0; //OK, we are about to scan the results section

            while (true)
            {
                //Assuming each URL is part of a result set
                var resultSet = ExtractSection(mainBody.SectionBody, setPatternStart, setPatternFinish, scanIdx);
                if (resultSet == null)
                {
                    break;
                }

                sectionResultSets++;

                var urls = ExtractUrlFromSection(resultSet, urlAttribute);
                if (urls != null && urls.Exists(u => u.SectionBody.Contains(url)))
                {
                    occurrences.Add(new Occurrence(pageNo, totalResultSets + sectionResultSets));
                }

                scanIdx = resultSet.FinishingIdx;
            }

            return occurrences;
        }

        public static List<Section> ExtractUrlFromSection(Section section, string urlAttribute)
        {
            var urlSections = new List<Section>();
            int scanIdx = 0;
            while (true)
            {
                int resultIdx = section.SectionBody.IndexOf(urlAttribute + "=\"", scanIdx);
                if (resultIdx == -1)
                {
                    //no more urls in this section. 
                    break;
                }

                int urlStartIdx = resultIdx + urlAttribute.Length + 2; //URL followed by the attribute (href/src)
                int urlFinishIdx = section.SectionBody.IndexOf("\"", urlStartIdx);
                string urlHtml = section.SectionBody.Substring(urlStartIdx, urlFinishIdx - urlStartIdx);
                if (!string.IsNullOrEmpty(urlHtml))
                {
                    urlSections.Add(new Section() { StartingIdx = urlStartIdx, FinishingIdx = urlFinishIdx, SectionBody = urlHtml });
                }
                scanIdx = urlFinishIdx;
            }

            return urlSections.Any() ? urlSections : null;
        }
    }


    public class Section
    {
        public int StartingIdx { get; set; }
        public int FinishingIdx { get; set; }
        public string SectionBody { get; set; }
    }
}

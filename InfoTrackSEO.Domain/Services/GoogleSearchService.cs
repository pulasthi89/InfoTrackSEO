using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using InfoTrackSEO.Core.Models;
using InfoTrackSEO.Domain.Helpers;

namespace InfoTrackSEO.Domain.Services
{
    public class GoogleSearchService : ISearchService
    {
        private readonly HttpClient _httpClient;

        public GoogleSearchService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<SearchResult> SearchByCriteria(SearchCriteria criteria)
        {
            ServiceHelper.ValidateCriteria(criteria);

            int totalResultSets = 0;
            var searchResult = new SearchResult();
            for (int pageNo = 1; pageNo <= 10; pageNo++) //10 Pages
            {
                var pageUrl = string.Format("Page{0}.html", pageNo.ToString("D2"));
                var pageBody = await _httpClient.GetStringAsync(pageUrl); //In the real world this is where we will pass the keywords into the search engine.
                if (string.IsNullOrEmpty(pageBody))
                    continue;


                int pageResultSets = 0;

                //Assuming the Google search results contain a section for ads
                var adsSection = ServiceHelper.ExtractSection(pageBody, "<div id=\"taw\">", "<div id=\"search\">");
                if (adsSection != null)
                {
                    int sectionResulsets;
                    var adOccurrences = ServiceHelper.GetUrlOccurrences(adsSection, "<li class=\"ads-fr\"", "<li class=\"ads-fr\"", criteria.Url, pageNo, totalResultSets, out sectionResulsets);
                    adOccurrences.ForEach(x => x.ResultUrl = _httpClient.BaseAddress + pageUrl);
                    searchResult.Occurrences.AddRange(adOccurrences);
                    pageResultSets += sectionResulsets;
                    searchResult.TotalOccurrences += adOccurrences.Count;
                }

                //Assuming the Google search results maintains a seperate section for result sets within a div with the id of 'search'
                var resultsSection = ServiceHelper.ExtractSection(pageBody, "<div id=\"search\">", "<div id=\"bottomads\">");
                if (resultsSection != null)
                {
                    int sectionResulsets;
                    var searchResultsOccurrences = ServiceHelper.GetUrlOccurrences(resultsSection, "<div class=\"g\">", "<div class=\"s\">", criteria.Url, pageNo, totalResultSets, out sectionResulsets);
                    searchResultsOccurrences.ForEach(x => x.ResultUrl = _httpClient.BaseAddress + pageUrl);
                    searchResult.Occurrences.AddRange(searchResultsOccurrences);
                    pageResultSets += sectionResulsets;
                    searchResult.TotalOccurrences += searchResultsOccurrences.Count;
                }

                totalResultSets += pageResultSets;

            }

            return searchResult;

        }




    }
}

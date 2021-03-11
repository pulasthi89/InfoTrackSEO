using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using InfoTrackSEO.Core.Models;
using InfoTrackSEO.Domain.Helpers;


namespace InfoTrackSEO.Domain.Services
{
    public class BingSearchService : ISearchService
    {
        private readonly HttpClient _httpClient;

        public BingSearchService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public  async Task<SearchResult> SearchByCriteria(SearchCriteria criteria)
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

                //Assuming the Bing search results contain ads and results in the same section
                var resultsSection = ServiceHelper.ExtractSection(pageBody, "<ol id=\"b_results\">", "</main>");
                if (resultsSection != null)
                {
                    //top ads
                    int sectionResulsets;
                    var topAdOccurrences = ServiceHelper.GetUrlOccurrences(resultsSection, "<li calss=\"b_ad\">", "<li class=\"b_ans", criteria.Url, pageNo, totalResultSets, out sectionResulsets);
                    topAdOccurrences.ForEach(x => x.ResultUrl = _httpClient.BaseAddress + pageUrl);
                    searchResult.Occurrences.AddRange(topAdOccurrences);
                    pageResultSets += sectionResulsets;
                    searchResult.TotalOccurrences += topAdOccurrences.Count;
                    //bottom ads
                    sectionResulsets = 0;
                    var bottomAdOccurrences = ServiceHelper.GetUrlOccurrences(resultsSection, "<li calss=\"b_ad b_adBottom\">", "<li class=\"b_ans", criteria.Url, pageNo, totalResultSets, out sectionResulsets);
                    bottomAdOccurrences.ForEach(x => x.ResultUrl = _httpClient.BaseAddress + pageUrl);
                    searchResult.Occurrences.AddRange(bottomAdOccurrences);
                    pageResultSets += sectionResulsets;
                    searchResult.TotalOccurrences += bottomAdOccurrences.Count;

                    //rest of the results
                    sectionResulsets = 0;
                    var searchResultsOccurrences = ServiceHelper.GetUrlOccurrences(resultsSection, "<li class=\"b_algo\">", "<div class=\"b_attribution\"", criteria.Url, pageNo, totalResultSets, out sectionResulsets);
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

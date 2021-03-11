using InfoTrackSEO.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace InfoTrackSEO.Domain.Services
{
    public class SearchServiceFactory
    {
        private readonly IServiceProvider serviceProvider;

        public SearchServiceFactory(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public ISearchService GetSearchService(string searchEngine)
        {
            if (string.IsNullOrEmpty(searchEngine))
                throw new DomainServiceException("Search engine name must be provided in the search criteria.");

            if (searchEngine == "Google")
                return (ISearchService)serviceProvider.GetService(typeof(GoogleSearchService));
            else if (searchEngine == "Bing")
                return (ISearchService)serviceProvider.GetService(typeof(BingSearchService));
            else
                throw new DomainServiceException("Unable to find the requested search engine.");

        }
    }
}

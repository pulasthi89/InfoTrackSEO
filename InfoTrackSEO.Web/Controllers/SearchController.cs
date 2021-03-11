using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using InfoTrackSEO.Core.Models;
using InfoTrackSEO.Domain.Services;

namespace InfoTrackSEO.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {

        private readonly SearchServiceFactory _searchServiceFactory;

        public SearchController(SearchServiceFactory searchServiceFactory)
        {
            _searchServiceFactory = searchServiceFactory;
        }

        [HttpGet]
        public async Task<SearchResult> Get(string searchEngine, string keywords, string url)
        {
            var service = _searchServiceFactory.GetSearchService(searchEngine);
            var criteria = new SearchCriteria() { Keywords = keywords, Url = url };
            var result = await service.SearchByCriteria(criteria);
            return result;
        }


    }
}

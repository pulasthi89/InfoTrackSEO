using InfoTrackSEO.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace InfoTrackSEO.Domain.Services
{
    public interface ISearchService
    {
        Task<SearchResult> SearchByCriteria(SearchCriteria criteria);
    }
}

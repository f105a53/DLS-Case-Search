using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Services
{
    public class SearchService
    {
        private readonly Common.Index index;

        public SearchService(Common.Index index)
        {
            this.index = index;
        }

        public SearchResults GetResults(string term)
        {
            return index.Search(term);
        }
    }
}

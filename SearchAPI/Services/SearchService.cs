using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Common.Data;

namespace Server.Services
{
    public class SearchService
    {
        private readonly DbContext db;

        public SearchService(DbContext db)
        {
            this.db = db;
        }

        public List<TermDoc> GetResults(string term)
        {
            lock (db)
            {
                if (db.Term.Contains(new Term {Value = term}))
                    return db.TermDoc.Where(td => td.TermId == term).ToList();
                return db.TermDoc.OrderBy(td=>DistanceFrom(term,td.TermId)).ToList();
            }
        }

        public string Normalize(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
        
        public int DistanceFrom(string term,string text) {
            // term = 'app' LOW
            // text = 'application' pretty high
            // return high distance
            var termCharArray = Normalize(term).ToCharArray();
            var textCharArray = Normalize(text).ToCharArray();
            var smallest = textCharArray.Length < termCharArray.Length ? textCharArray : termCharArray;
            var notSmallest = textCharArray.Length < termCharArray.Length ? termCharArray : textCharArray;
            int difference = 0;
            for (int i = 0; i < smallest.Length; i++) {
                bool contained = notSmallest.Contains(smallest[i]);
                 if (!contained) 
                    difference++;
            }

            difference += notSmallest.Length - smallest.Length;
            if (text.Contains(term) && !string.IsNullOrEmpty(term) )
                difference += text.IndexOf(term.Substring(0,1), StringComparison.Ordinal); //create a bigger difference if term is contained further into the word
            return difference;
            
        }
    }
}
using System.Net.NetworkInformation;
using System.Web;
using SpotifyRequestManagement.Services;

namespace SpotifyRequestManagement.Models
{
    public class QuerySearch
    {
        public string? search {  get; set; }
        public string? album { get; set; }
        public string? artist { get; set; }
        public string? track { get; set; }
        public int? year { get; set; }
        public string? genre { get; set; }
        public string[] type { get; set; }

        public string generateQuery() {

            string query = "";

            if(search != null)
                query += $"\"{search}\""; 
            if (album != null)
                query += $" album:\"{album}\"";
            if (artist != null)
                query += $" artist:\"{artist}\"";
            if (track != null)
                query += $" track:\"{track}\"";
            if (year != null)
                query += $" year:{year}";
            if (genre != null)
                query += $" genre:{genre}";

            string param = string.Join(",", type);
            string url = $"q={Uri.EscapeDataString(query)}&type={param}";

            return url; 
        }



    }
}

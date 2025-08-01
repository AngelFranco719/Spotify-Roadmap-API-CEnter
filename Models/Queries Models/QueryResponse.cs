using SpotifyRequestManagement.Models.Multiple_Entities;
using SpotifyRequestManagement.Models.Queries_Models;
using SpotifyRequestManagement.Services;
using System.Text.Json;

namespace SpotifyRequestManagement.Models
{
    public class QueryResponse
    {

        QuerySearch query;
        SpotifyApiRequest apiRequest;
        public string encodedQuery;

        public QueryResponse(QuerySearch query, SpotifyApiRequest apiRequest) {
            this.query = query;
            this.apiRequest = apiRequest;
            encodedQuery = query.generateQuery(); 
        }

        public async Task<QueryResultInterface<TItem>> getQueryResponse<TItem, TResponse>()
        where TResponse: class, QueryResultInterface<TItem> {

            JsonElement result = await apiRequest.searchByQuery(encodedQuery);

            var typeMap = new Dictionary<Type, string>
            {
                { typeof(Albums), "albums" },
                { typeof(Tracks), "tracks" },
                { typeof(Artists), "artists" },
                { typeof(Playlists), "playlists" }
            };

            if (!typeMap.TryGetValue(typeof(TResponse), out string propertyName))
                throw new InvalidOperationException($"El tipo {typeof(TResponse).Name} no esta mapeado");
            if (!result.TryGetProperty(propertyName, out var jsonElement))
                throw new InvalidOperationException($"No se encontro la propiedad {propertyName} en la respuesta JSON");

            try
            {
                var deserialized = JsonSerializer.Deserialize<TResponse>(jsonElement.GetRawText());
                Console.WriteLine(deserialized.ToString());
                return deserialized;
            }
            catch {
                Console.WriteLine("error en el JSON");
                return null; 
            }
           
        }   

    }
}

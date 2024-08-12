using Microsoft.Extensions.Options;
using N5Challenge.Core.Entities;
using N5Challenge.Infrastructure.Settings;
using Nest;

namespace N5Challenge.Infrastructure.Services
{
    public class ElasticsearchService : IElasticsearchService
    {
        private readonly IElasticClient _elasticClient;
        private readonly string _defaultIndex;
        public ElasticsearchService(IOptions<ElasticsearchSettings> settings)
        {
            var uri = new Uri(settings.Value.Uri);
            _defaultIndex = settings.Value.DefaultIndex;
            var connectionSettings = new ConnectionSettings(uri)
                .DefaultIndex(_defaultIndex);
            _elasticClient = new ElasticClient(connectionSettings);
        }
        public async Task<IEnumerable<Permission>> GetAllPermissionsAsync()
        {
            var searchResponse = await _elasticClient.SearchAsync<Permission>(s => s
                .Index(_defaultIndex)
                .Query(q => q.MatchAll())
            );

            return searchResponse.Documents;
        }

        public async Task<Permission> GetPermissionAsync(int id)
        {
            var response = await _elasticClient.GetAsync<Permission>(id, idx => idx.Index(_defaultIndex));
            return response.Source;
        }

        public async Task<bool> UpdatePermissionAsync(Permission permission)
        {
            var response = await _elasticClient.UpdateAsync<Permission>(permission.Id, u => u
                .Doc(permission)
                .DocAsUpsert());
            return response.IsValid;
        }

        public async Task<bool> IndexPermissionAsync(Permission permission)
        {
            var response = await _elasticClient.IndexDocumentAsync(permission);
            return response.IsValid;
        }
    }
}

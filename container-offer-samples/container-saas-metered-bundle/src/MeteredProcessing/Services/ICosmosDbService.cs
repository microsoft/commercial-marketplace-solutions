using SaaS.Metered.Processing.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SaaS.Metered.Processing.Services
{
    public interface ICosmosDbService
    {
        Task<IEnumerable<Item>> GetMultipleAsync(string query);
        Task<IEnumerable<DistinctItem>> GetMultipleDistinctAsync(string query);
        Task<Item> GetAsync(string id);
        Task AddAsync(Item item);
        Task UpdateAsync(string id, Item item);
        Task DeleteAsync(string id);
    }
}

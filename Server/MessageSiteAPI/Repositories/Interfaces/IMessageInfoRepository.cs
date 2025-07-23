using MessageSiteAPI.Models;
using Microsoft.AspNetCore.Http;
using System.Linq.Expressions;

namespace MessageSiteAPI.Repositories.Interfaces
{
    public interface IMessageInfoRepository
    {
        Task<IEnumerable<MessageInfo>> GetAllAsync(Expression<Func<MessageInfo, bool>>? filter = null, string? includeProperties = null);
        Task<MessageInfo?> GetAsync(Expression<Func<MessageInfo, bool>> filter, string? includeProperties = null);
        Task<bool> AddAsync(MessageInfo entity);
        Task<bool> RemoveAsync(MessageInfo entity);
        Task<bool> RemoveRangeAsync(IEnumerable<MessageInfo> entity);
        Task<bool> UpdateAsync(MessageInfo entity);

    }
}

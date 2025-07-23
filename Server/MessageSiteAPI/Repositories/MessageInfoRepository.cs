using MessageSiteAPI.Data;
using MessageSiteAPI.Models;
using MessageSiteAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace MessageSiteAPI.Repositories
{
    public class MessageInfoRepository : IMessageInfoRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<MessageInfoRepository> _logger;

        public MessageInfoRepository(AppDbContext context, ILogger<MessageInfoRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> AddAsync(MessageInfo entity)
        {
            try
            {
                await _context.MessageInfos.AddAsync(entity);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding message info to the database.");
                return false;
            }
        }

        public async Task<IEnumerable<MessageInfo>> GetAllAsync(Expression<Func<MessageInfo, bool>>? filter = null, string? includeProperties = null)
        {
            IQueryable<MessageInfo> query = _context.MessageInfos;
            if (filter != null)
            {
                query = query.Where(filter);
            }
            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProperty in includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty.Trim());
                }
            }

            return await query.ToListAsync();
        }

        public async Task<MessageInfo?> GetAsync(Expression<Func<MessageInfo, bool>> filter, string? includeProperties = null)
        {
            IQueryable<MessageInfo> query = _context.MessageInfos;

            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty.Trim());
                }
            }

            return await query.Where(filter).FirstOrDefaultAsync();
        }

        public async Task<bool> RemoveAsync(MessageInfo entity)
        {
            _context.MessageInfos.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveRangeAsync(IEnumerable<MessageInfo> entity)
        {
            _context.MessageInfos.RemoveRange(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateAsync(MessageInfo entity)
        {
            _context.MessageInfos.Update(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

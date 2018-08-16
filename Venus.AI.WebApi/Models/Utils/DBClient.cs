using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Venus.AI.WebApi.Models.DbModels;

namespace Venus.AI.WebApi.Models.Utils
{
    public class DbClient : IDisposable
    {
        AiDataContext _aiDb;
        public DbClient()
        {
            _aiDb = new AiDataContext();
        }

        public async Task AddMessage(Message message)
        {
            await _aiDb.AddAsync(message);
            await _aiDb.SaveChangesAsync();
        }
        public async Task AddMessage(IEnumerable<Message> messages)
        {
            await _aiDb.AddRangeAsync(messages);
            await _aiDb.SaveChangesAsync();
        }
        public async Task UpdateLastMessage(Message message)
        {
            var msg = _aiDb.Messages.OrderBy(x => x.OwnerId == message.OwnerId).Last();
            msg.Replic = message.Replic;
            await _aiDb.SaveChangesAsync();
        }

        public async Task<UserContext> GetContext(long id)
        {
            return await _aiDb.UserContexts.FindAsync(id);
        }
        public async Task AddOrUpdateContext(UserContext context)
        {
            var cont = await _aiDb.UserContexts.FindAsync(context.Id);
            if (cont == null)
                await _aiDb.AddAsync(context);
            else
                cont = context;
            await _aiDb.SaveChangesAsync();
        }
        public void Dispose()
        {
            _aiDb.Dispose();
        }
    }
}

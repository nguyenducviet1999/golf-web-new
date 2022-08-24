using Golf.Domain.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace Golf.EntityFrameworkCore.Repositories
{
    public class MessageRepository : BaseRepository<Message>
    {
        public MessageRepository(GolfDbContext context) : base(context)
        {

        }
    }
}

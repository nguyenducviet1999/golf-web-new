using Golf.Domain.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace Golf.EntityFrameworkCore.Repositories
{
   
    public class ConversationRepository : BaseRepository<Conversation>
    {
        public ConversationRepository(GolfDbContext context) : base(context)
        {

        }
    }
}

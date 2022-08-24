using System.Collections.Generic;
using System;
using System.Linq;
using Golf.Domain.GolferData;
using Golf.Core.Common.Golfer;
using AutoMapper;
using Golf.Domain.Notifications;

namespace Golf.EntityFrameworkCore.Repositories
{
    public class GolferRepository : BaseRepository<Golfer>
    {
        private readonly IMapper _mapper;
        public GolferRepository(GolfDbContext context,IMapper mapper) : base(context)
        {
            _mapper = mapper;
        }
        public List<Golfer> GetGolfers(List<Guid> GolferIds)
        {
            var golfers = GetAll().Where(golfer => GolferIds.Contains(golfer.Id)).ToList();
            return golfers;
        } 
   
        public MinimizedGolfer GetMinimizedGolfer(Guid GolferId)
        {
            MinimizedGolfer result = null;
            var golfer = this.Get(GolferId);
            if(golfer!=null)
            {
                result = _mapper.Map<MinimizedGolfer>(golfer);
            }    
            return result;
        }
    }
}

using Golf.Domain.Shared.System;
using System;
using System.Collections.Generic;
using System.Text;

namespace Golf.EntityFrameworkCore.Repositories
{
    public class SystemSettingRepository : BaseRepository<SystemSetting>
    {
        public SystemSettingRepository(GolfDbContext context) : base(context)
        {

        }
    }
}

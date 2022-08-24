using Golf.Domain.Report;
using System;
using System.Collections.Generic;
using System.Text;

namespace Golf.EntityFrameworkCore.Repositories
{
    public class ReportRepository : BaseRepository<Report>
    {
        public ReportRepository(GolfDbContext context) : base(context)
        {

        }

    }
}

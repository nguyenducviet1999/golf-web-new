using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Golf.Domain.Shared.System
{
    public class SystemSetting
    {
        public Guid ID { get; set; } = Guid.NewGuid();//Định danh
        [Column(TypeName = "jsonb")]
        public Setting Setting = new Setting();//CThieesst lập hệ thống;
    }
      public class Setting
    {
        public int NumberScorecardAutoConfirm=10;//số scorecard tự động xác nhận
        public int MaxLengthOfCourseNearest = 50;//Khoảng cách lờn nhất của các sân gần đây
    }

}

using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Models
{
    public class ExamSettingResponseModel
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public int? ExamId { get; set; }
        public int? TotalMark { get; set; }
        public int? TotalQuestion { get; set; }
        public int? MaxTime { get; set; }

    }
}

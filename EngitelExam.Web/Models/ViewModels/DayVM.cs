using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EngitelExam.Web.Models.ViewModels
{
    public class DayVM
    {
        public int DayId { get; set; }
        public DateTime Date { get; set; }
        public bool IsAvailable { get; set; }
        public int AppuntamentiCount { get; set; }

    }
}
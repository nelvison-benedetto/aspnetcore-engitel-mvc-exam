using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EngitelExam.Web.Models.ViewModels
{
    public class AppuntamentoVM
    {
        public int AppuntamentoId { get; set; }
        public string Status { get; set; }
        public int DayId { get; set; }
        public int FamigliaId { get; set; }
        public string NomeFamiglia { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EngitelExam.Web.Models.ViewModels
{
    public class FissaAppuntamentoVM
    {

        public int DayId { get; set; }

        [Display(Name = "Nome della famiglia")]  //renderizzato da LabelFor in view razor (Step1.cshtml)
        [Required, StringLength(50)]
        public string NomeFamiglia { get; set; }

    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EngitelExam.Web.Models.ViewModels
{
    public class Step1FamigliaVM
    {
        public int DayId { get; set; }
        public int AppuntamentoId { get; set; }

        [Display(Name = "Nome della famiglia")]  //renderizzato da LabelFor in view razor (Step1.cshtml)
        [Required, StringLength(50)]
        public string NomeFamiglia { get; set; }

        [Display(Name ="Numero componenti della famiglia")]
        [Range(1, 10)]
        public int NumeroComponenti { get; set; }

        [Display(Name ="Numero componenti che possiedono una macchina")]
        [Range(0, 10)]
        public int NumeroComponentiWCar { get; set; }

    }
}
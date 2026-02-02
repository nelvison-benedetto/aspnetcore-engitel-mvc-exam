using EngitelExam.Web.Models.Database;
using EngitelExam.Web.Models.ViewModels;
using EngitelExam.Web.Services.Contracts;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace EngitelExam.Web.Controllers
{
    public class CalendarioController : Controller
    {
        private readonly ICalendarioService _calendarioService;
        public CalendarioController(ICalendarioService calendarioService)
        {
            this._calendarioService = calendarioService;
        }


        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> Calendario(int? year, int? month) 
        { 
            var today = DateTime.Today;
            int y = year ?? today.Year;  //se il valore a sx è null, usa valore a dx
            int m = month ?? today.Month;
            var calendario = await _calendarioService.GetCalendarioMeseAsync(y, m);
            return View(calendario);
        }

        [HttpGet]
        public async Task<ActionResult> Giorno(int dayId)
        {
            var appuntamenti = await _calendarioService.GetAppuntamentiPerGiornoAsync(dayId);
            return View(appuntamenti); // Model: IEnumerable<AppuntamentoVM>
        }

        //[HttpPost]
        //public async Task<ActionResult> PrenotaAppuntamento(int dayId, int famigliaId)
        //{
        //    if (!ModelState.IsValid) return View(model);
        //    var appuntamento = await _calendarioService.AddAppuntamentoAsync(dayId, famigliaId);
        //    if(appuntamento == null) return HttpNotFound();
        //    return RedirectToAction(nameof(Calendario));
        //}

        [HttpGet]
        public async Task<ActionResult> Cancel(int appuntamentoId)
        {
            await _calendarioService.CancelAppuntamentoAsync(appuntamentoId);
            return RedirectToAction(nameof(Calendario));
        }

        [HttpGet]
        public ActionResult FissaAppuntamento(int dayId)
        {
            return View( new FissaAppuntamentoVM { DayId = dayId } );
        }

        [HttpPost]
        public async Task<ActionResult> FissaAppuntamento(FissaAppuntamentoVM model)
        {
            if (!ModelState.IsValid) return View(model);
            var fissaappuntamento = _calendarioService.FissaAppuntamentoAsync(model);
            if(fissaappuntamento == null ) return HttpNotFound();
            return RedirectToAction(nameof(Calendario));  //TODO savare NomeFamiglia su db!! magari su tab a partes
        }

    }
}
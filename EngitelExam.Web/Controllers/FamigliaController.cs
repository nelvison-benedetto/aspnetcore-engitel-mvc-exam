using EngitelExam.Web.Models.Database;
using EngitelExam.Web.Models.ViewModels;
using EngitelExam.Web.Services.Contracts;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Data.Entity;

namespace EngitelExam.Web.Controllers
{
    public class FamigliaController : Controller
    {
        private readonly IFamigliaService _famigliaService;
        private readonly ICalendarioService _calendarioService;

        public FamigliaController(IFamigliaService famigliaService, ICalendarioService calendarioService)
        {
            this._famigliaService = famigliaService;
            this._calendarioService = calendarioService;
        }

        //here uso le SESSION per pssare i dati da form a form, cosi da poter salvare relmente i dati su DB solo in conferma finale dell'utente. alternativa leggermente meno è
        //TempData["Step1"] = model; non adatto per wizard lunghi (redirect multipli).  
        //gli enterprise usano Session + Redis Session Store, o better Draft DB a parte. 

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
        
        //here non metto async Task xk intanto non uso DB! quindi non fetcho nulla
        [HttpGet]
        public async Task<ActionResult> Step1(int dayId, int appuntamentoId, int famigliaId)
        {
            //return View(new Step1FamigliaVM { DayId = dayId, AppuntamentoId = appuntamentoId });
            using (var db = new EngitelDbContext())
            {
                var appuntamento = await db.Appuntamento
                    .Include(a => a.Famiglia)
                    .FirstOrDefaultAsync(a => a.AppuntamentoId == appuntamentoId);
                if (appuntamento == null)
                    return HttpNotFound();
                var model = new Step1FamigliaVM
                {
                    DayId = dayId,
                    AppuntamentoId = appuntamentoId,
                    NomeFamiglia = appuntamento.Famiglia.Nome
                };
                return View(model);
            }
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Step1(Step1FamigliaVM model)
        { 
            if(!ModelState.IsValid) return View(model);
            if (model.NumeroComponentiWCar > model.NumeroComponenti) {
                ModelState.AddModelError("", "numero componenti della famiglia w auto è > di numero componenti della famiglia!");
                return View(model);
            }
            Session["Step1"] = model;  //USO DI SESSION!!X NON SALVARE SU DB
            return RedirectToAction(nameof(Step2));
        }
        
        [HttpGet]
        public ActionResult Step2()
        {
            var step1 = Session["Step1"] as Step1FamigliaVM;
            if (step1 == null) return RedirectToAction(nameof(Step1));
            var model = new Step2VeicoliVM();
            for (int i = 0; i < step1.NumeroComponentiWCar; i++) {
                model.Veicoli.Add(new VeicoloVM());
            }
            return View(model);  //passi il modello pronto alla pagina form x dove lo compilirà l'utente
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Step2(Step2VeicoliVM model)
        {
            if (!ModelState.IsValid) return View(model);
            Session["Step2"] = model;
            return RedirectToAction(nameof(Riepilogo));
        }

        [HttpGet]
        public ActionResult Riepilogo()
        {
            var step1 = Session["Step1"] as Step1FamigliaVM;
            var step2 = Session["Step2"] as Step2VeicoliVM;
            if (step1 == null || step2 == null) return RedirectToAction(nameof(Step1));
            return View( new RiepilogoFamigliaVM { Famiglia=step1, Veicoli=step2.Veicoli } );
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Conferma()   //async Task x uso DB x requests I/O
        {
            try 
            {
                var step1 = Session["Step1"] as Step1FamigliaVM;
                var step2 = Session["Step2"] as Step2VeicoliVM;
                if(step1 == null || step2 == null) return RedirectToAction(nameof(Step1));
                int famigliaId = await _famigliaService.SaveFamigliaAsync(step1, step2);
                //creo appuntamento legato a quel giorno!!!
                Session.Clear();  //PULISCI SESSIONE!!
                return RedirectToAction(nameof(Elenco));
            }
            catch (InvalidOperationException ex)  //catturi l'errore generato
            {
                ModelState.AddModelError("", ex.Message);
                return RedirectToAction(nameof(Step1));
            }
        }
        
        [HttpGet]
        public async Task<ActionResult> Elenco()
        {
            var famiglie = await _famigliaService.GetAllFamiglie();
            return View(famiglie);
        }

    }
}
using EngitelExam.Web.Enums;
using EngitelExam.Web.Models.Database;
using EngitelExam.Web.Models.ViewModels;
using EngitelExam.Web.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace EngitelExam.Web.Services.Implementations
{
    public class CalendarioService : ICalendarioService
    {
        
        public async Task<CalendarioVM> GetCalendarioMeseAsync(int year, int month)
        {
            using (var db = new EngitelDbContext()) {
                db.Database.Log = msg => Console.WriteLine(msg);
                var firstDay = new DateTime(year, month, 1);
                var lastDay = firstDay.AddMonths(1).AddDays(-1);

                var days = await db.Day  //AWAIT xk fetchi dal db!!
                    .Where(d => d.TheDate >= firstDay && d.TheDate <= lastDay)
                    .Include(d => d.Appuntamento)
                    .ToListAsync();

                var calendarioDays = new List<DayVM>();

                //return new CalendarioVM
                //{
                //    Year = year,
                //    Month = month,
                //    Days = days.Select(d => new DayVM
                //    {
                //        DayId = d.DayId,
                //        Date = d.TheDate,
                //        AppuntamentiCount = d.Appuntamento.Count(a=>a.Status=="Booked"),
                //        IsAvailable = true
                //        //hasAppointment = d.Appuntamento.Any(),  //true se almeno 1 exists
                //        //IsAvailable = !d.Appuntamento.Any(a => a.IsAvailable == false)  
                //            //IsAvailable is true se  "nessun nessuno soddisfa la condizione",(leggi tutto senza '!' e poi infine considera il ! invertendo il boolean)
                //    }).ToList()
                //};

                for (var date = firstDay; date <= lastDay; date = date.AddDays(1)) //è come un normale for fatto con i
                {
                    var dayInDb = days.FirstOrDefault(d => d.TheDate == date);
                    if (dayInDb != null)  //quindi esiste perche gia stato creato quel Day
                    {
                        calendarioDays.Add(new DayVM
                        {
                            DayId = dayInDb.DayId,
                            Date = date,
                            AppuntamentiCount = dayInDb.Appuntamento.Count(a => a.Status == "Booked"),
                            IsAvailable = true
                        });
                    }
                    else  //crea il day xk cmnq vuoi che nel calendar sia renderizzato, ma non ha datas all'interno
                    {
                        calendarioDays.Add(new DayVM
                        {
                            DayId = 0,   //0 indica che non esiste ancora in db
                            Date = date,
                            AppuntamentiCount = 0,
                            IsAvailable = true
                        });
                    }
                }
                return new CalendarioVM
                {
                    Year = year,
                    Month = month,
                    Days = calendarioDays
                };

            }
        }

        public async Task<IEnumerable<AppuntamentoVM>> GetAppuntamentiPerGiornoAsync(int dayId)
        {
            using (var db = new EngitelDbContext())
            {
                db.Database.Log = msg => Console.WriteLine(msg);
                var appuntamenti = await db.Appuntamento
                    .Where(a=> a.DayId == dayId)
                    .Include(a=>a.Famiglia)
                    .ToListAsync();
                return appuntamenti.Select(a=> new AppuntamentoVM {  //copy & paste in AppuntamentoVM per ciscuno dei appuntamenti trovati prima
                    AppuntamentoId = a.AppuntamentoId,
                    DayId = a.DayId,
                    FamigliaId = a.FamigliaId,
                    Status = a.Status,
                    NomeFamiglia = a.Famiglia.Nome,
                });
            }
        }

        public async Task<AppuntamentoVM> FissaAppuntamentoAsync(FissaAppuntamentoVM model)
        {
            using (var db = new EngitelDbContext())
            {
                db.Database.Log = msg => Console.WriteLine(msg);

                bool exists = await db.Appuntamento.AnyAsync(a => a.DayId == model.DayId && a.Status == "Booked");
                if (exists) throw new InvalidOperationException("Giorno già prenotato");

                var famiglia = new Famiglia  //crei famiglia minima, cosi persiste su db. 
                {
                    Nome = model.NomeFamiglia,
                    Componenti = 0,
                };
                db.Famiglia.Add(famiglia);
                await db.SaveChangesAsync(); //genera lato db FamigliaId per l'entita
                var appuntamento = new Appuntamento  //crei appuntamento 'vuoto'
                {
                    DayId = model.DayId,
                    FamigliaId = famiglia.FamigliaId,
                    Status = AppuntamentoStatus.Booked.ToString(),
                };
                db.Appuntamento.Add(appuntamento);
                await db.SaveChangesAsync();
                return new AppuntamentoVM
                {
                    AppuntamentoId = appuntamento.AppuntamentoId,
                    DayId = appuntamento.DayId,
                    FamigliaId = famiglia.FamigliaId,
                    Status = appuntamento.Status,
                    NomeFamiglia = famiglia.Nome
                };
            }
        }

        //non serve piu, tutta la logica ora la fai in FissaAppuntamentoAsync()
        //public async Task<AppuntamentoVM> AddAppuntamentoAsync(int dayId, int famigliaId)
        //{
        //    using (var db = new EngitelDbContext())
        //    {
        //        db.Database.Log = msg => Console.WriteLine(msg);
        //        var day = db.Day.FindAsync(dayId);
        //        if (day == null) return null;
        //        var appuntamento = new Appuntamento
        //        {
        //            DayId = dayId,
        //            FamigliaId = famigliaId,
        //            Status = AppuntamentoStatus.Booked.ToString(),  //uso di enum
        //        };
        //        db.Appuntamento.Add(appuntamento);
        //        await db.SaveChangesAsync();
        //        return new AppuntamentoVM
        //        {
        //            AppuntamentoId = appuntamento.AppuntamentoId,
        //            FamigliaId = appuntamento.FamigliaId,
        //            DayId = appuntamento.DayId,
        //            Status = appuntamento.Status,
        //        };
        //    }
        //}

        public async Task CancelAppuntamentoAsync(int appuntamentoId)
        {
            using (var db = new EngitelDbContext())
            {
                db.Database.Log = msg => Console.WriteLine(msg);
                var appuntamento = await db.Appuntamento.FindAsync(appuntamentoId);  //await!!
                if (appuntamento == null) return;
                appuntamento.Status = AppuntamentoStatus.Cancelled.ToString();  //uso di enum!
                await db.SaveChangesAsync();
            }
        }


    }
}
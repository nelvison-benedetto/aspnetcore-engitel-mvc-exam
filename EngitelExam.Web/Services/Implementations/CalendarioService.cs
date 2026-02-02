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
                return new CalendarioVM
                {
                    Year = year,
                    Month = month,
                    Days = days.Select(d => new DayVM
                    {
                        DayId = d.DayId,
                        Date = d.TheDate,
                        AppuntamentiCount = d.Appuntamento.Count(a=>a.Status=="Booked"),
                        IsAvailable = true
                        //hasAppointment = d.Appuntamento.Any(),  //true se almeno 1 exists
                        //IsAvailable = !d.Appuntamento.Any(a => a.IsAvailable == false)  
                            //IsAvailable is true se  "nessun nessuno soddisfa la condizione",(leggi tutto senza '!' e poi infine considera il ! invertendo il boolean)
                    }).ToList()
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
                return appuntamenti.Select(a=> new AppuntamentoVM { 
                    AppuntamentoId = a.AppuntamentoId,
                    DayId = a.DayId,
                    FamigliaId = a.FamigliaId,
                    Status = a.Status,
                    NomeFamiglia = a.Famiglia.Nome,
                });
            }
        }

        public async Task<AppuntamentoVM> AddAppuntamentoAsync(int dayId, int famigliaId)
        {
            using (var db = new EngitelDbContext())
            {
                db.Database.Log = msg => Console.WriteLine(msg);
                var day = db.Day.FindAsync(dayId);
                if (day == null) return null;
                var appuntamento = new Appuntamento
                {
                    DayId = dayId,
                    FamigliaId = famigliaId,
                    Status = AppuntamentoStatus.Booked.ToString(),  //uso di enum
                };
                db.Appuntamento.Add(appuntamento);
                await db.SaveChangesAsync();
                return new AppuntamentoVM
                {
                    AppuntamentoId = appuntamento.AppuntamentoId,
                    FamigliaId = appuntamento.FamigliaId,
                    DayId = appuntamento.DayId,
                    Status = appuntamento.Status,
                };
            }
        }

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
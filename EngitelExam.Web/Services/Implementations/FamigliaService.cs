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
    public class FamigliaService : IFamigliaService
    {

        //public async Task<int> AddFamigliaWithPersonAsync(CreateFamigliaPersonModel model)
        //{
        //    using (var db = new ExamDbContext2()) {
        //        db.Database.Log = msg => Console.WriteLine(msg);
        //        var famiglia = new Famiglia
        //        {
        //            Nome = model.NomeFamiglia,
        //            Componenti = model.NumeroComponenti,
        //        };
        //        if (model.NamePersonsWAuto != null) {
        //            foreach (PersonNameViewModel x in model.NamePersonsWAuto)
        //            {
        //                var person = new Person { Nome = x.NamePerson, Famiglia = famiglia };
        //                db.Person.Add(person);
        //            }
        //        }
        //        db.Famiglia.Add(famiglia);
        //        await db.SaveChangesAsync();
        //        return famiglia.FamigliaId;
        //    }
        //}

        public async Task SaveFamigliaAsync( Step1FamigliaVM step1, Step2VeicoliVM step2)
        {
            using (var db = new EngitelDbContext()) {
                db.Database.Log = msg => Console.WriteLine(msg);

                var famiglia = new Famiglia
                {
                    Nome = step1.NomeFamiglia,
                    Componenti = step1.NumeroComponenti
                };
                db.Famiglia.Add(famiglia);  //aggiungi, cmnq non ancora salvate su db.
                await db.SaveChangesAsync();
                foreach (var v in step2.Veicoli)
                {
                    var veicolo = new Veicolo
                    {
                        Targa = v.Targa,
                        Modello = v.Modello
                    };
                    db.Veicolo.Add(veicolo);
                    await db.SaveChangesAsync();
                    db.Person.Add(new Person
                    {
                        FamigliaId = famiglia.FamigliaId,
                        VeicoloId = veicolo.VeicoloId
                    });
                }
                await db.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<FamigliaListItemVM>> GetAllFamiglie()
        {
            using ( var db = new EngitelDbContext() ) {
                db.Database.Log = msg => Console.WriteLine(msg);
                return await db.Famiglia
                .Select(f => new FamigliaListItemVM
                {
                    NomeFamiglia = f.Nome,
                    NumeroComponenti = f.Componenti,
                    NumeroComponentiWCar = f.Person  //ricorda che dentro Famiglia.cs hai disponibile una collection di Person
                        .Count(p => p.VeicoloId != null)
                })
                .ToListAsync();
            }
        }


    }
}
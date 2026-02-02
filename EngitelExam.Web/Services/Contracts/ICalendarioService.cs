using EngitelExam.Web.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngitelExam.Web.Services.Contracts
{
    public interface ICalendarioService
    {
        Task<CalendarioVM> GetCalendarioMeseAsync(int year, int month);
        Task<AppuntamentoVM> AddAppuntamentoAsync(int dayId, int famigliaId);
    }
}

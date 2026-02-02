using EngitelExam.Web.Services.Contracts;
using EngitelExam.Web.Services.Implementations;
using System.Web.Mvc;
using Unity;
using Unity.Mvc5;

namespace EngitelExam.Web
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
			var container = new UnityContainer();

            // register all your components with the container here
            // it is NOT necessary to register your controllers

            // e.g. container.RegisterType<ITestService, TestService>();

            container.RegisterType<IFamigliaService, FamigliaService>();
            container.RegisterType<ICalendarioService, CalendarioService>();

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}
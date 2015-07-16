using System;
using System.Web.Mvc;
using System.Web.Routing;
using BankModel.Abstract;
using BankModel.Concrete;
using BankModel.Entities;
using BankModel.Entities.Credits;
using BankModel.Entities.Operations;
using JustBank.Authentification.Abstract;
using JustBank.Authentification.Concrete;
using Ninject;

namespace JustBank.Controllers
{
    public class NinjectControllerFactory : DefaultControllerFactory
    {
        private IKernel _ninjectKernel;
        public NinjectControllerFactory()
        {
            _ninjectKernel = new StandardKernel();
            AddBindings();
        }

        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            return controllerType == null
              ? null
              : (IController)_ninjectKernel.Get(controllerType);
        }

        private void AddBindings()
        {
            _ninjectKernel.Bind<IRepository<Client>>().To<EfClientRepository>();
            _ninjectKernel.Bind<IRepository<Operation>>().To<EfOperationRepository>();
            _ninjectKernel.Bind<IRepository<Credit>>().To<EfCreditRepository>();
            _ninjectKernel.Bind<IAuthProvider>().To<AuthProvider>();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Econocom.Model;
using Econocom.Model.Models.Benchmark;
using Econocom.Model.ViewModel;
using Econocom.Web4.Controllers;
using Econocom.Web4.Controllers.ApiControllers;
using Omu.ValueInjecter;

namespace Econocom.Web4.Areas.Admin.Controllers
{
    public class TypeObjetController : BaseController
    {
        private ServiceApiController service;

        public TypeObjetController()
        {
            service = new ServiceApiController();
        }

        [HttpPost]
        public ActionResult Index(ReferenceViewModel rm)
        {
            foreach (var entite in rm.Entitees)
            {
                int? e = entite.Id;
            }
            return View();
        }

        //
        // GET: /Admin/TypeObjet/
            
        public ActionResult Index()
        {
            try
            {
                var liste = service.GetListeTypeObjet();
                ReferenceViewModel rm = new ReferenceViewModel();
                List<EntityViewModel> collection = null;
                object objList = null;
                object newItem = null;
                foreach (Entity x in liste)
                {
                    IValueInjecter injecter = new ValueInjecter();
                    Type typeX = x.GetType();

                    if (objList == null)
                    {
                        objList = CreateGenericList(typeof(EntityViewModel));

                    }
                    if (typeX == typeof(TypeObjet))
                    {
                        newItem = Activator.CreateInstance(typeof(TypeObjetViewModel));
                        ((TypeObjetViewModel)newItem).Id = x.Id;
                    }
                    
                    MethodInfo mListAdd = objList.GetType().GetMethod("Add");
                    newItem.InjectFrom(x);
                    mListAdd.Invoke(objList, new object[] { newItem });
                }

                collection = (List<EntityViewModel>)objList;
                rm.Entitees = collection;
                return View(rm);
            }
            catch (Exception e)
            {
                //log e
                return base.ErreurPartielle();
            }
        }

        //
        // GET:  /Admin/TypeObjet/AfficherObjet
        [ValidateInput(false)]
        public PartialViewResult AfficherObjet(string nomObjet)
        {
            try
            {
                var liste = service.GetListeTypeObjet();
                ReferenceViewModel rm = new ReferenceViewModel();
                List<EntityViewModel> collection = null;
                object objList = null;
                object newItem = null;
                foreach (Entity x in liste)
                {
                    IValueInjecter injecter = new ValueInjecter();
                    Type typeX = x.GetType();

                    if (objList == null)
                    {
                        objList = CreateGenericList(typeof(EntityViewModel));

                    }
                    if (typeX == typeof(TypeObjet))
                    {
                        newItem = Activator.CreateInstance(typeof(TypeObjetViewModel));
                    }
                    MethodInfo mListAdd = objList.GetType().GetMethod("Add");
                    newItem.InjectFrom(x);
                    mListAdd.Invoke(objList, new object[] { newItem });
                }

                collection = (List<EntityViewModel>)objList;
                rm.Entitees = collection;
                return PartialView("TypeObjetPartial", rm);
            }
            catch (Exception e)
            {
                //log e
                return base.ErreurPartielle();
            }
        }



        public ActionResult Filter(string column, string order)
        {
            var columnName = column;
            var orderBy = order;
            return RedirectToAction("Index");
        }

        private Object CreateGenericList(Type typeX)
        {
            Type listType = typeof(List<>);
            Type[] typeArgs = { typeX };
            Type genericType = listType.MakeGenericType(typeArgs);
            object o = Activator.CreateInstance(genericType);
            return o;
        } 


        public ActionResult Detailles(string nomObjet)
        {
            try
            {
                var liste = service.GetListeTypeObjet();
                ReferenceViewModel rm = new ReferenceViewModel();
                List<EntityViewModel> collection = null;
                object objList = null;
                object newItem = null;
                foreach (Entity x in liste)
                {
                    IValueInjecter injecter = new ValueInjecter();
                    Type typeX = x.GetType();

                    if (objList == null)
                    {
                        objList = CreateGenericList(typeof(EntityViewModel));

                    }
                    if (typeX == typeof(TypeObjet))
                    {                       
                        newItem = Activator.CreateInstance(typeof(TypeObjetViewModel));
                        ((TypeObjetViewModel) newItem).Id = x.Id;
                    }
                    
                    MethodInfo mListAdd = objList.GetType().GetMethod("Add");
                    newItem.InjectFrom(x);
                    newItem.InjectFrom(x.Id);
                    mListAdd.Invoke(objList, new object[] { newItem });
                }

                collection = (List<EntityViewModel>)objList;
                rm.Entitees = collection;
                return View(rm);
            }
            catch (Exception e)
            {
                //log e
                return base.Erreur();
            }
        }

    }
}

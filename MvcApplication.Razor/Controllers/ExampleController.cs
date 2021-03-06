﻿using System.Web.Mvc;
using ApprovalTests.Asp.Mvc.Utilities;
using ApprovalUtilities.Asp.Mvc;
using MvcApplication1.Models;

namespace MvcApplication1.Controllers
{
    public class ExampleController : ControllerWithExplicitViews // Automatically adds the .Explicit to all views 
    {
        public ActionResult Index()
        {
            return View().Explicit();
            /*
             * .Explicit() 
             * is the same as 
             * View("Index");
             * and allows the view to work when called from the test controller
             * 
             * Note: this is not actually needed if you use the ControllerWithExplicitViews as a base class
             */
        }

        [HttpPost]
        public ActionResult SaveName(Person person)
        {
            return View(person);
        }
    }
}
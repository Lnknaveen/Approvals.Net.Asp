﻿using System;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Routing;
using ApprovalUtilities.SimpleLogger;

namespace ApprovalTests.Asp.Mvc.Bindings
{
    public class UnitTestControllerFactory : DefaultControllerFactory
    {
        private const string TESTABLE_CONTROLLER_TYPE = "TESTABLE_CONTROLLER_INSTANCE";
        private const string CONTROLLER_UNDER_TEST = "CONTROLLER_UNDER_TEST";
        private const string CONTROLLER_NAME = "controller";

        public UnitTestControllerFactory()
        {
            Logger.Event("Added ApprovalTests Bootstrap");
        }

        protected override Type GetControllerType(RequestContext requestContext, string controllerName)
        {
            var requestItems = requestContext.HttpContext.Items;
            Type returnValue = base.GetControllerType(requestContext, controllerName);

            if (!requestItems.Contains(TESTABLE_CONTROLLER_TYPE) && !requestItems.Contains(CONTROLLER_UNDER_TEST))
            {
                if (returnValue == null)
                    returnValue = TryResolveTestController(requestContext, controllerName);

                var controllerUnderTest = GetControllerUnderTest(returnValue);
                UpdateControllersInRequestContext(requestContext, controllerUnderTest, returnValue);
            }
            else
            {
                returnValue = requestItems[CONTROLLER_UNDER_TEST] as Type;
            }

            return returnValue;
        }

        private static void UpdateControllersInRequestContext(RequestContext requestContext, Type controllerUnderTest,
            Type testableController)
        {
            if (testableController != null && IsTestableType(testableController) &&
                !requestContext.HttpContext.Items.Contains(TESTABLE_CONTROLLER_TYPE))
            {
                requestContext.RouteData.Values[CONTROLLER_NAME] = controllerUnderTest.GetControllerName();
                requestContext.HttpContext.Items[CONTROLLER_UNDER_TEST] = controllerUnderTest;
                requestContext.HttpContext.Items[TESTABLE_CONTROLLER_TYPE] = testableController;
            }
        }

        private Type GetControllerUnderTest(Type testableController)
        {
            return IsTestableType(testableController)
                ? testableController.BaseType.GetGenericArguments().First()
                : testableController;
        }

        private static bool IsTestableType(Type testableController)
        {
            return typeof (TestableControllerBase).IsAssignableFrom(testableController);
        }

        private Type TryResolveTestController(RequestContext requestContext, string className)
        {
            Type returnValue = null;
            var assemblyPath = requestContext.HttpContext.Request.QueryString["assemblyPath"];
            if (!string.IsNullOrEmpty(assemblyPath))
            {
                returnValue = Assembly.LoadFile(assemblyPath).GetController(className);
            }
            return returnValue;
        }

        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            var contextItems = requestContext.HttpContext.Items;
            if (contextItems.Contains(TESTABLE_CONTROLLER_TYPE) && contextItems.Contains(CONTROLLER_UNDER_TEST))
            {
                var instance =
                    (Controller)
                        Activator.CreateInstance(contextItems[TESTABLE_CONTROLLER_TYPE] as Type,
                            new object[]
                            {base.GetControllerInstance(requestContext, contextItems[CONTROLLER_UNDER_TEST] as Type)});

                contextItems.Remove(TESTABLE_CONTROLLER_TYPE);
                contextItems.Remove(CONTROLLER_UNDER_TEST);
                
                return instance;
            }
            else
            {
                return base.GetControllerInstance(requestContext, controllerType);
            }
        }
    }
}
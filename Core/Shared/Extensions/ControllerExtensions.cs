using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Shared.Extensions
{
    //class ControllerExtensions
    //{
    //}

    // todo: get route values from action parameters?
    //public static RedirectToActionResult RedirectToAction<T>(this Controller controller, Expression<Action<T>> action, object routeValues) where T : Controller
    //{
    //    if (!(action?.Body is MethodCallExpression body)) throw new ArgumentException("Expression must be a method call.");
    //    if (body.Object != action.Parameters[0]) throw new ArgumentException("Method call must target lambda argument.");

    //    string actionName = body.Method.Name;

    //    var actionNameAttributes = body.Method.GetCustomAttributes(typeof(ActionNameAttribute), false);
    //    if (actionNameAttributes.Length > 0)
    //    {
    //        var actionNameAttr = (ActionNameAttribute)actionNameAttributes[0];
    //        actionName = actionNameAttr.Name;
    //    }

    //    string controllerName = typeof(T).Name;

    //    if (controllerName.EndsWith("Controller", StringComparison.OrdinalIgnoreCase))
    //    {
    //        controllerName = controllerName.Remove(controllerName.Length - 10, 10);
    //    }
    //    return new RedirectToActionResult(actionName, controllerName, routeValues);
    //}
}

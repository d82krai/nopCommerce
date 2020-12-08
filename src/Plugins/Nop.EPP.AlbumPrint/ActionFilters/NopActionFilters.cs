using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Controllers;

namespace Nop.EPP.AlbumPrint.ActionFilters
{
    public class NopActionFilters : IFilterProvider
    {


        //    public IEnumerable<Filter> GetFilters(ControllerContext controllerContext,
        //ActionDescriptor actionDescriptor)
        //    {
        //        if (controllerContext.Controller is ShoppingCartController &&
        //            actionDescriptor.ActionName.Equals("AddProductToCart_Details",
        //                StringComparison.InvariantCultureIgnoreCase))
        //        {
        //            return new List<Filter>() { new Filter(this, FilterScope.Action, 0) };
        //        }

        //        return new List<Filter>();
        //    }

        //public override void OnActionExecuting(ActionExecutingContext filterContext)
        //{
        //    // do your action here
        //    // first check customer role
        //    // if customer has Paid Member role, do nothing
        //    // otherwise, add product with SKU "paid-member" to the cart
        //}
        public int Order 
        {
            get
            {
                return 0;
            }
        }

        public void OnProvidersExecuted(FilterProviderContext context)
        {
            //throw new NotImplementedException();
        }

        public void OnProvidersExecuting(FilterProviderContext context)
        {
            object controller = "";
            object action = "";
            object sename = "";
            object productid = "";

            context.ActionContext.RouteData.Values.TryGetValue("controller", out controller);
            context.ActionContext.RouteData.Values.TryGetValue("action", out action);
            context.ActionContext.RouteData.Values.TryGetValue("sename", out sename);
            context.ActionContext.RouteData.Values.TryGetValue("productid", out productid);

            if (controller.ToString() == "Product" && action.ToString() == "ProductDetails"
                && int.Parse(productid.ToString()) > 0)
            {
                var routeValues = new RouteValueDictionary();
                routeValues.Add("action", action.ToString());
                routeValues.Add("controller", controller.ToString());
                routeValues.Add("sename", sename.ToString());
                routeValues.Add("productid", productid.ToString());

                //var routeData = new RouteData();
                //routeData
                //context.ActionContext.RouteData = 

                //context.Results = new List<FilterItem>();
                //var filterItem = new FilterItem(new FilterDescriptor(new FilterDescriptor))


                //context.Results.Add(new FilterItem())

            }
        }
    }

    public class YourCustomAuthorizeAttribute : IActionFilter
    {
        //Do something to prevent user from accessing the controller here
        public void OnActionExecuted(ActionExecutedContext context)
        {
            //throw new NotImplementedException();
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            //throw new NotImplementedException();
        }
    }

}

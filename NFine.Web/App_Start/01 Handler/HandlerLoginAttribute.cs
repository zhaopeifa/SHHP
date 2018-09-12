﻿using NFine.Code;
using System.Web.Mvc;

namespace NFine.Web
{
    public class HandlerLoginAttribute : AuthorizeAttribute
    {
        public bool Ignore = true;
        public HandlerLoginAttribute(bool ignore = true)
        {
            Ignore = ignore;
        }
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (Ignore == false)
            {
                return;
            }
            try
            {
                OperatorModel model = null;

                try
                {
                    model = OperatorProvider.Provider.GetCurrent();
                }
                catch { 
                    
                }

                if (model == null)
                {
                    WebHelper.WriteCookie("nfine_login_error", "overdue");
                    filterContext.HttpContext.Response.Write("<script>top.location.href = '/Login/Index';</script>");
                    return;
                }
            }
            catch
            {
                WebHelper.WriteCookie("nfine_login_error", "overdue");
                filterContext.HttpContext.Response.Write("<script>top.location.href = '/Login/Index';</script>");
                return;
            }
        }
    }
}
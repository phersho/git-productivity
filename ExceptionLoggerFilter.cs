using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Filters;
using NLog;

namespace api.Core
{
    public class ExceptionLoggerFilter: ExceptionFilterAttribute
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            _logger.ErrorException("There was a problem executing an api method.", 
                actionExecutedContext.Exception);           
        }
    }
}
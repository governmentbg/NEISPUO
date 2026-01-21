namespace MonProjects.Api.Controllers
{
    using Constants;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;

    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        protected object CheckIsParametersExist(string procedureParameters)
             => string.IsNullOrWhiteSpace(procedureParameters)
                        ? null
                        : this.ProcedureParametersToObject(procedureParameters);

        protected object ProcedureParametersToObject(string procedureParameters)
        {
            IEnumerable<string> parametersParts = procedureParameters
                                                .Split(new char[] { AppConstants.PipeChar }, StringSplitOptions.RemoveEmptyEntries);

            IDictionary<string, object> parameters = new ExpandoObject();

            foreach (var parameterPart in parametersParts)
            {
                IEnumerable<string> parts = parameterPart
                                .Split(new char[] { AppConstants.ColonChar }, StringSplitOptions.RemoveEmptyEntries);

                parameters.Add(parts.FirstOrDefault(), parts.LastOrDefault());
            }

            return parameters;
        }
    }
}

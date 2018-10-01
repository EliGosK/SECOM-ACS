using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CSI.Web.Mvc
{
    /// <summary>
    /// Attribute to validate whether a certain form name (or value) was submitted
    /// </summary>
    public class QueryStringRequiredAttribute : ActionMethodSelectorAttribute
    {
        public QueryStringRequiredAttribute(params string[] keys) :
            this(QueryStringRequirement.Equal, keys)
        {
        }

        public QueryStringRequiredAttribute(QueryStringRequirement requirement, params string[] keys) :
            this(requirement, true, keys)
        {
        }

        public QueryStringRequiredAttribute(QueryStringRequirement requirement, bool validateNameOnly, params string[] keys)
        {
            //at least one submit button should be found
            this.Keys = keys;
            this.ValidateNameOnly = validateNameOnly;
            this.Requirement = requirement;
        }

        public string[] Keys { get; private set; }
        public QueryStringRequirement Requirement { get; set; }
        public bool ValidateNameOnly { get; set; }

        public override bool IsValidForRequest(ControllerContext controllerContext, MethodInfo methodInfo)
        {
            foreach (string key in Keys)
            {
                try
                {
                    switch (this.Requirement)
                    {
                        case QueryStringRequirement.Equal:
                            {
                                if (ValidateNameOnly)
                                {
                                    //"name" only
                                    if (controllerContext.HttpContext.Request.QueryString.AllKeys.Any(x => x.Equals(key, StringComparison.InvariantCultureIgnoreCase)))
                                        return true;
                                }
                                else
                                {
                                    //validate "value"
                                    //do not iterate because "Invalid request" exception can be thrown
                                    string value = controllerContext.HttpContext.Request.QueryString[key];
                                    if (!String.IsNullOrEmpty(value))
                                        return true;
                                }
                            }
                            break;
                        case QueryStringRequirement.StartsWith:
                            {
                                if (ValidateNameOnly)
                                {
                                    //"name" only
                                    if (controllerContext.HttpContext.Request.QueryString.AllKeys.Any(x => x.StartsWith(key, StringComparison.InvariantCultureIgnoreCase)))
                                        return true;
                                }
                                else
                                {
                                    //validate "value"
                                    foreach (var formValue in controllerContext.HttpContext.Request.QueryString.AllKeys)
                                        if (formValue.StartsWith(key, StringComparison.InvariantCultureIgnoreCase))
                                        {
                                            var value = controllerContext.HttpContext.Request.QueryString[formValue];
                                            if (!String.IsNullOrEmpty(value))
                                                return true;
                                        }
                                }
                            }
                            break;
                    }
                }
                catch (Exception exc)
                {
                    //try-catch to ensure that no exception is throw
                    Debug.WriteLine(exc.Message);
                }
            }
            return false;
        }
    }

    public enum QueryStringRequirement
    {
        Equal,
        StartsWith
    }
}

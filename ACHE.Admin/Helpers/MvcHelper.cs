using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace ACHE.Admin.Helpers
{
    public static class MvcHelper
    {
        public static MvcHtmlString BootStrapValidationSummary(this HtmlHelper helper)
        {
            var sb = new StringBuilder();

            var anyErrors = helper.ViewData.ModelState.Values.Where(v => v.Errors.Count != 0).Any();
            var divBeginTag = @"<div class=""alert alert-danger"">";
            var divEndTag = @"</div>";

            if (anyErrors)
            {
                sb.AppendLine(divBeginTag);

                foreach (var key in helper.ViewData.ModelState.Keys)
                {
                    foreach (var error in helper.ViewData.ModelState[key].Errors)
                    {
                        sb.AppendLine(helper.Encode(error.ErrorMessage));
                    }
                }

                sb.AppendLine(divEndTag);
                return new MvcHtmlString(sb.ToString());
            }
            else
            {
                return new MvcHtmlString(sb.ToString());
            }
        }
    }
}
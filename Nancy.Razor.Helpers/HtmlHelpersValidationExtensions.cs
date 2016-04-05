using System;
using System.Linq;
using Nancy.Razor.Helpers.Tag;
using Nancy.ViewEngines.Razor;

namespace Nancy.Razor.Helpers
{
    public static class HtmlHelpersValidationExtensions
    {
        public static void AppendValidationResults<TModel>(this HtmlHelpers<TModel> htmlHelper, HtmlTag tag)
        {
            var validationResult = htmlHelper.RenderContext.Context.ModelValidationResult;
            var name = tag.Attribute("name").Value;
            if (validationResult == null) return;

            if (validationResult.Errors.Any(error => error.Key.Equals(name, StringComparison.InvariantCultureIgnoreCase)))
            {
                var classAttr = tag.Attribute("class");
                var @class = classAttr != null
                    ? string.Concat(classAttr.Value, " input-error")
                    : "input-error";
                tag.RemoveAttribute("class").WithAttribute("class", @class);
            }
        }
    }
}
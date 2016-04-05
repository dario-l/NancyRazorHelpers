using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Nancy.Razor.Helpers.Tag;

namespace Nancy.Razor.Helpers.Extensions
{
    public static class HtmlTagExtensions
    {
        public static void ApplyModelProperty(this HtmlTag tag, object model, string property)
        {
            if (model == null) return;

            var modelProperty = model.GetType().GetProperty(property);
            if (modelProperty != null && modelProperty.CanRead)
            {
                var displayFormat = modelProperty.GetCustomAttribute<DisplayFormatAttribute>();
                tag.WithNonEmptyAttribute(
                    "value",
                    displayFormat == null
                        ? modelProperty.GetValue(model)
                        : FormatForDisplay(displayFormat, modelProperty.GetValue(model)));
            }
        }

        private static object FormatForDisplay(DisplayFormatAttribute displayFormat, object value)
        {
            if (!string.IsNullOrEmpty(displayFormat.DataFormatString))
            {
                return string.Format(displayFormat.DataFormatString, value);
            }
            return value;
        }
    }
}

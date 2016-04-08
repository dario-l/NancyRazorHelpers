using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Nancy.Razor.Helpers.Tag;

namespace Nancy.Razor.Helpers.Extensions
{
    public static class HtmlTagExtensions
    {
        public static void ApplyValueProperty(this HtmlTag tag, object model, string property)
        {
            if (model == null) return;

            var modelProperty = model.GetType().GetProperty(property);
            if (modelProperty != null && modelProperty.CanRead)
            {
                var displayFormat = modelProperty.GetCustomAttribute<DisplayFormatAttribute>();
                var value = modelProperty.GetValue(model);
                if (value == null) return;
                tag.WithNonEmptyAttribute(
                    "value",
                    displayFormat == null ? value : FormatForDisplay(displayFormat, value));
            }
        }

        public static void ApplyInnerTextProperty(this HtmlTag tag, object model, string property)
        {
            if (model == null) return;

            var modelProperty = model.GetType().GetProperty(property);
            if (modelProperty != null && modelProperty.CanRead)
            {
                var displayFormat = modelProperty.GetCustomAttribute<DisplayFormatAttribute>();
                var value = modelProperty.GetValue(model);
                if(value == null) return;
                tag.RawContent = (displayFormat == null ? value : FormatForDisplay(displayFormat, value)).ToString();
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

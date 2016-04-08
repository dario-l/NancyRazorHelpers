using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using Nancy.Razor.Helpers.Extensions;
using Nancy.ViewEngines.Razor;

namespace Nancy.Razor.Helpers.Tag
{
    public static class HtmlTagBuilder
    {
        public static HtmlTag CreateInputElementFor<TModel, TR>(HtmlHelpers<TModel> html,
            Expression<Func<TModel, TR>> prop, HtmlInputType inputType, object htmlAttributes) where TModel : class
        {
            var propertyInfo = prop.AsPropertyInfo();
            if (propertyInfo == null) return null;
            var name = propertyInfo.Name;

            var tag = new HtmlTag("input").
                WithAttribute("type", inputType.ToString().ToLowerInvariant()).
                WithAttribute("name", name.ToLowerInvariant());

            if (inputType != HtmlInputType.Password)
            {
                tag.ApplyValueProperty(html.Model, propertyInfo.Name);
            }

            tag.WithAttributes(htmlAttributes);

            return tag;
        }

        public static HtmlTag CreateTextareaElementFor<TModel, TR>(
            HtmlHelpers<TModel> html,
            Expression<Func<TModel, TR>> prop,
            object htmlAttributes) where TModel : class
        {
            var propertyInfo = prop.AsPropertyInfo();
            if (propertyInfo == null) return null;
            var name = propertyInfo.Name;

            var tag = new HtmlTag("textarea").
                WithAttribute("name", name.ToLowerInvariant());

            tag.WithAttributes(htmlAttributes);
            tag.ApplyInnerTextProperty(html.Model, propertyInfo.Name);
            return tag;
        }

        public static HtmlTag CreateCheckBoxFor<TModel, TR>(HtmlHelpers<TModel> html,
            Expression<Func<TModel, TR>> prop, object htmlAttributes) where TModel : class
        {
            var property = prop.AsPropertyInfo();
            if (property == null) return null;

            if (typeof(TR) == typeof(bool) || typeof(TR) == typeof(bool?))
            {
                return CreateCheckboxForBool(html, property, htmlAttributes);
            }
            return CreateCheckboxForBool(html, property, htmlAttributes);
        }

        private static HtmlTag CreateCheckboxForBool<TModel>(HtmlHelpers<TModel> html, PropertyInfo property, object htmlAttributes) where TModel : class
        {
            var name = property.Name;

            var tag = new HtmlTag("input").
                WithAttribute("type", HtmlInputType.Checkbox.ToString().ToLowerInvariant()).
                WithAttribute("name", name.ToLowerInvariant()).
                WithAttribute("value", true.ToString());

            var propValue = html.Model != null && property.CanRead
                ? Convert.ToBoolean(property.GetValue(html.Model))
                : GetDefaultBoolValue(property);

            if (propValue)
            {
                tag.WithEmptyAttribute("checked");
            }

            tag.WithAttributes(htmlAttributes);

            return tag;
        }

        private static bool GetDefaultBoolValue(PropertyInfo property)
        {
            var defaultvalue = false;
            var defaultAttr = property.GetCustomAttribute<DefaultValueAttribute>();
            if (defaultAttr != null)
            {
                defaultvalue = Convert.ToBoolean(defaultAttr.Value);
            }
            return defaultvalue;
        }

        public static HtmlTag CreateLabelFor<TModel, TR>(Expression<Func<TModel, TR>> prop, object htmlAttributes) where TModel : class
        {
            var property = prop.AsPropertyInfo();
            if (property == null) return null;

            var tag = new HtmlTag("label")
            {
                RawContent = property.GetPresentationName()
            };

            tag.WithAttributes(htmlAttributes);
            return tag;
        }

        public static HtmlTag CreateSelectFor<TModel, TR>(TModel model, Expression<Func<TModel, TR>> prop, SelectList items, object htmlAttributes = null)
            where TModel : class
        {
            if (model == null) return CreateSelectFor(prop, items, htmlAttributes);

            var nameProperty = prop.AsPropertyInfo();
            if (nameProperty == null || !nameProperty.CanRead) return null;
            var currentValue = nameProperty.GetValueAsString(model);

            var tag = new HtmlTag("select").WithAttribute("name", nameProperty.Name.ToLowerInvariant());
            foreach (var item in items)
            {
                var isSelected = item.Selected || (currentValue != null && currentValue.Equals(item.Value));

                tag.WithChild("option").
                    WithEmptyAttributeIf("selected", isSelected).
                    WithAttribute("value", item.Value).
                    RawContent = item.Text;
            }
            tag.WithAttributes(htmlAttributes);
            return tag;
        }

        public static HtmlTag CreateSelectFor<TModel, TR>(Expression<Func<TModel, TR>> prop, SelectList items, object htmlAttributes = null)
            where TModel : class
        {
            var name = prop.AsPropertyInfo();
            if (name == null) return null;

            var tag = new HtmlTag("select").WithAttribute("name", name.Name.ToLowerInvariant());

            foreach (var item in items)
            {
                tag.WithChild("option").
                    WithEmptyAttributeIf("selected", item.Selected).
                    WithAttribute("value", item.Value).
                    RawContent = item.Text;
            }
            tag.WithAttributes(htmlAttributes);
            return tag;
        }
    }
}

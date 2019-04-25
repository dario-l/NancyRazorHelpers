using System;
using System.Text;

namespace Nancy.Razor.Helpers.Tag
{
    public class HtmlAttribute
    {
        private readonly HtmlTag _owner;

        public bool IsWithNoValue => Value == null;
        public string Value { get; private set; }

        internal HtmlAttribute(string name, HtmlTag owner)
        {
            Name = SafeAttributeName(name);
            _owner = owner;
            Value = string.Empty;
        }

        private static string SafeAttributeName(string name)
        {
            var sb = new StringBuilder(name);
            sb.Replace('_', '-');
            return sb.ToString().ToLowerInvariant();
        }

        public string Name { get; }

        public HtmlTag WithValue(object value)
        {
            Value = value != null ? (string)Convert.ChangeType(value, typeof(string)) : string.Empty;
            return _owner;
        }

        public HtmlTag WithNovalue()
        {
            Value = null;
            return _owner;
        }

        public override string ToString()
        {
            if (Value == null) return Name;
            return string.IsNullOrEmpty(Value) ? string.Empty : $"{Name}=\"{System.Net.WebUtility.HtmlEncode(Value ?? string.Empty)}\"";
        }
    }
}
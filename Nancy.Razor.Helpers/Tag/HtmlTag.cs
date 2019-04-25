using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nancy.Razor.Helpers.Tag
{
    public class HtmlTag
    {
        private readonly HtmlTag _parent;
        private readonly string _name;
        private readonly IDictionary<string, HtmlAttribute> _attributes;
        private readonly List<HtmlTag> _childs;
        public bool AutoClose { get; set; }
        public string RawContent { get; set; }

        public HtmlAttribute Attribute(string name)
        {
            var attrName = name.ToLowerInvariant();
            _attributes.TryGetValue(attrName, out var attr);
            return attr;
        }


        public HtmlTag(string name)
        {
            _name = name;
            _attributes = new Dictionary<string, HtmlAttribute>();
            _childs = new List<HtmlTag>();
            AutoClose = true;
        }

        private HtmlTag(string name, HtmlTag parent)
            : this(name)
        {
            _parent = parent;
        }

        public bool IsRoot => _parent == null;

        public IEnumerable<HtmlTag> Childs => _childs;

        public HtmlTag WithAttributes(object attributes)
        {
            if (attributes == null) return this;
            var properties = attributes.GetType().GetProperties();

            foreach (var property in properties.Where(p => p.CanRead))
            {
                WithNonEmptyAttribute(property.Name, property.GetValue(attributes));
            }

            return this;
        }

        public HtmlTag WithChild(string name)
        {
            var child = new HtmlTag(name, this);
            _childs.Add(child);
            return child;
        }

        public HtmlTag ToRoot()
        {
            var currentTag = this;
            while (currentTag._parent != null)
            {
                currentTag = currentTag._parent;
            }
            return currentTag;
        }

        public HtmlTag ToParent() => this._parent;

        public HtmlTag ToParentIfAny() => IsRoot ? this : this._parent;

        public HtmlTag WithNonEmptyAttribute(string name, object value)
        {
            if (value == null || HaveAttribute(name)) return this;
            var attribute = new HtmlAttribute(name, this);
            attribute.WithValue(value);
            _attributes.Add(attribute.Name, attribute);
            return this;
        }

        public HtmlTag WithEmptyAttribute(string name)
        {
            if (HaveAttribute(name)) return this;

            var attr = new HtmlAttribute(name, this);
            attr.WithNovalue();
            _attributes.Add(attr.Name, attr);
            return this;
        }

        public HtmlTag WithEmptyAttributeIf(string name, bool add) => add ? WithEmptyAttribute(name) : this;

        private bool HaveAttribute(string name) => _attributes.ContainsKey(name.ToLowerInvariant());

        public HtmlTag RemoveAttribute(string name)
        {
            if (HaveAttribute(name))
            {
                _attributes.Remove(name.ToLowerInvariant());
            }

            return this;
        }

        public HtmlTag WithAttribute(string name, object value)
        {
            if (HaveAttribute(name)) return this;

            var attribute = new HtmlAttribute(name, this);
            if (value == null)
            {
                attribute.WithNovalue();
            }
            else
            {
                attribute.WithValue(value);
                _attributes.Add(attribute.Name, attribute);
            }
            return this;
        }

        public override string ToString()
        {
            var renderAutoClose = AutoClose && !_childs.Any() && string.IsNullOrEmpty(RawContent);
            var sb = new StringBuilder();
            sb.Append('<');
            sb.Append(_name.ToLowerInvariant());
            sb.Append(' ');
            foreach (var attribute in _attributes.Values)
            {
                var attrstring = attribute.ToString();
                sb.Append(attrstring);
                if (!string.IsNullOrEmpty(attrstring))
                {
                    sb.Append(' ');
                }
            }

            sb.Append(renderAutoClose ? "/>" : ">");

            if (_childs.Any())
            {
                foreach (var child in _childs)
                {
                    sb.Append(child);
                }
            }
            else if (!string.IsNullOrEmpty(RawContent))
            {
                sb.Append(System.Net.WebUtility.HtmlEncode(RawContent));
            }

            if (!renderAutoClose)
            {
                sb.Append("</");
                sb.Append(_name.ToLowerInvariant());
                sb.Append('>');
            }

            sb.AppendLine();
            return sb.ToString();
        }
    }
}

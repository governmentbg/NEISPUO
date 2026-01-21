namespace Kontrax.Utility.Xsd.Models
{
    public class XsdElement : XsdNamedObject
    {
        private readonly XsdMultiplicity _multiplicity;
        private readonly XsdType _type;

        public XsdElement(string name, string qName, XsdMultiplicity multiplicity, XsdType type, string description)
            : base(name, qName, description)
        {
            _multiplicity = multiplicity;
            _type = type;
        }

        public XsdMultiplicity Multiplicity { get { return _multiplicity; } }

        public XsdType Type { get { return _type; } }

        public override string ToString()
        {
            string typeTitle = _type != null
                ? _type.Name ?? _type.SimpleTypeCode ?? _type.QName
                : "неизвестен";

            bool isInlineType = _type != null && typeTitle == null;
            return isInlineType
                ? $"Елемент {Name ?? QName} {_multiplicity}, {Description ?? "без описание"}{_nl}{_indent}inline {_type.ToString().Replace(_nl, _nl + _indent)}"
                : $"Елемент {Name ?? QName} {_multiplicity} тип {typeTitle}, {Description ?? "без описание"}";
        }
    }
}

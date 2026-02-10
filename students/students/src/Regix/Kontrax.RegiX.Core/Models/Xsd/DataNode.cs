using System;
using System.Collections.Generic;
using System.Linq;

namespace Kontrax.RegiX.Core.TestStandard.Models.Xsd
{
    public class DataNode
    {
        private readonly string _qName;
        private readonly string _value;
        private readonly DataNode[] _children;

        public DataNode(string qName, string value, DataNode[] children)
        {
            _qName = qName;
            _value = value;
            _children = children;
        }

        public string QName { get { return _qName; } }

        public string Value { get { return _value; } }

        public IReadOnlyList<DataNode> Children { get { return _children; } }

        #region Грубо XML форматиране само за debug цели.

        private static readonly string _nl = Environment.NewLine;
        protected const string _indent = "  ";

        /// <summary>
        /// Форматира обекта в неточен XML формат без namespaces.
        /// Да се използва само за предварителен преглед на резултата в дебъгер!
        /// </summary>
        public override string ToString()
        {
            if (_value == null && (_children == null || _children.Length == 0))
            {
                return null;
            }
            string localName = _qName != null ? _qName.Split(':').Last() : "NoName";
            string childrenText = _children != null ? string.Join(string.Empty, _children.Select(c => FormatChild(c.ToString()))) : null;
            if (!string.IsNullOrEmpty(childrenText))
            {
                string valueText = _value != null ? $"{_nl}{_indent}{_value}" : null;
                return $"<{localName}>{valueText}{childrenText}{_nl}</{localName}>";
            }
            return _value != null ? $"<{localName}>{_value}</{localName}>" : null;
        }

        private static string FormatChild(string text)
        {
            if (text == null)
            {
                return null;
            }
            return $"{_nl}{_indent}{text.Replace(_nl, _nl + _indent)}";
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;

namespace Kontrax.RegiX.Core.TestStandard.Models
{
    public class DataNodeModel
    {
        public string Value { get; set; }
        public string TypeName { get; set; }

        public bool IsDateTime
        {
            get
            {
                return (TypeName != null) && TypeName.ToUpper() == "DATETIME";
            }
        }

        public bool IsDate
        {
            get
            {
                return (TypeName != null) && TypeName.ToUpper() == "DATE";
            }
        }

        public bool IsMonth
        {
            get
            {
                return (TypeName != null) && TypeName.ToUpper() == "GMONTH";
            }
        }

        public bool IsDay
        {
            get
            {
                return (TypeName != null) && TypeName.ToUpper() == "GDAY";
            }
        }

        public bool IsTime
        {
            get
            {
                return (TypeName != null) && TypeName.ToUpper() == "TIME";
            }
        }

        public bool IsBoolean
        {
            get
            {
                return (TypeName != null) && TypeName.ToUpper() == "BOOLEAN";
            }
        }

        // !!! Важно е това property да е след TypeName, защото по TypeName се слага стойността на Value (чрез ValueForView(set)), а при post на формата към сървъра, 
        // property-тата се сетват в реда, в който са в обекта.
        // property-то е тип object, а не string, защото понякога стойността е дата, която се подава на темплейт за дата, който очаква DateTime
        public object ValueForView
        {
            get
            {
                if (IsMonth)
                {
                    return !string.IsNullOrEmpty(Value) && Value.Length == 4 ? Value.Substring(2) : Value;
                }
                else if (IsDay)
                {
                    return !string.IsNullOrEmpty(Value) && Value.Length == 5 ? Value.Substring(3) : Value;
                }
                else if (IsTime)
                {
                    return (!string.IsNullOrEmpty(Value)) ? DateTime.Parse(Value).ToShortTimeString() : Value;
                }
                else if (IsDate)
                {
                    return (!string.IsNullOrEmpty(Value)) ? DateTime.Parse(Value) : (DateTime?)null;
                }
                else if (IsDateTime)
                {
                    return (!string.IsNullOrEmpty(Value)) ? DateTime.Parse(Value) : (DateTime?)null;
                }
                else if (IsBoolean)
                {
                    return Value != null && Value.ToUpper() == "TRUE" ? true : false;
                }
                else
                {
                    return Value ?? "";
                }
            }
            set
            {
                string s =
                    value == null ? null :
                    value.GetType().IsArray ? (string)((value as IList<object>)[0]) :
                    (string)value;
                if (IsMonth)
                {
                    Value = !string.IsNullOrEmpty(s) ? "--" + s.ToString().PadLeft(2, '0') : s;
                }
                else if (IsDay)
                {
                    Value = !string.IsNullOrEmpty(s) ? "---" + s.ToString().PadLeft(2, '0') : s;
                }
                else if (IsTime)
                {
                    Value = DateTime.TryParse(s, out DateTime date) ? date.ToString("HH:mm:ss") : s;
                }
                else if (IsDate)
                {
                    Value = DateTime.TryParse(s, out DateTime date) ? date.ToString("yyyy-MM-dd") : s;
                }
                else if (IsDateTime)
                {
                    Value = DateTime.TryParse(s, out DateTime date) ? date.ToString("yyyy-MM-ddTHH:mm:ss") : s;
                }
                else if (IsBoolean)
                {
                    //от checkbox-а си идват стойностите както трябва - true и false
                    Value = s;
                }
                else
                {
                    Value = s;
                }
            }
        }

        public string[] EnumValues { get; set; }

        public bool IsRequired { get; set; }

        public IEnumerable<string> KeyTypeCodes { get; set; }

        public string ErrorMessage { get; set; }

        public MultiNodeModel[] Children { get; set; }

        public DataNodeModel CloneEmpty()
        {
            return new DataNodeModel
            {
                TypeName = TypeName,
                EnumValues = EnumValues,
                IsRequired = false,  // Допълнителните инстанции винаги са над Min бройката, затова не са задължителни.
                KeyTypeCodes = KeyTypeCodes,
                Children = Children?.Select(c => new MultiNodeModel
                {
                    QName = c.QName,
                    Name = c.Name,
                    Title = c.Title,
                    TypeName = c.TypeName,
                    TypeTitle = c.TypeTitle,
                    Min = c.Min,
                    Max = c.Max,
                    MultiplicitySymbol = c.MultiplicitySymbol,
                    KeyElement = c.KeyElement,
                    Instances = c.Instances?.Select(i => i.CloneEmpty())?.ToArray(),
                    EnumValues = c.EnumValues
                })?.ToArray()
            };
        }

        public bool SupportsValue
        {
            // Засега няма надежден начин да се определи дали елементът поддържа прости стойности или не.
            // Ползват се косвени признаци: а) Ако не поддържа поделементи би следвало да поддържа поне прости стойности.
            // б) Ако по някакъв начин се е сдобил със стойност се предполага, че поддържа прости стойности.
            get { return !SupportsChildren || Value != null; }
        }

        public bool SupportsChildren
        {
            get
            {
                // Children.Length == 0 НЕ означава, че не се поддържат поделементи. Възможно е да има complex type с празен sequence.
                return Children != null;
            }
            set
            {
                // При POST на 0 поделемента масивите Children и дори Instances се изгубват напълно.
                // За да не се изгуби информацията, че се поддържат поделементи, това се съхранява в hidden поле.
                if (value && !SupportsChildren)
                {
                    Children = new MultiNodeModel[0];
                }
            }
        }

        public bool IsEmpty
        {
            get
            {
                if (SupportsValue && !string.IsNullOrEmpty(Value))
                {
                    return false;
                }
                if (SupportsChildren)
                {
                    if (Children.Length == 0)
                    {
                        return false;  // Complex type с празен sequence се счита за попълнен.
                    }
                    foreach (MultiNodeModel child in Children)
                    {
                        if (child.Instances == null || child.Instances.Length == 0)
                        {
                            return false;  // Елементите с minOccurs==maxOccurs==0 се считат за попълнени.
                        }
                        foreach (DataNodeModel childInstance in child.Instances)
                        {
                            if (!childInstance.IsEmpty)
                            {
                                return false;
                            }
                        }
                    }
                }
                return true;
            }
        }

        private RequestBaseModel _request;

        public RequestBaseModel Request
        {
            get
            {
                return _request;
            }
            set
            {
                _request = value;
                if (SupportsChildren)
                {
                    foreach (MultiNodeModel child in Children)
                    {
                        child.Request = value;
                    }
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Diplomas.Public.Models
{
    public class PropertyDescription
    {
        public int Position { get; set; }
        public string Key { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
    }


    public class PropertyDescriptionValue: PropertyDescription
    {
        public string Value { get; set; }
    }
}

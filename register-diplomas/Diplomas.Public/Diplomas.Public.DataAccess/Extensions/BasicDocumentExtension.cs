using Diplomas.Public.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Diplomas.Public.DataAccess
{
    public partial class BasicDocument
    {
        // public List<PropertyDescription> Contents { get; set; }

        [NotMapped]
        public List<PropertyDescription> ContentsJson
        {
            get
            {
                return Contents != null ? (JsonConvert.DeserializeObject<List<PropertyDescription>>(Contents) ?? new List<PropertyDescription>()): new List<PropertyDescription>();
            }
            set
            {
                Contents = JsonConvert.SerializeObject(value);
            }
        }
    }
}

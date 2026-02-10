using Diplomas.Public.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Diplomas.Public.DataAccess
{
    public partial class Diploma
    {
        // В сучай, че искаме да използваме директно това поле, трябва да премахваме оригиналтото property
        // public List<KeyValue> Contents { get; set; }

        [NotMapped]
        public List<KeyValue> ContentsJson
        {
            get
            {
                try
                {
                    return JsonConvert.DeserializeObject<List<KeyValue>>(Contents) ?? new List<KeyValue>();
                }
                catch
                {
                    return new List<KeyValue>();
                }
            }
            set
            {
                Contents = JsonConvert.SerializeObject(value);
            }
        }
    }
}

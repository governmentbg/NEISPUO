using System;
using System.Collections.Generic;

namespace Helpdesk.Models.Question
{
    public class QuestionModel
    {
        public int? Id { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
    }
}

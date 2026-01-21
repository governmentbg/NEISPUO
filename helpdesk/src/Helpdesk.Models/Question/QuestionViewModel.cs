namespace Helpdesk.Models.Question
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class QuestionViewModel: QuestionModel
    {
        public DateTime CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }
    }
}

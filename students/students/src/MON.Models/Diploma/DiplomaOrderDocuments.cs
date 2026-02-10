namespace MON.Models.Diploma
{
    using System.Collections.Generic;

    public class DiplomaOrderDocuments
    {
        public int Id { get; set; }
        public List<OrderedDocument> DocumentPositions { get; set; }
    }

    public class OrderedDocument
    {
        public int Position { get; set; }
        public int Id { get; set; }
    }

}

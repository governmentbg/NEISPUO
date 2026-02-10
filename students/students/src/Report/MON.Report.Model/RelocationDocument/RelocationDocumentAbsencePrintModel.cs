namespace MON.Report.Model
{
    public class RelocationDocumentAbsencePrintModel
    {
        public float AbsencesForValidReasonsFirstTerm { get; set; }

        public float AbsencesForInvalidReasonsFirstTerm { get; set; }

        public float AbsencesForValidReasonsSecondTerm { get; set; }

        public float AbsencesForInvalidReasonsSecondTerm { get; set; }

        public float AnnualAbsencesForValidReasons => AbsencesForValidReasonsFirstTerm + AbsencesForValidReasonsSecondTerm;

        public float AnnualAbsencesForInvalidReasons => AbsencesForInvalidReasonsFirstTerm + AbsencesForInvalidReasonsSecondTerm;
    }
}

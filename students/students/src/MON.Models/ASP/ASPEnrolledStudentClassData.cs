using System;

namespace MON.Models.ASP
{
    public class ASPEnrolledStudentClassData : IEquatable<ASPEnrolledStudentClassData>
    {
        public int PersonId { get; set; }
        public int InstitutionCode { get; set; }
        public string PersonalId { get; set; }
        public int? PersonalIdType { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public int StudentEduFormId { get; set; }
        public int BasicClassId { get; set; }
        public int Status { get; set; }

        public bool Equals(ASPEnrolledStudentClassData other)
        {
            if (other == null)
            {
                return false;
            }

            return PersonId == other.PersonId && InstitutionCode == other.InstitutionCode && PersonalId == other.PersonalId && PersonalIdType == other.PersonalIdType
                && FirstName == other.FirstName && MiddleName == other.MiddleName && LastName == other.LastName && StudentEduFormId == other.StudentEduFormId
                && BasicClassId == other.BasicClassId && Status == other.Status;
        }

        public override bool Equals(object obj) => Equals(obj as ASPEnrolledStudentClassData);
        public override int GetHashCode() => (PersonId, PersonalId, InstitutionCode).GetHashCode();
    }
}

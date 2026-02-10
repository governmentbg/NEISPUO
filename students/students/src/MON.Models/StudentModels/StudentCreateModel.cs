namespace MON.Models.StudentModels
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class StudentCreateModel
    {
        /// <summary>
        /// Използва се при редакция
        /// </summary>
        public int? Id { get; set; }
        [Required]
        public string Pin { get; set; }
        [Required]
        public int PinTypeId { get; set; }
        [Required]
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        [Required]
        public string LastName { get; set; }
        public DateTime? BirthDate { get; set; }
        [Required]
        public int GenderId { get; set; }
        public int? BirthPlaceId { get; set; }
        public int? UsualResidenceId { get; set; }
        public int? PermanentResidenceId { get; set; }
        [Required]
        public string CurrentAddress { get; set; }
        [Required]
        public string PermanentAddress { get; set; }
        public string PhoneNumber { get; set; }
        [Required]
        public int? NationalityId { get; set; }
        [Required]
        public int? BirthPlaceCountryId { get; set; }
        public string BirthPlace { get; set; }
        public string Email { get; set; }
    }
}

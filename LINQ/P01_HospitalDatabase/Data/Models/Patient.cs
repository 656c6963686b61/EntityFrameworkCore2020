using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace P01_HospitalDatabase.Data.Models
{
    using static DataValidation.Patient;
    using System.ComponentModel.DataAnnotations;

    public class Patient
    {
        public Patient()
        {
            this.Visitations = new List<Visitation>();
            this.Prescriptions = new List<PatientMedicament>();
            this.Diagnoses = new List<Diagnosis>();
        }
        [Key]
        public int PatientId { get; set; }

        [Required]
        [MaxLength(FirstNameMaxLength)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(LastNameMaxLength)]
        public string LastName { get; set; }

        [Required]
        [MaxLength(AddressMaxLength)]
        public string Address { get; set; }

        [Required]
        [MaxLength(EmailMaxLength)]
        public string Email { get; set; }

        [Required]
        public bool HasInsurance { get; set; }

        public ICollection<Visitation> Visitations { get; set; }

        public ICollection<PatientMedicament> Prescriptions { get; set; }

        public ICollection<Diagnosis> Diagnoses { get; set; }
    }
}

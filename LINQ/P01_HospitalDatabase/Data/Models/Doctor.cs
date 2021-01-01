using System.Collections.Generic;

namespace P01_HospitalDatabase.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    using static DataValidation.Doctor;

    public class Doctor
    {
        public Doctor()
        {
            this.Visitations = new List<Visitation>();
        }
        [Key]
        public int DoctorId { get; set; }

        [Required]
        [MaxLength(DoctorMaxLength)]
        public string Name { get; set; }

        [Required]
        [MaxLength(DoctorMaxLength)]
        public string Specialty { get; set; }

        public ICollection<Visitation> Visitations { get; set; }
    }
}
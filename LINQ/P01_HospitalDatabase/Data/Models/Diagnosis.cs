namespace P01_HospitalDatabase.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using static DataValidation.Diagnosis;

    public class Diagnosis
    {
        [Key]
        public int DiagnosisId { get; set; }

        [MaxLength(NameMaxLength)]
        [Required]
        public string Name { get; set; }

        [MaxLength(CommentsMaxLength)]
        public string Comments { get; set; }
        
        [ForeignKey(nameof(Patient))]
        public int PatientId { get; set; }

        public Patient Patient { get; set; }
    }
}

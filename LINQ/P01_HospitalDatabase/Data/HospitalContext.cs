namespace P01_HospitalDatabase.Data
{
    using Microsoft.EntityFrameworkCore;
    using Models;
    using static DataSettings;

    public class HospitalContext : DbContext
    {
        public HospitalContext()
        {
            
        }

        public HospitalContext(DbContextOptions options)
            :base(options)
        {
            
        }

        //DbSets
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Visitation> Visitations { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<PatientMedicament> PatientMedicaments { get; set; }
        public DbSet<Medicament> Medicaments { get; set; }
        public DbSet<Diagnosis> Diagnoses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //unicode
            
            modelBuilder.Entity<Patient>()
                .Property(x => x.FirstName)
                .IsUnicode();
            modelBuilder.Entity<Patient>()
                .Property(x => x.LastName)
                .IsUnicode();
            modelBuilder.Entity<Patient>()
                .Property(x => x.Address)
                .IsUnicode();
            modelBuilder.Entity<Patient>()
                .Property(x => x.Email)
                .IsUnicode();

            modelBuilder.Entity<Visitation>()
                .Property(x => x.Comments)
                .IsUnicode();

            modelBuilder.Entity<Diagnosis>()
                .Property(x => x.Name)
                .IsUnicode();
            modelBuilder.Entity<Diagnosis>()
                .Property(x => x.Comments)
                .IsUnicode();

            modelBuilder.Entity<Medicament>()
                .Property(x => x.Name)
                .IsUnicode();

            //composite key
            modelBuilder.Entity<PatientMedicament>()
                .HasKey(x => new {x.PatientId, x.MedicamentId});
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(DefaultConnection);
            }
        }
    }
}

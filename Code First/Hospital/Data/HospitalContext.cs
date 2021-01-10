namespace P01_HospitalDatabase.Data
{
    using Microsoft.EntityFrameworkCore;
    using P01_HospitalDatabase.Data.Models;

    public class HospitalContext : DbContext
    {
        public HospitalContext()
        {

        }

        public HospitalContext(DbContextOptions options)
            : base(options)
        {

        }

        public DbSet<Diagnose> Diagnoses { get; set; }
        public DbSet<Medicament> Medicaments { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<PatientMedicament> PatientMedicaments { get; set; }
        public DbSet<Visitation> Visitations { get; set; }
        public DbSet<Doctor> Doctors { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(Configuration.ConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Patient>()
                .HasKey(x => x.PatientId);

            modelBuilder
                .Entity<Patient>()
                .Property(x => x.FirstName)
                .HasMaxLength(50)
                .IsRequired(true)
                .IsUnicode();

            modelBuilder
                .Entity<Patient>()
                .Property(x => x.LastName)
                .HasMaxLength(50)
                .IsRequired(true)
                .IsUnicode();

            modelBuilder
                .Entity<Patient>()
                .Property(x => x.Address)
                .HasMaxLength(250)
                .IsRequired(true)
                .IsUnicode();

            modelBuilder
                .Entity<Patient>()
                .Property(x => x.Email)
                .HasMaxLength(80);

            modelBuilder
                .Entity<Patient>()
                .Property(x => x.HasInsurance)
                .IsRequired(true);

            modelBuilder
                .Entity<Visitation>()
                .HasKey(x => x.VisitationId);

            modelBuilder
                .Entity<Visitation>()
                .Property(x => x.Date)
                .IsRequired(true);

            modelBuilder
                .Entity<Visitation>()
                .Property(x => x.Comments)
                .HasMaxLength(250)
                .IsUnicode();

            modelBuilder
                .Entity<Visitation>()
                .HasOne(x => x.Patient)
                .WithMany(x => x.Visitations)
                .HasForeignKey(x => x.PatientId);

            modelBuilder
                .Entity<Visitation>()
                .HasOne(x => x.Doctor)
                .WithMany(x => x.Visitations)
                .HasForeignKey(x => x.DoctorId);

            modelBuilder
                .Entity<Diagnose>()
                .HasKey(x => x.DiagnoseId);

            modelBuilder
                .Entity<Diagnose>()
                .Property(x => x.Name)
                .HasMaxLength(50)
                .IsRequired(true)
                .IsUnicode();

            modelBuilder
                .Entity<Diagnose>()
                .Property(x => x.Comments)
                .HasMaxLength(250)
                .IsUnicode();

            modelBuilder
                .Entity<Diagnose>()
                .HasOne(x => x.Patient)
                .WithMany(x => x.Diagnoses)
                .HasForeignKey(x => x.PatientId);

            modelBuilder
                .Entity<Medicament>()
                .HasKey(x => x.MedicamentId);

            modelBuilder
                .Entity<Medicament>()
                .Property(x => x.Name)
                .HasMaxLength(50)
                .IsRequired(true)
                .IsUnicode();

            modelBuilder
                .Entity<PatientMedicament>()
                .HasKey(x => new { x.PatientId, x.MedicamentId });

            modelBuilder
                .Entity<PatientMedicament>()
                .HasOne(x => x.Patient)
                .WithMany(x => x.Prescriptions)
                .HasForeignKey(x => x.PatientId);

            modelBuilder
                .Entity<PatientMedicament>()
                .HasOne(x => x.Medicament)
                .WithMany(x => x.Prescriptions)
                .HasForeignKey(x => x.MedicamentId);

            modelBuilder
                .Entity<Doctor>()
                .HasKey(x => x.DoctorId);

            modelBuilder
                .Entity<Doctor>()
                .Property(x => x.Name)
                .HasMaxLength(100)
                .IsRequired(true)
                .IsUnicode();

            modelBuilder
                .Entity<Doctor>()
                .Property(x => x.Specialty)
                .HasMaxLength(100)
                .IsRequired(true)
                .IsUnicode();
        }
    }
}

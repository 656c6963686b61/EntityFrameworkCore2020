namespace P01_HospitalDatabase.Data
{
    public class DataValidation
    {
        public class Doctor
        {
            public const int DoctorMaxLength = 100;
        }

        public class Diagnosis
        {
            public const int NameMaxLength = 50;
            public const int CommentsMaxLength = 250;
        }

        public class Medicament
        {
            public const int NameMaxLength = 50;
        }
        public class Visitation
        {
            public const int CommentsMaxLength = 250;
        }

        public class Patient
        {
            public const int FirstNameMaxLength = 50;
            public const int LastNameMaxLength = 50;
            public const int AddressMaxLength = 250;
            public const int EmailMaxLength = 80;
        }
    }
}

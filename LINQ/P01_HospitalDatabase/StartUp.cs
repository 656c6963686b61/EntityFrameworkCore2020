namespace P01_HospitalDatabase
{
    using Data;

    class StartUp
    {
        static void Main(string[] args)
        {
            var context = new HospitalContext();
            context.Database.EnsureCreated();
        }
    }
}

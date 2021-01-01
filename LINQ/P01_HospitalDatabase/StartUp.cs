namespace LINQ
{
    using P01_HospitalDatabase.Data;
    class StartUp
    {
        static void Main(string[] args)
        {
            var context = new HospitalContext();
            context.Database.EnsureCreated();
        }
    }
}

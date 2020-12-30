using System;
using System.Linq;
using dbFirst.Data;

namespace dbFirst
{
    class Program
    {
        //onlyTheLastProblem
        static void Main(string[] args)
        {
            var db = new SoftUniContext();

            var town = db.Towns
                .FirstOrDefault(x => x.Name == "Seattle");

            var addressesInSeattle = db.Addresses
                .Where(x => x.Town == town)
                .ToList();

            int count = addressesInSeattle.Count;
            foreach (var address in addressesInSeattle)
            {
                var employees = db.Employees.Where(x => x.Address == address).ToList();
                foreach (var employee in employees)
                {
                    employee.Address = null;
                    employee.AddressId = null;
                }
                db.Addresses.Remove(address);
            }

            db.Towns.Remove(town!);
            Console.WriteLine($"{count} addresses in Seattle were deleted");
            db.SaveChanges();
        }
    }
}

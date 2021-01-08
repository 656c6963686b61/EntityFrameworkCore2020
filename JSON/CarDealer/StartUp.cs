namespace CarDealer
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography.X509Certificates;
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Data;
    using DTO;
    using Models;
    using Newtonsoft.Json;

    public class StartUp
    {
        private static readonly string ResultsFileDirectory = "../../../Results/";

        public static void Main(string[] args)
        {
            CheckIfDirectoryExists(ResultsFileDirectory);

            var context = new CarDealerContext();

            //Console.WriteLine(ImportSuppliers(context, File.ReadAllText("../../../Datasets/suppliers.json")));
            //Console.WriteLine(ImportParts(context, File.ReadAllText("../../../Datasets/parts.json")));
            //Console.WriteLine(ImportCars(context, File.ReadAllText("../../../Datasets/cars.json")));
            //Console.WriteLine(ImportCustomers(context, File.ReadAllText("../../../Datasets/customers.json")));
            //Console.WriteLine(ImportSales(context, File.ReadAllText("../../../Datasets/sales.json")));

            var config = new MapperConfiguration(x =>
            {
                x.AddProfile(new CarDealerProfile());
            });

            //File.WriteAllText(ResultsFileDirectory + "ordered-customers.json", GetOrderedCustomers(context, config));
            //File.WriteAllText(ResultsFileDirectory + "toyota-cars.json", GetCarsFromMakeToyota(context, config));
            //File.WriteAllText(ResultsFileDirectory + "local-suppliers.json", GetLocalSuppliers(context, config));
            File.WriteAllText(ResultsFileDirectory + "cars-and-parts.json", GetCarsWithTheirListOfParts(context, config));
        }

        public static string ImportSuppliers(CarDealerContext context, string inputJson)
        {
            var json = JsonConvert.DeserializeObject<List<Supplier>>(inputJson);
            context.Suppliers.AddRange(json);
            context.SaveChanges();
            return $"Successfully imported {context.Suppliers.Count()}.";
        }

        public static string ImportParts(CarDealerContext context, string inputJson)
        {
            var validIds = context.Suppliers.Select(x => x.Id).ToList();
            var json = JsonConvert.DeserializeObject<List<Part>>(inputJson).Where(x => validIds.Contains(x.SupplierId)).ToList();        
            context.Parts.AddRange(json);
            context.SaveChanges();
            return $"Successfully imported {context.Parts.Count()}.";
        }

        public static string ImportCars(CarDealerContext context, string inputJson)
        {
            var json = JsonConvert.DeserializeObject<List<Car>>(inputJson);
            context.Cars.AddRange(json);
            context.SaveChanges();
            return $"Successfully imported {context.Cars.Count()}.";
        }

        public static string ImportCustomers(CarDealerContext context, string inputJson)
        {
            var json = JsonConvert.DeserializeObject<List<Customer>>(inputJson);
            context.Customers.AddRange(json);
            context.SaveChanges();
            return $"Successfully imported {context.Customers.Count()}.";
        }

        public static string ImportSales(CarDealerContext context, string inputJson)
        {
            var json = JsonConvert.DeserializeObject<List<Sale>>(inputJson);
            context.Sales.AddRange(json);
            context.SaveChanges();
            return $"Successfully imported {context.Sales.Count()}.";
        }

        public static string GetOrderedCustomers(CarDealerContext context, MapperConfiguration config)
        {
            var customers = context
                .Customers
                .ProjectTo<OrderedCustomers>(config)
                .OrderBy(x => x.BirthDate)
                .ThenBy(x => x.IsYoungDriver)
                .ToList();

            return JsonConvert.SerializeObject(customers, Formatting.Indented);
        }

        public static string GetCarsFromMakeToyota(CarDealerContext context, MapperConfiguration config)
        {
            var cars = context
                .Cars
                .ProjectTo<CarsFromToyota>(config)
                .Where(x => x.Make == "Toyota")
                .OrderBy(x => x.Model)
                .ThenByDescending(x => x.TravelledDistance)
                .ToList();

            return JsonConvert.SerializeObject(cars, Formatting.Indented);
        }

        public static string GetLocalSuppliers(CarDealerContext context, MapperConfiguration config)
        {
            var suppliers = context
                .Suppliers
                .Where(x => x.IsImporter == false)
                .ProjectTo<LocalSuppliers>(config)
                .OrderBy(x => x.Name)
                .ToList();

            return JsonConvert.SerializeObject(suppliers, Formatting.Indented);
        }

        public static string GetCarsWithTheirListOfParts(CarDealerContext context, MapperConfiguration config)
        {
            var carsAndParts = context
                .Cars
                .ProjectTo<CarWithParts>(config)
                .ToList();

            return JsonConvert.SerializeObject(carsAndParts, Formatting.Indented);
        }

        private static void CheckIfDirectoryExists(string path)
        {
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        }
    }
}
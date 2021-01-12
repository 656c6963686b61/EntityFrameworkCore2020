namespace CarDealer
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Xml.Serialization;
    using AutoMapper;
    using Data;
    using Dtos.Import;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;
    using Models;

    public class StartUp
    {
        private static string ImportPath = "../../../Datasets/";
        private static string ExportPath = "../../../Results/";
        private static IMapper mapper;
        public static void Main(string[] args)
        {
            var context = new CarDealerContext();
            context.Database.EnsureCreated();

            //check output directory
            EnsureDirectoryCreated(ExportPath);

            //autoMapper config
            var config = new MapperConfiguration(x =>
            {
                x.AddProfile(new CarDealerProfile());
            });
            mapper = config.CreateMapper();

            //Console.WriteLine(ImportSuppliers(context, ImportPath + "suppliers.xml"));
            //Console.WriteLine(ImportParts(context, ImportPath + "parts.xml"));
            //Console.WriteLine(ImportCars(context, ImportPath + "cars.xml"));

        }
        public static string ImportSuppliers(CarDealerContext context, string inputXml)
        {
            var serializer = new XmlSerializer(typeof(ImportSupplier[]), new XmlRootAttribute("Suppliers"));
            var dbSuppliers = new List<Supplier>();

            using (var reader = File.OpenRead(inputXml))
            {
                var suppliers = (ImportSupplier[])serializer.Deserialize(reader);
                dbSuppliers = mapper.Map<List<Supplier>>(suppliers);
            }

            context.Suppliers.AddRange(dbSuppliers);
            context.SaveChanges();

            return $"Successfully imported {context.Suppliers.Count()}"; ;
        }

        public static string ImportParts(CarDealerContext context, string inputXml)
        {
            var serializer = new XmlSerializer(typeof(ImportPart[]), new XmlRootAttribute("Parts"));
            var dbParts = new List<Part>();

            using (var reader = File.OpenRead(inputXml))
            {
                var xmlParts = (ImportPart[])serializer.Deserialize(reader);
                dbParts = mapper.Map<List<Part>>(xmlParts.Where(x => x.SupplierId > 0 && x.SupplierId <= context.Suppliers.Max(y => y.Id)));
            }
            context.Parts.AddRange(dbParts);
            context.SaveChanges();
            return $"Successfully imported {context.Parts.Count()}";
        }

        public static string ImportCars(CarDealerContext context, string inputXml)
        {
            var serializer = new XmlSerializer(typeof(List<ImportCar>), new XmlRootAttribute("Cars"));
            var dbCars = new List<Car>();
            var partCars = new List<PartCar>();
            
            using (var reader = File.OpenRead(inputXml))
            {
                var xmlCars = (List<ImportCar>) serializer.Deserialize(reader);

                foreach (var xmlCar in xmlCars)
                {
                    var parts = xmlCar
                        .PartCars
                        .Select(p => p.PartId)
                        .Where(p => context.Parts.Any(part => part.Id == p))
                        .Distinct();

                    var dbCar = new Car
                    {
                        Make = xmlCar.Make,
                        Model = xmlCar.Model,
                        TravelledDistance = xmlCar.TravelledDistance
                    };
                    
                    foreach (var partId in parts)
                    {
                        var dbPart = new PartCar
                        {
                            PartId = partId,
                            Car = dbCar,
                        };
                        partCars.Add(dbPart);
                    }
                    dbCars.Add(dbCar);
                }
                
            }

            
            context.Cars.AddRange(dbCars);
            context.PartCars.AddRange(partCars);
            context.SaveChanges();
            
            return $"Successfully imported {context.Cars.Count()} + {context.PartCars.Count()} "; ;
        }

        public static string ImportCustomers(CarDealerContext context, string inputXml)
        {

            return $"Successfully imported {context.Customers.Count()}";
        }

        public static string ImportSales(CarDealerContext context, string inputXml)
        {
            return $"Successfully imported {context.Sales.Count()}";
        }

        private static void EnsureDirectoryCreated(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
    }
}
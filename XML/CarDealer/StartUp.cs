namespace CarDealer
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Xml.Serialization;
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Data;
    using Dtos.Export;
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

            //ImportData
            //Console.WriteLine(ImportSuppliers(context, ImportPath + "suppliers.xml"));
            //Console.WriteLine(ImportParts(context, ImportPath + "parts.xml"));
            //Console.WriteLine(ImportCars(context, ImportPath + "cars.xml"));
            //Console.WriteLine(ImportCustomers(context, ImportPath + "customers.xml"));
            //Console.WriteLine(ImportSales(context, ImportPath + "sales.xml"));

            //Export
            //GetCarsWithDistance(context);
            //GetCarsFromMakeBmw(context);
            //GetLocalSuppliers(context);
            //GetCarsWithTheirListOfParts(context);
            //GetTotalSalesByCustomer(context);
            //GetSalesWithAppliedDiscount(context);
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

        //TODO: add automapper
        public static string ImportCars(CarDealerContext context, string inputXml)
        {
            var serializer = new XmlSerializer(typeof(List<ImportCar>), new XmlRootAttribute("Cars"));
            var dbCars = new List<Car>();
            var partCars = new List<PartCar>();

            using (var reader = File.OpenRead(inputXml))
            {
                var xmlCars = (List<ImportCar>)serializer.Deserialize(reader);

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
            var serializer = new XmlSerializer(typeof(ImportCustomer[]), new XmlRootAttribute("Customers"));
            List<Customer> dbCustomers;

            using (var reader = File.OpenRead(inputXml))
            {
                var xmlCustomers = (ImportCustomer[])serializer.Deserialize(reader);
                dbCustomers = mapper.Map<List<Customer>>(xmlCustomers);
            }
            context.Customers.AddRange(dbCustomers);
            context.SaveChanges();
            return $"Successfully imported {context.Customers.Count()}";
        }

        public static string ImportSales(CarDealerContext context, string inputXml)
        {
            var serializer = new XmlSerializer(typeof(ImportSale[]), new XmlRootAttribute("Sales"));
            List<Sale> dbSales;

            using (var reader = File.OpenRead(inputXml))
            {
                var xmlSales = (ImportSale[])serializer.Deserialize(reader);
                dbSales = mapper.Map<List<Sale>>(xmlSales);
            }
            context.Sales.AddRange(dbSales.Where(x => x.CarId > 0 && x.CarId <= context.Cars.Max(y => y.Id)));
            context.SaveChanges();
            return $"Successfully imported {context.Sales.Count()}";
        }

        public static string GetCarsWithDistance(CarDealerContext context)
        {
            var carsToExport = context
                .Cars
                .Where(c => c.TravelledDistance > 2000000)
                .OrderBy(c => c.Make)
                .ThenBy(c => c.Model)
                .Take(10)
                .ProjectTo<CarsWithDistance>(mapper.ConfigurationProvider)
                .ToArray();

            XmlSerializer serializer = new XmlSerializer(typeof(CarsWithDistance[]),
                new XmlRootAttribute("cars"));

            using (var writer = File.OpenWrite(ExportPath + "cars.xml"))
            {
                serializer.Serialize(writer, carsToExport);
            }

            return $"";
        }

        public static string GetCarsFromMakeBmw(CarDealerContext context)
        {
            var bmwCars = context
                .Cars
                .Where(x => x.Make == "BMW")
                .OrderBy(c => c.Model)
                .ThenByDescending(c => c.TravelledDistance)
                .ProjectTo<BmwCars>(mapper.ConfigurationProvider)
                .ToArray();

            XmlSerializer serializer = new XmlSerializer(typeof(BmwCars[]),
                new XmlRootAttribute("cars"));

            using (var writer = File.OpenWrite(ExportPath + "bmw-cars.xml"))
            {
                serializer.Serialize(writer, bmwCars);
            }

            return $"";
        }

        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var suppliers = context
                .Suppliers
                .Where(x => x.IsImporter == false)
                .ProjectTo<LocalSupplier>(mapper.ConfigurationProvider)
                .ToArray();

            XmlSerializer serializer = new XmlSerializer(typeof(LocalSupplier[]),
                new XmlRootAttribute("suppliers"));

            using (var writer = File.OpenWrite(ExportPath + "local-suppliers.xml"))
            {
                serializer.Serialize(writer, suppliers);
            }

            return $"";
        }

        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var carsWithParts = context
                .Cars
                .OrderByDescending(x => x.TravelledDistance)
                .ThenBy(x => x.Model)
                .ProjectTo<CarCAP>(mapper.ConfigurationProvider)
                .Take(5)
                .ToArray();

            XmlSerializer serializer = new XmlSerializer(typeof(CarCAP[]),
                new XmlRootAttribute("cars"));

            using (var writer = File.OpenWrite(ExportPath + "cars-and-parts.xml"))
            {
                serializer.Serialize(writer, carsWithParts);
            }

            return $"";
        }

        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            var customers = context
                .Customers
                .ProjectTo<CustomersTotalSales>(mapper.ConfigurationProvider)
                .Where(x => x.BoughtCars >= 1)
                .OrderByDescending(x => x.MoneySpent)
                .ToArray();

            XmlSerializer serializer = new XmlSerializer(typeof(CustomersTotalSales[]),
                new XmlRootAttribute("customers"));

            using (var writer = File.OpenWrite(ExportPath + "customers-total-sales.xml"))
            {
                serializer.Serialize(writer, customers);
            }

            return $"";
        }

        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            var sales = context
                .Sales
                .ProjectTo<SaleSD>(mapper.ConfigurationProvider)
                .ToArray();

            XmlSerializer serializer = new XmlSerializer(typeof(SaleSD[]),
                new XmlRootAttribute("sales"));

            using (var writer = File.OpenWrite(ExportPath + "sales-discounts.xml"))
            {
                serializer.Serialize(writer, sales);
            }

            return $"";
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
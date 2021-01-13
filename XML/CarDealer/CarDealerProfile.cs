using AutoMapper;

namespace CarDealer
{
    using System.Collections.Generic;
    using System.Linq;
    using Data;
    using Dtos.Export;
    using Dtos.Import;
    using Models;

    public class CarDealerProfile : Profile
    {
        private static CarDealerContext context;
        public CarDealerProfile()
        {
            this.CreateMap<ImportSupplier, Supplier>();
            this.CreateMap<ImportPart, Part>();
            this.CreateMap<ImportCustomer, Customer>();
            this.CreateMap<ImportSale, Sale>();
            
            this.CreateMap<ImportCar, Car>()
                .ForMember(x => x.PartCars,
                    opt => opt.MapFrom(x => x.PartCars.Select(y => new PartCar
                    {
                        PartId = y.PartId,
                        CarId = x.Id
                    }).ToList()));

            this.CreateMap<Car, CarsWithDistance>()
                .ForMember(x => x.TraveledDistance,
                    opt => opt.MapFrom(x => x.TravelledDistance));
            
            this.CreateMap<Car, BmwCars>()
                .ForMember(x => x.TraveledDistance,
                    opt => opt.MapFrom(x => x.TravelledDistance));

            this.CreateMap<Supplier, LocalSupplier>();
            this.CreateMap<Car, CarCAP>()
                .ForMember(x => x.Parts,
                    opt => opt.MapFrom(x => x.PartCars.Select(y => new PartCAP
                    {
                        Name = y.Part.Name,
                        Price = y.Part.Price
                    }).OrderByDescending(x => x.Price)));

            this.CreateMap<Customer, CustomersTotalSales>()
                .ForMember(x => x.BoughtCars,
                    opt => opt.MapFrom(x => x.Sales.Count))
                .ForMember(x => x.MoneySpent,
                    opt => opt.MapFrom(x => x.Sales.Select(p => p.Car.PartCars.Sum(y => y.Part.Price)).FirstOrDefault()));

            this.CreateMap<Sale, SaleSD>()
                .ForMember(x => x.Price,
                    opt => opt.MapFrom(x => x.Car.PartCars.Sum(p => p.Part.Price)))
                .ForMember(x => x.CustomerName,
                    opt => opt.MapFrom(x => x.Customer.Name))
                .ForMember(x => x.PriceWithDiscount,
                    opt => opt.MapFrom(x => (x.Car.PartCars.Sum(p => p.Part.Price)) - ((x.Car.PartCars.Sum(p => p.Part.Price)) * (x.Discount/100))))
                .ForMember(x => x.Car,
                opt => opt.MapFrom(x => new CarSD
                {
                    Make = x.Car.Make,
                    Model = x.Car.Model,
                    TravelledDistance = x.Car.TravelledDistance
                }));
        }
    }
}

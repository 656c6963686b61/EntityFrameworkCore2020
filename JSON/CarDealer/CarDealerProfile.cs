using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using CarDealer.Models;

namespace CarDealer
{
    using System.Linq;
    using DTO;

    public class CarDealerProfile : Profile
    {
        public CarDealerProfile()
        {
            this.CreateMap<Customer, OrderedCustomers>();
            this.CreateMap<Car, CarsFromToyota>();
            this.CreateMap<Supplier, LocalSuppliers>();

            this.CreateMap<Car, CarFromCarsAndParts>();
            this.CreateMap<Part, PartFromCarsAndParts>();

            this.CreateMap<Car, CarWithParts>()
                    .ForMember(x => x.Parts,
                        opt => opt.MapFrom(x => x.PartCars.Select(x => new PartFromCarsAndParts
                        {
                            Name = x.Part.Name,
                            Price = x.Part.Price
                        }).ToList()))
                    .ForMember(x => x.Car,
                        opt => opt.MapFrom(x => new CarFromCarsAndParts
                        {
                            Make = x.Make,
                            Model = x.Model,
                            TravelledDistance = x.TravelledDistance
                        }));

        }
    }
}

using AutoMapper;

namespace CarDealer
{
    using System.Collections.Generic;
    using System.Linq;
    using Data;
    using Dtos.Import;
    using Models;

    public class CarDealerProfile : Profile
    {
        private static CarDealerContext context;
        public CarDealerProfile()
        {
            this.CreateMap<ImportSupplier, Supplier>();
            this.CreateMap<ImportPart, Part>();

            this.CreateMap<ImportPartCar, PartCar>()
                .ForMember(x => x.PartId,
                    opt => opt.MapFrom(x => x.PartId))
                .ForMember(x => x.Car,
                    opt => opt.MapFrom(x => x.Car))
                .ForMember(x => x.CarId,
                    opt => opt.MapFrom(x => x.CarId));

            this.CreateMap<ImportCar, Car>()
                .ForMember(x => x.PartCars,
                    opt => opt.MapFrom(x => x.PartCars.Select(y => new PartCar
                    {
                        PartId = y.PartId
                    }).ToList()));
        }
    }
}

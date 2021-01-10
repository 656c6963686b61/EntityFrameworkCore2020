using AutoMapper;

namespace ProductShop
{
    using System.Linq;
    using Dtos.Export;
    using Models;

    public class ProductShopProfile : Profile
    {
        public ProductShopProfile()
        {
            this.CreateMap<Product, ProductsInRange>()
                .ForMember(x => x.BuyerFullName,
                    opt => opt.MapFrom(x => (x.Buyer.FirstName + " " + x.Buyer.LastName).Trim()));

            this.CreateMap<Product, ProductsUsersSoldProducts>();

            this.CreateMap<User, UsersSoldProducts>()
                .ForMember(x => x.SoldProducts,
                    opt => opt.MapFrom(x => x.ProductsSold.Select(y => new ProductsUsersSoldProducts
                    {
                        Name = y.Name,
                        Price = y.Price
                    })));
        }
    }
}

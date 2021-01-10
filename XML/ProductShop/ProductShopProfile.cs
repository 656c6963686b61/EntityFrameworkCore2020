using AutoMapper;

namespace ProductShop
{
    using System.Collections.Generic;
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

            this.CreateMap<Category, CategoryByProduct>()
                .ForMember(x => x.Count,
                    opt => opt.MapFrom(x => x.CategoryProducts.Select(y => y.Category).Count()))
                .ForMember(x => x.AveragePrice,
                    opt => opt.MapFrom(x => x.CategoryProducts.Select(y => y.Product.Price).Average()))
                .ForMember(x => x.AveragePrice,
                    opt => opt.MapFrom(x => x.CategoryProducts.Select(y => y.Product.Price).Sum()));

            this.CreateMap<Product, ProductUAP>();
            this.CreateMap<User, UserUAP>()
                .ForMember(x => x.SoldProducts,
                opt => opt.MapFrom(x => new SoldProductsUAP
                {
                    Count = x.ProductsSold.Count,
                    Products = x.ProductsSold.Select(p => new ProductUAP
                    {
                        Name = p.Name,
                        Price = p.Price
                    }).OrderByDescending(p => p.Price).ToList()
                }));
        }
    }
}

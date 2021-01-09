using AutoMapper;

namespace ProductShop
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using DTOs;
    using DTOs.Problem_8;
    using Models;

    public class ProductShopProfile : Profile
    {

        public ProductShopProfile()
        {
            this.CreateMap<Product, ProductsInRange>()
                .ForMember(x => x.seller,
                    opt => opt.MapFrom(x => (x.Seller.FirstName + " " + x.Seller.LastName).Trim()));

            this.CreateMap<Product, ProductSoldProducts>()
                .ForMember(x => x.buyerFirstName,
                    opt => opt.MapFrom(x => x.Buyer.FirstName))
                .ForMember(x => x.buyerLastName,
                    opt => opt.MapFrom(x => x.Buyer.LastName));

            this.CreateMap<User, UserSoldProducts>()
                .ForMember(x => x.soldProducts,
                    opt => opt.MapFrom(x => x.ProductsSold));

            this.CreateMap<Category, CategoryByProduct>()
                .ForMember(x => x.CategoryProductsAverage,
                    opt => opt.MapFrom(x => x.CategoryProducts.Average(x => x.Product.Price).ToString("f2")))
                .ForMember(x => x.CategoryProductsSum,
                    opt => opt.MapFrom(x => x.CategoryProducts.Sum(x => x.Product.Price).ToString("f2")));

            this.CreateMap<Product, ProductDTO>();

            this.CreateMap<User, UserDTO>()
                .ForMember(x => x.SoldProducts,
                    opt => opt.MapFrom(x => new ProductsDTO
                    {
                        ProductsSoldCount = x.ProductsSold.Count,
                        ProductsSold = (ICollection<ProductDTO>)x.ProductsSold.Select(x => new ProductDTO
                        {
                            Name = x.Name,
                            Price = x.Price
                        })
                    }));
        }
    }
}
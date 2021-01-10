using ProductShop.Data;

namespace ProductShop
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Xml.Serialization;
    using AutoMapper;
    using Dtos.Import;
    using Models;

    public class StartUp
    {
        private static string ImportFilePath = "../../../Datasets/"; // + filename
        private static string ExportFilePath = "../../../Results";
        public static void Main(string[] args)
        {
            var context = new ProductShopContext();

            EnsureFolderCreated(ExportFilePath);
            context.Database.EnsureCreated();

            var config = new MapperConfiguration(x => 
            {
                x.AddProfile(new ProductShopProfile());
            });
            var mapper = config.CreateMapper();

            Console.WriteLine(ImportUsers(context, ImportFilePath + "users.xml"));
            Console.WriteLine(ImportProducts(context, ImportFilePath + "products.xml"));
            Console.WriteLine(ImportCategories(context, ImportFilePath + "categories.xml"));
            Console.WriteLine(ImportCategoryProducts(context, ImportFilePath + "categories-products.xml"));
        }

        public static string ImportUsers(ProductShopContext context, string inputXml)
        {
            //deserializing xml to dto class
            var serializer = new XmlSerializer(typeof(ImportUser[]), new XmlRootAttribute("Users"));
            var users = (ImportUser[])serializer.Deserialize(File.OpenRead(inputXml));

            //mapping the classes manually
            var dbUsers = new List<User>();
            foreach (var user in users)
            {
                var dbUser = new User
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Age = user.Age
                };
                dbUsers.Add(dbUser);
            }

            context.Users.AddRange(dbUsers);
            context.SaveChanges();
            return $"Successfully imported {context.Users.Count()}";
        }

        public static string ImportProducts(ProductShopContext context, string inputXml)
        {
            var serializer = new XmlSerializer(typeof(ImportProduct[]), new XmlRootAttribute("Products"));
            var products = (ImportProduct[])serializer.Deserialize(File.OpenRead(inputXml));

            var dbProducts = new List<Product>();
            foreach (var product in products.Where(x => x.BuyerId > 0 && x.BuyerId <= context.Users.Max(y => y.Id)))
            {
                var dbProduct = new Product
                {
                    Name = product.Name,
                    Price = product.Price,
                    SellerId = product.SellerId,
                    BuyerId = product.BuyerId,
                };
                dbProducts.Add(dbProduct);
            }

            context.Products.AddRange(dbProducts);
            context.SaveChanges();
            return $"Successfully imported {context.Products.Count()}";
        }

        public static string ImportCategories(ProductShopContext context, string inputXml)
        {
            var serializer = new XmlSerializer(typeof(ImportCategory[]), new XmlRootAttribute("Categories"));
            var categories = (ImportCategory[])serializer.Deserialize(File.OpenRead(inputXml));
            
            var dbCategories = new List<Category>();
            foreach (var category in categories)
            {
                var dbCategory = new Category
                {
                    Name = category.Name,
                };
                dbCategories.Add(dbCategory);
            }

            context.Categories.AddRange(dbCategories);
            context.SaveChanges();
            return $"Successfully imported {context.Categories.Count()}";
        }

        public static string ImportCategoryProducts(ProductShopContext context, string inputXml)
        {
            var serializer = new XmlSerializer(typeof(ImportCategoryProduct[]), new XmlRootAttribute("CategoryProducts"));
            var categoriesProducts = (ImportCategoryProduct[])serializer.Deserialize(File.OpenRead(inputXml));
            
            var dbCategoryProducts = new List<CategoryProduct>();
            foreach (var categoryProduct in categoriesProducts)
            {
                if (CheckEntity(context, categoryProduct))
                {
                    var dbCategoryProduct = new CategoryProduct
                    {
                        CategoryId = categoryProduct.CategoryId,
                        ProductId = categoryProduct.ProductId,
                    };
                    dbCategoryProducts.Add(dbCategoryProduct);
                }
            }

            context.CategoryProducts.AddRange(dbCategoryProducts);
            context.SaveChanges();
            return $"Successfully imported {context.CategoryProducts.Count()}";
        }

        private static bool CheckEntity(ProductShopContext context, ImportCategoryProduct categoryProduct)
        {
            if (categoryProduct.ProductId <= context.Products.Max(x => x.Id) && categoryProduct.CategoryId <= context.Categories.Max(x => x.Id))
            {
                return true;
            }

            return false;
        }

        private static void EnsureFolderCreated(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
    }
}
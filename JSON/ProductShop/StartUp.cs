using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using ProductShop.Data;
using ProductShop.Models;

namespace ProductShop
{
    using System.Runtime.CompilerServices;
    using Microsoft.EntityFrameworkCore;

    public class StartUp
    {
        private static string ResultsDirectoryPath = "../../../Results";
        public static void Main(string[] args)
        {
            var db = new ProductShopContext();

            
            //Import Data
            /*
            Console.WriteLine(ImportUsers(db,File.ReadAllText("../../../Datasets/users.json")));
            Console.WriteLine(ImportProducts(db,File.ReadAllText("../../../Datasets/products.json")));
            Console.WriteLine(ImportCategories(db,File.ReadAllText("../../../Datasets/categories.json")));
            Console.WriteLine(ImportCategoryProducts(db,File.ReadAllText("../../../Datasets/categories-products.json")));
            */

            EnsureDirectoryCreated(ResultsDirectoryPath);

            //Queries
            
            //File.WriteAllText(ResultsDirectoryPath + "/products-in-range.json", GetProductsInRange(db));
            //File.WriteAllText(ResultsDirectoryPath + "/users-sold-products.json", GetSoldProducts(db));
            //File.WriteAllText(ResultsDirectoryPath + "/categories-by-products.json", GetCategoriesByProductsCount(db));
            File.WriteAllText(ResultsDirectoryPath + "/users-and-products.json", GetUsersWithProducts(db));
        

            db.Database.Migrate();
        }

        //Problem 1
        public static string ImportUsers(ProductShopContext context, string inputJson)
        {
            var json = JsonConvert.DeserializeObject<ICollection<User>>(inputJson);
            context.Users.AddRange(json);
            context.SaveChanges();
            return $"Successfully imported {context.Users.Count()} users";
        }

        //Problem 2
        public static string ImportProducts(ProductShopContext context, string inputJson)
        {
            var json = JsonConvert.DeserializeObject<Product[]>(inputJson);
            context.Products.AddRange(json);
            context.SaveChanges();
            return $"Successfully imported {context.Products.Count()} products";
        }

        //Problem 3
        public static string ImportCategories(ProductShopContext context, string inputJson)
        {
            var json = JsonConvert.DeserializeObject<Category[]>(inputJson);
            context.Categories.AddRange(json);
            context.SaveChanges();
            return $"Successfully imported {context.Categories.Count()} categories";
        }

        //Problem 4
        public static string ImportCategoryProducts(ProductShopContext context, string inputJson)
        {
            var json = JsonConvert.DeserializeObject<CategoryProduct[]>(inputJson);
            context.CategoryProducts.AddRange(json);
            context.SaveChanges();
            return $"Successfully imported {context.CategoryProducts.Count()} categories and products";
        }

        //Problem 5
        public static string GetProductsInRange(ProductShopContext context)
        {
            var products = context
                .Products
                .Select(x => new
                {
                    name = x.Name,
                    price = x.Price,
                    seller = (x.Seller.FirstName + " " + x.Seller.LastName).Trim(),

                })
                .Where(x => x.price >= 500 && x.price <= 1000)
                .OrderBy(x => x.price)
                .ToList();

            return JsonConvert.SerializeObject(products, Formatting.Indented); ;
        }

        //Problem 6
        public static string GetSoldProducts(ProductShopContext context)
        {
            var users = context
                .Users
                .Where(x => x.ProductsSold.Any(p => p.Buyer != null))
                .OrderBy(x => x.LastName)
                .ThenBy(x => x.FirstName)
                .Select(x => new
                {
                    firstName = x.FirstName,
                    lastName = x.LastName,
                    soldProducts = x.ProductsSold.Select(p => new
                    {
                        name = p.Name,
                        price = p.Price,
                        buyerFirstName = p.Buyer.FirstName,
                        buyerLastName = p.Buyer.LastName
                    })
                })
                .ToList();

            return JsonConvert.SerializeObject(users, Formatting.Indented); ;
        }

        //Problem 7
        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var categories = context
                .Categories
                .OrderByDescending(x => x.CategoryProducts.Count)
                .Select(x => new
                {
                    category = x.Name,
                    productsCount = x.CategoryProducts.Count,
                    averagePrice = x.CategoryProducts.Average(x => x.Product.Price).ToString("f2"),
                    totalRevenue = x.CategoryProducts.Sum(x => x.Product.Price).ToString("f2")
                });

            return JsonConvert.SerializeObject(categories, Formatting.Indented);
        }

        //Problem 8
        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var users = context
                .Users
                .Where(x => x.ProductsSold.Any(p => p.Buyer != null))
                .Select(x => new
                {
                    lastName = x.LastName,
                    age = x.Age,
                    soldProducts = new
                    {
                        count = x.ProductsSold.Count(x => x.Buyer != null),
                        products = x.ProductsSold.Where(x => x.Buyer != null).Select(p => new
                        {
                            name = p.Name,
                            price = p.Price
                        })
                    }
                })
                .OrderByDescending(x => x.soldProducts.count)
                .ToList();

            var obj = new
            {
                usersCount = users.Count,
                users = users
            };

            var settings = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented
            };

            return JsonConvert.SerializeObject(obj, settings);
        }

        private static void EnsureDirectoryCreated(string path)
        {
            if (!Directory.Exists(ResultsDirectoryPath))
            {
                Directory.CreateDirectory(ResultsDirectoryPath);
            }
        }
    }
}
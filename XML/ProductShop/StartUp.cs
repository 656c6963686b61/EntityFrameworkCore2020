namespace ProductShop
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml.Serialization;
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Data;
    using Dtos.Export;
    using Dtos.Import;
    using Models;

    public class StartUp
    {
        private static string ImportFilePath = "../../../Datasets/"; // + filename
        private static readonly string ExportFilePath = "../../../Results/";
        private static IMapper mapper;

        public static void Main(string[] args)
        {
            var context = new ProductShopContext();

            EnsureFolderCreated(ExportFilePath);
            context.Database.EnsureCreated();

            var config = new MapperConfiguration(x => { x.AddProfile(new ProductShopProfile()); });
            mapper = config.CreateMapper();

            //GetProductsInRange(context);
            //GetSoldProducts(context);
            //GetCategoriesByProductsCount(context);
            //GetUsersWithProducts(context);

            //Console.WriteLine(ImportUsers(context, ImportFilePath + "users.xml"));
            //Console.WriteLine(ImportProducts(context, ImportFilePath + "products.xml"));
            //Console.WriteLine(ImportCategories(context, ImportFilePath + "categories.xml"));
            //Console.WriteLine(ImportCategoryProducts(context, ImportFilePath + "categories-products.xml"));
        }

        public static string ImportUsers(ProductShopContext context, string inputXml)
        {
            //deserializing xml to dto class
            var serializer = new XmlSerializer(typeof(ImportUser[]), new XmlRootAttribute("Users"));

            using (FileStream reader = File.OpenRead(inputXml))
            {
                var users = (ImportUser[]) serializer.Deserialize(reader);

                //mapping the classes manually
                var dbUsers = new List<User>();
                foreach (ImportUser user in users)
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
            }

            return $"Successfully imported {context.Users.Count()}";
        }

        public static string ImportProducts(ProductShopContext context, string inputXml)
        {
            var serializer = new XmlSerializer(typeof(ImportProduct[]), new XmlRootAttribute("Products"));

            using (FileStream reader = File.OpenRead(inputXml))
            {
                var products = (ImportProduct[]) serializer.Deserialize(reader);

                var dbProducts = new List<Product>();
                foreach (ImportProduct product in products.Where(x =>
                    x.BuyerId > 0 && x.BuyerId <= context.Users.Max(y => y.Id)))
                {
                    var dbProduct = new Product
                    {
                        Name = product.Name,
                        Price = product.Price,
                        SellerId = product.SellerId,
                        BuyerId = product.BuyerId
                    };
                    dbProducts.Add(dbProduct);
                }

                context.Products.AddRange(dbProducts);
                context.SaveChanges();
            }

            return $"Successfully imported {context.Products.Count()}";
        }

        public static string ImportCategories(ProductShopContext context, string inputXml)
        {
            var serializer = new XmlSerializer(typeof(ImportCategory[]), new XmlRootAttribute("Categories"));

            using (FileStream reader = File.OpenRead(inputXml))
            {
                var categories = (ImportCategory[]) serializer.Deserialize(reader);

                var dbCategories = new List<Category>();
                foreach (ImportCategory category in categories)
                {
                    var dbCategory = new Category
                    {
                        Name = category.Name
                    };
                    dbCategories.Add(dbCategory);
                }

                context.Categories.AddRange(dbCategories);
                context.SaveChanges();
            }

            return $"Successfully imported {context.Categories.Count()}";
        }

        public static string ImportCategoryProducts(ProductShopContext context, string inputXml)
        {
            var serializer =
                new XmlSerializer(typeof(ImportCategoryProduct[]), new XmlRootAttribute("CategoryProducts"));

            using (FileStream reader = File.OpenRead(inputXml))
            {
                var categoriesProducts = (ImportCategoryProduct[]) serializer.Deserialize(reader);
                var dbCategoryProducts = new List<CategoryProduct>();
                foreach (ImportCategoryProduct categoryProduct in categoriesProducts)
                    if (CheckEntity(context, categoryProduct))
                    {
                        var dbCategoryProduct = new CategoryProduct
                        {
                            CategoryId = categoryProduct.CategoryId,
                            ProductId = categoryProduct.ProductId
                        };
                        dbCategoryProducts.Add(dbCategoryProduct);
                    }

                context.CategoryProducts.AddRange(dbCategoryProducts);
                context.SaveChanges();
            }

            return $"Successfully imported {context.CategoryProducts.Count()}";
        }

        public static string GetProductsInRange(ProductShopContext context)
        {
            var serializer = new XmlSerializer(typeof(ProductsInRange[]), new XmlRootAttribute("Products"));
            ProductsInRange[] products = context
                .Products
                .ProjectTo<ProductsInRange>(mapper.ConfigurationProvider)
                .Where(x => x.Price >= 500 && x.Price <= 1000)
                .OrderByDescending(x => x.Price)
                .Take(10)
                .ToArray();

            using (FileStream writer = File.OpenWrite(ExportFilePath + "products-in-range.xml"))
            {
                serializer.Serialize(writer, products);
            }

            return "";
        }

        public static string GetSoldProducts(ProductShopContext context)
        {
            var serializer = new XmlSerializer(typeof(UsersSoldProducts[]), new XmlRootAttribute("Users"));
            UsersSoldProducts[] users = context
                .Users
                .ProjectTo<UsersSoldProducts>(mapper.ConfigurationProvider)
                .Where(x => x.SoldProducts.Any())
                .OrderBy(x => x.LastName)
                .ThenBy(x => x.FirstName)
                .Take(5)
                .ToArray();

            using (FileStream writer = File.OpenWrite(ExportFilePath + "users-sold-products.xml"))
            {
                serializer.Serialize(writer, users);
            }

            return "";
        }

        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var serializer = new XmlSerializer(typeof(CategoryByProduct[]), new XmlRootAttribute("Categories"));

            CategoryByProduct[] categories = context
                .Categories
                .ProjectTo<CategoryByProduct>(mapper.ConfigurationProvider)
                .OrderByDescending(x => x.Count)
                .ThenBy(x => x.TotalRevenue)
                .ToArray();

            using (FileStream writer = File.OpenWrite(ExportFilePath + "categories-by-products.xml"))
            {
                serializer.Serialize(writer, categories);
            }

            return "";
        }

        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var serializer = new XmlSerializer(typeof(UsersUAP), new XmlRootAttribute("Users"));

            List<UserUAP> users = context
                .Users
                .ProjectTo<UserUAP>(mapper.ConfigurationProvider)
                .Where(x => x.SoldProducts.Count >= 1)
                .ToList();

            var obj = new UsersUAP
            {
                Count = users.Count,
                Users = users.Take(10).ToList()
            };

            using (FileStream writer = File.OpenWrite(ExportFilePath + "users-and-products.xml"))
            {
                serializer.Serialize(writer, obj);
            }

            return "";
        }

        private static bool CheckEntity(ProductShopContext context, ImportCategoryProduct categoryProduct)
        {
            switch (categoryProduct.ProductId <= context.Products.Max(x => x.Id))
            {
                case true when categoryProduct.CategoryId <= context.Categories.Max(x => x.Id):
                    return true;
                default:
                    return false;
            }
        }

        private static void EnsureFolderCreated(string path)
        {
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        }
    }
}
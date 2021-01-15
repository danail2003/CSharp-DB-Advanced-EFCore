using System.IO;
using System.Linq;
using Newtonsoft.Json;
using ProductShop.Data;
using ProductShop.Models;

namespace ProductShop
{
    public class StartUp
    {
        private readonly static string ResultDirectory = "../../../Datasets/Results";

        public static void Main()
        {
            ProductShopContext db = new ProductShopContext();

            string json = GetUsersWithProducts(db);

            if (!Directory.Exists(ResultDirectory))
            {
                Directory.CreateDirectory(ResultDirectory);
            }

            File.WriteAllText(ResultDirectory + "/users-and-products.json", json);
        }

        public static string ImportUsers(ProductShopContext context, string inputJson)
        {
            User[] users = JsonConvert.DeserializeObject<User[]>(inputJson);

            context.AddRange(users);
            context.SaveChanges();

            return $"Successfully imported {users.Length}";
        }

        public static string ImportProducts(ProductShopContext context, string inputJson)
        {
            Product[] products = JsonConvert.DeserializeObject<Product[]>(inputJson);

            context.AddRange(products);
            context.SaveChanges();

            return $"Successfully imported {products.Length}";
        }

        public static string ImportCategories(ProductShopContext context, string inputJson)
        {
            Category[] categories = JsonConvert.DeserializeObject<Category[]>(inputJson).Where(x => x.Name != null).ToArray();

            context.AddRange(categories);
            context.SaveChanges();

            return $"Successfully imported {categories.Length}";
        }

        public static string ImportCategoryProducts(ProductShopContext context, string inputJson)
        {
            CategoryProduct[] categoryProducts = JsonConvert.DeserializeObject<CategoryProduct[]>(inputJson);

            context.AddRange(categoryProducts);
            context.SaveChanges();

            return $"Successfully imported {categoryProducts.Length}";
        }

        public static string GetProductsInRange(ProductShopContext context)
        {
            var products = context.Products.Where(x => x.Price >= 500 && x.Price <= 1000).OrderBy(x => x.Price).Select(x => new
            {
                name = x.Name,
                price = x.Price,
                seller = x.Seller.FirstName + " " + x.Seller.LastName
            }).ToArray();

            string json = JsonConvert.SerializeObject(products, Formatting.Indented);

            return json;
        }

        public static string GetSoldProducts(ProductShopContext context)
        {
            var users = context.Users.Where(x => x.ProductsSold.Any(y=>y.Buyer != null))
                .OrderBy(x => x.LastName)
                .ThenBy(x => x.FirstName)
                .Select(x => new
            {
                firstName = x.FirstName,
                lastName = x.LastName,
                soldProducts = x.ProductsSold.Where(y=>y.Buyer != null)
                .Select(y => new
                {
                    name = y.Name,
                    price = y.Price,
                    buyerFirstName = y.Buyer.FirstName,
                    buyerLastName = y.Buyer.LastName
                }).ToArray()
             }).ToArray();

            string json = JsonConvert.SerializeObject(users, Formatting.Indented);

            return json;
        }

        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var categories = context.Categories.Select(x => new
            {
                category = x.Name,
                productsCount = x.CategoryProducts.Count(),
                averagePrice = x.CategoryProducts.Average(y => y.Product.Price).ToString("f2"),
                totalRevenue = x.CategoryProducts.Sum(y => y.Product.Price).ToString("f2")
            }).OrderByDescending(x => x.productsCount).ToArray();

            string json = JsonConvert.SerializeObject(categories, Formatting.Indented);

            return json;
        }

        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var users = context.Users
                .Where(x => x.ProductsSold.Any(y => y.Buyer != null))
                .ToArray()
                .OrderByDescending(x => x.ProductsSold.Count(y => y.Buyer != null))
                .Select(x => new
                {
                    firstName = x.FirstName,
                    lastName = x.LastName,
                    age = x.Age,
                    soldProducts = new
                    {
                        count = x.ProductsSold.Count(y => y.Buyer != null),
                        products = x.ProductsSold
                        .Where(y => y.Buyer != null)
                    .Select(y => new
                    {
                        name = y.Name,
                        price = y.Price
                    })
                    .ToArray()
                    }
                })
                .ToArray();

            var usersObj = new
            {
                usersCount = users.Length,
                users
            };

            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented
            };

            string json = JsonConvert.SerializeObject(usersObj, settings);

            return json;
        }
    }
}
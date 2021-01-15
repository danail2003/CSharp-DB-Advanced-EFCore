using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Globalization;
using AutoMapper;
using CarDealer.Data;
using CarDealer.DTO;
using CarDealer.Models;
using Newtonsoft.Json;

namespace CarDealer
{
    public class StartUp
    {
        private readonly static string DirectoryPath = "../../../Datasets/Results";

        public static void Main()
        {
            CarDealerContext db = new CarDealerContext();
            
            if (!Directory.Exists(DirectoryPath))
            {
                Directory.CreateDirectory(DirectoryPath);
            }

            string json = GetSalesWithAppliedDiscount(db);

            File.WriteAllText(DirectoryPath + "/sales-discounts.json", json);
        }

        public static string ImportSuppliers(CarDealerContext context, string inputJson)
        {
            Supplier[] suppliers = JsonConvert.DeserializeObject<Supplier[]>(inputJson);

            context.AddRange(suppliers);
            context.SaveChanges();

            return $"Successfully imported {suppliers.Length}.";
        }

        public static string ImportParts(CarDealerContext context, string inputJson)
        {
            Part[] parts = JsonConvert.DeserializeObject<Part[]>(inputJson).Where(x => x.SupplierId <= 31).ToArray();

            context.AddRange(parts);
            context.SaveChanges();

            return $"Successfully imported {parts.Length}.";
        }

        public static string ImportCars(CarDealerContext context, string inputJson)
        {
            CarDto[] cars = JsonConvert.DeserializeObject<CarDto[]>(inputJson);

            foreach (var car in cars)
            {
                Car currentCar = new Car
                {
                    Make = car.Make,
                    Model = car.Model,
                    TravelledDistance = car.TravelledDistance
                };

                context.Cars.Add(currentCar);

                foreach (var part in car.PartsId)
                {
                    PartCar partCar = new PartCar
                    {
                        CarId = currentCar.Id,
                        PartId = part
                    };

                    if (currentCar.PartCars.FirstOrDefault(x => x.PartId == part) == null)
                    {
                        context.PartCars.Add(partCar);
                    }
                }
            }
            
            context.SaveChanges();

            return $"Successfully imported {cars.Length}.";
        }

        public static string ImportCustomers(CarDealerContext context, string inputJson)
        {
            Customer[] customers = JsonConvert.DeserializeObject<Customer[]>(inputJson);

            context.AddRange(customers);
            context.SaveChanges();

            return $"Successfully imported {customers.Length}.";
        }

        public static string ImportSales(CarDealerContext context, string inputJson)
        {
            Sale[] sales = JsonConvert.DeserializeObject<Sale[]>(inputJson);

            context.AddRange(sales);
            context.SaveChanges();

            return $"Successfully imported {sales.Length}.";
        }

        public static string GetOrderedCustomers(CarDealerContext context)
        {
            var customers = context.Customers.OrderBy(x => x.BirthDate).ThenBy(x => x.IsYoungDriver).Select(x => new
            {
                x.Name,
                BirthDate = x.BirthDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                x.IsYoungDriver
            })
                .ToArray();

            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };

            string json = JsonConvert.SerializeObject(customers, settings);

            return json;
        }

        public static string GetCarsFromMakeToyota(CarDealerContext context)
        {
            var cars = context.Cars.Where(x => x.Make == "Toyota")
                .OrderBy(x => x.Model)
                .ThenByDescending(x => x.TravelledDistance)
                .Select(x => new
                {
                    x.Id,
                    x.Make,
                    x.Model,
                    x.TravelledDistance
                })
                .ToArray();

            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };

            string json = JsonConvert.SerializeObject(cars, settings);

            return json;
        }

        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var suppliers = context.Suppliers.Where(x => x.IsImporter == false)
                .Select(x => new
            {
                x.Id,
                x.Name,
                PartsCount = x.Parts.Count
            })
                .ToArray();

            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };

            string json = JsonConvert.SerializeObject(suppliers, settings);

            return json;
        }

        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var cars = context.Cars.Select(x => new
            {
                car = new
                {
                    x.Make,
                    x.Model,
                    x.TravelledDistance
                },
                parts = x.PartCars.Select(y => new
                {
                    y.Part.Name,
                    Price = y.Part.Price.ToString("f2")
                })
                .ToArray()
            })
                .ToArray();

            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };

            string json = JsonConvert.SerializeObject(cars, settings);

            return json;
        }

        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            var customers = context.Customers.Where(x => x.Sales.Count > 0).Select(x => new
            {
                fullName = x.Name,
                boughtCars = x.Sales.Count,
                spentMoney = x.Sales.Sum(y => y.Car.PartCars.Sum(z => z.Part.Price))
            })
                .OrderByDescending(x => x.spentMoney)
                .ThenByDescending(x => x.boughtCars)
                .ToArray();

            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };

            string json = JsonConvert.SerializeObject(customers, settings);

            return json;
        }

        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            var sales = context.Sales.Select(x => new
            {
                car = new
                {
                    x.Car.Make,
                    x.Car.Model,
                    x.Car.TravelledDistance,
                },
                customerName = x.Customer.Name,
                Discount = x.Discount.ToString("f2"),
                price = x.Car.PartCars.Sum(y => y.Part.Price).ToString("f2"),
                priceWithDiscount = (x.Car.PartCars.Sum(y => y.Part.Price) * (1 - x.Discount / 100)).ToString("f2")
            })
                .Take(10)
                .ToArray();

            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };

            string json = JsonConvert.SerializeObject(sales, settings);

            return json;
        }
    }
}
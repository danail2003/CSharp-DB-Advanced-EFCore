namespace VaporStore.DataProcessor
{
	using System;
    using System.IO;
    using System.Xml;
    using System.Linq;
    using System.Text;
    using System.Globalization;
    using System.Xml.Serialization;
    using Newtonsoft.Json;
    using Data;
    using VaporStore.Data.Models.Enums;
    using VaporStore.DataProcessor.Dto.Export;

    public static class Serializer
	{
		public static string ExportGamesByGenres(VaporStoreDbContext context, string[] genreNames)
		{
			var games = context.Genres
				.Where(x => genreNames.Contains(x.Name))
				.ToArray()
				.Select(x => new
				{
					x.Id,
					Genre = x.Name,
					Games = x.Games
					.Where(y => y.Purchases.Any())
					.ToArray()
					.Select(y => new
					{
						y.Id,
						Title = y.Name,
						Developer = y.Developer.Name,
						Tags = string.Join(", ", y.GameTags.Select(z => z.Tag.Name).ToArray()),
						Players = y.Purchases.Count
					})
					.OrderByDescending(x => x.Players)
					.ThenBy(x => x.Id),
					TotalPlayers = x.Games.Sum(x => x.Purchases.Count)
				})
				.ToArray()
				.OrderByDescending(x => x.TotalPlayers)
				.ThenBy(x => x.Id)
				.ToArray();

			var json = JsonConvert.SerializeObject(games, Newtonsoft.Json.Formatting.Indented);

			return json;
		}

		public static string ExportUserPurchasesByType(VaporStoreDbContext context, string storeType)
		{
			StringBuilder stringBuilder = new StringBuilder();

			XmlSerializer serializer = new XmlSerializer(typeof(ExportUserDto[]), new XmlRootAttribute("Users"));
			XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });

			PurchaseType purchaseType = (PurchaseType)Enum.Parse(typeof(PurchaseType), storeType);

			var users = context.Users
				.Where(x => x.Cards.Any(y => y.Purchases.Any()))
				.ToArray()
				.Select(x => new ExportUserDto()
				{
					Username = x.Username,
					Purchases = context.Purchases.Where(y => y.Type == purchaseType && y.Card.User.Username == x.Username)
					.ToArray()
					.OrderBy(x => x.Date)
					.Select(y => new ExportPurchaseDto()
					{
						CardNumber = y.Card.Number,
						Cvc = y.Card.Cvc,
						Date = y.Date.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture),
						Game = new ExportGameDto()
						{
							Name = y.Game.Name,
							Genre = y.Game.Genre.Name,
							Price = y.Game.Price
						}
					})
					.ToArray(),
					TotalSpent = context.Purchases.Where(y => y.Type == purchaseType && y.Card.User.Username == x.Username)
					.ToArray()
					.Sum(x => x.Game.Price)
				})
				.Where(x => x.Purchases.Length > 0)
				.OrderByDescending(x => x.TotalSpent)
				.ThenBy(x => x.Username)
				.ToArray();

			serializer.Serialize(new StringWriter(stringBuilder), users, namespaces);

			return stringBuilder.ToString().TrimEnd();
		}
	}
}
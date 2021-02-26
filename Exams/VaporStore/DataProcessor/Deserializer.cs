namespace VaporStore.DataProcessor
{
	using System;
    using System.Text;
    using System.Linq;
    using System.Globalization;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Newtonsoft.Json;
    using Data;
    using VaporStore.Data.Models;
    using VaporStore.DataProcessor.Dto.Import;
    using VaporStore.Data.Models.Enums;
    using System.Xml.Serialization;
    using System.IO;

    public static class Deserializer
	{
		private const string ErrorMessage = "Invalid Data";

		public static string ImportGames(VaporStoreDbContext context, string jsonString)
		{
			StringBuilder stringBuilder = new StringBuilder();

			var gamesDto = JsonConvert.DeserializeObject<ImportGameDto[]>(jsonString);

			List<Game> games = new List<Game>();
			List<Genre> genres = new List<Genre>();
			List<Tag> tags = new List<Tag>();
			List<Developer> developers = new List<Developer>();

            foreach (var dto in gamesDto)
            {
				if (!IsValid(dto))
                {
					stringBuilder.AppendLine(ErrorMessage);
					continue;
				}

				bool isDateValid = DateTime.TryParseExact(dto.ReleaseDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime validDate);

				if (!isDateValid)
                {
					stringBuilder.AppendLine(ErrorMessage);
					continue;
				}

				if (dto.Tags.Length == 0)
                {
					stringBuilder.AppendLine(ErrorMessage);
					continue;
				}

				Game game = new Game
				{
					Name = dto.Name,
					Price = dto.Price,
					ReleaseDate = validDate
				};

				Genre genre = genres.FirstOrDefault(x => x.Name == dto.Genre);

				if (genre == null)
                {
					genre = new Genre
					{
						Name = dto.Genre
					};

					genres.Add(genre);

					game.Genre = genre;
				}
                else
                {
					game.Genre = genre;
                }

				Developer developer = developers.FirstOrDefault(x => x.Name == dto.Developer);

				if (developer == null)
                {
					developer = new Developer
					{
						Name = dto.Developer,
					};

					developers.Add(developer);
					game.Developer = developer;
				}
                else
                {
					game.Developer = developer;
                }

                foreach (var tag in dto.Tags)
                {
					if (string.IsNullOrEmpty(tag))
                    {
						continue;
                    }

					Tag currentTag = tags.FirstOrDefault(x => x.Name == tag);

					if (currentTag == null)
                    {
						currentTag = new Tag
						{
							Name = tag
						};

						tags.Add(currentTag);
						game.GameTags.Add(new GameTag
						{
							Game = game,
							Tag = currentTag
						});
					}
                    else
                    {
						game.GameTags.Add(new GameTag
						{
							Game = game,
							Tag = currentTag
						});
					}
				}

				if (game.GameTags.Count == 0)
                {
					continue;
                }

				games.Add(game);
				stringBuilder.AppendLine(string.Format($"Added {game.Name} ({game.Genre.Name}) with {game.GameTags.Count} tags"));
			}

			context.Games.AddRange(games);
			context.SaveChanges();

			return stringBuilder.ToString().TrimEnd();
		}

		public static string ImportUsers(VaporStoreDbContext context, string jsonString)
		{
			StringBuilder stringBuilder = new StringBuilder();

			var usersDto = JsonConvert.DeserializeObject<ImportUserDto[]>(jsonString);

			List<User> users = new List<User>();

            foreach (var dto in usersDto)
            {
				if (!IsValid(dto))
                {
					stringBuilder.AppendLine(ErrorMessage);
					continue;
				}

				User user = new User
				{
					FullName = dto.FullName,
					Username = dto.Username,
					Email = dto.Email,
					Age = dto.Age
				};

                foreach (var cardDto in dto.Cards)
                {
					if (!IsValid(cardDto))
                    {
						stringBuilder.AppendLine(ErrorMessage);
						continue;
					}

					bool isTypeValid = Enum.TryParse<CardType>(cardDto.Type, out CardType type);

					if (!isTypeValid)
                    {
						stringBuilder.AppendLine(ErrorMessage);
						continue;
					}

					user.Cards.Add(new Card
					{
						Number = cardDto.Number,
						Cvc = cardDto.Cvc,
						Type = type
					});
				}

				if (user.Cards.Count == 0)
                {
					stringBuilder.AppendLine(ErrorMessage);
					continue;
                }

				users.Add(user);
				stringBuilder.AppendLine($"Imported {user.Username} with {user.Cards.Count} cards");
			}

			context.Users.AddRange(users);
			context.SaveChanges();

			return stringBuilder.ToString().TrimEnd();
		}

		public static string ImportPurchases(VaporStoreDbContext context, string xmlString)
		{
			StringBuilder stringBuilder = new StringBuilder();

			XmlSerializer serializer = new XmlSerializer(typeof(ImportPurchaseDto[]), new XmlRootAttribute("Purchases"));
			var purchasesDto = (ImportPurchaseDto[])serializer.Deserialize(new StringReader(xmlString));

			List<Purchase> purchases = new List<Purchase>();

			foreach (var dto in purchasesDto)
            {
				if (!IsValid(dto))
                {
					stringBuilder.AppendLine(ErrorMessage);
					continue;
				}

				bool isDateValid = DateTime.TryParseExact(dto.Date, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime validDate);

				if (!isDateValid)
                {
					stringBuilder.AppendLine(ErrorMessage);
					continue;
				}

				bool isTypeValid = Enum.TryParse<PurchaseType>(dto.Type, out PurchaseType validType);

				if (!isTypeValid)
                {
					stringBuilder.AppendLine(ErrorMessage);
					continue;
				}

				Game game = context.Games.FirstOrDefault(x => x.Name == dto.GameName);
				Card card = context.Cards.FirstOrDefault(x => x.Number == dto.Card);

				Purchase purchase = new Purchase
				{
					Type = validType,
					ProductKey = dto.ProductKey,
					Date = validDate,
					Game = game,
					Card = card
				};

				stringBuilder.AppendLine($"Imported {game.Name} for {card.User.Username}");

				purchases.Add(purchase);
			}

			context.Purchases.AddRange(purchases);
			context.SaveChanges();

			return stringBuilder.ToString().TrimEnd();
		}

		private static bool IsValid(object dto)
		{
			var validationContext = new ValidationContext(dto);
			var validationResult = new List<ValidationResult>();

			return Validator.TryValidateObject(dto, validationContext, validationResult, true);
		}
	}
}
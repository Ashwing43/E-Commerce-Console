using ECommerce.Models;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;

namespace DataStorage
{
	public class UserJsonConverter : JsonConverter<User>
	{
		public override User? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
			{
				JsonElement root = doc.RootElement;
				if (root.TryGetProperty("Role", out JsonElement roleProperty))
				{
					// Get role as string and parse it to UserRole
					if (Enum.TryParse<UserRole>(roleProperty.GetString(), out UserRole role))
					{
						if (role == UserRole.Admin)
						{
							return JsonSerializer.Deserialize<Admin>(root.GetRawText(), options);
						}
						else if (role == UserRole.Customer)
						{
							return JsonSerializer.Deserialize<Customer>(root.GetRawText(), options);
						}
					}
				}
			}
			return null;
		}

		public override void Write(Utf8JsonWriter writer, User value, JsonSerializerOptions options)
		{
			if (value is Admin admin)
			{
				JsonSerializer.Serialize(writer, admin, options);
			}
			else if (value is Customer customer)
			{
				JsonSerializer.Serialize(writer, customer, options);
			}
		}
	}
}

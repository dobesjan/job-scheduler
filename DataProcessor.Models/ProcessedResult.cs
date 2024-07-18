using System.Text.Json;

namespace DataProcessor.Models
{
	public class ProcessedResult
	{
		public int JobId { get; set; }
		public string Result { get; set; }

		public IResult GetResult()
		{
			if (string.IsNullOrEmpty(Result))
			{
				return null;
			}

			try
			{
				var resultObject = JsonSerializer.Deserialize<Result>(Result);
				return resultObject;
			}
			catch (JsonException ex)
			{
				Console.WriteLine($"Error deserializing Result: {ex.Message}");
				return null;
			}
		}
	}
}

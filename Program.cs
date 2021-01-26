using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using ConsoleTables;

namespace ApiClientCS
{

    class Brewery
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("brewery_type")]
        public string BreweryType { get; set; }
        [JsonPropertyName("street")]
        public string Street { get; set; }

        [JsonPropertyName("city")]
        public string City { get; set; }
        [JsonPropertyName("state")]
        public string State { get; set; }
        [JsonPropertyName("postal_code")]

        public string PostalCode { get; set; }
        [JsonPropertyName("longitude")]

        public string longitude { get; set; }
        [JsonPropertyName("latitude")]

        public string Latitude { get; set; }
        [JsonPropertyName("phone")]

        public string Phone { get; set; }
        [JsonPropertyName("website_url")]
        public string WebsiteUrl { get; set; }
    }
    class Program
    {
        static async Task ShowAllBreweriesAsync()
        {
            var client = new HttpClient();
            var responseAsStream = await client.GetStreamAsync("https://api.openbrewerydb.org/breweries?per_page=50");
            List<Brewery> breweries = await JsonSerializer.DeserializeAsync<List<Brewery>>(responseAsStream);

            Console.WriteLine($"There are {breweries.Count()} Breweries in the Breweries API");

            var table = new ConsoleTable("Id", "Name", "Brewery Type", "City", "State");
            foreach (var brewery in breweries)
            {
                table.AddRow(brewery.Id, brewery.Name, brewery.BreweryType, brewery.City, brewery.State);
            }
            table.Write(Format.Minimal);
        }

        static async Task ShowOneBreweriesAsync(int idToSendToApi)
        {
            try
            {
                var client = new HttpClient();
                var responseAsStream = await client.GetStreamAsync($"https://api.openbrewerydb.org/breweries/{idToSendToApi}");
                Brewery brewery = await JsonSerializer.DeserializeAsync<Brewery>(responseAsStream);

                var table = new ConsoleTable("Id", "Name", "Brewery Type", "City", "State", "Phone", "Website");

                table.AddRow(brewery.Id, brewery.Name, brewery.BreweryType, brewery.City, brewery.State, brewery.Phone, brewery.WebsiteUrl);
                table.Write(Format.Minimal);
            }
            catch (HttpRequestException)
            {
                Console.WriteLine("I'm sorry, we could not find the brewery by that Id Number");
            }
        }

        static async Task ShowBreweriesByMultipleAsync(string selectionToSendToApi, string answerToSendToApi)
        {
            try
            {
                var client = new HttpClient();
                var responseAsStream = await client.GetStreamAsync($"https://api.openbrewerydb.org/breweries?by_{selectionToSendToApi}={answerToSendToApi}");
                List<Brewery> breweries = await JsonSerializer.DeserializeAsync<List<Brewery>>(responseAsStream);

                var table = new ConsoleTable("Id", "Name", "Brewery Type", "City", "State");
                foreach (var brewery in breweries)
                {
                    table.AddRow(brewery.Id, brewery.Name, brewery.BreweryType, brewery.City, brewery.State);
                }

                table.Write(Format.Minimal);
            }
            catch (HttpRequestException)
            {
                Console.WriteLine("I'm sorry, we could not find any Breweries by that Type");
            }
        }
        static async Task Main(string[] args)
        {
            var keepGoing = true;
            while (keepGoing)
            {
                Console.Clear();
                Console.Write("View (A)ll Breweries, View (O)ne Brewery, View Breweries by (T)ypes, or (Q)uit ");
                var choice = Console.ReadLine().ToUpper();

                switch (choice)
                {
                    case "Q":
                        keepGoing = false;
                        break;

                    case "A":
                        await ShowAllBreweriesAsync();

                        Console.WriteLine("Press ENTER to continue");
                        Console.ReadLine();

                        break;

                    case "O":
                        Console.Write("Enter the ID of the Brewery to show: ");
                        var id = int.Parse(Console.ReadLine());

                        await ShowOneBreweriesAsync(id);

                        Console.WriteLine("Press ENTER to continue");
                        Console.ReadLine();

                        break;

                    case "T":
                        Console.WriteLine("We have many different ways to View Breweries:");
                        Console.WriteLine("city");
                        Console.WriteLine("type");
                        Console.WriteLine("state");
                        Console.Write("Which would you like to choose? ");
                        var selection = Console.ReadLine();
                        Console.Write($"Enter the {selection} of the Brewery to show: ");
                        var answer = Console.ReadLine();

                        await ShowBreweriesByMultipleAsync(selection, answer);

                        Console.WriteLine("Press ENTER to continue");
                        Console.ReadLine();

                        break;
                }
            }
        }
    }
}

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace DanFestaJuninaCore.Models
{
    public class GolangDishRepository : IDishRepository
    {
        public Task<IEnumerable<Dish>> _dishList { get; private set; }
        public bool GetDishesError { get; private set; }

        private HttpClient _httpClient;
        private Uri BaseEndpoint { get; set; }

        private IOptions<DanAppSettings> danappsettings; 

        public GolangDishRepository(IOptions<DanAppSettings> settings)
        {
            danappsettings = settings;
        }

        async Task<IEnumerable<Dish>> IDishRepository.GetAllDishes(IOptions<DanAppSettings> settings)
        {
            //var requestUrl = "http://localhost:1610/dishlist";
            var requestUrl = settings.Value;

            // requestUrl.DanAPIServiceDishes = "http://192.168.2.120:1610/dishlist";
            // MSAPIdishesIPAddress

            var fullurl = requestUrl.MSAPIdishesIPAddress + "/dishlist";

            System.Uri su = new Uri(fullurl);

            _httpClient = new HttpClient();

            var response = await _httpClient.GetAsync(su);
            response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadAsStringAsync();
            var conv = JsonConvert.DeserializeObject<List<Dish>>(data);

            return conv;
        }

        public async Task<Dish> GetDish(string Name)
        {

            var appsettings = danappsettings.Value;

            //System.Uri su = new Uri(appsettings.DanAPIServiceDishes);

            var urifull = string.Concat(appsettings.MSAPIdishesIPAddress, "/dishList");
            System.Uri su = new Uri(urifull);

            _httpClient = new HttpClient();

            var response = await _httpClient.GetAsync(su);
            response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadAsStringAsync();
            var conv = JsonConvert.DeserializeObject<Dish>(data);

            return conv;
        }


        public async Task<Dish> UpdateDish(Dish dish)
        {

            var appsettings = danappsettings.Value;

            var urifull = string.Concat(appsettings.MSAPIdishesIPAddress, "dishupdate");
            System.Uri su = new Uri(urifull);

            _httpClient = new HttpClient();

            // Preparing the POST
            //
            var content = new MultipartFormDataContent();
            content.Add(new StringContent(dish.Name), "dishname");
            content.Add(new StringContent(dish.Type), "dishtype");
            content.Add(new StringContent(dish.Price), "dishprice");
            content.Add(new StringContent(dish.GlutenFree), "dishglutenfree");
            content.Add(new StringContent(dish.DairyFree), "dishdairyfree");
            content.Add(new StringContent(dish.Vegetarian), "dishvegetarian");
            content.Add(new StringContent(dish.InitialAvailable), "dishinitialavailable");
            content.Add(new StringContent(dish.CurrentAvailable), "dishcurrentavailable");
            content.Add(new StringContent(dish.ImageName), "dishimagename");
            content.Add(new StringContent(dish.Description), "dishdescription");
            content.Add(new StringContent(dish.Descricao), "dishdescricao");
            content.Add(new StringContent(dish.ActivityType), "dishactivitytype");

            HttpResponseMessage response = await _httpClient.PostAsync(su.ToString(), content);
            response.EnsureSuccessStatusCode();

            string apiResponse = await response.Content.ReadAsStringAsync();

            //Dish dishback = new Dish();
            //dishback = JsonConvert.DeserializeObject<Dish>(apiResponse);

            dish = await response.Content.ReadAsAsync<Dish>();
            return dish;
        }

        private HttpContent CreateHttpContent<T>(T content)
        {
            var json = JsonConvert.SerializeObject(content, MicrosoftDateFormatSettings);
            return new StringContent(json, System.Text.Encoding.UTF8, "application/json");
        }

        private static JsonSerializerSettings MicrosoftDateFormatSettings
        {
            get
            {
                return new JsonSerializerSettings
                {
                    DateFormatHandling = DateFormatHandling.MicrosoftDateFormat
                };
            }
        }

    }
}

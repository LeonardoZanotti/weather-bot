// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with EchoBot .NET Template version v4.14.1.2

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System.Net.Http;
using EchoBot.Models;

namespace EchoBot.Bots
{
    public class EchoBot : ActivityHandler
    {
        private BotModels Models = new BotModels();
        private static readonly HttpClient client = new HttpClient();
        public string API_KEY = Environment.GetEnvironmentVariable("OPEN_WEATHER_API_KEY");

        protected float FahrenheitToCelsious(float fahrenheit)
        {
            return (float)Math.Round((fahrenheit - 32) * 5 / 9, 2);
        }

        protected float KelvinToCelsious(float kelvin)
        {
            return (float)Math.Round((kelvin - 273.15), 2);
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            // request
            string request = await client.GetStringAsync($"http://api.openweathermap.org/data/2.5/weather?q={turnContext.Activity.Text}&appid={API_KEY}");

            // response
            BotModels.API_RESPONSE response = Newtonsoft.Json.JsonConvert.DeserializeObject<BotModels.API_RESPONSE>(request);

            // convert the date of the response from seconds to date
            DateTime basicDate = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            DateTime sunriseDate = basicDate.AddSeconds(response.sys.sunrise).ToLocalTime();
            DateTime sunsetDate = basicDate.AddSeconds(response.sys.sunset).ToLocalTime();

            // convert temperature of the response from Kelvin to Celsius
            float temperature = KelvinToCelsious(response.main.temp);
            float maxTemperature = KelvinToCelsious(response.main.temp_max);
            float minTemperature = KelvinToCelsious(response.main.temp_min);
            float feelsLikeTemp = KelvinToCelsious(response.main.feels_like);

            // reply from the bot
            var replyText = $"{response.name} - {response.sys.country}\n\nSunrise: {sunriseDate}\n\n Sunset: {sunsetDate}\n\nWind speed: {response.wind.speed} m/s\n\nWeather: {response.weather[0].main} ({response.weather[0].description})\n\nTemperature: {temperature} °C\n\nFeels like: {feelsLikeTemp} °C\n\nMaximum temperature: {maxTemperature} °C\n\nMinimum temperature: {minTemperature} °C\n\nPressure: {response.main.pressure} hPa\n\nHumidity: {response.main.humidity}%";

            // send
            await turnContext.SendActivityAsync(MessageFactory.Text(replyText, replyText), cancellationToken);
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            var welcomeText = "Hello and welcome!";
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text(welcomeText, welcomeText), cancellationToken);
                }
            }
        }
    }
}

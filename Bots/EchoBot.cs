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

struct WeatherObject
{
    public string main;
    public string description;
}

struct MainObject
{
    public double temp;
    public double feels_like;
    public double temp_min;
    public double temp_max;
    public int pressure;
    public int humidity;
}

struct WindObject
{
    public double speed;
}

struct SysObject
{
    public string country;
    public int sunrise;
    public int sunset;
}

struct API_RESPONSE
{
    public string name;
    public WeatherObject[] weather;
    public MainObject main;
    public WindObject wind;
    public SysObject sys;

}

namespace EchoBot.Bots
{
    public class EchoBot : ActivityHandler
    {
        private static readonly HttpClient client = new HttpClient();
        public string API_KEY = System.Environment.GetEnvironmentVariable("OPEN_WEATHER_API_KEY", EnvironmentVariableTarget.Machine);

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            Console.WriteLine($"dale {API_KEY}");
            string request = await client.GetStringAsync($"http://api.openweathermap.org/data/2.5/weather?q={turnContext.Activity.Text}&appid={API_KEY}");
            API_RESPONSE response = Newtonsoft.Json.JsonConvert.DeserializeObject<API_RESPONSE>(request);
            var replyText = $"{response.name} status: ";
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

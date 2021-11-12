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

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            Console.WriteLine($"dale {API_KEY}");
            string request = await client.GetStringAsync($"http://api.openweathermap.org/data/2.5/weather?q={turnContext.Activity.Text}&appid={API_KEY}");
            BotModels.API_RESPONSE response = Newtonsoft.Json.JsonConvert.DeserializeObject<BotModels.API_RESPONSE>(request);
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

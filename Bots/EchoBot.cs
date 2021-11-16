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

            var message = ((Activity)turnContext.Activity).CreateReply();

            // generate reply from the bot
            // if (turnContext.Activity.Text.StartsWith("c"))
            // message.AttachmentLayout = AttachmentLayoutTypes.Carousel;

            // message.Attachments.Add(CreateAudioCard(response));
            // message.Attachments.Add(CreateAnimationCard(response));
            // message.Attachments.Add(CreateVideoCard(response));
            message.Attachments.Add(CreateHeroCard(response));

            // send
            await turnContext.SendActivityAsync(message, cancellationToken);
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

        private Attachment CreateAudioCard(BotModels.API_RESPONSE response)
        {
            // Audio card response
            var audioCard = new AudioCard
            {
                Title = "Havana",
                Subtitle = "Camila Cabello",
                Image = new ThumbnailUrl
                {
                    Url = "https://en.wikipedia.org/wiki/Havana_(Camila_Cabello_song)#/media/File:Havana_(featuring_Young_Thug)_(Official_Single_Cover)_by_Camila_Cabello.png"
                },
                Media = new List<MediaUrl>
                {
                    new MediaUrl()
                    {
                        Url = "https://www.soundhelix.com/examples/mp3/SoundHelix-Song-1.mp3"
                    }
                },
                Buttons = new List<CardAction>
                {
                    new CardAction()
                    {
                        Title = "Read More",
                        Type = ActionTypes.OpenUrl,
                        Value = "https://en.wikipedia.org/wiki/Havana_(Camila_Cabello_song)"
                    }
                }
            };

            return audioCard.ToAttachment();
        }

        private Attachment CreateVideoCard(BotModels.API_RESPONSE response)
        {
            // Video card response
            var videoCard = new VideoCard();
            videoCard.Title = "Weather video information";
            videoCard.Subtitle = $"{response.name} - {response.sys.country}";
            videoCard.Autoloop = false;
            videoCard.Autostart = true;
            videoCard.Media = new List<MediaUrl> {
                new MediaUrl("https://www.onirikal.com/videos/mp4/animatic_caronte.mp4")
            };

            return videoCard.ToAttachment();
        }

        private Attachment CreateAnimationCard(BotModels.API_RESPONSE response)
        {
            // Animation card response
            var animationCard = new AnimationCard();
            animationCard.Title = "Weather animation information";
            animationCard.Subtitle = $"{response.name} - {response.sys.country}";
            animationCard.Autoloop = false;
            animationCard.Autostart = true;
            animationCard.Media = new List<MediaUrl> {
                new MediaUrl("https://media0.giphy.com/media/26gsad5RsZVhKsUDe/giphy.gif")
            };

            return animationCard.ToAttachment();
        }

        private Attachment CreateHeroCard(BotModels.API_RESPONSE response)
        {
            // convert the date of the response from seconds to date
            DateTime basicDate = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            DateTime sunriseDate = basicDate.AddSeconds(response.sys.sunrise).ToLocalTime();
            DateTime sunsetDate = basicDate.AddSeconds(response.sys.sunset).ToLocalTime();

            // convert temperature of the response from Kelvin to Celsius
            float temperature = KelvinToCelsious(response.main.temp);
            float maxTemperature = KelvinToCelsious(response.main.temp_max);
            float minTemperature = KelvinToCelsious(response.main.temp_min);
            float feelsLikeTemp = KelvinToCelsious(response.main.feels_like);

            // Hero card response
            var images = new List<CardImage>{
                new CardImage("https://images-na.ssl-images-amazon.com/images/I/51ZgFK-FbwL.png", "image", new CardAction(ActionTypes.OpenUrl, "Microsoft", value: "https://www.microsoft.com")),
            };

            var buttons = new List<CardAction>{
                new CardAction {
                    Text = "First button",
                    DisplayText = "Display",
                    Title = "Title",
                    Type = ActionTypes.PostBack,
                    Value = "Value"
                }
            };

            var heroCard = new HeroCard(
                "Weather information",
                $"{response.name} - {response.sys.country}",
                $"Sunrise: {sunriseDate}\n\n Sunset: {sunsetDate}\n\nWind speed: {response.wind.speed} m/s\n\nWeather: {response.weather[0].main} ({response.weather[0].description})\n\nTemperature: {temperature} °C\n\nFeels like: {feelsLikeTemp} °C\n\nMaximum temperature: {maxTemperature} °C\n\nMinimum temperature: {minTemperature} °C\n\nPressure: {response.main.pressure} hPa\n\nHumidity: {response.main.humidity}%",
                images
            // buttons
            );

            return heroCard.ToAttachment();
        }
    }
}

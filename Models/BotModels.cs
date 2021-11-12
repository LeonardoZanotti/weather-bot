namespace EchoBot.Models
{
    public class BotModels
    {
        public struct WeatherObject
        {
            public string main;
            public string description;
        }

        public struct MainObject
        {
            public float temp;
            public float feels_like;
            public float temp_min;
            public float temp_max;
            public int pressure;
            public int humidity;
        }

        public struct WindObject
        {
            public float speed;
        }

        public struct SysObject
        {
            public string country;
            public int sunrise;
            public int sunset;
        }

        public struct API_RESPONSE
        {
            public string name;
            public WeatherObject[] weather;
            public MainObject main;
            public WindObject wind;
            public SysObject sys;

        }
    }
}
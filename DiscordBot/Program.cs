using System;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using System.Text.Json;
using System.Net;


namespace DiscordBot
{
    class Config
    {
        public static string PATH { get; private set; } = "../../config.json";
        public string Prefix { get; set; }
        public string Token { get; set; }
        public string CatUrl { get; set; }
        public string DogUrl { get; set; }
        public string Help { get; set; }
    }
    class Program
    {
        static Config config;
        Random random = new Random();
        static void Main(string[] args)
        {

            //Считывание конфигурационного файла                
            using (FileStream file = File.OpenRead(Config.PATH))
            {
                byte[] array = new byte[file.Length];
                file.Read(array, 0, array.Length);
                string json = System.Text.Encoding.Default.GetString(array);
                config = JsonSerializer.Deserialize<Config>(json);
            }
            ////////////////////////////////////


            new Program().AsyncMain().GetAwaiter().GetResult();
        }

        private async Task AsyncMain()
        {
            var Client = new DiscordSocketClient();

            Client.MessageReceived += CommandController;
            Client.Log += Logger;

            await Client.LoginAsync(TokenType.Bot, config.Token);
            await Client.StartAsync();
            Console.ReadLine();
        }

        private Task Logger(LogMessage message)
        {
            Console.WriteLine(message.ToString());
            return Task.CompletedTask;
        }

        private Task CommandController(SocketMessage responce)
        {
            if (!responce.Author.IsBot)
                if (responce.ToString() == $"{config.Prefix}test")
                {
                    responce.Channel.SendMessageAsync("test");
                }
                else if (responce.ToString() == $"{config.Prefix}cat")
                {
                    GetCat();
                    responce.Channel.SendFileAsync("image.jpg");

                }
                else if (responce.ToString() == $"{config.Prefix}roll")
                {
                    responce.Channel.SendMessageAsync("(от 1 до 100): " + random.Next(1, 100).ToString());

                }
                else if (responce.ToString() == $"{config.Prefix}coin")
                {
                    int coin = random.Next(0, 2);
                    Console.WriteLine(coin);
                    responce.Channel.SendMessageAsync(coin == 1 ? "ОРЕЛ" : "РЕШКА");

                }
                else if (responce.ToString() == $"{config.Prefix}dog")
                {
                    GetDog();
                    responce.Channel.SendFileAsync("image.jpg");

                }
                else if (responce.ToString() == $"{config.Prefix}chgPrefix")
                {
                    //TODO : сделать изменение преффикса
                    responce.Channel.SendMessageAsync("You do not have permission for this operation");
                }
                else if (responce.ToString() == $"{config.Prefix}help" ||  responce.ToString() == "help")
                {
                    responce.Channel.SendMessageAsync($"Преффикс: {config.Prefix} \n{config.Help}");
                }
            return Task.CompletedTask;
        }
        private void GetCat()
        {
            string url = config.CatUrl;
            byte[] image = new WebClient().DownloadData(url);
            using (FileStream file = File.OpenWrite("image.jpg"))
            {
                file.Write(image, 0, image.Length);
            }
        }

        private void GetDog()
        {
            string url = config.DogUrl;
            using (FileStream file = File.OpenWrite("image.jpg"))
            {
                var image = new WebClient().DownloadData(url);
                file.Write(image, 0, image.Length);
            }
        }

    }
}

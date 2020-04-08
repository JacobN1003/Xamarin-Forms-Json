using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace app6_json
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        async void GrabData(object sender, System.EventArgs e)
        {
            if (entrybox.Text == "") //If no input was made and the button was clicked will not crash the app
            {
                entrybox.Placeholder = "Enter a word first..";
                return;
            }

            if (CrossConnectivity.Current.IsConnected) //if internet is connected..
            {
                HttpClient client = new HttpClient(); 
                client.DefaultRequestHeaders.Add("Authorization", "Token " + "3a5bb5c9f4a8d817a752e26dc2eb2df8ed9f23b0");//api key used
                var uri = new Uri(string.Format($"https://owlbot.info/api/v4/dictionary/" + $"{entrybox.Text}"));//uri used
                var request = new HttpRequestMessage();// our request message to the server
                request.Method = HttpMethod.Get;// request type
                request.RequestUri = uri;// what the request will send to the server
                HttpResponseMessage response = await client.SendAsync(request);//server's response to our get request
                Words wordData = null;// initialize our object
                if (response.IsSuccessStatusCode)//if an ok response happens
                {
                    var content = await response.Content.ReadAsStringAsync();//make content hold the data sent from the server
                    wordData = Words.FromJson(content);//deserialize the json to an object
                    BindingContext = wordData;// allows us to use our data object with binding context
                }
                else await DisplayAlert("No Results!", "Check your spelling and search again.", "OK").ConfigureAwait(false);
            }// if an invalid word is searched resulting in no data from the server
            else await DisplayAlert("Oops","Please Connect Your Device To The Internet", "OK").ConfigureAwait(false);
        }//if there was no internet connection
    }

    public partial class Words // our data object
    {
        [JsonProperty("definitions")]
        public Definitions[] Definition { get; set; }
        [JsonProperty("word")]
        public string word { get; set; }
        [JsonProperty("pronunciation")]
        public string pronunciation { get; set; }
        //converting from json to a deserialized object
        public static Words FromJson(string json) => JsonConvert.DeserializeObject<Words>(json, app6_json.Converter.Settings);
    }

    public partial class Definitions// our array of objects of definitions data
    {
        [JsonProperty("type")]
        public string type { get; set; }
        [JsonProperty("definition")]
        public string definition { get; set; }
        [JsonProperty("example")]
        public string example { get; set; }
        [JsonProperty("image_url")]
        public object imageUrl { get; set; }
        [JsonProperty("emoji")]
        public object emoji { get; set; }
        //converting from json to a deserialized object
        public static Definitions FromJson(string json) => JsonConvert.DeserializeObject<Definitions>(json, app6_json.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this Words self) => JsonConvert.SerializeObject(self, app6_json.Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
        {
            new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
        },
    };
    }
}

    

    


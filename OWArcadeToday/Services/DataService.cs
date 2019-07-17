﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Web.Http;
using Newtonsoft.Json;
using OWArcadeToday.Models;
using Windows.Web.Http.Headers;
using OWArcadeToday.Helpers;
using Microsoft.Toolkit.Uwp.Helpers;

namespace OWArcadeToday.Services
{
    public sealed class DataService
    {
        private const string API_TODAY = "https://overwatcharcade.today/api/today";
        private const string API_WEEK = "https://overwatcharcade.today/api/week";

        private readonly string UserAgent;

        public DataService()
        {
            var ver = SystemInformation.ApplicationVersion;
            var osVer = SystemInformation.OperatingSystemVersion;
            var arch = SystemInformation.OperatingSystemArchitecture;
            UserAgent = $"OWArcadeToday/{ver.Major}.{ver.Minor}.{ver.Build}.{ver.Revision} (Windows {osVer.Major}.{osVer.Minor}.{osVer.Build}; {arch}; UWP)";
        }

        public async Task<ArcadeDailyData> GetTodayArcadeAsync()
        {
            string json = await GetResponseAsync(API_TODAY);
            var data = JsonConvert.DeserializeObject<ArcadeDailyData>(json);
            return data;
        }

        public async Task<List<ArcadeDailyData>> GetWeekHistoryAsync()
        {
            string json = await GetResponseAsync(API_WEEK);
            var data = JsonConvert.DeserializeObject<List<ArcadeDailyData>>(json);
            return data;
        }

        private async Task<string> GetResponseAsync(string url)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders["User-Agent"] = UserAgent;

                var response = await client.GetAsync(new Uri(url));
                string responseString = await response.Content.ReadAsStringAsync();

                if (responseString == "[]")
                    throw new NoDataException();

                return responseString;
            }
        }
    }
}

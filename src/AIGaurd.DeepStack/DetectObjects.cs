﻿using AIGaurd.Broker;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace AIGaurd.DeepStack
{
    public class DetectObjects : IDetectObjects
    {
        private readonly HttpClient _client;
        private readonly string _endPoint;

        public DetectObjects(string endPoint)
        {
            _client = new HttpClient();
            _endPoint = endPoint;
        }
        public async Task<IPredictions> DetectObjectsAsync(string imagePath)
        {
            var request = new MultipartFormDataContent();
            var image_data = File.OpenRead(imagePath);
            request.Add(new StreamContent(image_data), "image", Path.GetFileName(imagePath));
            var output = await _client.PostAsync(_endPoint, request);
            var jsonString = await output.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Predictions>(jsonString);
        }

      
    }
}

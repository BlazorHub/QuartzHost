﻿using Microsoft.AspNetCore.Components;
using QuartzHost.Contract.Common;
using QuartzHost.Contract.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace QuartzHost.UI.Common
{
    public static class HttpJsonExtensions
    {
        public const string TOKEN = "7e07d2b6c1444d6bb94c87547916b18d";
        public static string ApiHost = "http://localhost:60000/";

        public static async Task<T> GetHttpJsonAsync<T>(this HttpClient Http, string requestUri, bool isAuth = true)
        {
            if (isAuth) Http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TOKEN);

            return await Http.GetFromJsonAsync<T>($"{ApiHost}{requestUri}", options: QuartzHostExtensions.JsonOptions());
        }

        public static async Task<T> PostHttpAsync<T, C>(this HttpClient Http, string requestUri, C content, string nodeName = "")
        {
            if (!string.IsNullOrWhiteSpace(nodeName))
            {
                var node = await GetNodeAsync(Http, nodeName);
                Http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", node[0]);
                ApiHost = node[1];
            }
            var req = await Http.PostAsJsonAsync($"{ApiHost}{requestUri}", content);
            var str = await req.Content.ReadAsStringAsync();
            return str.ToObj<T>();
        }

        public static async Task<T> PostHttpAsync<T>(this HttpClient Http, string requestUri, object content = null, string nodeName = "")
        {
            if (!string.IsNullOrWhiteSpace(nodeName))
            {
                var node = await GetNodeAsync(Http, nodeName);
                Http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", node[0]);
                ApiHost = node[1];
            }
            var req = await Http.PostAsJsonAsync($"{ApiHost}{requestUri}", content);
            var str = await req.Content.ReadAsStringAsync();
            return str.ToObj<T>();
        }

        private static async Task<string[]> GetNodeAsync(HttpClient Http, string nodeName)
        {
            var list = CacheHelper.Get<List<JobNodesEntity>>("job_node_all");
            if (list == null || list.Any() == false)
            {
                var model = await Http.GetHttpJsonAsync<Result<List<JobNodesEntity>>>("job/node/all", false);
                CacheHelper.Set("job_node_all", model.Data);
                list = model.Data;
            }
            var node = list?.FirstOrDefault(p => p.NodeName == nodeName && p.Status == NodeStatus.Run);
            return new string[]
            {
                node?.AccessSecret ?? "",
                $"{node?.AccessProtocol ?? "http"}://{node?.Host ?? ""}/"
            };
        }
    }
}
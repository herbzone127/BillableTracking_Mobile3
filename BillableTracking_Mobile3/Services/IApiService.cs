using AutoMapper;
using BillableTracking_Mobile3.Models;
using BillableTracking_Mobile3.Services;

using Microsoft.Maui.Networking;
using Microsoft.Maui.Storage;
using MonkeyCache.FileStore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
namespace BillableTracking_Mobile3.Services
{
    public interface IApiService
    {
        Task<List<T>> GetUnauthenticatedAsync<T>(string endpoint) where T : class, new();
        Task<List<T>> GetAsync<T>(string endpoint) where T : class, new();
        Task<T> GetSingleAsync<T>(string endpoint) where T : class, new();
        Task<T> PostExternalLoginAsync<T>(string endpoint, object data) where T : class, new();
        Task<T> PostAsync<T>(string endpoint, object data) where T : class, new();
        Task<bool> PutAsync<T>(string endpoint, T item) where T : class, new();
        Task<bool> DeleteAsync<T>(string endpoint, string id) where T : class, new();
        Task SyncDataAsync<T>(string endpoint) where T : class, new();
        bool IsOnline { get; }
    }


        public class ApiService : IApiService
        {
            private readonly HttpClient _httpClient;
            private readonly IMapper _mapper;
            private readonly IDeviceInfoService _deviceInfoService;
            private readonly string _baseAddress = "https://billabletracking.azurewebsites.net/"; // Configure this
            private bool _isOnline;

            public bool IsOnline => _isOnline;

            public ApiService(IMapper mapper, IDeviceInfoService deviceInfoService)
            {
                _httpClient = new HttpClient();
                _mapper = mapper;
                _deviceInfoService = deviceInfoService;
                _isOnline = false;
                Barrel.ApplicationId = "BillableTracking_Mobile3"; // Set a unique application ID for MonkeyCache
                CheckConnectivityAsync().ConfigureAwait(false); // Start background check
            }

            private async Task CheckConnectivityAsync()
            {
                while (true)
                {
                    _isOnline = await _deviceInfoService.IsConnectedToInternetAsync();
                    Console.WriteLine($"Connectivity Status: {_isOnline}");
                    await Task.Delay(5000); // Check every 5 seconds
                }
            }

            private async Task AddAuthHeaderAsync()
            {
                var token = await SecureStorage.GetAsync("auth_token");
                if (!string.IsNullOrEmpty(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }
                else
                {
                    _httpClient.DefaultRequestHeaders.Authorization = null;
                }
            }

            private void CacheList<T>(string endpoint, List<T> items) where T : class, new()
            {
                var key = $"{endpoint}_list";
                var json = JsonConvert.SerializeObject(items);
                Barrel.Current.Add(key, json, TimeSpan.FromDays(30)); // Cache for 30 days
            }

            private List<T> GetCachedList<T>(string endpoint) where T : class, new()
            {
                var key = $"{endpoint}_list";
                var json = Barrel.Current.Get<string>(key);
                return string.IsNullOrEmpty(json) ? new List<T>() : JsonConvert.DeserializeObject<List<T>>(json) ?? new List<T>();
            }

            public async Task<List<T>> GetUnauthenticatedAsync<T>(string endpoint) where T : class, new()
            {
                if (IsOnline)
                {
                    try
                    {
                        // Ensure no token for unauthenticated requests
                        _httpClient.DefaultRequestHeaders.Authorization = null;

                        var response = await _httpClient.GetAsync($"{_baseAddress}{endpoint}");
                        response.EnsureSuccessStatusCode();
                        var content = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<List<T>>(content) ?? new List<T>();

                        // Save to cache
                        CacheList(endpoint, result);

                        return result;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"API GetUnauthenticated Error: {ex.Message}");
                    }
                }
                // Offline: Return cached data for this endpoint
                return GetCachedList<T>(endpoint); // Single line retrieval
            }

            public async Task<List<T>> GetAsync<T>(string endpoint) where T : class, new()
            {
                if (IsOnline)
                {
                    try
                    {
                        await AddAuthHeaderAsync();
                        var response = await _httpClient.GetAsync($"{_baseAddress}{endpoint}");
                        response.EnsureSuccessStatusCode();
                        var content = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<List<T>>(content) ?? new List<T>();

                        // Save to cache
                        CacheList(endpoint, result);

                        return result;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"API Get Error: {ex.Message}");
                    }
                }
                // Offline: Return cached data for this endpoint
                return GetCachedList<T>(endpoint); // Single line retrieval
            }
        public async Task<T> GetSingleAsync<T>(string endpoint) where T : class, new()
        {
            List<T> lst = null;
            if (IsOnline)
            {
                try
                {
                    await AddAuthHeaderAsync();
                    var response = await _httpClient.GetAsync($"{_baseAddress}{endpoint}");
                    response.EnsureSuccessStatusCode();
                    var content = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<T>(content) ?? new T();

                     lst = new List<T>();
                    // Save to cache
                    lst.Add(result);
                    CacheList(endpoint, lst);

                    return result;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"API Get Error: {ex.Message}");
                }
            }
            // Offline: Return cached data for this endpoint
            return GetCachedList<T>(endpoint).FirstOrDefault(); // Single line retrieval
        }

        public async Task<T> PostExternalLoginAsync<T>(string endpoint, object data) where T : class, new()
            {
                if (IsOnline)
                {
                    try
                    {
                        // Ensure no token for external login requests
                        _httpClient.DefaultRequestHeaders.Authorization = null;

                        var jsonData = JsonConvert.SerializeObject(data);
                        var content = new StringContent(jsonData, System.Text.Encoding.UTF8, "application/json");
                        var response = await _httpClient.PostAsync($"{_baseAddress}{endpoint}", content);
                        response.EnsureSuccessStatusCode();
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<T>(responseContent) ?? default;

                        // Cache the result if it's a user or similar entity
                        if (result != null && typeof(T) == typeof(ExternalLoginResponse))
                        {
                            var existingList1 = GetCachedList<T>(endpoint);
                            existingList1.Add(result);
                            CacheList(endpoint, existingList1);
                        }

                        return result;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"API PostExternalLogin Error: {ex.Message}");
                    }
                }
                // Offline: Store locally and return default
                var item = (T)data;
                var existingList = GetCachedList<T>(endpoint);
                existingList.Add(item);
                CacheList(endpoint, existingList);
                return default;
            }

            public async Task<T> PostAsync<T>(string endpoint, object data) where T : class, new()
            {
                if (IsOnline)
                {
                    try
                    {
                        await AddAuthHeaderAsync();
                        var jsonData = JsonConvert.SerializeObject(data);
                        var content = new StringContent(jsonData, System.Text.Encoding.UTF8, "application/json");
                        var response = await _httpClient.PostAsync($"{_baseAddress}{endpoint}", content);
                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<T>(responseContent) ?? default;

                        // Cache the result
                        var existingList1 = GetCachedList<T>(endpoint);
                        existingList1.Add(result);
                        CacheList(endpoint, existingList1);

                        return result;
                    }
                    else
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        await Application.Current.MainPage.DisplayAlert("ERROR",responseContent, "OK");
                    }
                        
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"API Post Error: {ex.Message}");
                    }
                }
                // Offline: Store locally and return default
                var item = (T)data;
                var existingList = GetCachedList<T>(endpoint);
                existingList.Add(item);
                CacheList(endpoint, existingList);
                return default;
            }

            public async Task<bool> PutAsync<T>(string endpoint, T item) where T : class, new()
            {
                if (IsOnline)
                {
                    try
                    {
                        await AddAuthHeaderAsync();
                        var idProperty = typeof(T).GetProperty("Id");
                        if (idProperty != null)
                        {
                            var id = idProperty.GetValue(item)?.ToString();
                            var jsonData = JsonConvert.SerializeObject(item);
                            var content = new StringContent(jsonData, System.Text.Encoding.UTF8, "application/json");
                            var response = await _httpClient.PutAsync($"{_baseAddress}{endpoint}/{id}", content);
                            response.EnsureSuccessStatusCode();
                            return true;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"API Put Error: {ex.Message}");
                    }
                }
                // Offline: Update cache
                var existingList = GetCachedList<T>(endpoint);
                var innerIdProperty = typeof(T).GetProperty("Id"); // Renamed to avoid conflict
                var innerId = innerIdProperty?.GetValue(item)?.ToString(); // Renamed to avoid conflict
                var existingIndex = existingList.FindIndex(x => innerIdProperty?.GetValue(x)?.ToString() == innerId);
                if (existingIndex >= 0)
                {
                    existingList[existingIndex] = item;
                }
                else
                {
                    existingList.Add(item);
                }
                CacheList(endpoint, existingList);
                return false;
            }

            public async Task<bool> DeleteAsync<T>(string endpoint, string id) where T : class, new()
            {
                if (IsOnline)
                {
                    try
                    {
                        await AddAuthHeaderAsync();
                        var response = await _httpClient.DeleteAsync($"{_baseAddress}{endpoint}/{id}");
                        response.EnsureSuccessStatusCode();
                        var existingList1 = GetCachedList<T>(endpoint);
                        var innerIdProperty1 = typeof(T).GetProperty("Id"); // Renamed to avoid conflict
                        existingList1.RemoveAll(x => innerIdProperty1?.GetValue(x)?.ToString() == id);
                        CacheList(endpoint, existingList1);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"API Delete Error: {ex.Message}");
                    }
                }
                // Offline: Remove from cache
                var existingList = GetCachedList<T>(endpoint);
                var innerIdProperty = typeof(T).GetProperty("Id"); // Renamed to avoid conflict
                existingList.RemoveAll(x => innerIdProperty?.GetValue(x)?.ToString() == id);
                CacheList(endpoint, existingList);
                return false;
            }

            public async Task SyncDataAsync<T>(string endpoint) where T : class, new()
            {
                if (!IsOnline)
                {
                    Console.WriteLine("Offline: Skipping sync.");
                    return;
                }

                try
                {
                    Console.WriteLine($"Starting synchronization for {typeof(T).Name}...");

                    // Fetch online data
                    await AddAuthHeaderAsync();
                    var onlineData = await GetAsync<T>(endpoint) ?? new List<T>();
                    var localData = GetCachedList<T>(endpoint);
                    var idProperty = typeof(T).GetProperty("Id");

                    foreach (var onlineItem in onlineData)
                    {
                        var onlineId = idProperty?.GetValue(onlineItem)?.ToString();
                        var localItem = localData.FirstOrDefault(i => idProperty?.GetValue(i)?.ToString() == onlineId);

                        if (localItem != null)
                        {
                            // Update existing cached record
                            _mapper.Map(onlineItem, localItem);
                            localData.Remove(localItem);
                            localData.Add(localItem);
                            CacheList(endpoint, localData);
                            Console.WriteLine($"Updated {typeof(T).Name} (Id: {onlineId})");
                        }
                        else
                        {
                            // Insert new record
                            localData.Add(onlineItem);
                            CacheList(endpoint, localData);
                            Console.WriteLine($"Inserted new {typeof(T).Name} (Id: {onlineId})");
                        }
                    }

                    // Push local records not present online
                    foreach (var localItem in localData.ToList()) // ToList to avoid modification during iteration
                    {
                        var localId = idProperty?.GetValue(localItem)?.ToString();
                        if (!onlineData.Any(i => idProperty?.GetValue(i)?.ToString() == localId))
                        {
                            await PostAsync<T>(endpoint, localItem);
                            Console.WriteLine($"Pushed new {typeof(T).Name} to server (Id: {localId})");
                        }
                    }

                    Console.WriteLine($"Synchronization completed for {typeof(T).Name}.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Sync Error for {typeof(T).Name}: {ex.Message}\nStackTrace: {ex.StackTrace}");
                }
            }
        }

    
}
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json;
using Incremental.Core.DTOs.Common;

namespace IncrementGame.WPF.Services
{
    public class ApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly int _maxRetries;
        private readonly int _retryDelayMs;

        public ApiClient(
            string baseUrl = "https://localhost:7261/api",
            int maxRetries = 3,
            int retryDelayMs = 1000)
        {
            _baseUrl = baseUrl;
            _maxRetries = maxRetries;
            _retryDelayMs = retryDelayMs;
            _httpClient = new HttpClient();
        }

        private async Task<ApiResult<T>> ExecuteWithRetryAsync<T>(Func<Task<ApiResult<T>>> action)
        {
            int attempt = 0;
            while (true)
            {
                try
                {
                    attempt++;
                    var result = await action();

                    // Если успешно или это не ошибка подключения - возвращаем
                    if (result.Success || !result.Message?.Contains("подключение") == true)
                        return result;

                    // Если это последняя попытка - возвращаем ошибку
                    if (attempt >= _maxRetries)
                        return result;

                    // Ждем перед следующей попыткой
                    await Task.Delay(_retryDelayMs * attempt); // Увеличиваем задержку с каждой попыткой
                }
                catch (Exception ex)
                {
                    if (attempt >= _maxRetries)
                        return ApiResult<T>.Fail($"Не удалось подключиться после {_maxRetries} попыток: {ex.Message}");

                    await Task.Delay(_retryDelayMs * attempt);
                }
            }
        }

        public async Task<ApiResult<T>> GetAsync<T>(string endpoint)
        {
            return await ExecuteWithRetryAsync(async () =>
            {
                try
                {
                    var response = await _httpClient.GetAsync($"{_baseUrl}{endpoint}");
                    var content = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        return JsonConvert.DeserializeObject<ApiResult<T>>(content)
                               ?? ApiResult<T>.Fail("Пустой ответ от сервера");
                    }
                    else
                    {
                        return ApiResult<T>.Fail($"Ошибка сервера: {response.StatusCode}");
                    }
                }
                catch (HttpRequestException ex)
                {
                    return ApiResult<T>.Fail($"Ошибка подключения: {ex.Message}");
                }
                catch (TaskCanceledException)
                {
                    return ApiResult<T>.Fail("Таймаут подключения");
                }
                catch (Exception ex)
                {
                    return ApiResult<T>.Fail($"Неизвестная ошибка: {ex.Message}");
                }
            });
        }

        public async Task<ApiResult<T>> PostAsync<T>(string endpoint, object data = null)
        {
            return await ExecuteWithRetryAsync(async () =>
            {
                try
                {
                    var json = data != null ? JsonConvert.SerializeObject(data) : "";
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await _httpClient.PostAsync($"{_baseUrl}{endpoint}", content);
                    var responseContent = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        return JsonConvert.DeserializeObject<ApiResult<T>>(responseContent)
                               ?? ApiResult<T>.Fail("Пустой ответ от сервера");
                    }
                    else
                    {
                        return ApiResult<T>.Fail($"Ошибка сервера: {response.StatusCode}");
                    }
                }
                catch (HttpRequestException ex)
                {
                    return ApiResult<T>.Fail($"Ошибка подключения: {ex.Message}");
                }
                catch (TaskCanceledException)
                {
                    return ApiResult<T>.Fail("Таймаут подключения");
                }
                catch (Exception ex)
                {
                    return ApiResult<T>.Fail($"Неизвестная ошибка: {ex.Message}");
                }
            });
        }
    }
}
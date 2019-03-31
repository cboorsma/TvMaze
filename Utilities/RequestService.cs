using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Model.Utilities
{
    public interface IRequestService
    {
        Task<Response<T>> Get<T>(Uri uri);
        Task<Response<TResponse>> Post<TData, TResponse>(Uri uri, TData data);
        Task<Response<TResponse>> Delete<TData, TResponse>(Uri uri, TData data);
        Task<Response<TResponse>> Put<TData, TResponse>(Uri uri, TData data);
        Task<Response<T>> PostForm<T>(Uri uri, IDictionary<string, string> vals);
    }
    public class RequestService : IRequestService
    {
        public RequestService()
        {
            JsonApiClient = new HttpClient { Timeout = TimeSpan.FromSeconds(15) };
        }
        private HttpClient JsonApiClient { get; }

        private async Task<Response<T>> SendRequest<T>(HttpRequestMessage request)
        {
            HttpResponseMessage result = null;

            try
            {
                result = await JsonApiClient.SendAsync(request);

                if (!result.IsSuccessStatusCode)
                {
                    IDictionary<string, string[]> validation = null;

                    try
                    {
                        var validationContent = await result.Content.ReadAsStringAsync();
                        validation = JsonConvert.DeserializeObject<Dictionary<string, string[]>>(validationContent);
                    }
                    catch { }

                    return new Response<T>(default(T), result.StatusCode, result.ReasonPhrase, false, true, validation: validation);
                }
                var resultContent = await result.Content.ReadAsStringAsync();

                if (typeof(T) == typeof(string))
                    return new Response<T>((T)(object)resultContent, result.StatusCode, result.ReasonPhrase, true, true);

                var data = JsonConvert.DeserializeObject<T>(resultContent);

                return new Response<T>(data, result.StatusCode, result.ReasonPhrase, true, true);
            }
            catch (WebException e)
            {
                return new Response<T>(default(T), result?.StatusCode, result?.ReasonPhrase ?? e.Message, false, true);
            }
            catch (IOException e)
            {
                return new Response<T>(default(T), result?.StatusCode, result?.ReasonPhrase ?? e.Message, false, true);
            }
            catch (HttpRequestException e)
            {
                return new Response<T>(default(T), result?.StatusCode, result?.ReasonPhrase ?? e.Message, false, true);
            }
            catch (Exception e)
            {
                return new Response<T>(default(T), result?.StatusCode, result?.ReasonPhrase ?? e.Message, false, true);
            }
        }
        public async Task<Response<T>> PostForm<T>(Uri uri, IDictionary<string, string> vals)
        {
            var request = new HttpRequestMessage()
            {
                RequestUri = uri,
                Method = HttpMethod.Post,
                Content = new FormUrlEncodedContent(vals)
            };
            return await SendRequest<T>(request);
        }
        public async Task<Response<T>> Get<T>(Uri uri)
        {
            var request = new HttpRequestMessage()
            {
                RequestUri = uri,
                Method = HttpMethod.Get,
            };
            return await SendRequest<T>(request);
        }
        public async Task<Response<TResponse>> Post<TData, TResponse>(Uri uri, TData data)
        {
            var dataString = JsonConvert.SerializeObject(data);

            var request = new HttpRequestMessage()
            {
                RequestUri = uri,
                Method = HttpMethod.Post,
                Content = new StringContent(dataString, Encoding.UTF8, "application/json")
            };

            return await SendRequest<TResponse>(request);
        }
        public async Task<Response<TResponse>> Put<TData, TResponse>(Uri uri, TData data)
        {
            var dataString = JsonConvert.SerializeObject(data);

            var request = new HttpRequestMessage()
            {
                RequestUri = uri,
                Method = HttpMethod.Put,
                Content = new StringContent(dataString, Encoding.UTF8, "application/json")
            };

            return await SendRequest<TResponse>(request);
        }

        public async Task<Response<TResponse>> Delete<TData, TResponse>(Uri uri, TData data)
        {
            var dataString = JsonConvert.SerializeObject(data);

            var request = new HttpRequestMessage()
            {
                RequestUri = uri,
                Method = HttpMethod.Delete,
                Content = new StringContent(dataString, Encoding.UTF8, "application/json")
            };

            return await SendRequest<TResponse>(request);
        }
    }
}
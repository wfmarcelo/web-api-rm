using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Http;
using Uniarp.Extensions;

namespace WebApiRM
{
    public class ConsultaSqlService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _accessor;
        private string _auth = string.Empty;

        public ConsultaSqlService(HttpClient httpClient,
            IHttpContextAccessor accessor)
        {
            _httpClient = httpClient;
            _accessor = accessor;
        }


        public void SetAuthorization(string auth)
        {
            _auth = auth;
        }

        public void AddAuthorization(string auth)
        {
            var authorization = new AuthenticationHeaderValue("Basic", auth);
            _httpClient.DefaultRequestHeaders.Authorization = authorization;
        }

        public void AddAuthorization()
        {
            string auth;

            if (_accessor.HttpContext == null)
            {
                auth = _auth;
            }
            else
            {
                if (_accessor.HttpContext.User.Claims.Any() == false)
                {
                    auth = _auth;
                }
                else
                {
                    auth = _accessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "authenticationHeader").Value;
                }

                if (auth == null)
                {
                    throw new ArgumentException("Token não foi salvo");
                }
            }


            AddAuthorization(auth);
        }

        public async Task<IList<T>> GetAsync<T>(string codSentenca, int codColigada, string codSistema, string parameters = null)
        {
            AddAuthorization();

            var url = $"framework/v1/consultaSQLServer/RealizaConsulta/{codSentenca}/{codColigada}/{codSistema}";

            if (parameters != null)
            {
                url += $"/?parameters={ HttpUtility.UrlEncode(parameters)}";
            }

            var response = await _httpClient.GetAsync(url);
            await response.EnsureSuccessStatusCodeAsync();

            return await response.Content.ReadAsJsonAsync<IList<T>>();

        }


    }
}
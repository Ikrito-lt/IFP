﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFP.Modules
{
    class RESTClient
    {

        private readonly string BaseUrl;

        public RESTClient(string url)
        {
            BaseUrl = url;
        }

        public string ExecGetHeader(string Endpoint, Dictionary<string, string> Params)
        {
            //setup
            string RequestUrl = BaseUrl + Endpoint;
            RestClient client = new(RequestUrl);
            RestRequest request = new();

            //adding params to request
            foreach (KeyValuePair<string, string> pair in Params)
            {
                request.AddHeader(pair.Key, pair.Value);
            }

            //executing request and checking for response
            var response = client.Execute(request);
            if (response.IsSuccessful)
            {

                string responseContent = response.Content;
                return responseContent;
            }
            else
            {
                MessageBox.Show("An exception just occurred Executing GET:\n" + response.StatusDescription + "\n\nResponse content:\n" + response.Content + "\n\nRequest URL:\n" + RequestUrl + "\n\nSend screenshot you know where.", "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
                throw new Exception(response.StatusDescription);
            }
        }

        public string ExecGetParams(string Endpoint, Dictionary<string, string> Params)
        {
            //setup
            string RequestUrl = BaseUrl + Endpoint;
            RestClient client = new(RequestUrl);
            RestRequest request = new();

            //adding params to request
            foreach (KeyValuePair<string, string> pair in Params)
            {
                request.AddParameter(pair.Key, pair.Value);
            }

            //executing request and checking for response
            var response = client.Execute(request);
            if (response.IsSuccessful)
            {

                string responseContent = response.Content;
                return responseContent;
            }
            else
            {

                MessageBox.Show("An exception just occurred Executing GET:\n" + response.StatusDescription + "\n\nResponse content:\n" + response.Content + "\n\nRequest URL:\n" + RequestUrl + "\n\nSend screenshot you know where.", "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
                throw new Exception(response.StatusDescription);
                //return ExecGet(Endpoint, Params);
            }
        }

        public IRestResponse ExecAddProd(string Endpoint, Dictionary<string, string> Params, string requestBody)
        {
            //setup
            string RequestUrl = BaseUrl + Endpoint;
            RestClient client = new(RequestUrl);
            RestRequest request = new(Method.POST);

            //adding params to request
            foreach (KeyValuePair<string, string> pair in Params)
            {
                request.AddHeader(pair.Key, pair.Value);
            }

            //adding body to request
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", requestBody, ParameterType.RequestBody);
            //executing request and checking for response
            var response = client.Post(request);

            return response;
        }

        public IRestResponse ExecPutProd(string Endpoint, Dictionary<string, string> Params, string requestBody)
        {
            //setup
            string RequestUrl = BaseUrl + Endpoint;
            RestClient client = new(RequestUrl);
            RestRequest request = new(Method.PUT);

            //adding params to request
            foreach (KeyValuePair<string, string> pair in Params)
            {
                request.AddHeader(pair.Key, pair.Value);
            }

            //adding body to request
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", requestBody, ParameterType.RequestBody);
            //executing request and checking for response
            var response = client.Put(request);

            return response;
        }

        public bool ExecDeleteProd(Dictionary<string, string> Params, string shopifyID)
        {
            //setup
            string RequestUrl = BaseUrl + $"products/{shopifyID}.json";
            RestClient client = new(RequestUrl);
            RestRequest request = new(Method.DELETE);

            //adding params to request
            foreach (KeyValuePair<string, string> pair in Params)
            {
                request.AddHeader(pair.Key, pair.Value);
            }

            //executing request and checking for response
            var response = client.Delete(request);

            if (response.IsSuccessful)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}

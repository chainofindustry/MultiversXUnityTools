using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MultiversXUnityTools
{
    public class API
    {
        public string apiName;
        public string baseAddress;
        public List<APIEndpoint> endpoints;

        public API()
        {

        }

        public API(string apiName, string baseAddress)
        {
            this.apiName = apiName;
            this.baseAddress = baseAddress;
            endpoints = new List<APIEndpoint>();

            var values = System.Enum.GetValues(typeof(EndpointNames)).Cast<EndpointNames>();
            foreach (var value in values)
            {
                endpoints.Add(new APIEndpoint(value.ToString(), baseAddress));
            }
        }

        public string GetEndpoint(EndpointNames endpointName)
        {
            APIEndpoint endpoint = endpoints.FirstOrDefault(cond => cond.name == endpointName.ToString());
            if (endpoint != null)
            {
                return $"{endpoint.baseAddress}/{endpoint.resourceName}";
            }
            return null;
        }
    }

    [System.Serializable]
    public class APIEndpoint
    {
        public string name;
        public string baseAddress;
        public string resourceName;

        public APIEndpoint(string name, string baseAddress)
        {
            this.name = name;
            this.baseAddress = baseAddress;
        }
    }
}

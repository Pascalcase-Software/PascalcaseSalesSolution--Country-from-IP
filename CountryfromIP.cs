using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using System.Activities;
using System.Net;
using System.ServiceModel;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.IO;

namespace CountryfromIP
{
    public class customwrokflow : CodeActivity
    {
        // private object collection;

        [Input("Ip")]
        public InArgument<string> Ip { get; set; }
        [Output("Countryname")]
        public OutArgument<string> Countryname { get; set; }
        [Output("Cityname")]
        public OutArgument<string> Cityname { get; set; }
        //Lead entity


        protected override void Execute(CodeActivityContext executionContext)
        {

            ITracingService tracingService = executionContext.GetExtension<ITracingService>();

            //Create the context
            IWorkflowContext context = executionContext.GetExtension<IWorkflowContext>();
            IOrganizationServiceFactory serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            string ipaddress = Ip.Get(executionContext);

            var url = "http://api.ipstack.com/" + ipaddress + "?access_key=bdabebdf8e4b43763d51c4a88401e22f";
            WebClient w = new System.Net.WebClient();
            string jsonResponse = w.DownloadString(url);

            Location location = JSONSerializer<Location>.DeSerialize(jsonResponse);


            Countryname.Set(executionContext, location.country_name);
            Cityname.Set(executionContext, location.city);

        }
    }

    public class Location
    {
        public string country_name { get; set; }
        public string city { get; set; }

    }

    public static class JSONSerializer<TType> where TType : class
    {
        /// <summary>
        /// Serializes an object to JSON
        /// </summary>
        public static string Serialize(TType instance)
        {
            var serializer = new DataContractJsonSerializer(typeof(TType));
            using (var stream = new MemoryStream())
            {
                serializer.WriteObject(stream, instance);
                return Encoding.Default.GetString(stream.ToArray());
            }
        }
        /// <summary>
        /// DeSerializes an object from JSON
        /// </summary>
        public static TType DeSerialize(string json)
        {
            using (var stream = new MemoryStream(Encoding.Default.GetBytes(json)))
            {
                var serializer = new DataContractJsonSerializer(typeof(TType));
                return serializer.ReadObject(stream) as TType;
            }
        }
    }

}
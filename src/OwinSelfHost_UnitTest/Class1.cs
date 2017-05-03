using Microsoft.Owin.Hosting;
using NUnit.Framework;
using Owin;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;
using Newtonsoft.Json;

namespace OwinSelfHost_UnitTest
{
    [SetUpFixture]
    public class GlobalSetup
    {
        private static IDisposable _webapp = null;

        [OneTimeSetUp]
        public static void GlobalOneTimeSetUp()
        {
            _webapp = WebApp.Start<OwinSelfHost_UnitTest.Startup>("http://localhost:9000");
        }

        [OneTimeTearDown]
        public static void GlobalOneTimeTearDown()
        {
            if (_webapp != null)
            {
                _webapp.Dispose();
            }
        }
    }

    [TestFixture]
    public class Class1
    {
        [Test]
        public void Test01()
        {
            string baseAddress = "http://localhost:9000/";

            // Start OWIN host 
            //using (WebApp.Start<Startup>(url: baseAddress))
            {
                // Create HttpCient and make a request to api/values 
                HttpClient client = new HttpClient();

                HttpResponseMessage response = client.GetAsync(baseAddress + "api/values").Result;

                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

                string json = response.Content.ReadAsStringAsync().Result;
                string[] content = JsonConvert.DeserializeObject<string[]>(json);
                Assert.AreEqual(2, content.Length);

                Console.WriteLine(json); // prints ["value1","value2"]
            }
        }
    }

    public class Startup
    {
        // This code configures Web API. The Startup class is specified as a type
        // parameter in the WebApp.Start method.
        public void Configuration(IAppBuilder appBuilder)
        {
            // Configure Web API for self-host. 
            HttpConfiguration config = new HttpConfiguration();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            appBuilder.UseWebApi(config);
        }
    }

    public class ValuesController : ApiController
    {
        // GET api/values 
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5 
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values 
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5 
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5 
        public void Delete(int id)
        {
        }
    }
}

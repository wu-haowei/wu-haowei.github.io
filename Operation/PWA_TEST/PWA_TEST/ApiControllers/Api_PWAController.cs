using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using PWA_TEST.Models;
using PWA_TEST.Service;

namespace PWA_TEST
{
    public class Api_PWAController : ApiController
    {

        private SearchService _searchService;
        public Api_PWAController()
        {
            _searchService = new SearchService();

        }
        //object aaa;
        [HttpPost]
        public void Add_Post([FromBody] Rootobject input)
        {
            if (input != null)
            {
               _searchService.create(input);
            }

        }

        public PWA_Table Get()
        {

            return _searchService.get();
        }

        // GET api/<controller>/5
        //public string Get(int input)
        //{
        //    return "value";
        //}

        // PUT api/<controller>/5
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<controller>/5
        [HttpPost]
        public void POST_Delete([FromBody] Rootobject input)
        {
            if (input != null)
            {
                _searchService.delete(input);
            }

        }
    }

    public class Rootobject
    {
        public string endpoint { get; set; }
        public object expirationTime { get; set; }
        public Keys keys { get; set; }
    }

    public class Keys
    {
        public string p256dh { get; set; }
        public string auth { get; set; }
    }
}
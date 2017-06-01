using System;
using Nalco_Consumables.Models;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
using System.Xml.Serialization;

namespace Nalco_Consumables.Controllers
{
    public class MaterialsController : ApiController
    {
        Materials[] materials = new Materials[]
        {
            new Materials { id = 1, materialcode="ABC",materialdescription="sdjfds",printerdescription="sdf",printercount=0,centralstorage=true,criticalflag=false,quantity=100,reorderleve=5}
        };

        // GET api/<controller>
        public string Get()
        {
            return JsonConvert.SerializeObject(materials);
        }


        // GET api/<controller>/5
        public string Get(int id)
        {
            var material = materials.FirstOrDefault((p) => p.id == id);
            if (material == null)
            {
                return JsonConvert.SerializeObject("type:error");
            }
            return JsonConvert.SerializeObject(material);
        }

        // POST api/<controller>
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}
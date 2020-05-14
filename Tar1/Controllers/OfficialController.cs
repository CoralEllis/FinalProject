﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Tar1.Models;
namespace Tar1.Controllers
{
    public class OfficialController : ApiController
    {
        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        [HttpGet]
        [Route("api/Official/{Unitid:int}")]
        public List<OfficialShift> Get(int Unitid)
        {
            OfficialShift OS = new OfficialShift();
            return OS.GetOS(Unitid);
        }
        [HttpGet]
        [Route("api/Official/Unit")]
        public List<OfficialShift> Get(string unit)
        {
            int Unitid = Convert.ToInt32(unit);
            OfficialShift OS = new OfficialShift();
            return OS.GetEmptyOfficial(Unitid);
        }

        // POST api/<controller>
        public void Post([FromBody]List<OfficialShift> OffShiftArr)
        {
            OfficialShift OS = new OfficialShift();
            OS.InsertOffShift(OffShiftArr);
        }

        // PUT api/<controller>/5
        public void Put([FromBody] OfficialShift OS)
        {
            OS.UpdateOfficialShift();
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}

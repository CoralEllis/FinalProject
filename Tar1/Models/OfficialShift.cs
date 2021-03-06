﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tar1.Models.DAL;


namespace Tar1.Models
{
    public class OfficialShift
    {
        string userid;
        DateTime startshifthour;
        DateTime endshifthour;
        int unitid;
        string shifttype;
        DateTime shiftdate;
        int numofguides;
        String name;

        public string Userid { get => userid; set => userid = value; }
        public DateTime Startshifthour { get => startshifthour; set => startshifthour = value; }
        public DateTime Endshifthour { get => endshifthour; set => endshifthour = value; }
        public int Unitid { get => unitid; set => unitid = value; }
        public string Shifttype { get => shifttype; set => shifttype = value; }
        public DateTime Shiftdate { get => shiftdate; set => shiftdate = value; }
        public int Numofguides { get => numofguides; set => numofguides = value; }
        public string Name { get => name; set => name = value; }

        public OfficialShift() { }
        public OfficialShift(string user,DateTime start, DateTime end,int unit, string type, DateTime date, int num,string _name)
        {
            Userid = user;
            Startshifthour = start;
            Endshifthour = end;
            Unitid = unit;
            Shifttype = type;
            Shiftdate = date;
            Numofguides = num;
            Name = _name;
        }
        public void InsertOffShift(List<OfficialShift> OSArr)
        {
            DBservices dbs = new DBservices();
            dbs.InsertOSList(OSArr);
        }
        public List<OfficialShift> GetOS(int id)
        {
            DBservices dbs = new DBservices();
            return dbs.GetOS(id);
        }
        public void UpdateOfficialShift()
        {
            DBservices dbs = new DBservices();
            dbs.updateOS(this);
        }

        public List<OfficialShift> GetEmptyOfficial(int unitid,string start, string end)
        {
            DBservices dbs = new DBservices();
            return dbs.GetEmptyOfficial(unitid,start,end);
        }

        public bool CheckLongBreak()
        {
            DBservices dbs = new DBservices();
            return dbs.CheckLongBreak(this);
        }
         public void PutMish(string idbefore)
        {
            DBservices dbs = new DBservices();
             dbs.PutMish(this, idbefore);
        }

        public List<OfficialShift> GetallShifts(DateTime start, DateTime end, int unit)
        {
            DBservices dbs = new DBservices();
          return  dbs.GetallShifts(start, end, unit);


        }
        public List<OfficialShift> GetWorkSchdual(int unit)
        {
            DBservices dbs = new DBservices();
            return dbs.GetWorkSchdual(unit);
        }

        public List<OfficialShift> getShiftFromAllOrgan(int unit, string id)
        {
            DBservices dbs = new DBservices();
            return dbs.getShiftFromAllOrgan(unit,id);
        }



    }
}
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tar1.Models;
using System.Data.SqlClient;
using System.Web.Configuration;
using System.Data;
using System.Text;

namespace Tar1.Models.DAL
{
    public class DBservices
    {
        public SqlConnection connect(String conString)
        {

            string cStr = WebConfigurationManager.ConnectionStrings[conString].ConnectionString;
            SqlConnection con = new SqlConnection(cStr);
            con.Open();
            return con;
        }
        private SqlCommand CreateCommand(String CommandSTR, SqlConnection con)
        {

            SqlCommand cmd = new SqlCommand(); // create the command object

            cmd.Connection = con;              // assign the connection to the command object

            cmd.CommandText = CommandSTR;      // can be Select, Insert, Update, Delete 

            cmd.CommandTimeout = 30;           // Time to wait for the execution' The default is 30 seconds

            cmd.CommandType = System.Data.CommandType.Text; // the type of the command, can also be stored procedure

            return cmd;
        }
        public List<ApartmentType> GetApartmentType()
        {

            List<ApartmentType> A = new List<ApartmentType>();
            SqlConnection con = null;

            try
            {
                con = connect("DBConnectionString");
                String selectSTR = "SELECT * FROM ApartmentType_2020";
                SqlCommand cmd = new SqlCommand(selectSTR, con);

                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                while (dr.Read())
                {
                    ApartmentType ApartmentType = new ApartmentType();
                    ApartmentType.Apartmenttype = (string)dr["ApartmentType"];
                    ApartmentType.Id = Convert.ToInt32(dr["ApartmentTypeId"]);
                    A.Add(ApartmentType);
                }

                return A;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }

            }

        }
        public void PostUnit(OrganizeUnit unit)
        {
            SqlConnection con;
            SqlCommand cmd;
            try
            {
                con = connect("DBConnectionString"); // create the connection
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            String cStr = BuildInsertCommand(unit);
            cmd = CreateCommand(cStr, con);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }
            if (unit.Unittype == "דירה")
            {
                GetApartmentId(unit);
            }
        }
        private String BuildInsertCommand(OrganizeUnit unit)
        {
            String command;
            StringBuilder sb = new StringBuilder();

            string G = unit.Numofguides.ToString();
            string R = unit.Numofresidents.ToString();
            sb.AppendFormat("Values('{0}', '{1}','{2}', '{3}','{4}','{5}')", unit.Unitname, R, G, unit.City, unit.Street_hnumber, unit.Unittype);
            String prefix = "INSERT INTO OrganizeUnit_2020" + "(UnitName,NumOfResidents,NumOfGuides,City,Street_HNumber,UnitType)";
            command = prefix + sb.ToString();
            return command;
        }
        private void GetApartmentId(OrganizeUnit unit)
        {
            int id = 0;
            SqlConnection con = null;

            try
            {
                con = connect("DBConnectionString");
                String selectSTR = "select * from Apartment_2020 where ApartmentTypeId is null";
                SqlCommand cmd1 = new SqlCommand(selectSTR, con);
                SqlDataReader dr = cmd1.ExecuteReader(CommandBehavior.CloseConnection);
                dr.Read();
                id = Convert.ToInt32(dr["UnitId"]);
            }
            catch (Exception ex)
            {
                throw (ex);
            }

            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }
            UpdateApartmentType(unit.ApartmentType1, id);
        }
        private void UpdateApartmentType(int ApartmentType, int id)
        {
            SqlConnection con;

            try
            {
                con = connect("DBConnectionString"); // create the connection
            }
            catch (Exception ex)
            {
                throw (ex);
            }

            String cStr = "UPDATE Apartment_2020 SET ApartmentTypeId = " + ApartmentType.ToString() + " WHERE UnitId = " + id.ToString();
            SqlCommand cmd = CreateCommand(cStr, con);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw (ex);
            }

            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }

        }
        public List<OrganizeUnit> GetUnit()
        {

            List<OrganizeUnit> OU = new List<OrganizeUnit>();
            List<Apartment> A = new List<Apartment>();
            A = GetAP();

            SqlConnection con = null;

            try
            {
                con = connect("DBConnectionString");
                String selectSTR = "SELECT * FROM OrganizeUnit_2020";
                SqlCommand cmd = new SqlCommand(selectSTR, con);
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                while (dr.Read())
                {
                    OrganizeUnit Unit = new OrganizeUnit();
                    Unit.Id = Convert.ToInt32(dr["UnitId"]);
                    Unit.Unitname = (string)dr["UnitName"];
                    Unit.City = (string)dr["City"];
                    Unit.Street_hnumber = (string)dr["Street_HNumber"];
                    Unit.Numofguides = Convert.ToInt32(dr["NumOfGuides"]);
                    Unit.Numofresidents = Convert.ToInt32(dr["NumOfResidents"]);
                    Unit.Unittype = (string)dr["UnitType"];
                    if (Unit.Unittype == "דירה")
                    {
                        foreach (var item in A)
                        {
                            if (item.UnitId == Unit.Id)
                            {
                                Unit.ApartmentType1 = item.ApartmenttypeId;
                                A.Remove(item);
                                break;
                            }
                        }
                    }

                    OU.Add(Unit);
                }
                return OU;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }


        }
        public List<Apartment> GetAP()
        {

            List<Apartment> A = new List<Apartment>();

            SqlConnection con = null;

            try
            {
                con = connect("DBConnectionString");
                String selectSTR = "SELECT * FROM Apartment_2020";
                SqlCommand cmd = new SqlCommand(selectSTR, con);
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                while (dr.Read())
                {
                    Apartment ap = new Apartment();

                    ap.UnitId = Convert.ToInt32(dr["UnitId"]);
                    ap.ApartmenttypeId = Convert.ToInt32(dr["ApartmentTypeId"]);

                    A.Add(ap);
                }
                return A;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }


        }
        public void InsertTrainingLevel(TrainingLevel tl)
        {
            SqlConnection con;
            SqlCommand cmd;
            try
            {
                con = connect("DBConnectionString"); // create the connection
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            String cStr = BuildInsertCommand(tl);
            cmd = CreateCommand(cStr, con);
            try
            {
                cmd.ExecuteNonQuery(); // execute the command
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            finally
            {

                if (con != null)
                {
                    con.Close();
                }
            }
        }
        private String BuildInsertCommand(TrainingLevel tl)
        {
            String command;

            String prefix = "INSERT INTO TrainingLevel_2020 (TrainingLevel)";
            String str = "Values('" + tl.Traininglevel + "')";
            command = prefix + str;
            return command;
        }
        public List<TrainingLevel> GetTrainingLevel()
        {

            List<TrainingLevel> TL = new List<TrainingLevel>();
            SqlConnection con = null;

            try
            {
                con = connect("DBConnectionString");
                String selectSTR = "SELECT * FROM TrainingLevel_2020";
                SqlCommand cmd = new SqlCommand(selectSTR, con);
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                while (dr.Read())
                {
                    TrainingLevel trininglevel = new TrainingLevel();
                    trininglevel.Traininglevel = (string)dr["TrainingLevel"];
                    trininglevel.Id = Convert.ToInt32(dr["TrainingLevelId"]);
                    TL.Add(trininglevel);
                }
                return TL;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }
        }
        public void UpdateGuide(TrainingLevel tl, int id)
        {
            SqlConnection con;
            SqlCommand cmd;
            try
            {
                con = connect("DBConnectionString"); // create the connection
            }
            catch (Exception ex)
            {
                throw (ex);

            }
            String cStr = BuildInsertCommand(tl, id);
            cmd = CreateCommand(cStr, con);
            try
            {
                cmd.ExecuteNonQuery(); // execute the command
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }
        }
        private String BuildInsertCommand(TrainingLevel tl, int id)
        {
            string p = id.ToString();
            string trl = tl.Id.ToString();

            return "UPDATE Guide_2020 SET TrainingLevelId =" + trl + " WHERE UserId =" + p;
        }
        public void InsertUser(User user)
        {
            SqlConnection con;
            SqlCommand cmd;
            try
            {
                con = connect("DBConnectionString"); // create the connection
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            String cStr = BuildInsertCommand(user);
            cmd = CreateCommand(cStr, con);
            try
            {
                cmd.ExecuteNonQuery(); // execute the command
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }

        }
        private String BuildInsertCommand(User user)
        {
            String command;
            string Uid;
            StringBuilder sb = new StringBuilder();
            int day = user.Birthdate.Day;
            int month = user.Birthdate.Month;
            int year = user.Birthdate.Year;
            string bdate = month.ToString() + "/" + day.ToString() + "/" + year.ToString();
            string active = user.Isactive.ToString();
            //Insert big manager
            if (user.Unitid == 0)
            {
                string um = user.Unitmanager.ToString();
                string bm = user.Bigmanager.ToString();
                String prefix = "INSERT INTO User_2020 (UserId,UPassword,FirstName,LastName,Birthdate,Telephone,UserRole,IsUnitManager,BigManager,Active)";
                sb.AppendFormat("Values('{0}', '{1}','{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}')", user.Userid, user.Password, user.Firstname, user.Lastname, bdate, user.Telephone, user.Role, um, bm, active);
                command = prefix + sb;
                return command;
            }
            //Insert Guide
            else if (user.Role == "מדריך")
            {
                Uid = user.Unitid.ToString();
                String prefix = "INSERT INTO User_2020 (UserId,UPassword,FirstName,LastName,Birthdate,Telephone,UserRole,UnitId,Active)";
                sb.AppendFormat("Values('{0}', '{1}','{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}')", user.Userid, user.Password, user.Firstname, user.Lastname, bdate, user.Telephone, user.Role, Uid, active);
                command = prefix + sb;
                return command;

            }
            //Insert unit manager
            else
            {
                string um = user.Unitmanager.ToString();
                string bm = user.Bigmanager.ToString();
                Uid = user.Unitid.ToString();
                String prefix = "INSERT INTO User_2020 (UserId,UPassword,FirstName,LastName,Birthdate,Telephone,UserRole,IsUnitManager,BigManager,UnitId,Active)";
                sb.AppendFormat("Values('{0}', '{1}','{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}')", user.Userid, user.Password, user.Firstname, user.Lastname, bdate, user.Telephone, user.Role, um, bm, Uid, active);
                command = prefix + sb;
                return command;
            }
        }
        public User GetUser(string id, string password)
        {
            User U = new User();
            SqlConnection con = null;
            try
            {
                con = connect("DBConnectionString");
                String selectSTR = "SELECT * FROM User_2020 where Active = '1' and UserId = '" + id + "' and UPassword = '" + password + "'";
                SqlCommand cmd = new SqlCommand(selectSTR, con);
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                dr.Read();
                if (dr.HasRows)
                {
                    U.Firstname = (string)dr["FirstName"];
                    U.Lastname = (string)dr["LastName"];
                    U.Password = (string)dr["UPassword"];
                    U.Userid = (string)dr["UserId"];
                    U.Role = (string)dr["UserRole"];
                    if (U.Role == "מנהל מערך הדיור" || U.Role == "סמנכלית")
                    {
                        U.Unitid = 0;
                    }
                    else
                    {
                        U.Unitid = Convert.ToInt32(dr["UnitId"]);
                    }
                    return U;
                }
                else
                {
                    return U;
                }
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }
        }
        public void updateTLTable(TrainingLevel tl, int id)
        {
            SqlConnection con;
            SqlCommand cmd;
            try
            {
                con = connect("DBConnectionString"); // create the connection
            }
            catch (Exception ex)
            {
                throw (ex);

            }
            String cStr = BuildInsertCommand1(tl, id);
            cmd = CreateCommand(cStr, con);
            try
            {
                cmd.ExecuteNonQuery(); // execute the command
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }

        }
        private String BuildInsertCommand1(TrainingLevel tl, int id)
        {
            string p = id.ToString();
            return "UPDATE TrainingLevel_2020 SET TrainingLevel ='" + tl.Traininglevel + "' WHERE TrainingLevelId =" + p;
        }
        public bool GetPer(int id)
        {
            SqlConnection con = null;
            string today = DateTime.Today.ToString("yyyy-MM-dd");
            bool preprdness = false;
            try
            {
                con = connect("DBConnectionString");
                String checkSTR = "select preparedness from SchedulingPeriod_2020 where StartPeriod >'" + today + "' and UnitId=" + id;
                SqlCommand cmd1 = new SqlCommand(checkSTR, con);
                SqlDataReader dr1 = cmd1.ExecuteReader(CommandBehavior.CloseConnection);
                preprdness = dr1.Read();
                return preprdness;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }


        }
        public List<User> GetUnitUser(int id, DateTime date)
        {
            List<User> U = new List<User>();
            SqlConnection con = null;
            bool preprdness = GetPer(id);
            try
            {
                con = connect("DBConnectionString");
                String selectSTR = "select * from User_2020 where UnitId = " + id + " and UserRole = 'מדריך' and Active ='1'";
                SqlCommand cmd = new SqlCommand(selectSTR, con);
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                while (dr.Read())
                {
                    User us = new User();
                    us.Firstname = (string)dr["FirstName"];
                    us.Lastname = (string)dr["LastName"];
                    us.Userid = (string)dr["UserId"];
                    User us1 = GuideInfo(us.Userid, preprdness);
                    us.MonthlyHours = us1.MonthlyHours;
                    us.MonthlyExtraHours = us1.MonthlyExtraHours;
                    User us2 = CountShift(us.Userid);
                    us.NumOfPref = us2.NumOfPref;
                    us.TrainingLevelId = GetGuideTrainLev(us.Userid);
                    us.Weeklyhours = GetWeeklyHours(us.Userid, date);

                    U.Add(us);
                }
                return U;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }
        }
        public User GuideInfo(string Userid, bool preprdness)
        {
            User Os = new User();
            double extra = 0;
            double normal = 0;
            SqlConnection con = null;
            List<Constraint> ConstList = getConstraintM();
            float RegularShift = ConstList[6].ConstraintValue;
            float SpeNightShift = ConstList[7].ConstraintValue;
            try
            {
                string sMonth = DateTime.Now.ToString("MM");
                string year = DateTime.Today.Year.ToString();

                con = connect("DBConnectionString");

                String selectSTR = "select * from OfficialShift_2020 where UserId = " + Userid + " and ShiftDate > '" + year + "-" + sMonth + "-" + "01'";
                SqlCommand cmd = new SqlCommand(selectSTR, con);
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                while (dr.Read())
                {
                    OfficialShift OffiS = new OfficialShift();
                    OffiS.Shifttype = (string)dr["ShiftType"];
                    TimeSpan myTimeSpan = ((dr).GetTimeSpan(dr.GetOrdinal("StartShift")));
                    OffiS.Startshifthour = new DateTime(myTimeSpan.Ticks);
                    myTimeSpan = ((dr).GetTimeSpan(dr.GetOrdinal("EndShift")));
                    OffiS.Endshifthour = new DateTime(myTimeSpan.Ticks);
                    TimeSpan interval = OffiS.Endshifthour - OffiS.Startshifthour;
                    double x = interval.TotalHours;
                    if (preprdness == true && OffiS.Shifttype == "לילה")
                    {
                        double x1 = 0.0;
                        if (x < 0)
                        {
                            x1 = x + 24.0;
                        }
                        else if (x > 0)
                        {
                            x1 = x;
                        }
                        if (x1 > SpeNightShift)
                        {
                            normal += SpeNightShift;
                            extra += x1 - SpeNightShift;
                        }
                        else
                        {
                            normal += x1;
                        }
                    }
                    else
                    {
                        if (x > RegularShift)
                        {
                            normal += RegularShift;
                            extra += x - RegularShift;
                        }
                        else
                        {
                            normal += x;
                        }
                    }
                    Os.MonthlyHours = normal;
                    Os.MonthlyExtraHours = extra;
                }
                return Os;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }
        }
        public User CountShift(string Userid)
        {
            User CS = new User();
            SqlConnection con = null;
            try
            {
                string today = DateTime.Today.ToString("yyyy-MM-dd");
                con = connect("DBConnectionString");
                String selectSTR = "SELECT COUNT(isApply)";
                selectSTR += " FROM BlockShift_2020 left join Shift_2020 on BlockShift_2020.ShiftDate = Shift_2020.ShiftDate and BlockShift_2020.ShiftType = Shift_2020.ShiftType";
                selectSTR += " WHERE Shift_2020.StartPeriod > '" + today + "' and BlockShift_2020.UserId = '" + Userid + "' and BlockShift_2020.isApply = '1'";
                SqlCommand cmd = new SqlCommand(selectSTR, con);
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                CS.NumOfPref = Convert.ToInt32(dr.Read());
                return CS;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }
        }
        public void InsertPeriod(Period period)
        {
            SqlConnection con;
            SqlCommand cmd;
            try
            {
                con = connect("DBConnectionString"); // create the connection
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            String cStr = BuildInsertCommand(period);
            cmd = CreateCommand(cStr, con);
            try
            {
                cmd.ExecuteNonQuery(); // execute the command
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            finally
            {

                if (con != null)
                {
                    con.Close();
                }
            }




        }
        private String BuildInsertCommand(Period period)
        {
            String command;
            string Uid;
            StringBuilder sb = new StringBuilder();
            int day = period.Startdate.Day;
            int month = period.Startdate.Month;
            int year = period.Startdate.Year;
            string startsD = month.ToString() + "/" + day.ToString() + "/" + year.ToString();
            int Eday = period.Enddate.Day;
            int Emonth = period.Enddate.Month;
            int Eyear = period.Enddate.Year;
            string EndD = Emonth.ToString() + "/" + Eday.ToString() + "/" + Eyear.ToString();
            Uid = period.Unitid.ToString();
            string s = startsD.ToString();
            string e = EndD.ToString();
            string p = period.Preparedness1.ToString();
            String prefix = "INSERT INTO SchedulingPeriod_2020 (StartPeriod,EndPeriod,UnitId,preparedness)";
            sb.AppendFormat("Values('{0}', '{1}','{2}','{3}')", s, e, Uid, p);
            command = prefix + sb;
            return command;

        }
        public void InsertShiftList(List<Shift> shiftArr)
        {
            SqlConnection con = null;
            SqlCommand cmd;

            try
            {
                con = connect("DBConnectionString");
                foreach (var item in shiftArr)
                {
                    String cStr = BuildInsertCommand(item);
                    cmd = CreateCommand(cStr, con);
                    try
                    {
                        cmd.ExecuteNonQuery(); // execute the command
                    }
                    catch (Exception ex)
                    {
                        throw (ex);
                    }
                }
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }

        }
        private String BuildInsertCommand(Shift shift)
        {
            string Shiftdate;
            String command;
            StringBuilder sb = new StringBuilder();
            int day = shift.Startperiod.Day;
            int month = shift.Startperiod.Month;
            int year = shift.Startperiod.Year;
            //convert start date of period to string
            string startsPD = month.ToString() + "/" + day.ToString() + "/" + year.ToString();
            int Eday = shift.Endperiod.Day;
            int Emonth = shift.Endperiod.Month;
            int Eyear = shift.Endperiod.Year;
            //convert end date of period to string
            string EndPD = Emonth.ToString() + "/" + Eday.ToString() + "/" + Eyear.ToString();
            //convert start hour of shift to string
            string s = shift.Start.ToString();
            string[] DateTime = s.Split(' ');
            string strH = DateTime[1];
            //convert end hour of shift to string
            string e = shift.End.ToString();
            string[] DateTime1 = e.Split(' ');
            string endH = DateTime1[1];
            //convert unit id to string
            string uid = shift.Uid.ToString();
            day = shift.Shiftdate.Day;
            month = shift.Shiftdate.Month;
            year = shift.Shiftdate.Year;
            //convert date of shift to string
            Shiftdate = month.ToString() + "/" + day.ToString() + "/" + year.ToString();
            //convert number of guides on shift to string
            string num = shift.GuideNum1.ToString();
            sb.AppendFormat("Values('{0}', '{1}','{2}', '{3}', '{4}', '{5}', '{6}', '{7}')", shift.Type, strH, endH, uid, startsPD, EndPD, Shiftdate, num);
            String prefix = "INSERT INTO Shift_2020 " + "(ShiftType,StartShift,EndShift,UnitId,StartPeriod,EndPeriod,ShiftDate,NumOfGuides)";
            command = prefix + sb.ToString();
            return command;
        }
        public List<Shift> GetShiftList(int id)
        {
            List<Shift> S = new List<Shift>();
            SqlConnection con = null;
            string today = DateTime.Today.ToString("yyyy-MM-dd");
            string unitId = id.ToString();
            try
            {
                con = connect("DBConnectionString");
                String selectSTR = "select * from Shift_2020 where UnitId = " + unitId + " AND StartPeriod > '" + today + "'";
                SqlCommand cmd = new SqlCommand(selectSTR, con);
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                while (dr.Read())
                {
                    Shift shist = new Shift();
                    shist.Startperiod = Convert.ToDateTime(dr["StartPeriod"]).Date;
                    shist.Endperiod = Convert.ToDateTime(dr["EndPeriod"]).Date;
                    shist.Shiftdate = Convert.ToDateTime(dr["ShiftDate"]).Date;
                    shist.Type = Convert.ToString(dr["ShiftType"]);
                    shist.GuideNum1 = Convert.ToInt32(dr["NumOfGuides"]);
                    TimeSpan myTimeSpan = ((dr).GetTimeSpan(dr.GetOrdinal("StartShift")));
                    shist.Start = new DateTime(myTimeSpan.Ticks);
                    myTimeSpan = ((dr).GetTimeSpan(dr.GetOrdinal("EndShift")));
                    //shist.Start = TimeSpan(dr["StartShift"]);
                    shist.End = new DateTime(myTimeSpan.Ticks);
                    S.Add(shist);
                }
                return S;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }



        }
        public void InsertApplyShift(List<ApplyShift> AS)
        {
            SqlConnection con = null;
            SqlCommand cmd;

            try
            {
                con = connect("DBConnectionString");
                foreach (var item in AS)
                {
                    String cStr = BuildInsertCommand(item);
                    cmd = CreateCommand(cStr, con);
                    try
                    {
                        cmd.ExecuteNonQuery(); // execute the command
                    }
                    catch (Exception ex)
                    {
                        throw (ex);
                    }
                }
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }

        }
        private String BuildInsertCommand(ApplyShift AppS)
        {

            String command;
            StringBuilder sb = new StringBuilder();
            int day = AppS.Shiftdate.Day;
            int month = AppS.Shiftdate.Month;
            int year = AppS.Shiftdate.Year;
            string shiftdate = month.ToString() + "/" + day.ToString() + "/" + year.ToString();
            string Unitid = AppS.Unitid.ToString();
            string isApply = AppS.Isaplly1.ToString();
            sb.AppendFormat("Values('{0}', '{1}','{2}', '{3}', '{4}', '{5}')", AppS.Userid, AppS.Comment, Unitid, AppS.Shifttype, shiftdate, isApply);
            String prefix = "INSERT INTO BlockShift_2020 " + "(UserId,Comments,UnitId,ShiftType,ShiftDate,isApply)";
            command = prefix + sb.ToString();
            return command;
        }
        public List<ApplyShift> GetAS(int id)
        {
            List<ApplyShift> AS = new List<ApplyShift>();
            SqlConnection con = null;
            string today = DateTime.Today.ToString("yyyy-MM-dd");
            string UnitId = id.ToString();

            try
            {
                con = connect("DBConnectionString");
                String selectSTR = "select BlockShift_2020.ShiftType, BlockShift_2020.ShiftDate, BlockShift_2020.UserId,BlockShift_2020.isApply, BlockShift_2020.Comments,  User_2020.FirstName,  User_2020.LastName";
                selectSTR += " from BlockShift_2020 left join Shift_2020 on BlockShift_2020.ShiftDate = Shift_2020.ShiftDate and BlockShift_2020.ShiftType = Shift_2020.ShiftType left join User_2020 on BlockShift_2020.UserId = User_2020.UserId";
                selectSTR += " where BlockShift_2020.UnitId = " + UnitId + " AND Shift_2020.StartPeriod >'" + today + "' and User_2020.Active ='1'";
                SqlCommand cmd = new SqlCommand(selectSTR, con);
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                while (dr.Read())
                {
                    ApplyShift Applyshift = new ApplyShift();
                    Applyshift.Userid = Convert.ToString(dr["UserId"]);
                    Applyshift.Name = Convert.ToString(dr["FirstName"]) + " " + Convert.ToString(dr["LastName"]);
                    Applyshift.Shiftdate = Convert.ToDateTime(dr["ShiftDate"]).Date;
                    Applyshift.Shifttype = Convert.ToString(dr["ShiftType"]);
                    Applyshift.Isaplly1 = Convert.ToBoolean(dr["isApply"]);
                    Applyshift.Comment = Convert.ToString(dr["Comments"]);
                    AS.Add(Applyshift);
                }
                return AS;

            }
            catch (Exception ex)
            {
                throw (ex);
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }
        }
        public List<ApplyShift> GetApplyShift(string IdUnit, string IdUser)
        {
            List<ApplyShift> AS = new List<ApplyShift>();
            SqlConnection con = null;
            string today = DateTime.Today.ToString("yyyy-MM-dd");

            try
            {
                con = connect("DBConnectionString");
                String selectSTR = "select BlockShift_2020.ShiftType, BlockShift_2020.ShiftDate, BlockShift_2020.UserId,BlockShift_2020.isApply, BlockShift_2020.Comments,  User_2020.FirstName,  User_2020.LastName";
                selectSTR += " from BlockShift_2020 left join Shift_2020 on BlockShift_2020.ShiftDate = Shift_2020.ShiftDate and BlockShift_2020.ShiftType = Shift_2020.ShiftType left join User_2020 on BlockShift_2020.UserId = User_2020.UserId";
                selectSTR += " where BlockShift_2020.UnitId = " + IdUnit + " AND Shift_2020.StartPeriod >'" + today + "' AND BlockShift_2020.UserId ='" + IdUser + "' and User_2020.Active ='1'";
                SqlCommand cmd = new SqlCommand(selectSTR, con);
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                while (dr.Read())
                {
                    ApplyShift Applyshift = new ApplyShift();
                    Applyshift.Userid = Convert.ToString(dr["UserId"]);
                    Applyshift.Name = Convert.ToString(dr["FirstName"]) + " " + Convert.ToString(dr["LastName"]);
                    Applyshift.Shiftdate = Convert.ToDateTime(dr["ShiftDate"]).Date;
                    Applyshift.Shifttype = Convert.ToString(dr["ShiftType"]);
                    Applyshift.Isaplly1 = Convert.ToBoolean(dr["isApply"]);
                    Applyshift.Comment = Convert.ToString(dr["Comments"]);
                    AS.Add(Applyshift);
                }
                return AS;

            }
            catch (Exception ex)
            {
                throw (ex);
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }
        }
        public List<Constraint> getConstraintM()
        {
            List<Constraint> C = new List<Constraint>();
            SqlConnection con = null;
            try
            {
                con = connect("DBConnectionString");
                String selectSTR = "SELECT * FROM ConstraintManagement_2020";
                SqlCommand cmd = new SqlCommand(selectSTR, con);
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                while (dr.Read())
                {
                    Constraint co = new Constraint();
                    co.ConstraintID = Convert.ToInt32(dr["ConstraintID"]);
                    co.ConstraintName = (string)dr["ConstraintName"];
                    co.ConstraintValue = (float)Convert.ToDouble(dr["ConstraintValue"]);
                    C.Add(co);
                }
                return C;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }
        }
        public void PutConstraint(Constraint c)
        {
            SqlConnection con;
            SqlCommand cmd;

            try
            {
                con = connect("DBConnectionString");
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            String cStr = BuildPutCommandSale(c);
            cmd = CreateCommand(cStr, con);
            try
            {
                cmd.ExecuteNonQuery();

            }
            catch (Exception ex)
            {

                throw (ex);
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }
        }
        private String BuildPutCommandSale(Constraint c)
        {
            string value = c.ConstraintValue.ToString();
            string id = c.ConstraintID.ToString();
            String prefix = "UPDATE ConstraintManagement_2020 SET [ConstraintName] = '" + c.ConstraintName + "', [ConstraintValue] =  '" + value + "' WHERE [ConstraintID] = " + id + "";
            return prefix;
        }
        public void updateAS(ApplyShift AS)
        {
            SqlConnection con;
            SqlCommand cmd;
            try
            {
                con = connect("DBConnectionString"); // create the connection
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            String cStr = BuildUpdateCommand(AS);
            cmd = CreateCommand(cStr, con);
            try
            {
                cmd.ExecuteNonQuery(); // execute the command
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }
        }
        private String BuildUpdateCommand(ApplyShift AS)
        {
            string u = AS.Userid.ToString();
            string unit = AS.Unitid.ToString();
            string t = AS.Shifttype;
            int day = AS.Shiftdate.Day;
            int month = AS.Shiftdate.Month;
            int year = AS.Shiftdate.Year;
            string d = month.ToString() + "/" + day.ToString() + "/" + year.ToString();
            string comment = AS.Comment;
            string isApl = AS.Isaplly1.ToString();
            string str = "UPDATE BlockShift_2020 SET Comments ='" + comment + "' WHERE UserId =" + u + " and UnitId = " + unit + " and ShiftType = '" + t + "' and ShiftDate = '" + d + "'";
            str += " UPDATE BlockShift_2020 SET isApply ='" + isApl + "' WHERE UserId =" + u + " and UnitId = " + unit + " and ShiftType = '" + t + "' and ShiftDate = '" + d + "'";
            return str;
        }
        public void InsertExceptionsList(List<Exceptions> ExceptionsArr)
        {
            SqlConnection con = null;
            SqlCommand cmd;

            try
            {
                con = connect("DBConnectionString");
                foreach (var item in ExceptionsArr)
                {
                    String cStr = BuildInsertCommand1(item);
                    cmd = CreateCommand(cStr, con);
                    try
                    {
                        cmd.ExecuteNonQuery(); // execute the command
                    }
                    catch (Exception ex)
                    {
                        throw (ex);
                    }
                }
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }
        }
        private String BuildInsertCommand1(Exceptions Excpt)
        {
            String command;
            StringBuilder sb = new StringBuilder();

            string UsId = Excpt.User.ToString();
            string Unid = Excpt.Unit.ToString();
            int indx = GetExceptionType(Excpt.Index);
            string index = indx.ToString();

            int day = Excpt.ShiftDate.Day;
            int month = Excpt.ShiftDate.Month;
            int year = Excpt.ShiftDate.Year;
            string ShiftDate = month.ToString() + "/" + day.ToString() + "/" + year.ToString();

            sb.AppendFormat("Values('{0}', '{1}', '{2}', '{3}', '{4}')", index, UsId, Unid, Excpt.ShiftType, ShiftDate);
            String prefix = "INSERT INTO Exception_2020 " + "(TypeofExceptionId,UserId,UnitId,ShiftType,ShiftDate)";
            command = prefix + sb.ToString();
            return command;

        }
        public int GetExceptionType(string str)
        {
            SqlConnection con = null;
            int ind;
            try
            {
                con = connect("DBConnectionString");
                String checkSTR = "select TypeofException_2020.TypeofExceptionId from TypeofException_2020 where TypeofException = '" + str + "'";
                SqlCommand cmd = new SqlCommand(checkSTR, con);
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                dr.Read();
                ind = Convert.ToInt32(dr["TypeofExceptionId"]);
                return ind;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }
        }
        public void InsertOSList(List<OfficialShift> OSArr)
        {
            SqlConnection con = null;
            SqlCommand cmd;

            try
            {
                con = connect("DBConnectionString");
                foreach (var item in OSArr)
                {
                    String cStr = BuildInsertCommand(item);
                    cmd = CreateCommand(cStr, con);
                    try
                    {
                        cmd.ExecuteNonQuery(); // execute the command
                    }
                    catch (Exception ex)
                    {
                        throw (ex);
                    }
                }
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }

        }
        private String BuildInsertCommand(OfficialShift OShift)
        {
            String command;
            StringBuilder sb = new StringBuilder();

            //convert start hour of shift to string
            string s = OShift.Startshifthour.ToString();
            string[] DateTime = s.Split(' ');
            string strH = DateTime[1];
            //convert end hour of shift to string
            string e = OShift.Endshifthour.ToString();
            string[] DateTime1 = e.Split(' ');
            string endH = DateTime1[1];
            string Unid = OShift.Unitid.ToString();
            int day = OShift.Shiftdate.Day;
            int month = OShift.Shiftdate.Month;
            int year = OShift.Shiftdate.Year;
            string ShiftDate = month.ToString() + "/" + day.ToString() + "/" + year.ToString();

            sb.AppendFormat("Values('{0}', '{1}', '{2}', '{3}', '{4}', '{5}')", OShift.Userid, strH, endH, Unid, OShift.Shifttype, ShiftDate);
            String prefix = "INSERT INTO OfficialShift_2020 " + "(UserId,StartShift,EndShift,UnitId,ShiftType,ShiftDate)";
            command = prefix + sb.ToString();
            return command;
        }

        public List<OfficialShift> GetOS(int id)
        {
            List<OfficialShift> Shifts = new List<OfficialShift>();
            SqlConnection con = null;
            string today = DateTime.Today.ToString("yyyy-MM-dd");
            string UnitId = id.ToString();

            try
            {
                con = connect("DBConnectionString");
                String selectSTR = "select OfficialShift_2020.UserId, OfficialShift_2020.StartShift, OfficialShift_2020.EndShift, OfficialShift_2020.UnitId, OfficialShift_2020.ShiftType, OfficialShift_2020.ShiftDate, User_2020.FirstName, User_2020.LastName";
                selectSTR += " from OfficialShift_2020 left join Shift_2020 on OfficialShift_2020.ShiftDate = Shift_2020.ShiftDate and OfficialShift_2020.ShiftType = Shift_2020.ShiftType and OfficialShift_2020.UnitId = Shift_2020.UnitId left join User_2020 on OfficialShift_2020.UserId =  User_2020.UserId";
                selectSTR += " where Shift_2020.StartPeriod >'" + today + "' and Shift_2020.UnitId = " + UnitId;
                SqlCommand cmd = new SqlCommand(selectSTR, con);
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                while (dr.Read())
                {
                    OfficialShift OffS = new OfficialShift();
                    OffS.Userid = Convert.ToString(dr["UserId"]) + "," + Convert.ToString(dr["FirstName"]) + " " + Convert.ToString(dr["LastName"]);
                    TimeSpan myTimeSpan = ((dr).GetTimeSpan(dr.GetOrdinal("StartShift")));
                    OffS.Startshifthour = new DateTime(myTimeSpan.Ticks);
                    TimeSpan myTimeSpan1 = ((dr).GetTimeSpan(dr.GetOrdinal("EndShift")));
                    OffS.Endshifthour = new DateTime(myTimeSpan1.Ticks);
                    OffS.Unitid = id;
                    OffS.Shiftdate = Convert.ToDateTime(dr["ShiftDate"]).Date;
                    OffS.Shifttype = Convert.ToString(dr["ShiftType"]);

                    Shifts.Add(OffS);
                }
                return Shifts;
            }

            catch (Exception ex)
            {
                throw (ex);
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }
        }

        public void updateATTable(ApartmentType AP, int id)
        {
            SqlConnection con;
            SqlCommand cmd;
            try
            {
                con = connect("DBConnectionString"); // create the connection
            }
            catch (Exception ex)
            {
                throw (ex);

            }
            String cStr = BuildInsertCommand2(AP, id);
            cmd = CreateCommand(cStr, con);
            try
            {
                cmd.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                throw (ex);
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }
        }

        private String BuildInsertCommand2(ApartmentType ap, int id)
        {
            string p = id.ToString();
            return "UPDATE ApartmentType_2020 SET ApartmentType ='" + ap.Apartmenttype + "' WHERE ApartmentTypeId =" + p;
        }
        public void InsertApaType(ApartmentType apty)
        {
            SqlConnection con;
            SqlCommand cmd;
            try
            {
                con = connect("DBConnectionString"); // create the connection
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            String cStr = BuildInsertCommand(apty);
            cmd = CreateCommand(cStr, con);
            try
            {
                cmd.ExecuteNonQuery(); // execute the command
            }

            catch (Exception ex)
            {
                throw (ex);
            }
            finally
            {

                if (con != null)
                {
                    con.Close();
                }
            }
        }

        private String BuildInsertCommand(ApartmentType at)
        {
            String command;

            String prefix = "INSERT INTO ApartmentType_2020 (ApartmentType)";
            String str = "Values('" + at.Apartmenttype + "')";
            command = prefix + str;
            return command;
        }

        private int GetGuideTrainLev(string id)
        {
            SqlConnection con = null;
            int trainLevId = 0;
            try
            {
                con = connect("DBConnectionString");

                String cStr = "select TrainingLevelId from Guide_2020 where UserId = '" + id + "'";
                //String cStr = "select * from Guide_2020 where UserId = '" + id + "'";

                SqlCommand cmd = new SqlCommand(cStr, con);

                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                dr.Read();
                return trainLevId = Convert.ToInt32(dr["TrainingLevelId"]);


            }
            catch (Exception ex)
            {
                throw (ex);
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }
        }

        public List<User> getListGiudesUsers(int id)
        {
            List<User> U = new List<User>();
            SqlConnection con = null;
            try
            {
                con = connect("DBConnectionString");
                String selectSTR = "select * from User_2020 where UnitId = " + id + " and UserRole = 'מדריך'";
                SqlCommand cmd = new SqlCommand(selectSTR, con);
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                while (dr.Read())
                {
                    User us = new User();
                    us.Firstname = (string)dr["FirstName"];
                    us.Lastname = (string)dr["LastName"];
                    us.Userid = (string)dr["UserId"];
                    us.Birthdate = Convert.ToDateTime(dr["Birthdate"]).Date;
                    us.Password = (string)dr["UPassword"];
                    us.Telephone = (string)dr["Telephone"];
                    us.TrainingLevelId = GetGuideTrainLev(us.Userid);
                    us.Isactive = Convert.ToBoolean(dr["Active"]);
                    U.Add(us);
                }
                return U;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }
        }


        public void UpdateUserDet(User u, string id)
        {
            SqlConnection con;
            SqlCommand cmd;
            try
            {
                con = connect("DBConnectionString"); // create the connection
            }
            catch (Exception ex)
            {
                throw (ex);

            }
            int day = u.Birthdate.Day;
            int month = u.Birthdate.Month;
            int year = u.Birthdate.Year;
            string bdate = month.ToString() + "/" + day.ToString() + "/" + year.ToString();
            string active = u.Isactive.ToString();
            string cStr = "UPDATE User_2020 SET UserId ='" + u.Userid + "' , FirstName = '" + u.Firstname + "' , LastName = '" + u.Lastname + "' , Birthdate = '" + bdate + "' , Telephone = '" + u.Telephone + "' , UPassword = '" + u.Password + "' , Active = '" + active + "' WHERE UserId =" + id;

            cmd = CreateCommand(cStr, con);
            try
            {
                cmd.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                throw (ex);
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }
        }

        public List<User> getListManager()
        {
            List<User> U = new List<User>();
            SqlConnection con = null;
            try
            {
                con = connect("DBConnectionString");
                String selectSTR = "select * from User_2020 where UserRole = 'מנהל יחידה ארגונית' or UserRole = 'מנהל מערך הדיור' or UserRole = 'סמנכלית' ";
                SqlCommand cmd = new SqlCommand(selectSTR, con);
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                while (dr.Read())
                {
                    User us = new User();
                    us.Firstname = (string)dr["FirstName"];
                    us.Lastname = (string)dr["LastName"];
                    us.Password = (string)dr["UPassword"];
                    us.Userid = (string)dr["UserId"];
                    us.Role = (string)dr["UserRole"];
                    us.Telephone = (string)dr["Telephone"];
                    us.Isactive = Convert.ToBoolean(dr["Active"]);
                    us.Birthdate = Convert.ToDateTime(dr["Birthdate"]).Date;
                    if (us.Role == "מנהל מערך הדיור" || us.Role == "סמנכלית")
                    {
                        us.Unitid = 0;
                    }
                    else
                    {
                        us.Unitid = Convert.ToInt32(dr["UnitId"]);
                    }
                    U.Add(us);
                }
                return U;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }
        }

        public void UpdateManagerDet(User u)
        {
            SqlConnection con;
            SqlCommand cmd;
            try
            {
                con = connect("DBConnectionString"); 
            }
            catch (Exception ex)
            {
                throw (ex);

            }
            int day = u.Birthdate.Day;
            int month = u.Birthdate.Month;
            int year = u.Birthdate.Year;
            string bdate = month.ToString() + "/" + day.ToString() + "/" + year.ToString();
            string active = u.Isactive.ToString();

            string cStr = "";
            if (u.Role == "מנהל יחידה ארגונית")
            {
                string unitID = u.Unitid.ToString();
                cStr = "UPDATE User_2020 SET UserId ='" + u.Userid + "' , FirstName = '" + u.Firstname + "' , LastName = '" + u.Lastname + "' , Birthdate = '" + bdate + "' , Telephone = '" + u.Telephone + "' , UPassword = '" + u.Password + "' , Active = '" + active + "' , UserRole = '" + u.Role + "', IsUnitManager = '1' , BigManager = '0' , UnitId = '" + unitID + "' WHERE UserId =" + u.Userid;

            }
            else if (u.Role == "מנהל מערך הדיור")
            {
                cStr = "UPDATE User_2020 SET UserId ='" + u.Userid + "' , FirstName = '" + u.Firstname + "' , LastName = '" + u.Lastname + "' , Birthdate = '" + bdate + "' , Telephone = '" + u.Telephone + "' , UPassword = '" + u.Password + "' , Active = '" + active + "' , UserRole = '" + u.Role + "' , IsUnitManager = '0' , BigManager = '1' WHERE UserId =" + u.Userid;

            }
            else if (u.Role == "סמנכלית	")
            {
                cStr = "UPDATE User_2020 SET UserId ='" + u.Userid + "' , FirstName = '" + u.Firstname + "' , LastName = '" + u.Lastname + "' , Birthdate = '" + bdate + "' , Telephone = '" + u.Telephone + "' , UPassword = '" + u.Password + "' , Active = '" + active + "' , UserRole = '" + u.Role + "' , IsUnitManager = '0' , BigManager = '0' WHERE UserId =" + u.Userid;

            }
            cmd = CreateCommand(cStr, con);
            try
            {
                cmd.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                throw (ex);
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }
        }

        public void updateOS(OfficialShift OS)
        {
            SqlConnection con = null;
            SqlCommand cmd;
            try
            {
                con = connect("DBConnectionString"); // create the connection
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            String cStr = BuildUpdateCommand(OS);
            cmd = CreateCommand(cStr, con);
            try
            {
                cmd.ExecuteNonQuery(); // execute the command
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }
        }
        private String BuildUpdateCommand(OfficialShift OS)
        {
            string u = OS.Userid.ToString();
            //convert start hour of shift to string
            string s = OS.Startshifthour.ToString();
            string[] DateTime = s.Split(' ');
            string strH = DateTime[1];
            //convert end hour of shift to string
            string e = OS.Endshifthour.ToString();
            string[] DateTime1 = e.Split(' ');
            string endH = DateTime1[1];
            string unit = OS.Unitid.ToString();
            int day = OS.Shiftdate.Day;
            int month = OS.Shiftdate.Month;
            int year = OS.Shiftdate.Year;
            string d = month.ToString() + "/" + day.ToString() + "/" + year.ToString();

            string str = "UPDATE OfficialShift_2020 SET UserId =" + u + ", StartShift ='" + strH + "', EndShift ='" + endH + "' WHERE UnitId = " + unit + " and ShiftType = '" + OS.Shifttype + "' and ShiftDate = '" + d + "'";
            return str;
        }

        public List<OfficialShift> GetEmptyOfficial(int unitid, string start, string end)
        {
            List<OfficialShift> OS = new List<OfficialShift>();
            SqlConnection con = null;
            string today = DateTime.Today.ToString("yyyy-MM-dd");

            try
            {
                con = connect("DBConnectionString");
                String selectSTR = "SELECT os.ShiftDate, sf.StartShift, sf.EndShift, os.ShiftType, sf.NumOfGuides,os.UserId";
                selectSTR += " FROM Shift_2020 as sf inner join OfficialShift_2020 as os on sf.ShiftType = os.ShiftType and sf.ShiftDate = os.ShiftDate";
                selectSTR += " WHERE('" + start + "'= sf.StartPeriod and '" + end + "' = sf.EndPeriod)";
                selectSTR += " AND (os.UserId = '666666666' or os.UserId = '777777777' or os.UserId = '888888888' or os.UserId = '999999999') AND(sf.UnitId = '" + unitid + "') AND (os.ShiftDate> '" + today + "')";
                SqlCommand cmd = new SqlCommand(selectSTR, con);
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                while (dr.Read())
                {
                    OfficialShift realshift = new OfficialShift();
                    realshift.Shiftdate = Convert.ToDateTime(dr["ShiftDate"]).Date;
                    realshift.Shifttype = Convert.ToString(dr["ShiftType"]);
                    TimeSpan myTimeSpan = ((dr).GetTimeSpan(dr.GetOrdinal("StartShift")));
                    realshift.Startshifthour = new DateTime(myTimeSpan.Ticks);
                    TimeSpan myTimeSpan2 = ((dr).GetTimeSpan(dr.GetOrdinal("EndShift")));
                    realshift.Endshifthour = new DateTime(myTimeSpan2.Ticks);
                    realshift.Numofguides = Convert.ToInt32(dr["NumOfGuides"]);
                    realshift.Userid = Convert.ToString(dr["UserId"]);
                    OS.Add(realshift);
                }
                return OS;

            }
            catch (Exception ex)
            {
                throw (ex);
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }
        }

        public List<Period> GetAllRelavnt(int unit)
        {
            List<Period> P = new List<Period>();
            SqlConnection con = null;
            string today = DateTime.Today.ToString("yyyy-MM-dd");

            try
            {
                con = connect("DBConnectionString");
                String selectSTR = "select StartPeriod, EndPeriod from SchedulingPeriod_2020";
                selectSTR += " where ('" + today + "' between StartPeriod and EndPeriod OR '" + today + "' < StartPeriod )";
                selectSTR += " AND UnitId = '" + unit + "'";
                SqlCommand cmd = new SqlCommand(selectSTR, con);
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                while (dr.Read())
                {
                    Period period = new Period();
                    period.Startdate = Convert.ToDateTime(dr["StartPeriod"]).Date;
                    period.Enddate = Convert.ToDateTime(dr["EndPeriod"]).Date;
                    P.Add(period);
                }
                return P;

            }
            catch (Exception ex)
            {
                throw (ex);
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }


        }
        public List<User> GetAvailableGuides(string shift)
        {
            List<User> G = new List<User>();
            SqlConnection con = null;
            string[] shiftdet = shift.Split('|');
            DateTime d = Convert.ToDateTime(shiftdet[0]);
            try
            {
                con = connect("DBConnectionString");
                String selectSTR = CreateGetCommand(shift);
                SqlCommand cmd = new SqlCommand(selectSTR, con);
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                while (dr.Read())
                {
                    User u = new User();
                    u.Userid = Convert.ToString(dr["UserId"]);
                    u.Firstname = Convert.ToString(dr["FirstName"]) + " " + Convert.ToString(dr["LastName"]);
                    u.Telephone = Convert.ToString(dr["Telephone"]);
                    u.Role = Convert.ToString(dr["TrainingLevel"]);
                    u.Password = Convert.ToString(dr["UnitName"]);
                    u.Unitid = Convert.ToInt32(dr["UnitId"]);
                    bool p = GetPer(u.Unitid);
                    User u1 = GuideInfo(u.Userid, p);
                    u.Weeklyhours =
                    u.MonthlyHours = u1.MonthlyHours;
                    u.MonthlyExtraHours = u1.MonthlyExtraHours;

                    G.Add(u);
                }

                return G;

            }
            catch (Exception ex)
            {
                throw (ex);
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }

        }
        private string CreateGetCommand(string shift)
        {
            string[] shiftdet = shift.Split('|');
            string command = "";
            DateTime d = Convert.ToDateTime(shiftdet[0]);
            string tommorow = d.AddDays(1).ToString("yyyy-MM-dd");
            string yesterday = d.AddDays(-1).ToString("yyyy-MM-dd");
            if (shiftdet[1] == "בוקר")
            {
                command = "select User_2020.UserId, User_2020.FirstName,User_2020.LastName,User_2020.Telephone, TrainingLevel_2020.TrainingLevel,OrganizeUnit_2020.UnitName,User_2020.UnitId ";
                command += " from User_2020 inner join Guide_2020 on User_2020.UserId = Guide_2020.UserId inner join TrainingLevel_2020 on Guide_2020.TrainingLevelId = TrainingLevel_2020.TrainingLevelId inner join OrganizeUnit_2020 on User_2020.UnitId = OrganizeUnit_2020.UnitId";
                command += " where User_2020.UserId not in(";
                command += " select UserId";
                command += " from OfficialShift_2020";
                command += " where(ShiftType = '" + shiftdet[1] + "' and ShiftDate = '" + shiftdet[0] + "') or (ShiftType = 'ערב' and ShiftDate = '" + shiftdet[0] + "') or (ShiftType = 'לילה' and ShiftDate = '" + yesterday + "'))";
                command += "  and UserRole = 'מדריך' and Active = '1'";
                return command;
            }
            else if (shiftdet[1] == "ערב")
            {
                command = "select User_2020.UserId, User_2020.FirstName,User_2020.LastName,User_2020.Telephone, TrainingLevel_2020.TrainingLevel,OrganizeUnit_2020.UnitName,User_2020.UnitId ";
                command += " from User_2020 inner join Guide_2020 on User_2020.UserId = Guide_2020.UserId inner join TrainingLevel_2020 on Guide_2020.TrainingLevelId = TrainingLevel_2020.TrainingLevelId inner join OrganizeUnit_2020 on User_2020.UnitId = OrganizeUnit_2020.UnitId";
                command += " where User_2020.UserId not in(";
                command += " select UserId";
                command += " from OfficialShift_2020";
                command += " where(ShiftType = '" + shiftdet[1] + "' and ShiftDate = '" + shiftdet[0] + "') or (ShiftType = 'בוקר' and ShiftDate = '" + shiftdet[0] + "') or (ShiftType = 'לילה' and ShiftDate = '" + shiftdet[0] + "'))";
                command += "  and UserRole = 'מדריך' and Active = '1'";
                return command;
            }
            else if (shiftdet[1] == "לילה")
            {
                command = "select User_2020.UserId, User_2020.FirstName,User_2020.LastName,User_2020.Telephone, TrainingLevel_2020.TrainingLevel,OrganizeUnit_2020.UnitName,User_2020.UnitId ";
                command += " from User_2020 inner join Guide_2020 on User_2020.UserId = Guide_2020.UserId inner join TrainingLevel_2020 on Guide_2020.TrainingLevelId = TrainingLevel_2020.TrainingLevelId inner join OrganizeUnit_2020 on User_2020.UnitId = OrganizeUnit_2020.UnitId";
                command += " where User_2020.UserId not in(";
                command += " select UserId";
                command += " from OfficialShift_2020";
                command += " where(ShiftType = '" + shiftdet[1] + "' and ShiftDate = '" + shiftdet[0] + "') or (ShiftType = 'ערב' and ShiftDate = '" + shiftdet[0] + "') or(ShiftType = 'בוקר' and ShiftDate = '" + tommorow + "'))";
                command += "  and UserRole = 'מדריך' and Active = '1'";
                return command;
            }

            return command;

        }

        public double GetWeeklyHours(string userId, DateTime date)
        {
            string sunday = date.AddDays(-(int)date.DayOfWeek).ToString("yyyy-MM-dd");
            string saturday = date.AddDays(DayOfWeek.Saturday - date.DayOfWeek).ToString("yyyy-MM-dd");
            double weeklyhours = 0;
            SqlConnection con = null;
            try
            {
                con = connect("DBConnectionString");
                string selectSTR = "select * from OfficialShift_2020 where UserId = '" + userId + "'";
                selectSTR += " and (ShiftDate between '" + sunday + "' and '" + saturday + "')";
                SqlCommand cmd = new SqlCommand(selectSTR, con);
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                while (dr.Read())
                {
                    OfficialShift OffiS = new OfficialShift();
                    TimeSpan myTimeSpan = ((dr).GetTimeSpan(dr.GetOrdinal("StartShift")));
                    OffiS.Startshifthour = new DateTime(myTimeSpan.Ticks);
                    myTimeSpan = ((dr).GetTimeSpan(dr.GetOrdinal("EndShift")));
                    OffiS.Endshifthour = new DateTime(myTimeSpan.Ticks);
                    TimeSpan interval = OffiS.Endshifthour - OffiS.Startshifthour;
                    double x = interval.TotalHours;
                    double x1 = 0.0;
                    if (x < 0)
                    {
                        x1 = x + 24.0;
                    }
                    else if (x > 0)
                    {
                        x1 = x;
                    }
                    weeklyhours += x1;

                }
                return weeklyhours;

            }

            catch (Exception ex)
            {
                throw (ex);
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }

        }

        public bool CheckLongBreak(OfficialShift o1)
        {
            string sunday = o1.Shiftdate.AddDays(-(int)o1.Shiftdate.DayOfWeek).ToString("yyyy-MM-dd");
            string saturday = o1.Shiftdate.AddDays(DayOfWeek.Saturday - o1.Shiftdate.DayOfWeek).ToString("yyyy-MM-dd");
            List<Constraint> ConstList = getConstraintM();
            double BreakTime = ConstList[3].ConstraintValue;
            bool HasABreak = false;
            List<OfficialShift> shifts = new List<OfficialShift>();
            SqlConnection con = null;
            try
            {
                con = connect("DBConnectionString");
                string selectSTR = "select * from OfficialShift_2020 where UserId = '" + o1.Userid + "'";
                selectSTR += " and (ShiftDate between '" + sunday + "' and '" + saturday + "')";
                SqlCommand cmd = new SqlCommand(selectSTR, con);
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                while (dr.Read())
                {
                    OfficialShift OffiS = new OfficialShift();

                    OffiS.Shiftdate = Convert.ToDateTime(dr["ShiftDate"]).Date;
                    TimeSpan myTimeSpan = ((dr).GetTimeSpan(dr.GetOrdinal("StartShift")));
                    OffiS.Startshifthour = new DateTime(myTimeSpan.Ticks);
                    OffiS.Startshifthour = new DateTime(OffiS.Shiftdate.Year, OffiS.Shiftdate.Month, OffiS.Shiftdate.Day, OffiS.Startshifthour.Hour, OffiS.Startshifthour.Minute, OffiS.Startshifthour.Second);
                    myTimeSpan = ((dr).GetTimeSpan(dr.GetOrdinal("EndShift")));
                    OffiS.Endshifthour = new DateTime(myTimeSpan.Ticks);
                    OffiS.Endshifthour = new DateTime(OffiS.Shiftdate.Year, OffiS.Shiftdate.Month, OffiS.Shiftdate.Day, OffiS.Endshifthour.Hour, OffiS.Endshifthour.Minute, OffiS.Endshifthour.Second);
                    shifts.Add(OffiS);
                }
                o1.Startshifthour = new DateTime(o1.Shiftdate.Year, o1.Shiftdate.Month, o1.Shiftdate.Day, o1.Startshifthour.Hour, o1.Startshifthour.Minute, o1.Startshifthour.Second);
                o1.Endshifthour = new DateTime(o1.Shiftdate.Year, o1.Shiftdate.Month, o1.Shiftdate.Day, o1.Endshifthour.Hour, o1.Endshifthour.Minute, o1.Endshifthour.Second);
                shifts.Add(o1);
                shifts = shifts.OrderBy(p => p.Startshifthour).ToList(); ;
                for (int i = 0; i < shifts.Count; i++)
                {
                    double x = 0;
                    if (i == 0)
                    {
                        DateTime sunday1 = o1.Shiftdate.AddDays(-(int)o1.Shiftdate.DayOfWeek);
                        sunday1 = new DateTime(sunday1.Year, sunday1.Month, sunday1.Day, 00, 00, 00);
                        TimeSpan interval = shifts[i].Startshifthour - sunday1;
                        x = interval.TotalHours;
                        if (x >= BreakTime)
                        {
                            HasABreak = true;
                            return HasABreak;
                        }

                    }
                    else if (i > 0 && i < (shifts.Count) - 1)
                    {
                        TimeSpan interval = shifts[i + 1].Startshifthour - shifts[i].Endshifthour;
                        x = interval.TotalHours;
                        if (x >= BreakTime)
                        {
                            HasABreak = true;
                            return HasABreak;

                        }
                    }
                    else if (i == (shifts.Count) - 1)
                    {
                        DateTime saturday1 = o1.Shiftdate.AddDays(DayOfWeek.Saturday - o1.Shiftdate.DayOfWeek);
                        saturday1 = new DateTime(saturday1.Year, saturday1.Month, saturday1.Day, 23, 59, 00);
                        TimeSpan interval = saturday1 - shifts[i].Endshifthour;
                        x = interval.TotalHours;
                        if (x >= BreakTime)
                        {
                            HasABreak = true;
                            return HasABreak;
                        }
                    }


                }
                return HasABreak;

            }

            catch (Exception ex)
            {
                throw (ex);
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }

        }

        public void PutMish(OfficialShift os, string idbefore)
        {
            checkExcep(os, idbefore);
            SqlConnection con;
            SqlCommand cmd;
            try
            {
                con = connect("DBConnectionString");
            }
            catch (Exception ex)
            {
                throw (ex);

            }
            string s = os.Startshifthour.ToString();
            string[] DateTime = s.Split(' ');
            string strH = DateTime[1];
            string s1 = os.Endshifthour.ToString();
            string[] DateTime1 = s1.Split(' ');
            string endH = DateTime1[1];
            string dateShif = os.Shiftdate.ToString("yyyy-MM-dd");
            string cStr = "update OfficialShift_2020 SET UserId='" + os.Userid + "' , StartShift='" + strH + "', EndShift='" + endH + "'";
            cStr += " WHERE ShiftDate='" + dateShif + "' and ShiftType='" + os.Shifttype + "' and UserId='" + idbefore + "'";
            cmd = CreateCommand(cStr, con);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }
        }


        public void UpdateOrganizeUnit(OrganizeUnit OrUn)
        {
            SqlConnection con;
            SqlCommand cmd;

            try
            {
                con = connect("DBConnectionString");
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            String cStr = BuildPutCommandSale(OrUn);
            cmd = CreateCommand(cStr, con);
            try
            {
                cmd.ExecuteNonQuery();

            }
            catch (Exception ex)
            {

                throw (ex);
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }
            if (OrUn.Unittype == "דירה")
            {
                UpdateApartmentType(OrUn.ApartmentType1, OrUn.Id);

            }
        }

        private String BuildPutCommandSale(OrganizeUnit OrgU)
        {
            string id = OrgU.Id.ToString();
            string N_Guides = OrgU.Numofguides.ToString();
            string N_Res = OrgU.Numofresidents.ToString();
            string A_Type = OrgU.Unittype.ToString();
            String prefix = "UPDATE OrganizeUnit_2020 SET [UnitName] = '" + OrgU.Unitname + "', [City] =  '" + OrgU.City + "', [Street_HNumber] = '" + OrgU.Street_hnumber + "', [NumOfGuides] = '" + N_Guides + "', [NumOfResidents] = '" + N_Res + "', [UnitType] = '" + A_Type + "' WHERE [UnitId] = " + id + "";
            return prefix;

        }

        public List<Shift> GetShifByPer(int Uid, DateTime SPdate)
        {
            List<Shift> S = new List<Shift>();
            SqlConnection con = null;
            //string today = DateTime.Today.ToString("yyyy-MM-dd");
            string unitId = Uid.ToString();
            int day = SPdate.Day;
            int month = SPdate.Month;
            int year = SPdate.Year;
            string startP = month.ToString() + "/" + day.ToString() + "/" + year.ToString();

            try
            {
                con = connect("DBConnectionString");
                String selectSTR = "select * from Shift_2020 where UnitId = " + unitId + " AND StartPeriod = '" + startP + "'";
                SqlCommand cmd = new SqlCommand(selectSTR, con);
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                while (dr.Read())
                {
                    Shift shist = new Shift();
                    shist.Startperiod = Convert.ToDateTime(dr["StartPeriod"]).Date;
                    shist.Endperiod = Convert.ToDateTime(dr["EndPeriod"]).Date;
                    shist.Shiftdate = Convert.ToDateTime(dr["ShiftDate"]).Date;
                    shist.Type = Convert.ToString(dr["ShiftType"]);
                    shist.GuideNum1 = Convert.ToInt32(dr["NumOfGuides"]);
                    shist.Uid = Convert.ToInt32(dr["NumOfGuides"]);
                    TimeSpan myTimeSpan = ((dr).GetTimeSpan(dr.GetOrdinal("StartShift")));
                    shist.Start = new DateTime(myTimeSpan.Ticks);
                    myTimeSpan = ((dr).GetTimeSpan(dr.GetOrdinal("EndShift")));
                    //shist.Start = TimeSpan(dr["StartShift"]);
                    shist.End = new DateTime(myTimeSpan.Ticks);
                    S.Add(shist);
                }
                return S;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }
        }

        public void updateSI(Shift ShiftInfo)
        {
            SqlConnection con;
            SqlCommand cmd;
            try
            {
                con = connect("DBConnectionString"); // create the connection
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            String cStr = BuildUpdateCommand(ShiftInfo);
            cmd = CreateCommand(cStr, con);
            try
            {
                cmd.ExecuteNonQuery(); // execute the command
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }
        }
        private String BuildUpdateCommand(Shift shift)
        {
            string Shiftdate;
            //convert start date of period to string
            int day = shift.Startperiod.Day;
            int month = shift.Startperiod.Month;
            int year = shift.Startperiod.Year;
            string startsPD = month.ToString() + "/" + day.ToString() + "/" + year.ToString();
            //convert end date of period to string
            int Eday = shift.Endperiod.Day;
            int Emonth = shift.Endperiod.Month;
            int Eyear = shift.Endperiod.Year;
            string EndPD = Emonth.ToString() + "/" + Eday.ToString() + "/" + Eyear.ToString();
            //convert start hour of shift to string
            string s = shift.Start.ToString();
            string[] DateTime = s.Split(' ');
            string strH = DateTime[1];
            //convert end hour of shift to string
            string e = shift.End.ToString();
            string[] DateTime1 = e.Split(' ');
            string endH = DateTime1[1];
            //convert unit id to string
            string uid = shift.Uid.ToString();
            //convert date of shift to string
            day = shift.Shiftdate.Day;
            month = shift.Shiftdate.Month;
            year = shift.Shiftdate.Year;
            Shiftdate = month.ToString() + "/" + day.ToString() + "/" + year.ToString();
            //convert number of guides on shift to string
            string num = shift.GuideNum1.ToString();

            string str = "UPDATE Shift_2020 SET StartShift ='" + strH + "',EndShift ='" + endH + "',NumOfGuides ='" + num + "' WHERE ShiftType ='" + shift.Type + "' and UnitId = " + uid + " and StartPeriod = '" + startsPD + "' and EndPeriod = '" + EndPD + "' and ShiftDate = '" + Shiftdate + "'";
            return str;
        }

        public List<Exceptions> GetSpecialExcep(DateTime start, DateTime end, int unitid)
        {
            List<Exceptions> S = new List<Exceptions>();
            SqlConnection con = null;

            string unit = unitid.ToString();
            string startP = start.ToString("yyyy-MM-dd");
            string endp = end.ToString("yyyy-MM-dd");
            try
            {
                con = connect("DBConnectionString");

                String selectSTR = "select Exception_2020.ShiftDate,Exception_2020.ShiftType,TypeofException_2020.TypeofException,User_2020.FirstName,User_2020.LastName from Exception_2020 right join TypeofException_2020 on Exception_2020.TypeofExceptionId = TypeofException_2020.TypeofExceptionId inner";
                selectSTR += " join User_2020 on Exception_2020.UserId = User_2020.UserId";
                selectSTR += " where Exception_2020.UnitId = '" + unit + "' and Exception_2020.ShiftDate between '" + startP + "' and '" + endp + "'";
                SqlCommand cmd = new SqlCommand(selectSTR, con);
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                while (dr.Read())
                {
                    Exceptions EX = new Exceptions();
                    EX.Name = Convert.ToString(dr["FirstName"]) + " " + Convert.ToString(dr["LastName"]);
                    EX.ShiftDate = Convert.ToDateTime(dr["ShiftDate"]).Date;
                    EX.ShiftType = Convert.ToString(dr["ShiftType"]);
                    EX.Index = Convert.ToString(dr["TypeofException"]);

                    S.Add(EX);
                }
                return S;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }


        }

        public List<Exceptions> GetExceptionsNum(DateTime start, DateTime end, int unitid)
        {
            List<Exceptions> S = new List<Exceptions>();
            SqlConnection con = null;

            string unit = unitid.ToString();
            string startP = start.ToString("yyyy-MM-dd");
            string endp = end.ToString("yyyy-MM-dd");
            try
            {
                con = connect("DBConnectionString");
                String selectSTR = "Select count(Exception_2020.TypeofExceptionId)as NumOfEx, TypeofException_2020.TypeofException";
                selectSTR += "  from Exception_2020 inner join TypeofException_2020 on Exception_2020.TypeofExceptionId = TypeofException_2020.TypeofExceptionId";
                selectSTR += " where Exception_2020.UnitId = '" + unit + "' and Exception_2020.ShiftDate between '" + startP + "' and '" + endp + "'";
                selectSTR += " GROUP BY TypeofException_2020.TypeofException";
                SqlCommand cmd = new SqlCommand(selectSTR, con);
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                while (dr.Read())
                {
                    Exceptions EX = new Exceptions();
                    EX.Unit = Convert.ToInt32(dr["NumOfEx"]);
                    EX.Index = Convert.ToString(dr["TypeofException"]);

                    S.Add(EX);
                }
                return S;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }


        }

        public List<User> GetGuidesHours(DateTime start, DateTime end, int unit)
        {

            List<User> G = new List<User>();
            SqlConnection con = null;
            string unitid = unit.ToString();

            try
            {
                con = connect("DBConnectionString");
                String selectSTR = "select * from User_2020 where UserRole = 'מדריך' and Active='1' and UnitId='" + unitid + "'";
                SqlCommand cmd = new SqlCommand(selectSTR, con);
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                while (dr.Read())
                {
                    User u = new User();
                    u.Userid = Convert.ToString(dr["UserId"]);
                    u.Firstname = Convert.ToString(dr["FirstName"]) + " " + Convert.ToString(dr["LastName"]);
                    u.Telephone = Convert.ToString(dr["Telephone"]);
                    //u.Role = Convert.ToString(dr["TrainingLevel"]);
                    bool p = GetPer(unit);
                    User u1 = GuideInfoByDates(u.Userid, p, start, end);
                    u.MonthlyHours = u1.MonthlyHours;
                    u.MonthlyExtraHours = u1.MonthlyExtraHours;

                    G.Add(u);
                }

                return G;

            }
            catch (Exception ex)
            {
                throw (ex);
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }



        }
        public User GuideInfoByDates(string Userid, bool preprdness, DateTime start, DateTime end)
        {
            User Os = new User();
            double extra = 0;
            double normal = 0;
            SqlConnection con = null;
            List<Constraint> ConstList = getConstraintM();
            float RegularShift = ConstList[6].ConstraintValue;
            float SpeNightShift = ConstList[7].ConstraintValue;
            string startP = start.ToString("yyyy-MM-dd");
            string endp = end.ToString("yyyy-MM-dd");
            try
            {
                string sMonth = DateTime.Now.ToString("MM");
                string year = DateTime.Today.Year.ToString();

                con = connect("DBConnectionString");

                String selectSTR = "select * from OfficialShift_2020 where UserId = '" + Userid + "' and ShiftDate between '" + startP + "' and '" + endp + "'";
                SqlCommand cmd = new SqlCommand(selectSTR, con);
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                while (dr.Read())
                {
                    OfficialShift OffiS = new OfficialShift();
                    OffiS.Shifttype = (string)dr["ShiftType"];
                    TimeSpan myTimeSpan = ((dr).GetTimeSpan(dr.GetOrdinal("StartShift")));
                    OffiS.Startshifthour = new DateTime(myTimeSpan.Ticks);
                    myTimeSpan = ((dr).GetTimeSpan(dr.GetOrdinal("EndShift")));
                    OffiS.Endshifthour = new DateTime(myTimeSpan.Ticks);
                    TimeSpan interval = OffiS.Endshifthour - OffiS.Startshifthour;
                    double x = interval.TotalHours;
                    if (x < 0)
                    {
                        x = x + 24.0;
                    }
                    if (preprdness == true && OffiS.Shifttype == "לילה")
                    {
                        double x1 = x;
                     
                        if (x1 > SpeNightShift)
                        {
                            normal += SpeNightShift;
                            extra += x1 - SpeNightShift;
                        }
                        else
                        {
                            normal += x1;
                        }
                    }
                    else
                    {
                        if (x > RegularShift)
                        {
                            normal += RegularShift;
                            extra += x - RegularShift;
                        }
                        else
                        {
                            normal += x;
                        }
                    }
                    Os.MonthlyHours = normal;
                    Os.MonthlyExtraHours = extra;
                }
                return Os;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }
        }

        public List<OfficialShift> GetallShifts(DateTime start, DateTime end, int unit)
        {
            List<OfficialShift> OS = new List<OfficialShift>();
            SqlConnection con = null;
            string today = DateTime.Today.ToString("yyyy-MM-dd");
            string start1 = start.ToString("yyyy-MM-dd");
            string end1 = end.ToString("yyyy-MM-dd");
            string unitid = unit.ToString();
            try
            {
                con = connect("DBConnectionString");

                String selectSTR = "SELECT os.ShiftDate, sf.StartShift, sf.EndShift, os.ShiftType,os.UserId,U.FirstName,U.LastName";
                selectSTR += " FROM Shift_2020 as sf inner join OfficialShift_2020 as os on sf.ShiftType = os.ShiftType and sf.ShiftDate = os.ShiftDate inner join User_2020 as U on os.UserId = U.UserId";
                selectSTR += " WHERE('" + start1 + "'= sf.StartPeriod and '" + end1 + "' = sf.EndPeriod)";
                selectSTR += " AND (sf.UnitId = '" + unitid + "')";
                SqlCommand cmd = new SqlCommand(selectSTR, con);
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                while (dr.Read())
                {
                    OfficialShift realshift = new OfficialShift();
                    realshift.Shiftdate = Convert.ToDateTime(dr["ShiftDate"]).Date;
                    realshift.Shifttype = Convert.ToString(dr["ShiftType"]);
                    TimeSpan myTimeSpan = ((dr).GetTimeSpan(dr.GetOrdinal("StartShift")));
                    realshift.Startshifthour = new DateTime(myTimeSpan.Ticks);
                    TimeSpan myTimeSpan2 = ((dr).GetTimeSpan(dr.GetOrdinal("EndShift")));
                    realshift.Endshifthour = new DateTime(myTimeSpan2.Ticks);
                    realshift.Userid = Convert.ToString(dr["UserId"]);
                    realshift.Name = Convert.ToString(dr["FirstName"]) + " " + Convert.ToString(dr["LastName"]);
                    OS.Add(realshift);
                }
                return OS;

            }
            catch (Exception ex)
            {
                throw (ex);
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }
        }

        private void checkExcep(OfficialShift os, string idbefore)
        {
            SqlConnection con;
            SqlCommand cmd;
            try
            {
                con = connect("DBConnectionString");
            }
            catch (Exception ex)
            {
                throw (ex);

            }
            string dateShif = os.Shiftdate.ToString("yyyy-MM-dd");
            string unit = os.Unitid.ToString();

            string cStr = " DELETE FROM Exception_2020 WHERE  UserId = '" + idbefore + "' and UnitId = '" + unit + "' and ShiftDate = '" + dateShif + "' and ShiftType = '" + os.Shifttype + "'";
            cmd = CreateCommand(cStr, con);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }
        }
        public List<Exceptions> GetExceptions(int unitid, DateTime dt)
        {
            List<Exceptions> EXC = new List<Exceptions>();
            SqlConnection con = null;

            string unit = unitid.ToString();
            string startP = dt.ToString("yyyy-MM-dd");
            try
            {
                con = connect("DBConnectionString");
                String selectSTR = "select Exception_2020.TypeofExceptionId, Exception_2020.ShiftDate, Exception_2020.ShiftType, Exception_2020.UnitId, Exception_2020.UserId";
                selectSTR += " from Exception_2020 left join Shift_2020 on Exception_2020.ShiftDate = Shift_2020.ShiftDate and Exception_2020.ShiftType = Shift_2020.ShiftType";
                selectSTR += " where Exception_2020.UnitId = '" + unit + "' and Exception_2020.ShiftDate >= '" + startP + "'";
                SqlCommand cmd = new SqlCommand(selectSTR, con);
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                while (dr.Read())
                {
                    Exceptions EX = new Exceptions();
                    EX.User = Convert.ToInt32(dr["UserId"]);
                    EX.ShiftDate = Convert.ToDateTime(dr["ShiftDate"]).Date;
                    EX.ShiftType = Convert.ToString(dr["ShiftType"]);
                    EX.Index = Convert.ToString(dr["TypeofException"]);
                    EXC.Add(EX);
                }
                return EXC;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }
        }
        //public void DeleteException(Exceptions Excpt)
        //{
        //    SqlConnection con;
        //    SqlCommand cmd;
        //    try
        //    {
        //        con = connect("DBConnectionString");
        //    }
        //    catch (Exception ex)
        //    {
        //        throw (ex);
        //    }
        //    string date = Excpt.ShiftDate.ToString("yyyy-MM-dd");
        //    string unit = Excpt.ToString();
        //    string us = Excpt.User.ToString();
        //    string index = Excpt.Index.ToString();
        //    string cStr = "DELETE FROM Exception_2020 WHERE  UserId = '" + us + "' and UnitId = '" + unit + "' and ShiftDate = '" + date + "' and ShiftType = '" + Excpt.ShiftType + "' and TypeofExceptionId = '" + index + "'";
        //    cmd = CreateCommand(cStr, con);
        //    try
        //    {
        //        cmd.ExecuteNonQuery();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw (ex);
        //    }
        //    finally
        //    {
        //        if (con != null)
        //        {
        //            con.Close();
        //        }
        //    }
        //}
        //public void UpdateException(Exceptions Excpt)
        //{
        //    SqlConnection con;
        //    SqlCommand cmd;
        //    string ShiftDate = "";
        //    string UsId = Excpt.User.ToString();
        //    string Unid = Excpt.Unit.ToString();
        //    int indx = GetExceptionType(Excpt.Index);
        //    string index = indx.ToString();
        //    if (Excpt.ShiftDate != null)
        //    {
        //        int day = Excpt.ShiftDate.Day;
        //        int month = Excpt.ShiftDate.Month;
        //        int year = Excpt.ShiftDate.Year;
        //        ShiftDate = month.ToString() + "/" + day.ToString() + "/" + year.ToString();
        //    }
        //    try
        //    {
        //        con = connect("DBConnectionString"); // create the connection
        //    }
        //    catch (Exception ex)
        //    {
        //        throw (ex);
        //    }
        //    string cStr = "UPDATE Exception_2020 SET UserId =" + UsId + ",TypeofExceptionId ='" + index + "' WHERE UnitId ='" + Unid + "' and ShiftDate = '" + ShiftDate + "' and ShiftType = '" + Excpt.ShiftType + "'";
        //    cmd = CreateCommand(cStr, con);
        //    try
        //    {
        //        cmd.ExecuteNonQuery(); // execute the command
        //    }
        //    catch (Exception ex)
        //    {
        //        throw (ex);
        //    }
        //    finally
        //    {
        //        if (con != null)
        //        {
        //            con.Close();
        //        }
        //    }
        //}
        public List<Period> GetHistoryPeriod(int unit)
        {
            List<Period> P = new List<Period>();
            SqlConnection con = null;
            int todayMonth = DateTime.Today.Month - 2;
            string day = "01";
            string month = todayMonth.ToString("00");
            int year = DateTime.Today.Year;
            string TwoMonthBefore = year.ToString() + "-" + month + "-" + day;
            string today = DateTime.Today.ToString("yyyy-MM-dd");
            string unitid = unit.ToString();
            try
            {
                con = connect("DBConnectionString");
                String selectSTR = "select * from SchedulingPeriod_2020 where StartPeriod > '" + TwoMonthBefore + "' and EndPeriod<'" + today + "' and UnitId='" + unitid + "'";
                SqlCommand cmd = new SqlCommand(selectSTR, con);
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                while (dr.Read())
                {
                    Period period = new Period();
                    period.Startdate = Convert.ToDateTime(dr["StartPeriod"]).Date;
                    period.Enddate = Convert.ToDateTime(dr["EndPeriod"]).Date;
                    P.Add(period);
                }
                return P;

            }
            catch (Exception ex)
            {
                throw (ex);
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }


        }

        public List<OfficialShift> GetWorkSchdual(int unit)
        {
            List<OfficialShift> OS = new List<OfficialShift>();
            SqlConnection con = null;
            string today = DateTime.Today.ToString("yyyy-MM-dd");

            string unitid = unit.ToString();
            try
            {
                con = connect("DBConnectionString");
                String selectSTR = "select OfficialShift_2020.EndShift,OfficialShift_2020.ShiftDate,OfficialShift_2020.ShiftType,OfficialShift_2020.StartShift,OfficialShift_2020.UserId, User_2020.FirstName,User_2020.LastName from OfficialShift_2020 inner join Shift_2020 on OfficialShift_2020.ShiftDate = Shift_2020.ShiftDate and OfficialShift_2020.ShiftType = Shift_2020.ShiftType";
                selectSTR += " inner join User_2020 on OfficialShift_2020.UserId= User_2020.UserId where (('" + today + "' between Shift_2020.StartPeriod and Shift_2020.EndPeriod) or '" + today + "' < Shift_2020.StartPeriod) and OfficialShift_2020.UnitId = '" + unitid + "'";
                SqlCommand cmd = new SqlCommand(selectSTR, con);
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                while (dr.Read())
                {
                    OfficialShift realshift = new OfficialShift();
                    realshift.Shiftdate = Convert.ToDateTime(dr["ShiftDate"]).Date;
                    realshift.Shifttype = Convert.ToString(dr["ShiftType"]);
                    TimeSpan myTimeSpan = ((dr).GetTimeSpan(dr.GetOrdinal("StartShift")));
                    realshift.Startshifthour = new DateTime(myTimeSpan.Ticks);
                    TimeSpan myTimeSpan2 = ((dr).GetTimeSpan(dr.GetOrdinal("EndShift")));
                    realshift.Endshifthour = new DateTime(myTimeSpan2.Ticks);
                    realshift.Userid = Convert.ToString(dr["UserId"]);
                    realshift.Name = Convert.ToString(dr["FirstName"]) + " " + Convert.ToString(dr["LastName"]);
                    OS.Add(realshift);
                }
                return OS;

            }
            catch (Exception ex)
            {
                throw (ex);
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }
        }

        public List<Period> getAllPeriods(int unit)
        {
            List<Period> P = new List<Period>();
            SqlConnection con = null;

            DateTime TodayYearAgo = DateTime.Now.AddYears(-1);
            string today = TodayYearAgo.ToString("yyyy-MM-dd");
            string unitid = unit.ToString();
            try
            {
                con = connect("DBConnectionString");
                String selectSTR = "select * from SchedulingPeriod_2020 where '" + today + "' < StartPeriod and UnitId='" + unitid + "'";
                SqlCommand cmd = new SqlCommand(selectSTR, con);
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                while (dr.Read())
                {
                    Period period = new Period();
                    period.Startdate = Convert.ToDateTime(dr["StartPeriod"]).Date;
                    period.Enddate = Convert.ToDateTime(dr["EndPeriod"]).Date;
                    P.Add(period);
                }
                return P;

            }
            catch (Exception ex)
            {
                throw (ex);
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }


        }

        public List<OfficialShift> getShiftFromAllOrgan(int unit, string id)
        {

            List<OfficialShift> OS = new List<OfficialShift>();
            SqlConnection con = null;
            string today = DateTime.Today.ToString("yyyy-MM-dd");

            string unitid = unit.ToString();
            try
            {
                
                con = connect("DBConnectionString");
                String selectSTR = "select OfficialShift_2020.EndShift,OfficialShift_2020.ShiftDate,OfficialShift_2020.ShiftType,OfficialShift_2020.StartShift,OrganizeUnit_2020.UnitName FROM OfficialShift_2020 inner join OrganizeUnit_2020 on OfficialShift_2020.UnitId =OrganizeUnit_2020.UnitId WHERE UserId = '" + id+"' and ShiftDate> '"+ today+ "' and OfficialShift_2020.UnitId<>'" + unitid+"'";
               SqlCommand cmd = new SqlCommand(selectSTR, con);
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                while (dr.Read())
                {
                    OfficialShift realshift = new OfficialShift();
                    realshift.Shiftdate = Convert.ToDateTime(dr["ShiftDate"]).Date;
                    realshift.Shifttype = Convert.ToString(dr["ShiftType"]);
                    TimeSpan myTimeSpan = ((dr).GetTimeSpan(dr.GetOrdinal("StartShift")));
                    realshift.Startshifthour = new DateTime(myTimeSpan.Ticks);
                    TimeSpan myTimeSpan2 = ((dr).GetTimeSpan(dr.GetOrdinal("EndShift")));
                    realshift.Endshifthour = new DateTime(myTimeSpan2.Ticks);    
                    realshift.Name= Convert.ToString(dr["UnitName"]);
                    OS.Add(realshift);
                }
                return OS;

            }
            catch (Exception ex)
            {
                throw (ex);
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }
        }


    }
}

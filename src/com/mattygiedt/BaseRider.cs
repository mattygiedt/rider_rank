using System;
using System.IO;
using System.Windows;
using System.Security;
using System.Threading;
using System.Configuration;
using System.ComponentModel;
using System.Security.Permissions;

namespace com.mattygiedt
{
    public abstract class BaseRider
    {
        protected string points;
        protected string name;
        protected string first_name;
        protected string last_name;
        protected string city;
        protected string state;
        protected string license;
        protected string age;
        protected string gender;
        protected string discipline;
        protected string category;
        protected string entry_date;
        protected string team;
        protected string message;

        public string Points { get { return points; } }
        public string Name { get { return FirstName + " " + LastName; } }
        public string FirstName { get { return first_name; } }
        public string LastName { get { return last_name; } }
        public string Discipline { get { return discipline; } }
        public string License { get { return license; }
                                set { license = value; } }
        public string EntryDate { get { return entry_date; }
                                  set { entry_date = value; } }
        public string Message { get { return message; }
                                set { message = value; } }

        public abstract BaseRider CloneRider();
        public abstract string ToDataCSV( );
        public abstract string ToRankCSV( );
        public abstract string RiderType( );

        protected void Init()
        {
            points = "";
            name = "";
            first_name = "";
            last_name = "";
            city = "";
            state = "";
            license = "";
            age = "";
            gender = "";
            discipline = "";
            category = "";
            entry_date = "";
            team = "";
            message = "";
        }
    }
}

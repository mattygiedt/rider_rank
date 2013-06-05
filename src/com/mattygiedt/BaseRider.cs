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
        protected string license2;
        protected string uci_code;
        protected string age;
        protected string gender;
        protected string discipline;
        protected string category;
        protected string category_entered;
        protected string entry_date;
        protected string team;
        protected string message;
        protected string email;
        protected string racer_id;
        protected string rank;

        protected string [] data;

        public string Name { get { return FirstName + " " + LastName; } }
        public string FirstName { get { return first_name; } }
        public string LastName { get { return last_name; } }
        public string Discipline { get { return discipline; } }

        public string License { get { return license; }
                                set { license = value; } }
        public string ManualLicense { get { return license2; }
                                      set { license2 = value; } }
        public string EntryDate { get { return entry_date; }
                                  set { entry_date = value; } }
        public string Message { get { return message; }
                                set { message = value; } }
        public string Points { get { return points; }
                               set { points = value; } }
        public string RacerID { get { return racer_id; }
                                set { racer_id = value; } }
        public string Rank { get { return rank; }
                             set { rank = value; } }

        public abstract BaseRider CloneRider();
        public abstract string ToDataCSV( );
        public abstract string ToRankCSV( );
        public abstract string RiderType( );
        public abstract string CanRideInRace( string ageLimit, string categoryLimit );

        protected void Init()
        {
            data = null;

            points = "0";
            name = "";
            first_name = "";
            last_name = "";
            city = "";
            state = "";
            license = "";
            license2 = "";
            uci_code = "";
            age = "0";
            gender = "";
            discipline = "";
            category = "";
            category_entered = "";
            entry_date = "";
            team = "";
            message = "";
            email = "";
            racer_id = "";
            rank = "";
        }

        public void AddPoints( string new_points )
        {
            int total_points =
                Convert.ToInt32( points ) +
                Convert.ToInt32( new_points );

            points = Convert.ToString( total_points );
        }

        public bool EqualsByRacerID( BaseRider otherRider )
        {
            if( "".Equals( racer_id ) ) return false;

            return RacerID.Equals( otherRider.RacerID );
        }

        public bool EqualsByName( BaseRider otherRider )
        {
            return Name.Equals( otherRider.Name );
        }

        public bool EqualsByLastName( BaseRider otherRider )
        {
            return LastName.Equals( otherRider.LastName );
        }

        public bool EqualsByFirstName( BaseRider otherRider )
        {
            return FirstName.Equals( otherRider.FirstName );
        }

        public bool EqualsByLicense( BaseRider otherRider )
        {
            return License.Equals( otherRider.License );
        }
    }
}

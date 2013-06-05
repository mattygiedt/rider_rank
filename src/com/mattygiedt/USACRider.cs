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
    public class USACRider : BaseRider
    {
        private static readonly log4net.ILog log =
		    log4net.LogManager.GetLogger( typeof( USACRider ) );

        public static readonly string RIDER_TYPE = "USAC";

        public USACRider( string [] fields )
        {
            Init();

            if( null != fields )
            {
                data = new string[ fields.Length ];
                fields.CopyTo( data, 0 );

                this.points = fields[ 0 ];
                this.first_name = fields[ 1 ].ToUpper();
                this.last_name = fields[ 2 ].ToUpper();
                this.city = fields[ 3 ];
                this.state = fields[ 4 ];
                this.license = fields[ 5 ];
                this.age = fields[ 6 ];
                this.gender = fields[ 7 ];
                this.discipline = fields[ 8 ];
                this.category = fields[ 9 ];
            }
        }

        public USACRider( string points,
                          string first_name,
                          string last_name,
                          string entry_date )
        {
            Init();
            this.points = points;
            this.first_name = first_name;
            this.last_name = last_name;
            this.entry_date = entry_date;
        }

        public USACRider( string points,
                          string first_name,
                          string last_name,
                          string city,
                          string state,
                          string license,
                          string age,
                          string gender,
                          string discipline,
                          string category )
        {
            Init();
            this.points = points;
            this.first_name = first_name;
            this.last_name = last_name;
            this.city = city;
            this.state = state;
            this.license = license;
            this.age = age;
            this.gender = gender;
            this.discipline = discipline;
            this.category = category;
        }

        public USACRider( string points,
                          string name,
                          string city_state,
                          string license,
                          string age,
                          string gender,
                          string discipline,
                          string category )
        {
            Init();
            this.points = points.Trim();
            this.name = name.Trim();
            this.license = license.Trim();
            this.age = age.Trim();
            this.gender = gender;
            this.discipline = discipline;
            this.category = category;

            if( city_state.Split( new char[] { ',' } ).Length == 2 )
            {
                city = city_state.Split( new char[] { ',' } )[ 0 ].Trim();
                state = city_state.Split( new char[] { ',' } )[ 1 ].Trim();
            }
            else
            {
                city = "UNKNOWN";
                state = "UNKNOWN";
            }

            if( this.name.IndexOf( ' ' ) <= 1 )
            {
                first_name = name;
                last_name = name;
            }
            else
            {
                first_name = this.name.Substring( 0, this.name.IndexOf( ' ' ) );
                last_name = this.name.Substring( this.name.IndexOf( ' ' ) );
            }
        }

        public override BaseRider CloneRider()
        {
            BaseRider r = new USACRider(
                points,
                first_name,
                last_name,
                city,
                state,
                license,
                age,
                gender,
                discipline,
                category );

            r.EntryDate = entry_date;
            r.Message = message;
            return r;
        }

        public override string ToDataCSV( )
        {
            return points + "," +
                   first_name.Trim() + "," +
                   last_name.Trim() + "," +
                   city + "," +
                   state + "," +
                   license + "," +
                   age + "," +
                   gender + "," +
                   discipline + "," +
                   category;
        }

        public override string ToRankCSV( )
        {
            return ToDataCSV() + "," +
                   entry_date + "," + message;
        }

        public override string RiderType( )
        {
            return USACDataSource.DataSourceType;
        }

        public override string CanRideInRace( string ageLimit, string categoryLimit )
        {
            int riderAge = Convert.ToInt32( age );
            int ageMin = Convert.ToInt32( ageLimit.Split( new char[] { '-' } )[ 0 ] );
            int ageMax = Convert.ToInt32( ageLimit.Split( new char[] { '-' } )[ 1 ] );

            //
            //  Age check
            //

            if( riderAge == 0 )
            {
                return "UNKNOWN";
            }
            else if( riderAge < ageMin )
            {
                return "INVALID AGE: " + riderAge + " < " + ageMin;
            }
            else if( riderAge > ageMax )
            {
                return "INVALID AGE: " + riderAge + " > " + ageMax;
            }

            //
            //  Category check
            //

            if(  "5".Equals( categoryLimit ) == true &&
                "05".Equals( category ) == false )
            {
                return "INVALID CATEGORY: " + category + " <> " + categoryLimit;
            }
            else if(  "4".Equals( categoryLimit ) == true &&
                     "04".Equals( category ) == false )
            {
                return "INVALID CATEGORY: " + category + " <> " + categoryLimit;
            }
            else if(  "3".Equals( categoryLimit ) == true &&
                     "03".Equals( category ) == false )
            {
                return "INVALID CATEGORY: " + category + " <> " + categoryLimit;
            }
            else if(  "2".Equals( categoryLimit ) == true &&
                     "02".Equals( category ) == false )
            {
                return "INVALID CATEGORY: " + category + " <> " + categoryLimit;
            }
            else if(  "1".Equals( categoryLimit ) == true &&
                     "01".Equals( category ) == false )
            {
                return "INVALID CATEGORY: " + category + " <> " + categoryLimit;
            }
            else if( "4-5".Equals( categoryLimit ) == true &&
                     ( ( "05".Equals( category ) == false ) &&
                       ( "04".Equals( category ) == false ) ) )
            {
                return "INVALID CATEGORY: " + category + " <> " + categoryLimit;
            }
            else if( "3-4".Equals( categoryLimit ) == true &&
                     ( ( "03".Equals( category ) == false ) &&
                       ( "04".Equals( category ) == false ) ) )
            {
                return "INVALID CATEGORY: " + category + " <> " + categoryLimit;
            }
            else if( "2-3".Equals( categoryLimit ) == true &&
                     ( ( "03".Equals( category ) == false ) &&
                       ( "02".Equals( category ) == false ) ) )
            {
                return "INVALID CATEGORY: " + category + " <> " + categoryLimit;
            }
            else if( "1-2-3".Equals( categoryLimit ) == true &&
                     ( ( "03".Equals( category ) == false ) &&
                       ( "02".Equals( category ) == false ) &&
                       ( "01".Equals( category ) == false ) ) )
            {
                return "INVALID CATEGORY: " + category + " <> " + categoryLimit;
            }
            else if( "1-2-3-4".Equals( categoryLimit ) == true &&
                     ( ( "04".Equals( category ) == false ) &&
                       ( "03".Equals( category ) == false ) &&
                       ( "02".Equals( category ) == false ) &&
                       ( "01".Equals( category ) == false ) ) )
            {
                return "INVALID CATEGORY: " + category + " <> " + categoryLimit;
            }
            else if( "1-2-3-4-5".Equals( categoryLimit ) == true &&
                     ( ( "05".Equals( category ) == false ) &&
                       ( "04".Equals( category ) == false ) &&
                       ( "03".Equals( category ) == false ) &&
                       ( "02".Equals( category ) == false ) &&
                       ( "01".Equals( category ) == false ) ) )
            {
                return "INVALID CATEGORY: " + category + " <> " + categoryLimit;
            }

            //
            //  Rider is OK by me
            //

            return "TRUE";
        }
    }
}

using System;
using System.IO;
using System.Windows;
using System.Security;
using System.Threading;
using System.Configuration;
using System.ComponentModel;
using System.Collections.Generic;
using System.Security.Permissions;

namespace com.mattygiedt
{
    public class BikeRegRider : BaseRider
    {
        private static readonly log4net.ILog log =
		    log4net.LogManager.GetLogger( typeof( BikeRegRider ) );

        private List<string> events = new List<string>();

        public List<string> Events { get { return new List<string>( events ); } }

        /*
            <bikereg>
                <column name="First Name" column_id="A" />
                <column name="Last Name" column_id="B" />
                <column name="City" column_id="C" />
                <column name="State" column_id="D" />
                <column name="Team" column_id="E" />
                <column name="Email" column_id="F" />
                <column name="BikeReg USAC License" column_id="G" />
                <column name="BikeReg UCI CODE" column_id="H" />
                <column name="Sex" column_id="I" />
                <column name="Age" column_id="J" />
                <column name="BikeReg USAC Category Road" column_id="K" />
                <column name="Category Entered" column_id="L" />
                <column name="Entry Date and Time" column_id="M" />
                <column name="Manual USAC License" column_id="N" />
                <column name="RacerID" column_id="O" />
            </bikereg>
        */

        public BikeRegRider( string [] fields )
        {
            Init();

            if( null != fields )
            {
                data = new string[ fields.Length ];
                fields.CopyTo( data, 0 );

                this.first_name = data[ BikeRegData.ColumnIndex( "First Name" ) ].ToUpper();
                this.last_name = data[ BikeRegData.ColumnIndex( "Last Name" ) ].ToUpper();
                this.city = data[ BikeRegData.ColumnIndex( "City" ) ].ToUpper();
                this.state = data[ BikeRegData.ColumnIndex( "State" ) ].ToUpper();
                this.team = data[ BikeRegData.ColumnIndex( "Team" ) ].ToUpper();
                this.email = data[ BikeRegData.ColumnIndex( "Email" ) ];
                this.license = data[ BikeRegData.ColumnIndex( "BikeReg USAC License" ) ];
                this.uci_code = data[ BikeRegData.ColumnIndex( "BikeReg UCI CODE" ) ];
                this.gender = data[ BikeRegData.ColumnIndex( "Sex" ) ];
                this.age = data[ BikeRegData.ColumnIndex( "Age" ) ];
                this.category = data[ BikeRegData.ColumnIndex( "BikeReg USAC Category Road" ) ];
                this.category_entered = data[ BikeRegData.ColumnIndex( "Category Entered" ) ];
                this.entry_date = data[ BikeRegData.ColumnIndex( "Entry Date and Time" ) ];
                this.license2 = data[ BikeRegData.ColumnIndex( "Manual USAC License" ) ];
                this.racer_id = data[ BikeRegData.ColumnIndex( "RacerID" ) ];
            }
        }

        public BikeRegRider( string license,
                             string first_name,
                             string last_name,
                             string entry_date )
        {
            Init();

            this.license = license;
            this.first_name = first_name;
            this.last_name = last_name;
            this.entry_date = entry_date;
        }

        public void AddEvent( string evnt )
        {
            events.Add( evnt );
        }

        public override BaseRider CloneRider()
        {
            BaseRider r = new BikeRegRider( data );

            foreach( string evnt in events )
            {
                ((BikeRegRider)r).AddEvent( evnt );
            }

            return r;
        }

        public override string ToDataCSV()
        {
            string s =
                   license.Trim() + "," +
                   first_name.Trim() + "," +
                   last_name.Trim() + "," +
                   entry_date + ",[ ";

            foreach( string evnt in events )
            {
                s += "'" + evnt + "' ";
            }

            s += "]";
            return s;
        }

        public override string ToRankCSV( )
        {
            return points + "," +
                   rank + "," +
                   first_name + "," +
                   last_name + "," +
                   city + "," +
                   state + "," +
                   team + "," +
                   email + "," +
                   license + "," +
                   uci_code + "," +
                   gender + "," +
                   age + "," +
                   category + "," +
                   category_entered + "," +
                   entry_date + "," +
                   message;
        }

        public override string RiderType( )
        {
            return USACDataSource.DataSourceType;
        }

        public override string CanRideInRace( string ageLimit, string categoryLimit )
        {
            return "TRUE";
        }
    }
}

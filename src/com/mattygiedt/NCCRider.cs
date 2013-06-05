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
    public class NCCRider : BaseRider
    {
        private static readonly log4net.ILog log =
		    log4net.LogManager.GetLogger( typeof( NCCRider ) );

        public static readonly string RIDER_TYPE = "NCC";

        public NCCRider( string [] fields )
        {
            Init();
            this.points = fields[ 0 ];
            this.first_name = fields[ 1 ].ToUpper();
            this.last_name = fields[ 2 ].ToUpper();
            this.gender = fields[ 3 ];
        }

        public NCCRider(
            string points,
            string name,
            string gender )
        {
            Init();
            this.points = points.Trim();
            this.name = name.Trim();
            this.gender = gender.Trim();

            //int token_idx = 0;
            string [] tokens = this.name.Split( new char[] { ' ' } );

            first_name = tokens[ 0 ];

            for( int i=1; i<tokens.Length; i++ )
            {
                if( tokens[ i ].Contains( "(" ) )
                {
                    break;
                }

                last_name += tokens[ i ] + " ";
            }

            last_name = last_name.Trim( new char[] { ' ', '*' } );

            log.Debug( " FIRST NAME: " + first_name );
            log.Debug( " LAST NAME: " + last_name );
        }

        public NCCRider(
            string points,
            string first_name,
            string last_name,
            string gender )
        {
            Init();
            this.points = points;
            this.first_name = first_name;
            this.last_name = last_name;
            this.gender = gender;
        }

        public override BaseRider CloneRider()
        {
            BaseRider r = new NCCRider(
                this.points,
                this.first_name,
                this.last_name,
                this.gender );

            r.EntryDate = entry_date;
            r.Message = message;
            return r;
        }

        public override string ToDataCSV( )
        {
            return points + "," +
                   first_name.Trim() + "," +
                   last_name.Trim() + "," +
                   gender;
        }

        public override string ToRankCSV( )
        {
            return ToDataCSV() + "," + entry_date + "," + message;
        }

        public override string RiderType( )
        {
            return NCCDataSource.DataSourceType;
        }

        public override string CanRideInRace( string ageLimit, string categoryLimit )
        {
            return "TRUE";
        }
    }
}

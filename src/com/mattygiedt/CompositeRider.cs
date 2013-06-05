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
    public class CompositeRider : BaseRider
    {
        private static readonly log4net.ILog log =
		    log4net.LogManager.GetLogger( typeof( CompositeRider ) );

        public static readonly string RIDER_TYPE = "COMPOSITE";

        public CompositeRider( string [] fields )
        {
            Init();
            this.points = fields[ 0 ];
            this.first_name = fields[ 1 ].ToUpper();
            this.last_name = fields[ 2 ].ToUpper();
            this.gender = fields[ 3 ];
        }

        public CompositeRider(
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
            BaseRider r = new CompositeRider(
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

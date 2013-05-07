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
    public class USACritsRider : BaseRider
    {
        private static readonly log4net.ILog log =
		    log4net.LogManager.GetLogger( typeof( USACritsRider ) );

        public USACritsRider( string [] fields )
        {
            Init();
            this.points = fields[ 0 ];
            this.first_name = fields[ 1 ].ToUpper();
            this.last_name = fields[ 2 ].ToUpper();
            this.gender = fields[ 3 ];
            this.team = fields[ 4 ];
        }

        public USACritsRider(
            string points,
            string name,
            string gender,
            string team )
        {
            Init();
            this.points = points.Trim();
            this.name = name.Trim();
            this.gender = gender.Trim();
            this.team = team.Trim();

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

        public USACritsRider(
            string points,
            string first_name,
            string last_name,
            string gender,
            string team )
        {
            Init();
            this.points = points;
            this.first_name = first_name;
            this.last_name = last_name;
            this.gender = gender;
            this.team = team;
        }

        public override BaseRider CloneRider()
        {
            BaseRider r = new USACritsRider(
                this.points,
                this.first_name,
                this.last_name,
                this.gender,
                this.team );

            r.EntryDate = entry_date;
            r.Message = message;
            return r;
        }

        public override string ToDataCSV( )
        {
            return points + "," +
                   first_name.Trim() + "," +
                   last_name.Trim() + "," +
                   gender + "," +
                   team;
        }

        public override string ToRankCSV( )
        {
            return ToDataCSV() + "," + entry_date + "," + message;
        }

        public override string RiderType( )
        {
            return USACritsDataSource.DataSourceType;
        }
    }
}

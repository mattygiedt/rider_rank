using System;
using System.IO;
using System.Windows;
using System.Security;
using System.Threading;
using System.Globalization;
using System.Configuration;
using System.ComponentModel;
using System.Collections.Generic;
using System.Security.Permissions;


namespace com.mattygiedt
{
    public class NCCRankSort : IComparer<BaseRider>
    {
        private static readonly log4net.ILog log =
    		log4net.LogManager.GetLogger( typeof( NCCRankSort ) );

        public int Compare( BaseRider x, BaseRider y )
        {
            double xPoints = 0.0;
            double yPoints = 0.0;

            if( Double.TryParse( x.Points, out xPoints ) == false )
            {
                log.Warn( "Invalid points value: " + x.Points + " for rider: " + x.Name );
                return 0;
            }

            if( Double.TryParse( y.Points, out yPoints ) == false )
            {
                log.Warn( "Invalid points value: " + y.Points + " for rider: " + y.Name );
                return 0;
            }

            if( xPoints == yPoints )
            {
                DateTime xDate = DateTime.Parse( x.EntryDate, CultureInfo.CreateSpecificCulture("en-us"), DateTimeStyles.None );
                DateTime yDate = DateTime.Parse( y.EntryDate, CultureInfo.CreateSpecificCulture("en-us"), DateTimeStyles.None );

                return xDate.CompareTo( yDate );
            }
            else
            {
                return yPoints.CompareTo( xPoints );
            }
        }
    }

    public class USACRankSort : IComparer<BaseRider>
    {
        private static readonly log4net.ILog log =
    		log4net.LogManager.GetLogger( typeof( USACRankSort ) );

        public int Compare( BaseRider x, BaseRider y )
        {
            double xPoints = 0.0;
            double yPoints = 0.0;

            if( Double.TryParse( x.Points, out xPoints ) == false )
            {
                log.Warn( "Invalid points value: " + x.Points + " for rider: " + x.Name );
                return 0;
            }

            if( Double.TryParse( y.Points, out yPoints ) == false )
            {
                log.Warn( "Invalid points value: " + y.Points + " for rider: " + y.Name );
                return 0;
            }

            if( xPoints == yPoints )
            {
                DateTime xDate = DateTime.Parse( x.EntryDate, CultureInfo.CreateSpecificCulture("en-us"), DateTimeStyles.None );
                DateTime yDate = DateTime.Parse( y.EntryDate, CultureInfo.CreateSpecificCulture("en-us"), DateTimeStyles.None );

                return xDate.CompareTo( yDate );
            }
            else
            {
                return xPoints.CompareTo( yPoints );
            }
        }
    }

    public class EntryDateRankSort : IComparer<BaseRider>
    {
        private static readonly log4net.ILog log =
    		log4net.LogManager.GetLogger( typeof( EntryDateRankSort ) );

        public int Compare( BaseRider x, BaseRider y )
        {
            DateTime xDate = DateTime.Parse( x.EntryDate, CultureInfo.CreateSpecificCulture("en-us"), DateTimeStyles.None );
            DateTime yDate = DateTime.Parse( y.EntryDate, CultureInfo.CreateSpecificCulture("en-us"), DateTimeStyles.None );

            return xDate.CompareTo( yDate );
        }
    }

    public sealed class RankingEngine
    {
        private static readonly log4net.ILog log =
		    log4net.LogManager.GetLogger( typeof( RankingEngine ) );

        private Dictionary<string, List<BaseRider>> data =
            new Dictionary<string, List<BaseRider>>( );

        public void LoadRiderData(
            string rider_type,
            string data_source,
            bool skipFirstLine )
        {
            //
            //  Load the data source
            //

            string line;
            int lineCount = 0;
            List<BaseRider> riders = new List<BaseRider>();
            StreamReader file = new StreamReader( data_source );

            while( ( line = file.ReadLine() ) != null )
            {
                lineCount += 1;

                if( lineCount == 1 && skipFirstLine == true )
                {
                    //
                    //  Skip header
                    //

                    continue;
                }

                string [] riderFields = line.Split( ',' );

                if( USACDataSource.DataSourceType.Equals( rider_type ) )
                {
                    riders.Add( new USACRider( riderFields ) );
                }
                else if( USACritsDataSource.DataSourceType.Equals( rider_type ) )
                {
                    riders.Add( new USACritsRider( riderFields ) );
                }
                else if( NCCDataSource.DataSourceType.Equals( rider_type ) )
                {
                    riders.Add( new NCCRider( riderFields ) );
                }
                else
                {
                    log.Warn( "Unknown rider type: " + rider_type );
                }
            }

            file.Close();

            if( data.ContainsKey( rider_type ) == false )
            {
                data.Add( rider_type, new List<BaseRider>() );
            }

            data[ rider_type ].AddRange( riders );
        }

        public void SortRiderData( string rider_type, List<BaseRider> riders )
        {
            if( USACDataSource.DataSourceType.Equals( rider_type ) )
            {
                riders.Sort( new USACRankSort() );
            }
            else if( USACritsDataSource.DataSourceType.Equals( rider_type ) ||
                     NCCDataSource.DataSourceType.Equals( rider_type ) )
            {
                riders.Sort( new NCCRankSort() );
            }
        }

        public List<BaseRider> Rank( EventElement race, BikeRegData bikeRegData )
        {
            return Rank( race.RiderType, race.Name, race.Discipline, bikeRegData );
        }

        public List<BaseRider> Rank(
            string riderType,
            string raceName,
            string raceDiscipline,
            BikeRegData bikeRegData )
        {
            if( data.ContainsKey( riderType ) == false )
            {
                log.Warn( "Unknown rider type: " + riderType );
                return new List<BaseRider>();
            }
            else if( NCCDataSource.DataSourceType.Equals( riderType ) == false &&
                     USACDataSource.DataSourceType.Equals( riderType ) == false &&
                     USACritsDataSource.DataSourceType.Equals( riderType ) == false )
            {
                log.Warn( "Unknown rider type: " + riderType );
                return new List<BaseRider>();
            }

            //
            //  Loop through each row of the bike reg data, finding riders
            //  for this race.
            //

            List<BaseRider> rankedRiders = new List<BaseRider>();
            List<BaseRider> unknownRiders = new List<BaseRider>();

            foreach( string row in bikeRegData.Rows( raceName ) )
            {
                bool found = false;
                string [] bikeRegRow = row.Split( ',' );

                BikeRegRider bikeRegRider = new BikeRegRider( row.Split( ',' ) );

                //
                //  Try to find the bike reg rider in the ranking engine data
                //

                log.Debug( "Finding " + riderType + " rider: '" +
                    bikeRegRider.FirstName + "' '" +
                    bikeRegRider.LastName + "' [" +
                    bikeRegRider.License + "," +
                    bikeRegRider.ManualLicense + "] " +
                    bikeRegRider.EntryDate );

                foreach( BaseRider rankedRider in data[ riderType ] )
                {
                    //log.Debug( "               '" + rider.FirstName + "' '" + rider.LastName + "'" );

                    if( USACDataSource.DataSourceType.Equals( riderType ) )
                    {
                        if( rankedRider.Discipline.Equals( raceDiscipline ) == false )
                        {
                            continue;
                        }

                        //
                        //  Rank order is bikereg license, then manual
                        //

                        if( bikeRegRider.License.Equals( rankedRider.License ) )
                        {
                            found = true;

                            if( bikeRegRider.FirstName.Equals( rankedRider.FirstName ) == false ||
                                bikeRegRider.LastName.Equals( rankedRider.LastName ) == false )
                            {
                                log.Warn( " BIKEREG LICENSE [" + bikeRegRider.License + "] WITH NAME CONFLICT: [["
                                    + bikeRegRider.FirstName + "][" + rankedRider.FirstName + "]["
                                    + bikeRegRider.LastName + "][" + rankedRider.LastName + "]]" );

                                bikeRegRider.Message = "BIKEREG LICENSE WITH NAME CONFLICT";
                            }
                            else
                            {
                                bikeRegRider.Message = "BIKEREG LICENSE";
                            }

                            bikeRegRider.Points = rankedRider.Points;
                            rankedRiders.Add( bikeRegRider );
                            break;
                        }
                        else if( bikeRegRider.ManualLicense.Equals( rankedRider.License ) )
                        {
                            found = true;

                            if( bikeRegRider.FirstName.Equals( rankedRider.FirstName ) == false ||
                                bikeRegRider.LastName.Equals( rankedRider.LastName ) == false )
                            {
                                log.Warn( " MANUAL LICENSE [" + bikeRegRider.ManualLicense + "] WITH NAME CONFLICT: [["
                                    + bikeRegRider.FirstName + "][" + rankedRider.FirstName + "]["
                                    + bikeRegRider.LastName + "][" + rankedRider.LastName + "]]" );

                                bikeRegRider.Message = "MANUAL LICENSE WITH NAME CONFLICT";
                            }
                            else
                            {
                                bikeRegRider.Message = "MANUAL LICENSE";
                            }

                            bikeRegRider.Points = rankedRider.Points;
                            bikeRegRider.License = bikeRegRider.ManualLicense;
                            rankedRiders.Add( bikeRegRider );
                            break;
                        }
                    }
                    else if( USACritsDataSource.DataSourceType.Equals( riderType ) ||
                             NCCDataSource.DataSourceType.Equals( riderType ) )
                    {
                        //
                        //  Only find by name
                        //

                        if( bikeRegRider.FirstName.Equals( rankedRider.FirstName ) == true &&
                            bikeRegRider.LastName.Equals( rankedRider.LastName ) == true )
                        {
                            found = true;
                            bikeRegRider.Points = rankedRider.Points;
                            bikeRegRider.Message = riderType + " FOUND BY NAME [" + rankedRider.Points + "]";
                            rankedRiders.Add( bikeRegRider );
                            break;
                        }
                        //else
                        //{
                        //    log.Debug( "    [[ " + firstName + " ][ " + rider.FirstName +
                        //               " ]] [[ " + lastName + " ][ " + rider.LastName + " ]]" );
                        //}
                    }
                }

                if( found == false )
                {
                    log.Warn( "UNKNOWN " + riderType + " rider: '" +
                        bikeRegRider.FirstName + "' '" +
                        bikeRegRider.LastName + "' [" +
                        bikeRegRider.License + "," +
                        bikeRegRider.ManualLicense + "] " +
                        bikeRegRider.EntryDate );

                    if( USACDataSource.DataSourceType.Equals( riderType ) )
                    {
                        bikeRegRider.Points = "1000";

                        if( bikeRegRider.License.Equals( bikeRegRider.ManualLicense ) )
                        {
                            bikeRegRider.Message = "UNKNOWN LICENSE " + bikeRegRider.License;
                            unknownRiders.Add( bikeRegRider );
                        }
                        else if( "".Equals( bikeRegRider.License ) == true &&
                                 "".Equals( bikeRegRider.ManualLicense ) == false )
                        {
                            bikeRegRider.Message = "UNKNOWN MANUAL LICENSE " + bikeRegRider.ManualLicense;
                            bikeRegRider.License = bikeRegRider.ManualLicense;
                            unknownRiders.Add( bikeRegRider );
                        }
                        else if( "".Equals( bikeRegRider.License ) == false &&
                                 "".Equals( bikeRegRider.ManualLicense ) == true )
                        {
                            bikeRegRider.Message = "UNKNOWN BIKEREG LICENSE " + bikeRegRider.License;
                            unknownRiders.Add( bikeRegRider );
                        }
                        else if( "".Equals( bikeRegRider.License ) == true &&
                                 "".Equals( bikeRegRider.ManualLicense ) == true )
                        {
                            bikeRegRider.Message = "NO LICENSE";
                            unknownRiders.Add( bikeRegRider );
                        }
                        else
                        {
                            bikeRegRider.Message = "UNKNOWN LICENSE";
                            unknownRiders.Add( bikeRegRider );
                        }
                    }
                    else if( USACritsDataSource.DataSourceType.Equals( riderType ) ||
                             NCCDataSource.DataSourceType.Equals( riderType ) )
                    {
                        bikeRegRider.Points = "0";
                        bikeRegRider.Message = "UNKNOWN " + riderType;
                        unknownRiders.Add( bikeRegRider );
                    }
                }
            }

            //
            //  Sort the riders
            //

            if( USACDataSource.DataSourceType.Equals( riderType ) )
            {
                //
                //  USAC: lower is better
                //

                rankedRiders.Sort( new USACRankSort() );
                unknownRiders.Sort( new EntryDateRankSort() );
            }
            else if( USACritsDataSource.DataSourceType.Equals( riderType ) ||
                     NCCDataSource.DataSourceType.Equals( riderType ) )
            {
                //
                //  NCC / USACrits: higher is better
                //

                rankedRiders.Sort( new NCCRankSort() );
                unknownRiders.Sort( new EntryDateRankSort() );
            }

            //
            //  Add the unknown riders to the end of the ranked riders
            //

            rankedRiders.AddRange( unknownRiders );

            //
            //  Assign race rank to each rider
            //

            for( int i=0; i<rankedRiders.Count; i++ )
            {
                rankedRiders[ i ].Rank = Convert.ToString( i + 1 );
            }

            //
            //  Return the complete ranked rider list
            //

            return rankedRiders;
        }
    }
}

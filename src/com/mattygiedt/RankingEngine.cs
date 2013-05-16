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
    public class RankSort : IComparer<BaseRider>
    {
        private static readonly log4net.ILog log =
    		log4net.LogManager.GetLogger( typeof( RankSort ) );

        public int Compare( BaseRider x, BaseRider y )
        {
            if( "1000".Equals( x.Points ) && "1000".Equals( y.Points ) )
            {
                DateTime xDate = DateTime.Parse( x.EntryDate, CultureInfo.CreateSpecificCulture("en-us"), DateTimeStyles.None );
                DateTime yDate = DateTime.Parse( y.EntryDate, CultureInfo.CreateSpecificCulture("en-us"), DateTimeStyles.None );

                return xDate.CompareTo( yDate );
            }
            else
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
                else if( x.RiderType().Equals( USACDataSource.DataSourceType ) )
                {
                    return xPoints.CompareTo( yPoints );
                }
                else
                {
                    return yPoints.CompareTo( xPoints );
                }
            }
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

        public void SortRiderData()
        {
            foreach( List<BaseRider> list in data.Values )
            {
                list.Sort( new RankSort() );
            }
        }

        public List<BaseRider> Rank( EventElement race, BikeRegData bikeRegData )
        {
            if( data.ContainsKey( race.RiderType ) == false )
            {
                log.Warn( "Unknown rider type: " + race.RiderType );
                return new List<BaseRider>();
            }

            //
            //  Loop through each row of the bike reg data, finding riders
            //  for this race.
            //

            List<BaseRider> rankedRiders = new List<BaseRider>();
            List<BaseRider> unknownRiders = new List<BaseRider>();

            foreach( string row in bikeRegData.Rows( race.Name ) )
            {
                bool found = false;
                string [] bikeRegRow = row.Split( ',' );

                //
                //  Try to find the rider in the ranking engine data
                //

                string bikeRegLicense = bikeRegRow[ bikeRegData.ColumnIndex( "BikeReg USAC License" ) ];
                string manualLicense = bikeRegRow[ bikeRegData.ColumnIndex( "Manual USAC License" ) ];

                string firstName = bikeRegRow[ bikeRegData.ColumnIndex( "First Name" ) ].ToUpper();
                string lastName = bikeRegRow[ bikeRegData.ColumnIndex( "Last Name" ) ].ToUpper();
                string entryDate = bikeRegRow[ bikeRegData.ColumnIndex( "Entry Date and Time" ) ];

                log.Debug( "Finding rider: '" + firstName + "' '" + lastName + "' [" + bikeRegLicense + "," + manualLicense + "] " + entryDate );

                foreach( BaseRider rider in data[ race.RiderType ] )
                {
                    //log.Debug( "               '" + rider.FirstName + "' '" + rider.LastName + "'" );

                    if( rider.Discipline.Equals( race.Discipline ) == false )
                    {
                        continue;
                    }

                    //
                    //  Rank order is bikereg license, then name, then manual
                    //

                    if( bikeRegLicense.Equals( rider.License ) )
                    {
                        found = true;
                        rider.EntryDate = entryDate;

                        if( firstName.Equals( rider.FirstName ) == false ||
                            lastName.Equals( rider.LastName ) == false )
                        {
                            log.Warn( " BIKEREG LICENSE [" + bikeRegLicense + "] WITH NAME CONFLICT: [["
                                + firstName + "][" + rider.FirstName + "]["
                                + lastName + "][" + rider.LastName + "]]" );
                            rider.Message = "BIKEREG LICENSE WITH NAME CONFLICT";
                        }
                        else
                        {
                            rider.Message = "BIKEREG LICENSE";
                        }

                        rankedRiders.Add( rider.CloneRider() );
                        break;
                    }
                    else if( manualLicense.Equals( rider.License ) )
                    {
                        found = true;
                        rider.EntryDate = entryDate;

                        if( firstName.Equals( rider.FirstName ) == false ||
                            lastName.Equals( rider.LastName ) == false )
                        {
                            log.Warn( " MANUAL LICENSE [" + manualLicense + "] WITH NAME CONFLICT: [["
                                + firstName + "][" + rider.FirstName + "]["
                                + lastName + "][" + rider.LastName + "]]" );
                            rider.Message = "MANUAL LICENSE WITH NAME CONFLICT";
                        }
                        else
                        {
                            rider.Message = "MANUAL LICENSE";
                        }

                        rankedRiders.Add( rider.CloneRider() );
                        break;
                    }
                    //else if( firstName.Equals( rider.FirstName ) && lastName.Equals( rider.LastName ) )
                    //{
                    //    //
                    //    //  Only find by name if license is invalid.
                    //    //
                    //
                    //    if( "".Equals( manualLicense ) == true &&
                    //        "".Equals( bikeRegLicens ) == true )
                    //    {
                    //        //log.Debug( "FOUND BY NAME  '" + rider.FirstName + "' '" + rider.LastName + "'" );
                    //
                    //        found = true;
                    //        rider.EntryDate = entryDate;
                    //        rider.Message = "FOUND BY NAME";
                    //        rankedRiders.Add( rider.CloneRider() );
                    //        break;
                    //    }
                    //}
                }

                if( found == false )
                {
                    if( USACDataSource.DataSourceType.Equals( race.RiderType ) )
                    {
                        BaseRider rider = new USACRider(
                            "1000",
                            firstName,
                            lastName,
                            entryDate );

                        if( bikeRegLicense.Equals( manualLicense ) )
                        {
                            rider.License = bikeRegLicense;
                            rider.Message = "UNKNOWN LICENSE " + bikeRegLicense;
                            unknownRiders.Add( rider );
                        }
                        else if( "".Equals( bikeRegLicense ) == true && "".Equals( manualLicense ) == false )
                        {
                            rider.License = manualLicense;
                            rider.Message = "UNKNOWN MANUAL LICENSE " + manualLicense;
                            unknownRiders.Add( rider );
                        }
                        else if( "".Equals( bikeRegLicense ) == false && "".Equals( manualLicense ) == true )
                        {
                            rider.License = bikeRegLicense;
                            rider.Message = "UNKNOWN BIKEREG LICENSE " + bikeRegLicense;
                            unknownRiders.Add( rider );
                        }
                        else if( "".Equals( bikeRegLicense ) == true && "".Equals( manualLicense ) == true )
                        {
                            rider.License = "";
                            rider.Message = "NO LICENSE";
                            unknownRiders.Add( rider );
                        }
                        else
                        {
                            rider.License = "[" + bikeRegLicense + " | " + manualLicense + "]";
                            rider.Message = "UNKNOWN LICENSE";
                            unknownRiders.Add( rider );
                        }
                    }
                    else if( USACritsDataSource.DataSourceType.Equals( race.RiderType ) )
                    {
                        USACritsRider unknownRider =
                            new USACritsRider(
                                "1000",
                                firstName,
                                lastName,
                                "", "" );

                        unknownRider.License = "[" + bikeRegLicense + " | " + manualLicense + "]";
                        unknownRider.Message = "UNKNOWN LICENSE";
                        unknownRider.EntryDate = entryDate;

                        unknownRiders.Add( unknownRider );
                    }
                }
            }

            //
            //  Sort the riders
            //

            rankedRiders.Sort( new RankSort() );
            unknownRiders.Sort( new RankSort() );

            //
            //  Add the unknown riders to the end of the ranked riders
            //

            rankedRiders.AddRange( unknownRiders );

            //
            //  Return the complete ranked rider list
            //

            return rankedRiders;
        }
    }
}

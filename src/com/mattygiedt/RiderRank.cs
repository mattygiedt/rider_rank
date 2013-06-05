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
    public sealed class RiderRank
    {
        private static readonly log4net.ILog log =
		    log4net.LogManager.GetLogger( typeof( RiderRank ) );

        public static string DatePattern;

        [STAThreadAttribute()]
        [SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public static int Main( string[] args )
        {
            log4net.Config.XmlConfigurator.Configure();

            try
            {
                if( args.Length != 1 )
                {
                    log.Fatal( "invalid command line parameters" );
                    log.Fatal( " usage: rider_rank.exe [BikeReg Input File]" );
                }

                //
                //  load configuration
                //

                RiderRankConfiguration configSection =
                    ConfigurationManager.GetSection( "rider_rank_config" )
                        as RiderRankConfiguration;

                DatePattern = configSection.DatetimeFormat;

                //
                //  Load the bikereg file
                //

                log.Info( " loading BikeReg data file: " + args[ 0 ] );
                BikeRegData bikeRegData = new BikeRegData( args[ 0 ] );
                bikeRegData.LoadData( true );

                for( int i=0; i<configSection.BikeRegConfig.Count; i++ )
                {
                    bikeRegData.AddColumn(
                        configSection.BikeRegConfig[ i ].Name,
                        configSection.BikeRegConfig[ i ].ColumnID );
                }

                //
                //  Load the ranking system
                //

                RankingEngine rankingEngine = new RankingEngine();

                for( int i=0; i<configSection.RankSystemConfig.Count; i++ )
                {
                    log.Info( " loading " + configSection.RankSystemConfig[ i ].Name +
                              " rider data from: " + configSection.RankSystemConfig[ i ].DataSource );

                    rankingEngine.LoadRiderData(
                        configSection.RankSystemConfig[ i ].RiderType,
                        configSection.RankSystemConfig[ i ].DataSource,
                        true );
                }

                //
                //  Iterate over each event and rank the riders
                //

                bool top10Append = false;
                List<BaseRider> rankedRiders = new List<BaseRider>();
                List<BaseRider> allRankedRiders = new List<BaseRider>();

                for( int i=0; i<configSection.EventsConfig.Count; i++ )
                {
                    rankedRiders.Clear();

                    log.Info( " generating rider list for: " +
                        configSection.EventsConfig[ i ].Name );

                    if( configSection.EventsConfig[ i ].RiderType.Contains( "COMPOSITE:" ) == true )
                    {
                        log.Debug( " composite rider types: " + configSection.EventsConfig[ i ].RiderType );

                        //
                        //  COMPOSITE RIDER: for each riderType rank as if
                        //  they were individual, and join into single list.
                        //

                        string riderTypes = configSection.EventsConfig[ i ].RiderType;
                        string [] compositeRiderTypes = riderTypes.Split( new Char [] { ':' } );
                        string [] riderTypeArr = compositeRiderTypes[ 1 ].Split( new Char [] { ',' } );

                        log.Info( " rider types: " + compositeRiderTypes[ 1 ] );

                        foreach( string riderType in riderTypeArr )
                        {
                            log.Info( " ranking composite rider type: " + riderType );

                            //
                            //  Assign the composite riderType to the event
                            //

                            List<BaseRider> compositeRiders =
                                rankingEngine.Rank( riderType,
                                    configSection.EventsConfig[ i ].Name,
                                    configSection.EventsConfig[ i ].Discipline,
                                    bikeRegData );

                            foreach( BaseRider compositeRider in compositeRiders )
                            {
                                bool found = false;
                                foreach( BaseRider rankedRider in rankedRiders )
                                {
                                    if( compositeRider.EqualsByFirstName( rankedRider ) &&
                                        compositeRider.EqualsByLastName( rankedRider ) )
                                    {
                                        found = true;
                                        rankedRider.AddPoints( compositeRider.Points );
                                        rankedRider.Message += " | " + compositeRider.Message;
                                        break;
                                    }
                                }

                                if( found == false )
                                {
                                    rankedRiders.Add( compositeRider );
                                }
                            }

                            rankingEngine.SortRiderData( riderType, rankedRiders );
                        }

                        //
                        //  We now need to re-bib the rankedRider list
                        //

                        for( int j=0; j<rankedRiders.Count; j++ )
                        {
                            rankedRiders[ j ].Rank = Convert.ToString( j+1 );
                        }
                    }
                    else
                    {
                        rankedRiders.AddRange( rankingEngine.Rank(
                            configSection.EventsConfig[ i ], bikeRegData ) );
                    }

                    allRankedRiders.AddRange( rankedRiders );

                    //
                    //  Print out the rider rankings
                    //

                    using( StreamWriter outfile = new StreamWriter(
                           configSection.OutputDir + "\\" +
                           configSection.EventsConfig[ i ].Output ) )
                    {
                        outfile.WriteLine( "POINTS,BIB_NUMBER,FIRST_NAME,LAST_NAME,CITY,STATE,TEAM,EMAIL,LICENSE,UCI_CODE,GENDER,AGE,CATEGORY,CATEGORY_ENTERED,ENTRY_DATE,MESSAGE" );

                        foreach( BaseRider rider in rankedRiders )
                        {
                            outfile.WriteLine( rider.ToRankCSV() );
                        }
                    }

                    //
                    //  Add ranked riders to top-10 file
                    //

                    using( StreamWriter outfile = new StreamWriter(
                        configSection.OutputDir + "\\TOP-10.csv", top10Append ) )
                    {
                        top10Append = true; // one shot
                        outfile.WriteLine( configSection.EventsConfig[ i ].Name );

                        for( int j=0; j<rankedRiders.Count; j++ )
                        {
                            outfile.WriteLine( rankedRiders[ j ].ToRankCSV() );

                            if( j == 9 ) break;
                        }

                        outfile.WriteLine();
                    }
                }

                //
                //  Print out all the rider rankings
                //

                using( StreamWriter outfile = new StreamWriter(
                       configSection.OutputDir + "\\REGISTRATION.csv" ) )
                {
                    outfile.WriteLine( "POINTS,BIB_NUMBER,FIRST_NAME,LAST_NAME,CITY,STATE,TEAM,EMAIL,LICENSE,UCI_CODE,GENDER,AGE,CATEGORY,CATEGORY_ENTERED,ENTRY_DATE,MESSAGE" );

                    foreach( BaseRider rider in allRankedRiders )
                    {
                        outfile.WriteLine( rider.ToRankCSV() );
                    }
                }

                //
                //  List riders in multiple races
                //

                List<BikeRegRider> bikeRegRiders = bikeRegData.GetRidersByEvent( "ALL" );

                foreach( string evnt in bikeRegData.GetEvents() )
                {
                    foreach( BikeRegRider eventRider in bikeRegData.GetRidersByEvent( evnt ) )
                    {
                        foreach( BikeRegRider rider in bikeRegRiders )
                        {
                            if( eventRider.EqualsByName( rider ) )
                            {
                                if( eventRider.EqualsByLicense( rider ) == false )
                                {
                                    log.Warn( "DUPLICATE RIDER NAME: " + eventRider.Name );
                                }
                                else
                                {
                                    rider.AddEvent( evnt );
                                    break;
                                }
                            }
                        }
                    }
                }

                using( StreamWriter outfile = new StreamWriter(
                        configSection.OutputDir + "\\" +
                        "MultiEventRiders.csv" ) )
                {
                    foreach( BikeRegRider rider in bikeRegRiders )
                    {
                        if( rider.Events.Count > 1 )
                        {
                            log.Warn( rider.ToDataCSV() );
                            outfile.WriteLine( rider.ToDataCSV( ) );
                        }
                    }
                }
            }
            catch( Exception ex )
            {
                log.Fatal( "RiderRank error: ", ex );
                return -1;
            }

            Console.WriteLine( "Press any key to exit." );
            Console.ReadKey();
            return 0;
        }
    }
}

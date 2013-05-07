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

                for( int i=0; i<configSection.EventsConfig.Count; i++ )
                {
                    log.Info( " generating rider list for: " +
                        configSection.EventsConfig[ i ].Name );

                    List<BaseRider> rankedRiders = rankingEngine.Rank(
                        configSection.EventsConfig[ i ], bikeRegData );

                    using( StreamWriter outfile = new StreamWriter(
                        configSection.OutputDir + "\\" +
                        configSection.EventsConfig[ i ].Output ) )
                    {
                        foreach( BaseRider rider in rankedRiders )
                        {
                            outfile.WriteLine( rider.ToRankCSV( ) );
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
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
    public sealed class DataDownloader
    {
        private static readonly log4net.ILog log =
		    log4net.LogManager.GetLogger( typeof( DataDownloader ) );

        [STAThreadAttribute()]
        [SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public static int Main( string[] args )
        {
            log4net.Config.XmlConfigurator.Configure();

            try
            {
                DataDownloadConfiguration configSection =
                    ConfigurationManager.GetSection( "data_download_config" )
                        as DataDownloadConfiguration;

                for( int i=0; i<configSection.OutputFileGroup.Count; i++ )
                {
                    log.Info( "Generating file: " + configSection.OutputFileGroup[ i ].Name );

                    OutputFileElement output_file = configSection.OutputFileGroup[ i ];

                    if( USACDataSource.DataSourceType.Equals( output_file.DataSourceType ) )
                    {
                        DataSource dataSource = new USACDataSource();

                        for( int j=0; j<output_file.DataSourceGroup.Count; j++ )
                        {
                            log.Info( " loading data source: " + output_file.DataSourceGroup[ j ].Name );

                            dataSource.LoadRiders(
                                output_file.DataSourceGroup[ j ].URI,
                                output_file.DataSourceGroup[ j ].Gender,
                                output_file.DataSourceGroup[ j ].Discipline,
                                output_file.DataSourceGroup[ j ].Category );

                        }

                        if( "CSV".Equals( output_file.Format ) )
                        {
                            log.Info( " writing file: " + configSection.OutputFileGroup[ i ].Name );
                            dataSource.SaveRidersToCSV( output_file.Name, true );
                        }
                        else
                        {
                            log.Warn( "Unknown file format: " + output_file.Format );
                        }
                    }
                    else if( USACritsDataSource.DataSourceType.Equals( output_file.DataSourceType ) )
                    {
                        DataSource dataSource = new USACritsDataSource();

                        for( int j=0; j<output_file.DataSourceGroup.Count; j++ )
                        {
                            log.Info( " loading data source: " + output_file.DataSourceGroup[ j ].Name );

                            dataSource.LoadRiders(
                                output_file.DataSourceGroup[ j ].URI,
                                output_file.DataSourceGroup[ j ].Gender,
                                "", "" );
                        }

                        if( "CSV".Equals( output_file.Format ) )
                        {
                            log.Info( " writing file: " + configSection.OutputFileGroup[ i ].Name );
                            dataSource.SaveRidersToCSV( output_file.Name, true );
                        }
                        else
                        {
                            log.Warn( "Unknown file format: " + output_file.Format );
                        }
                    }
                    else if( NCCDataSource.DataSourceType.Equals( output_file.DataSourceType ) )
                    {
                        DataSource dataSource = new NCCDataSource();

                        for( int j=0; j<output_file.DataSourceGroup.Count; j++ )
                        {
                            log.Info( " loading data source: " + output_file.DataSourceGroup[ j ].Name );

                            dataSource.LoadRiders(
                                output_file.DataSourceGroup[ j ].URI,
                                output_file.DataSourceGroup[ j ].Gender,
                                "", "" );
                        }

                        if( "CSV".Equals( output_file.Format ) )
                        {
                            log.Info( " writing file: " + configSection.OutputFileGroup[ i ].Name );
                            dataSource.SaveRidersToCSV( output_file.Name, true );
                        }
                        else
                        {
                            log.Warn( "Unknown file format: " + output_file.Format );
                        }
                    }
                    else
                    {
                        log.Warn( "Unknown data source type: " + output_file.DataSourceType );
                    }
                }
            }
            catch( Exception ex )
            {
                log.Fatal( "fatal error: ", ex );
                return -1;
            }

            Console.WriteLine( "Press any key to exit." );
            Console.ReadKey();
            return 0;
        }
    }
}

using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Security;
using System.Threading;
using System.Configuration;
using System.ComponentModel;
using System.Collections.Generic;
using System.Security.Permissions;
using System.Collections.Specialized;

using HtmlAgilityPack;

namespace com.mattygiedt
{
    public sealed class USACDataSource : DataSource
    {
        private static readonly log4net.ILog log =
		    log4net.LogManager.GetLogger( typeof( USACDataSource ) );

        public static string DataSourceType = "USAC";
        public static string CSVHeader = "POINTS,FIRST_NAME,LAST_NAME,CITY,STATE,LICENSE,AGE,GENDER,DISCIPLINE,CATEGORY";

        private List<BaseRider> riders = new List<BaseRider>();

        public override void LoadRiders( string uri, string gender, string discipline, string category )
        {
            this.uri = uri.Trim();
            this.gender = gender.Trim();
            this.discipline = discipline.Trim();
            this.category = category.Trim();

            int page_id = 0;
            bool doUploadString = true;

            List<string> tableData = new List<string>();

            while( doUploadString == true )
            {
                int newRiderCount = 0;
                int currRiderCount = riders.Count;

                string queryString = "mode=show_rank&page=" +
                    page_id + "&region=&state=&sex=" +
                    gender + "&disc=" + discipline + "&cat=" +
                    category + "&agemin=1&agemax=99";

                log.Debug( "query string: " + queryString );

                using( WebClient client = new WebClient() )
                {
                    client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";

                    string html = client.UploadString( uri, queryString );

                    //using( StreamWriter outfile = new StreamWriter( category + "." + page_id + ".html" ) )
                    //{
                    //    outfile.Write( html );
                    //}

                    HtmlDocument doc = new HtmlDocument();
                    doc.LoadHtml( html );

                    foreach( HtmlNode tr in doc.DocumentNode.SelectNodes( "//tr" ) )
                    {
                        if( null != tr )
                        {
                            tableData.Clear();
                            foreach( HtmlNode td in tr.Elements( "td" ) )
                            {
                                tableData.Add( td.InnerText );
                            }

                            if( tableData.Count == 7 )
                            {
                                newRiderCount++;
                                riders.Add( new USACRider(
                                    tableData[ 1 ],
                                    tableData[ 2 ],
                                    tableData[ 3 ],
                                    tableData[ 4 ],
                                    tableData[ 5 ],
                                    gender,
                                    discipline,
                                    category ) );
                            }
                        }
                    }

                    if( newRiderCount == 0 )
                    {
                        doUploadString = false;
                        return;
                    }
                    else
                    {
                        page_id += 1;

                        //
                        //  Play nice with USAC web server
                        //

                        Thread.Sleep( 2000 );
                    }
                }

                if( "".Equals( category ) )
                {
                    log.Info( " added " + ( riders.Count - currRiderCount ) + " " + discipline +
                          " " + gender + " riders." );
                }
                else
                {
                    log.Info( " added " + ( riders.Count - currRiderCount ) + " " + discipline +
                          " " + gender + " " + category + " riders." );
                }
            }
        }

        public override void SaveRidersToCSV( string file, bool printHeader )
        {
            using( StreamWriter outfile = new StreamWriter( file ) )
            {
                if( printHeader == true )
                {
                    outfile.WriteLine( CSVHeader );
                }

                foreach( BaseRider rider in riders )
                {
                    outfile.WriteLine( rider.ToDataCSV( ) );
                }
            }
        }

        public override void Join( List<BaseRider> otherRiders )
        {
            otherRiders.AddRange( riders );
        }
    }
}

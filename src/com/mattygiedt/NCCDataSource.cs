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
    public sealed class NCCDataSource : DataSource
    {
        private static readonly log4net.ILog log =
		    log4net.LogManager.GetLogger( typeof( NCCDataSource ) );

        public static string DataSourceType = "NCC";
        public static string CSVHeader = "POINTS,FIRST_NAME,LAST_NAME,GENDER";

        private List<BaseRider> riders = new List<BaseRider>();

        public override void LoadRiders( string uri, string gender, string discipline, string category )
        {
            this.uri = uri.Trim();
            this.gender = gender.Trim();
            this.discipline = discipline.Trim();
            this.category = category.Trim();

            string html = "";
            List<string> tableData = new List<string>();

            using( WebClient client = new WebClient() )
            {
                log.Info( "NCC request: " + uri );

                html = client.DownloadString( uri );

                //using( StreamWriter outfile = new StreamWriter( "NCC." + gender  + ".html" ) )
                //{
                //    outfile.Write( html );
                //}
            }

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml( html );
            //doc.Load( "NCC.html" );

            bool process_div = false;
            bool have_reached_women = false;

            foreach( HtmlNode series_rider_standings_div in doc.DocumentNode.SelectNodes(
                "//div[contains(@id,'series_rider_standings')]/div" ) )
            {
                //log.Debug( series_rider_standings_div.InnerText.Trim() );

                process_div = false;

                if( "M".Equals( gender ) && have_reached_women == false )
                {
                    process_div = true;
                }
                else if( "F".Equals( gender ) && have_reached_women == true )
                {
                    process_div = true;
                }

                if( process_div == true && null != series_rider_standings_div )
                {
                    log.Debug( "PROCESS_DIV!" );
                    log.Debug( series_rider_standings_div.InnerText.Trim() );

                    foreach( HtmlNode div in series_rider_standings_div.SelectNodes(
                        "div[contains(@class,'standing even') or contains(@class,'standing odd')]" ) )
                    {
                        if( null != div )
                        {
                            //log.Debug( div.InnerText.Trim() );

                            tableData.Clear();
                            foreach( HtmlNode node in div.Elements( "div" ) )
                            {
                                tableData.Add( node.InnerText.Trim() );
                            }
                        }

                        for( int j=0; j<tableData.Count; j++ )
                        {
                            log.Debug( " " + j + ": " + tableData[ j ] );
                        }

                        riders.Add( new NCCRider(
                            tableData[ 2 ],
                            tableData[ 1 ],
                            gender ) );
                    }

                    return;
                }

                //
                //  one-shot, as the women are after the men on USAC website
                //

                have_reached_women = true;
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

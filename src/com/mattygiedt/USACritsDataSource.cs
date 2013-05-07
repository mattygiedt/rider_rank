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
    public sealed class USACritsDataSource : DataSource
    {
        private static readonly log4net.ILog log =
		    log4net.LogManager.GetLogger( typeof( USACDataSource ) );

        public static string DataSourceType = "USACRITS";
        public static string CSVHeader = "POINTS,FIRST_NAME,LAST_NAME,GENDER,TEAM";

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
                log.Info( "USACrits request: " + uri );

                html = client.DownloadString( uri );

                //using( StreamWriter outfile = new StreamWriter( "USACRITS." + gender  + ".html" ) )
                //{
                //    outfile.Write( html );
                //}
            }

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml( html );
            //doc.Load( "USACRITS." + gender  + ".html" );

            foreach( HtmlNode tr in doc.DocumentNode.SelectNodes( "//tr[contains(@class,'datarow1') or contains(@class,'datarow2')]" ) )
            {
                if( null != tr )
                {
                    tableData.Clear();
                    foreach( HtmlNode td in tr.Elements( "td" ) )
                    {
                        tableData.Add( td.InnerText.Trim() );
                    }
                }

                riders.Add( new USACritsRider(
                    tableData[ 4 ],
                    tableData[ 1 ],
                    gender,
                    tableData[ 2 ] ) );
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

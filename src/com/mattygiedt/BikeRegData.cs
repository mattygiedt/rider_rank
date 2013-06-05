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
    public sealed class BikeRegData
    {
        private static readonly log4net.ILog log =
		    log4net.LogManager.GetLogger( typeof( BikeRegData ) );

        private string filename;
        private List<string> rows = new List<string>();
        private static Dictionary<string, string>
            columnMap = new Dictionary<string,string>();

        public List<string> Rows( string event_name )
        {
            List<string> list = new List<string>();
            int event_idx = ColumnIndex( "Category Entered" );

            foreach( string row in rows )
            {
                string [] row_arr = row.Split( ',' );

                if( event_name.Equals( row_arr[ event_idx ] ) )
                {
                    list.Add( row );
                }
            }

            return list;
        }

        public List<string> Columns
        {
            get
            {
                List<string> cols = new List<string>();
                foreach( string c in columnMap.Keys )
                {
                    cols.Add( c );
                }

                return cols;
            }
        }

        public BikeRegData( string filename )
        {
            this.filename = filename;
        }

        public void AddColumn( string key, string value )
        {
            columnMap.Add( key, value );
        }

        public void LoadData( bool skipFirstLine )
        {
            int lineCount = 0;
            string line;
            rows.Clear();

            StreamReader file = new StreamReader( filename );

            while( ( line = file.ReadLine() ) != null )
            {
                lineCount += 1;

                if( lineCount == 1 && skipFirstLine == true )
                {
                    continue;
                }

                rows.Add( line.Replace( "\"", "" ) );
            }

            file.Close();
        }

        public List<BikeRegRider> GetRidersByEvent( string evnt )
        {
            List<BikeRegRider> riders = new List<BikeRegRider>();

            foreach( string row in rows )
            {
                string [] row_arr = row.Split( ',' );
                string categoryEntered = row_arr[ ColumnIndex( "Category Entered" ) ].ToUpper();

                if( "ALL".Equals( evnt ) )
                {
                    riders.Add( new BikeRegRider( row_arr ) );
                }
                else if( categoryEntered.Equals( evnt.ToUpper() ) )
                {
                    riders.Add( new BikeRegRider( row_arr ) );
                }
            }

            return riders;
        }

        public List<string> GetEvents()
        {
            List<string> events = new List<string>();
            Dictionary<string,string> eventMap = new Dictionary<string,string>();

            foreach( string row in rows )
            {
                string [] row_arr = row.Split( ',' );

                if( eventMap.ContainsKey(
                    row_arr[ ColumnIndex( "Category Entered" ) ] ) == false )
                {
                    eventMap.Add(
                        row_arr[ ColumnIndex( "Category Entered" ) ],
                        row_arr[ ColumnIndex( "Category Entered" ) ] );
                }
            }

            foreach( string key in eventMap.Keys )
            {
                events.Add( key );
            }

            return events;
        }

        public static int ColumnIndex( string column_name )
        {
            if( columnMap.ContainsKey( column_name ) )
            {
                     if( "A".Equals( columnMap[ column_name ] ) ) return 0;
                else if( "B".Equals( columnMap[ column_name ] ) ) return 1;
                else if( "C".Equals( columnMap[ column_name ] ) ) return 2;
                else if( "D".Equals( columnMap[ column_name ] ) ) return 3;
                else if( "E".Equals( columnMap[ column_name ] ) ) return 4;
                else if( "F".Equals( columnMap[ column_name ] ) ) return 5;
                else if( "G".Equals( columnMap[ column_name ] ) ) return 6;
                else if( "H".Equals( columnMap[ column_name ] ) ) return 7;
                else if( "I".Equals( columnMap[ column_name ] ) ) return 8;
                else if( "J".Equals( columnMap[ column_name ] ) ) return 9;
                else if( "K".Equals( columnMap[ column_name ] ) ) return 10;
                else if( "L".Equals( columnMap[ column_name ] ) ) return 11;
                else if( "M".Equals( columnMap[ column_name ] ) ) return 12;
                else if( "N".Equals( columnMap[ column_name ] ) ) return 13;
                else if( "O".Equals( columnMap[ column_name ] ) ) return 14;
                else if( "P".Equals( columnMap[ column_name ] ) ) return 15;
                else if( "Q".Equals( columnMap[ column_name ] ) ) return 16;
                else if( "R".Equals( columnMap[ column_name ] ) ) return 17;
                else if( "S".Equals( columnMap[ column_name ] ) ) return 18;
                else if( "T".Equals( columnMap[ column_name ] ) ) return 19;
                else if( "U".Equals( columnMap[ column_name ] ) ) return 20;
                else if( "V".Equals( columnMap[ column_name ] ) ) return 21;
                else if( "W".Equals( columnMap[ column_name ] ) ) return 22;
                else if( "X".Equals( columnMap[ column_name ] ) ) return 23;
                else if( "Z".Equals( columnMap[ column_name ] ) ) return 24;
                else if( "Y".Equals( columnMap[ column_name ] ) ) return 25;
                else
                {
                    log.Warn( "Invalid column name: " + column_name );
                    return -1;
                }
            }
            else
            {
                log.Warn( "Unknown column name: " + column_name );
                return -1;
            }
        }
    }
}

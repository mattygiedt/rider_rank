using System;
using System.Xml;
using System.Text;
using System.Collections;
using System.Configuration;

//
//  http://msdn.microsoft.com/en-us/library/2tw134k3.aspx#4
//  http://www.codeproject.com/KB/dotnet/mysteriesofconfiguration.aspx
//

namespace com.mattygiedt
{
    public class RiderDataElement : ConfigurationElement
    {
        [ConfigurationProperty( "name", IsRequired=true )]
        public string Name
        {
            get { return (string)this[ "name" ]; }
        }

        [ConfigurationProperty( "discipline", IsRequired=true )]
        public string Discipline
        {
            get { return (string)this[ "discipline" ]; }
        }

        [ConfigurationProperty( "rider_type", IsRequired=true )]
        public string RiderType
        {
            get { return (string)this[ "rider_type" ]; }
        }

        [ConfigurationProperty( "data_source", IsRequired=true )]
        public string DataSource
        {
            get { return (string)this[ "data_source" ]; }
        }
    }

    public class ColumnElement : ConfigurationElement
    {
        [ConfigurationProperty( "name", IsRequired=true )]
        public string Name
        {
            get { return (string)this[ "name" ]; }
        }

        [ConfigurationProperty( "column_id", IsRequired=true )]
        public string ColumnID
        {
            get { return (string)this[ "column_id" ]; }
        }
    }

    public class EventElement : ConfigurationElement
    {
        [ConfigurationProperty( "name", IsRequired=true )]
        public string Name
        {
            get { return (string)this[ "name" ]; }
        }

        [ConfigurationProperty( "output", IsRequired=true )]
        public string Output
        {
            get { return (string)this[ "output" ]; }
        }

        [ConfigurationProperty( "rider_type", IsRequired=true )]
        public string RiderType
        {
            get { return (string)this[ "rider_type" ]; }
        }

        [ConfigurationProperty( "discipline", IsRequired=true )]
        public string Discipline
        {
            get { return (string)this[ "discipline" ]; }
        }

        [ConfigurationProperty( "age", IsRequired=true )]
        public string Age
        {
            get { return (string)this[ "age" ]; }
        }

        [ConfigurationProperty( "categories", IsRequired=true )]
        public string Categories
        {
            get { return (string)this[ "categories" ]; }
        }
    }

    [ConfigurationCollection( typeof(RiderDataElement), AddItemName="rider_data", CollectionType=ConfigurationElementCollectionType.BasicMap )]
    public class RankSystemConfig : ConfigurationElementCollection
    {
        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.BasicMap; }
        }

        protected override string ElementName
        {
            get { return "rider_data"; }
        }

        public RiderDataElement this[ int index ]
        {
            get { return (RiderDataElement)base.BaseGet( index ); }
            set
            {
                if( base.BaseGet( index ) != null )
                {
                    base.BaseRemoveAt( index );
                }

                base.BaseAdd( index, value );
            }
        }

        public new RiderDataElement this[ string name ]
        {
            get { return (RiderDataElement)base.BaseGet( name ); }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new RiderDataElement();
        }

        protected override object GetElementKey( ConfigurationElement element )
        {
            return (element as RiderDataElement).Name;
        }
    }

    [ConfigurationCollection( typeof(ColumnElement), AddItemName="column", CollectionType=ConfigurationElementCollectionType.BasicMap )]
    public class BikeRegConfig : ConfigurationElementCollection
    {
        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.BasicMap; }
        }

        protected override string ElementName
        {
            get { return "column"; }
        }

        public ColumnElement this[ int index ]
        {
            get { return (ColumnElement)base.BaseGet( index ); }
            set
            {
                if( base.BaseGet( index ) != null )
                {
                    base.BaseRemoveAt( index );
                }

                base.BaseAdd( index, value );
            }
        }

        public new ColumnElement this[ string name ]
        {
            get { return (ColumnElement)base.BaseGet( name ); }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new ColumnElement();
        }

        protected override object GetElementKey( ConfigurationElement element )
        {
            return (element as ColumnElement).Name;
        }
    }

    [ConfigurationCollection( typeof(EventElement), AddItemName="event", CollectionType=ConfigurationElementCollectionType.BasicMap )]
    public class EventsConfig : ConfigurationElementCollection
    {
        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.BasicMap; }
        }

        protected override string ElementName
        {
            get { return "event"; }
        }

        public EventElement this[ int index ]
        {
            get { return (EventElement)base.BaseGet( index ); }
            set
            {
                if( base.BaseGet( index ) != null )
                {
                    base.BaseRemoveAt( index );
                }

                base.BaseAdd( index, value );
            }
        }

        public new EventElement this[ string name ]
        {
            get { return (EventElement)base.BaseGet( name ); }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new EventElement();
        }

        protected override object GetElementKey( ConfigurationElement element )
        {
            return (element as EventElement).Name;
        }
    }

    public class RiderRankConfiguration : ConfigurationSection
    {
        public RiderRankConfiguration() { }

        [ConfigurationProperty( "output_dir", IsRequired=true )]
        public string OutputDir
        {
            get { return (string)this[ "output_dir" ]; }
        }

        [ConfigurationProperty( "datetime_format", IsRequired=true )]
        public string DatetimeFormat
        {
            get { return (string)this[ "datetime_format" ]; }
        }

        [ConfigurationProperty( "rank_system" )]
        public RankSystemConfig RankSystemConfig
        {
            get { return (RankSystemConfig)this[ "rank_system" ]; }
        }

        [ConfigurationProperty( "bikereg" )]
        public BikeRegConfig BikeRegConfig
        {
            get { return (BikeRegConfig)this[ "bikereg" ]; }
        }

        [ConfigurationProperty( "events" )]
        public EventsConfig EventsConfig
        {
            get { return (EventsConfig)this[ "events" ]; }
        }
    }
}
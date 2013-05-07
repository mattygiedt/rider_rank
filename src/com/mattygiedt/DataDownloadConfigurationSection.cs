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
    public class DataSourceElement : ConfigurationElement
    {
        public string Name
        {
            get { return Gender + "." + Discipline + "." + Category; }
        }

        [ConfigurationProperty( "uri", IsRequired=true )]
        public string URI
        {
            get { return (string)this[ "uri" ]; }
        }

        [ConfigurationProperty( "gender", IsRequired=true )]
        public string Gender
        {
            get { return (string)this[ "gender" ]; }
        }

        [ConfigurationProperty( "discipline", IsRequired=true )]
        public string Discipline
        {
            get { return (string)this[ "discipline" ]; }
        }

        [ConfigurationProperty( "category", IsRequired=true )]
        public string Category
        {
            get { return (string)this[ "category" ]; }
        }
    }

    public class OutputFileElement : ConfigurationElement
    {
        [ConfigurationProperty( "name", IsRequired=true )]
        public string Name
        {
            get { return (string)this[ "name" ]; }
        }

        [ConfigurationProperty( "format", IsRequired=true )]
        public string Format
        {
            get { return (string)this[ "format" ]; }
        }

        [ConfigurationProperty( "data_source_type", IsRequired=true )]
        public string DataSourceType
        {
            get { return (string)this[ "data_source_type" ]; }
        }

        [ConfigurationProperty( "data_source_group" )]
        public DataSourceGroup DataSourceGroup
        {
            get { return (DataSourceGroup)this[ "data_source_group" ]; }
        }
    }

    [ConfigurationCollection( typeof(DataSourceElement), AddItemName="data_source", CollectionType=ConfigurationElementCollectionType.BasicMap )]
    public class DataSourceGroup : ConfigurationElementCollection
    {
        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.BasicMap; }
        }

        protected override string ElementName
        {
            get { return "data_source"; }
        }

        public DataSourceElement this[ int index ]
        {
            get { return (DataSourceElement)base.BaseGet( index ); }
            set
            {
                if( base.BaseGet( index ) != null )
                {
                    base.BaseRemoveAt( index );
                }

                base.BaseAdd( index, value );
            }
        }

        public new DataSourceElement this[ string name ]
        {
            get { return (DataSourceElement)base.BaseGet( name ); }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new DataSourceElement();
        }

        protected override object GetElementKey( ConfigurationElement element )
        {
            return (element as DataSourceElement).Name;
        }
    }

    [ConfigurationCollection( typeof(OutputFileElement), AddItemName="output_file", CollectionType=ConfigurationElementCollectionType.BasicMap )]
    public class OutputFileGroup : ConfigurationElementCollection
    {
        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.BasicMap; }
        }

        protected override string ElementName
        {
            get { return "output_file"; }
        }

        public OutputFileElement this[ int index ]
        {
            get { return (OutputFileElement)base.BaseGet( index ); }
            set
            {
                if( base.BaseGet( index ) != null )
                {
                    base.BaseRemoveAt( index );
                }

                base.BaseAdd( index, value );
            }
        }

        public new OutputFileElement this[ string name ]
        {
            get { return (OutputFileElement)base.BaseGet( name ); }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new OutputFileElement();
        }

        protected override object GetElementKey( ConfigurationElement element )
        {
            return (element as OutputFileElement).Name;
        }
    }

    public class DataDownloadConfiguration : ConfigurationSection
    {
        public DataDownloadConfiguration() { }

        [ConfigurationProperty( "output_file_group" )]
        public OutputFileGroup OutputFileGroup
        {
            get { return (OutputFileGroup)this[ "output_file_group" ]; }
        }
    }
}
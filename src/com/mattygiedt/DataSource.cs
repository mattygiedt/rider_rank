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
    public abstract class DataSource
    {
        protected string uri;
        protected string gender;
        protected string discipline;
        protected string category;

        public abstract void LoadRiders( string uri, string sex, string discipline, string category );
        public abstract void SaveRidersToCSV( string file, bool printHeader );
        public abstract void Join( List<BaseRider> otherRiders );
    }
}

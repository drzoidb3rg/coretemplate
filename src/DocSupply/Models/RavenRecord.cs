using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocSupply.Models
{
    public abstract class RavenRecord
    {
        string _id;
        public string Id
        {
            get
            {
                if (String.IsNullOrEmpty(_id))
                    _id = this.GetType().Name + "/";

                return _id;
            }
            set
            {
                _id = value;
            }
        }

        public string GetIntegerId()
        {
            if (String.IsNullOrEmpty(_id))
                return "";

            var name = this.GetType().Name;

            return _id.Replace(name + "/", "");
        }

    }
}

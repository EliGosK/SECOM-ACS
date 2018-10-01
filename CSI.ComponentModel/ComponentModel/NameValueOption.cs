using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSI.ComponentModel
{
    public class NameValueOptions : List<NameValueOption>
    {

    }

    public class NameValueOption : NameValueOption<string>
    {
        public NameValueOption(string name) : base(name, name) { }
        public NameValueOption(string name, string value) : base(name, value) { }
        public NameValueOption(string key,string name, string value) : base(key,name, value) { }
    }

    public class NameValueOption<TValue>
    {
        public NameValueOption() { }
        public NameValueOption(string name, TValue value) : this(name,name,value)
        {
          
        }

        public NameValueOption(string key,string name, TValue value)
        {
            this.Key = key;
            this.Name = name;
            this.Value = value;
        }

        public string Key { get; set; }
        public string Name { get; set; }
        public TValue Value { get; set; }
    }
}

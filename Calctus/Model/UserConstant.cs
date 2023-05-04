using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shapoco.Calctus.Model {
    class UserConstant {
        public string Id { get; set; }
        public string ValueString { get; set; }
        public string Description { get; set; }

        public UserConstant(string id, string valueStr, string desc) {
            Id = id;
            ValueString = valueStr;
            Description = desc;
        }   
    }
}

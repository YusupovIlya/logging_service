using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Buffer.Service;

public class LogEntity
{
    public Guid user_id { get; set; }

    public string endpoint { get; set; }

    public string ip { get; set; }

    public string action_description { get; set; }
}
﻿using System.Collections.Generic;
using Microsoft.Azure.Devices.Applications.RemoteMonitoring.Common.Models;

namespace Microsoft.Azure.Devices.Applications.RemoteMonitoring.DeviceAdmin.Infrastructure.Models
{
    public class NameCacheEntity
    {
        public string Name { get; set; }
        public List<Parameter> Parameters { get; set; }
        public string Description { get; set; }
        public string UserName { get; set; }
    }
}

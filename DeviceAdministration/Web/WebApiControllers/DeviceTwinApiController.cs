﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Azure.Devices.Applications.RemoteMonitoring.Common.Extensions;
using Microsoft.Azure.Devices.Applications.RemoteMonitoring.DeviceAdmin.Infrastructure.BusinessLogic;
using Microsoft.Azure.Devices.Applications.RemoteMonitoring.DeviceAdmin.Infrastructure.Repository;
using Microsoft.Azure.Devices.Applications.RemoteMonitoring.DeviceAdmin.Web.Security;
using Microsoft.Azure.Devices.Shared;
using Microsoft.Azure.Devices.Applications.RemoteMonitoring.DeviceAdmin.Web.Models;

namespace Microsoft.Azure.Devices.Applications.RemoteMonitoring.DeviceAdmin.Web.WebApiControllers
{
    [RoutePrefix("api/v1/devices")]
    public class DeviceTwinApiController : WebApiControllerBase
    {
        private INameCacheLogic _nameCacheLogic;
        private IDeviceRegistryCrudRepository _deviceRepository;

        public DeviceTwinApiController(INameCacheLogic nameCacheLogic, IDeviceRegistryCrudRepository deviceRepositor)
        {
            this._nameCacheLogic = nameCacheLogic;
            _deviceRepository = deviceRepositor;
        }

        [HttpGet]
        [Route("{deviceId}/twin/desired")]
        [WebApiRequirePermission(Permission.ViewDevices)]
        public async Task<HttpResponseMessage> GetDeviceTwinDesired(string deviceId)
        {
            var twin = await this._deviceRepository.GetTwinAsync(deviceId);
            IEnumerable<KeyValuePair<string, TwinCollectionExtension.TwinValue>> flattenReportedTwin = twin.Properties.Reported.AsEnumerableFlatten("reported.").Where(t => !t.Key.IsReservedTwinName());
            IEnumerable<KeyValuePair<string, TwinCollectionExtension.TwinValue>> flattenTwin = twin.Properties.Desired.AsEnumerableFlatten("desired.").Where(t => !t.Key.IsReservedTwinName());
            return await GetServiceResponseAsync<dynamic>(async () =>
            {
                return await Task.FromResult(new { desired = flattenTwin, reported = flattenReportedTwin });
            });
        }

        [HttpGet]
        [Route("{deviceId}/twin/tag")]
        [WebApiRequirePermission(Permission.ViewDevices)]
        public async Task<HttpResponseMessage> GetDeviceTwinTag(string deviceId)
        {
            var twin = await this._deviceRepository.GetTwinAsync(deviceId);
            IEnumerable<KeyValuePair<string, TwinCollectionExtension.TwinValue>> flattenTwin = twin.Tags.AsEnumerableFlatten("tags.").Where(t => !t.Key.IsReservedTwinName());
            return await GetServiceResponseAsync<IEnumerable<KeyValuePair<string, TwinCollectionExtension.TwinValue>>>(async () =>
            {
                return await Task.FromResult(flattenTwin);
            });
        }

        [HttpPut]
        [Route("{deviceId}/twin/desired")]
        [WebApiRequirePermission(Permission.ViewDevices)]
        public async Task UpdateDeviceTwinDesiredProps(string deviceId, IEnumerable<PropertyViewModel> newtwin)
        {

            Twin updatetwin = new Twin();
            foreach (var twin in newtwin)
            {
                if (String.IsNullOrEmpty(twin.Key))
                {
                    continue;
                }
                var key = twin.Key;
                if (key.ToLower().StartsWith("desired."))
                {
                    key = key.Substring(8);
                }
                setTwinProperties(twin, updatetwin.Properties.Desired, key);
                var addnametask = _nameCacheLogic.AddNameAsync(twin.Key);
            }
            var exist = await _deviceRepository.GetTwinAsync(deviceId);
            exist.Properties.Desired = updatetwin.Properties.Desired;
            exist.ETag = "*";
            await _deviceRepository.UpdateTwinAsync(deviceId, exist);
        }

        [HttpPut]
        [Route("{deviceId}/twin/tag")]
        [WebApiRequirePermission(Permission.ViewDevices)]
        public async Task UpdateDeviceTwinTags(string deviceId, IEnumerable<PropertyViewModel> newtwin)
        {
            Twin updatetwin = new Twin();
            foreach (var twin in newtwin)
            {
                if (String.IsNullOrEmpty(twin.Key))
                {
                    continue;
                }
                var key = twin.Key;
                if (key.ToLower().StartsWith("tags."))
                {
                    key = key.Substring(5);
                }
                setTwinProperties(twin, updatetwin.Tags, key);
                var addnametask = _nameCacheLogic.AddNameAsync(twin.Key);
            }
            var exist = await _deviceRepository.GetTwinAsync(deviceId);
            if (string.IsNullOrWhiteSpace(updatetwin.Tags.Get(Constants.DeviceUserTagName)?.ToString()))
            {
                updatetwin.Tags.Set(Constants.DeviceUserTagName, exist.Tags.Get(Constants.DeviceUserTagName)as string);
            }
            exist.Tags = updatetwin.Tags;
            exist.ETag = "*";
            await _deviceRepository.UpdateTwinAsync(deviceId, exist);
        }

        private void setTwinProperties(PropertyViewModel twin, TwinCollection prop, string key)
        {
            if (twin.IsDeleted)
            {
                prop.Set(key, null);
            }
            else
            {
                switch (twin.DataType)
                {
                    case Infrastructure.Models.TwinDataType.String:
                        string valueString = twin.Value.Value.ToString();
                        prop.Set(key, valueString);
                        break;
                    case Infrastructure.Models.TwinDataType.Number:
                        int valueInt;
                        float valuefloat;
                        if (int.TryParse(twin.Value.Value.ToString(), out valueInt))
                        {
                            prop.Set(key, valueInt);
                        }
                        else if (float.TryParse(twin.Value.Value.ToString(), out valuefloat))
                        {
                            prop.Set(key, valuefloat);
                        }
                        else
                        {
                            prop.Set(key, twin.Value.Value as string);
                        }
                        break;
                    case Infrastructure.Models.TwinDataType.Boolean:
                        bool valueBool;
                        if (bool.TryParse(twin.Value.Value.ToString(), out valueBool))
                        {
                            prop.Set(key, valueBool);
                        }
                        else
                        {
                            prop.Set(key, twin.Value.Value as string);
                        }
                        break;
                }
            }
        }
    }
}

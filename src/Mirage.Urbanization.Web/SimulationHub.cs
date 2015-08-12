using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Mirage.Urbanization.Simulation;

namespace Mirage.Urbanization.Web
{
    public struct ClientZoneInfo
    {
        public string key { get; set; }
        public int bitmapLayerOne { get; set; }
        public int bitmapLayerTwo { get; set; }
        public ClientZonePoint point { get; set; }
        public string color { get; set; }
        public string GetIdentityString() => $"{key}_{bitmapLayerOne}_{bitmapLayerTwo}_{point.GetIdentityString()}_{color}";
    }

    public struct ClientZonePoint
    {
        public int x { get; set; }
        public int y { get; set; }
        public string GetIdentityString() => $"{x}_{y}";
    }

    public class SimulationHub : Hub
    {
        public void SubmitZoneInfos(List<ClientZoneInfo> clientZoneInfos)
        {
            Clients.All.submitZoneInfos(clientZoneInfos);
        }
    }
}

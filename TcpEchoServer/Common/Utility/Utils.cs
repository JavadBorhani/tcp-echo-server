using CommandLine;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Common.Utility
{
    public static class Utils
    {
        public static T ReadArguments<T>(string[] args) where T : new()
        {
            ParserResult<T> result = Parser.Default.ParseArguments<T>(args);
            T arguments = new T();
            if (result.Tag == ParserResultType.Parsed)
                arguments = ((Parsed<T>)result).Value;
            return arguments;
        }


        public static IPEndPoint ReadIPAddress(string ipEndPoint)
        {
            string[] ipAdressAndPort = ipEndPoint.Split(":");
            string ip = ipAdressAndPort[0].Trim();
            int port = int.Parse(ipAdressAndPort[1].Trim());
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            return endPoint;
        }

        public static List<IPEndPoint> GenerateIPAddressWithMultiplePorts(string ipAddressWithPorts)
        {
            string[] ipAdressesAndPorts = ipAddressWithPorts.Split(":");
            var ipAddress = IPAddress.Parse(ipAdressesAndPorts[0]);

            List<IPEndPoint> ipEndPoints = new List<IPEndPoint>(ipAdressesAndPorts.Length - 1);

            for (int i = 1; i < ipAdressesAndPorts.Length; ++i)
            {
                int port = int.Parse(ipAdressesAndPorts[i].Trim());
                IPEndPoint endPoint = new IPEndPoint(ipAddress, port);
                ipEndPoints.Add(endPoint);
            }
                
            return ipEndPoints;
        }

        public static void NoAwait(this Task task) { }

    }
}

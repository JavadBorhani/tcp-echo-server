using CommandLine;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Common.Utility
{
    public static class Utils
    {

        public static IPEndPoint ParseIPAddress(string stringAddress)
        {
            string[] ipAdressAndPort = stringAddress.Trim().Split(":");
            string ip = ipAdressAndPort[0];
            int port = int.Parse(ipAdressAndPort[1]);
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(ip), port);

            return endPoint;
        }

        public static List<IPEndPoint> ParseMultipleIPAddress(IEnumerable<string> multipleIpAddress)
        {
            List<IPEndPoint> ipEndPoints = new List<IPEndPoint>();

            foreach(var ipAddress in multipleIpAddress)
            {
                IPEndPoint endPoint = ParseIPAddress(ipAddress);
                ipEndPoints.Add(endPoint);
            }

            return ipEndPoints;
        }

        public static T ReadArguments<T>(string[] args) where T : new()
        {
            ParserResult<T> result = Parser.Default.ParseArguments<T>(args);
            T arguments = new T();
            if (result.Tag == ParserResultType.Parsed)
                arguments = ((Parsed<T>)result).Value;
            return arguments;
        }


        public static void NoAwait(this Task task) { }

    }
}

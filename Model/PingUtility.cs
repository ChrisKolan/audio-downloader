﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    internal static class PingUtility
    {
        private static long _roundtripTime;

        public static bool Pinger(out string pingException)
        {
            Ping pingSender = new Ping();
            PingOptions options = new PingOptions();
            var hostNameOrAddress = "youtube.com";

            // Use the default Ttl value which is 128,
            // but change the fragmentation behavior.
            options.DontFragment = true;

            // Create a buffer of 32 bytes of data to be transmitted.
            string data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
            byte[] buffer = Encoding.ASCII.GetBytes(data);
            int timeout = 250;
            PingReply reply;

            try
            {
                reply = pingSender.Send(hostNameOrAddress, timeout, buffer, options);
            }
            catch (PingException exception)
            {
                pingException = exception.Message;
                pingSender.Dispose();
                _roundtripTime = -1;
                return false;
            }
            if (reply.Status != IPStatus.Success)
            {
                pingException = Localization.Properties.Resources.SendingOfAnInternetControlMessageProtocolNotSuccessful;
                _roundtripTime = -1;
                pingSender.Dispose();
                return false;
            }
            pingException = Localization.Properties.Resources.SendingOfAnInternetControlMessageProtocolSuccessful;
            _roundtripTime = reply.RoundtripTime;
            pingSender.Dispose();
            return true;
        }

        public static string AddPingToHelpButtonToolTip(string helpButtonToolTip)
        {
            if (helpButtonToolTip == null)
            {
                return string.Join(Environment.NewLine, ComposePingStatus().ToArray());
            }
            var pingData = ComposePingStatus();
            var helpButtonToolTipList = helpButtonToolTip.Split( new[] { Environment.NewLine }, StringSplitOptions.None).ToList();
            var index = helpButtonToolTipList.FindIndex(a => a == Localization.Properties.Resources.AdvancedInformationAvailableYouTubeFileFormats);
            if (index > 0)
            {
                var advancedInformationCount = helpButtonToolTipList.Count - index;
                helpButtonToolTipList.InsertRange(7, pingData);
                if (helpButtonToolTipList.Count > advancedInformationCount + 12)
                {
                    helpButtonToolTipList.RemoveRange(10, 3);
                }
                return string.Join(Environment.NewLine, helpButtonToolTipList.ToArray());
            }

            var listWithoutAdvancedInformation = new List<string>();
            listWithoutAdvancedInformation = helpButtonToolTipList.Concat(pingData).ToList();

            if (listWithoutAdvancedInformation.Count > 12)
            {
                listWithoutAdvancedInformation.RemoveRange(8, 3);
            }
            return string.Join(Environment.NewLine, listWithoutAdvancedInformation.ToArray());
        }

        private static List<string> ComposePingStatus()
        {
            var status = (_roundtripTime == -1) ? "Offline" : "Online";
            var pingData = new List<string>
            {
                "===========================",
                "Status \t\t   |\t" + status,
                Localization.Properties.Resources.RoundtripTime + _roundtripTime + " [ms]",
            };

            return pingData;
        }
    }
}

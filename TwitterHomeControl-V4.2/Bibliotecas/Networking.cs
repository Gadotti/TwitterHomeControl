using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using GHIElectronics.NETMF.FEZ;
using GHIElectronics.NETMF.Net;
using GHIElectronics.NETMF.Net.Sockets;
using GHIElectronics.NETMF.Net.NetworkInformation;
using System.Text;
using Socket = GHIElectronics.NETMF.Net.Sockets.Socket;

namespace Networking
{

    /// <summary>
    /// This class lets you connect to a TCP socket and send and receive string data. 
    /// I added the ability to connect using strings for the addresses instead of byte arrays. 
    /// I also added the dhcp connection feature included in the new SDK. 
    /// </summary>
    public static class EthernetW5100
    {
        static Socket Socket;
        public static int SendTimeout = 3000;

        /// <summary>
        /// Indicates how many bytes are available to receive.
        /// </summary>
        public static long Available
        {
            get
            {
                return Socket.Available;
            }
        }

        static bool myConnected = false;


        /// <summary>
        /// Indicates if the socket is currently connected to anything.
        /// </summary>
        public static bool Connected
        {
            get
            {
                return myConnected;
            }
        }


        /// <summary>
        /// Initalizes the network shield to use a DHCP connection and the provided MAC address.
        /// </summary>        
        /// <param name="mac">
        ///  This must be a unique MAC address value represented in the standard colon seperated hex value format.
        ///  Here is a resource for generating random MAC addresses: http://www.macvendorlookup.com/
        /// </param>
        public static void Initialize(string mac)
        {
            Initialize(MacAddressStringToBytes(mac));
        }

        /// <summary>
        /// Initalizes the network shield to use a DHCP connection and the provided MAC address.
        /// </summary>        
        /// <param name="mac">
        ///  This must be a unique MAC address value represented as a byte array.
        ///  Here is a resource for generating random MAC addresses: http://www.macvendorlookup.com/
        /// </param>
        public static void Initialize(byte[] mac)
        {
            // Configure the ethernet port
            WIZnet_W5100.Enable(SPI.SPI_module.SPI1, (Cpu.Pin)FEZ_Pin.Digital.Di10, (Cpu.Pin)FEZ_Pin.Digital.Di9, true); // WIZnet interface on FEZ Panda
            Dhcp.EnableDhcp(mac, "FEZDomino");
        }

        /// <summary>
        /// Initalizes the network shield to use a static IP connection and the provided MAC address.
        /// </summary>        
        /// <param name="ip">The IP address assigned to your device.</param>
        /// <param name="subnet">Your connection's subnet address</param>
        /// <param name="gateway">You connection'sgateway address</param>    
        /// <param name="mac">
        ///  This must be a unique MAC address value represented in the standard colon seperated hex format.
        ///  Here is a resource for generating random MAC addresses: http://www.macvendorlookup.com/
        /// </param>
        /// <param name="dns">The IP address of your DNS server</param>
        public static void Initialize(string ip, string subnet, string gateway, string mac, string dns)
        {
            Initialize(AddressStringToBytes(ip)
                        , AddressStringToBytes(subnet)
                        , AddressStringToBytes(gateway)
                        , MacAddressStringToBytes(mac)
                        , AddressStringToBytes(dns));
        }

        /// <summary>
        /// Initalizes the network shield to use a static IP connection and the provided MAC address.
        /// </summary>      
        /// <param name="ip">The IP address assigned to your device.</param>
        /// <param name="subnet">Your connection's subnet address</param>
        /// <param name="gateway">You connection'sgateway address</param>    
        /// <param name="mac">
        /// This must be a unique MAC address value represented as a byte array.
        ///  Here is a resource for generating random MAC addresses: http://www.macvendorlookup.com/
        /// </param>
        /// <param name="dns">The IP address of your DNS server</param>
        public static void Initialize(byte[] ip, byte[] subnet, byte[] gateway, byte[] mac, byte[] dns)
        {
            // Configure the ethernet port
            WIZnet_W5100.Enable(SPI.SPI_module.SPI1, (Cpu.Pin)FEZ_Pin.Digital.Di10, (Cpu.Pin)FEZ_Pin.Digital.Di9, true); // WIZnet interface on FEZ Panda
            NetworkInterface.EnableStaticIP(ip, subnet, gateway, mac);
            NetworkInterface.EnableStaticDns(dns);
        }

        /// <summary>
        /// Initalizes a tcp connectio to the given IP address and port.
        /// </summary>
        /// <param name="destIp">The IP address you are connecting to.</param>
        /// <param name="port">The port you are connecting to.</param>
        public static void ConnectTCP(string destIp, int port)
        {
            ConnectTCP(AddressStringToBytes(destIp), port);
        }

        /// <summary>
        /// Initalizes a tcp connectio to the given IP address and port.
        /// </summary>
        /// <param name="destIp">The IP address you are connecting to.</param>
        /// <param name="port">The port you are connecting to.</param>
        public static void ConnectTCP(byte[] destIp, int port)
        {
            if (Connected)
                Disconnect();

            Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Socket.SendTimeout = SendTimeout;
            Socket.Connect(new IPEndPoint(new IPAddress(destIp), port));
            myConnected = true;
        }

        /// <summary>
        /// Disconnects the active socket.
        /// </summary>
        public static void Disconnect()
        {
            myConnected = false;

            try
            {
                Socket.Close();
            }
            catch { }
        }

        /// <summary>
        /// Send string data to the active socket connection.
        /// </summary>
        /// <param name="data">Data string to send.</param>
        public static void Send(string data)
        {
            try
            {
                Socket.Send(Encoding.UTF8.GetBytes(data));
            }
            catch (Exception ex)
            {
                myConnected = false;
                throw ex;
            }
        }

        /// <summary>
        /// Returns any data that may be in the buffer as a string.
        /// </summary>        
        /// <returns>Data received as a string.</returns>
        public static string Receive()
        {
            if (Available > 0)
            {
                byte[] buffer = new byte[Available];
                try
                {
                    Socket.Receive(buffer);
                }
                catch (Exception ex)
                {
                    myConnected = false;
                    throw ex;
                }
                return new String(Encoding.UTF8.GetChars(buffer));
            }
            else
                return "";
        }


        /// <summary>
        /// Converts an IP address string to a byte array
        /// </summary>
        /// <param name="data">IP address you wish to convert.</param>      
        /// <returns>The IP address as a byte array.</returns>
        private static byte[] AddressStringToBytes(string address)
        {
            return DelimitedStringToBytes(address, '.');
        }

        /// <summary>
        /// Converts a MAC address string to a byte array
        /// </summary>
        /// <param name="data">The MAC address you wish to convert.</param>      
        /// <returns>The MAC address as a byte array.</returns>
        private static byte[] MacAddressStringToBytes(string address)
        {
            return DelimitedStringToBytes(address, true, ':');
        }

        /// <summary>
        /// Converts the given string array to a byte array using the seperator to delimit the values in the string.
        /// </summary>
        /// <param name="data">The string you wish to convert.</param> 
        /// <param name="seperator">The character(s) used to delimit the data elements in the string</param>      
        /// <returns>The values inthe string as a byte array.</returns>
        private static byte[] DelimitedStringToBytes(string data, params char[] seperator)
        {

            return DelimitedStringToBytes(data, false, seperator);
        }

        /// <summary>
        /// Converts the given string array to a byte array using the seperator to delimit the values in the string.
        /// </summary>
        /// <param name="data">The string you wish to convert.</param> 
        /// <param name="HexConversion">Indicates if the values in the string are hex as opposed  decimal</param>    
        /// <param name="seperator">The character(s) used to delimit the data elements in the string</param>      
        /// <returns>The values in the string as a byte array.</returns>
        private static byte[] DelimitedStringToBytes(string data, bool HexConversion, params char[] seperator)
        {
            string[] segments = data.Split(seperator);
            byte[] returnBytes = new byte[segments.Length];

            int i = 0;
            foreach (string segment in segments)
            {
                if (HexConversion)
                    returnBytes[i] = HexStringToByte(segment);
                else
                    returnBytes[i] = byte.Parse(segment);
                i++;
            }

            return returnBytes;
        }

        //Barrowed from BlueHairBob

        /// <summary>
        /// Converts the given string representation of a hex value to a byte.
        /// </summary>
        /// <param name="hexNumber">The hex string you wish to convert.</param>     
        /// <returns>The value in the string as a byte.</returns>
        public static byte HexStringToByte(string hexNumber)
        {
            int lowDigit = 0;
            int highDigit = 0;
            if (hexNumber.Length > 2)
                throw new InvalidCastException("The number to convert is too large for a byte, or not hexadecimal");
            if (hexNumber.Length == 2)
            {
                lowDigit = hexNumber[1] - '0';
                highDigit = hexNumber[0] - '0';
            }
            else if (hexNumber.Length == 1)
            {
                lowDigit = hexNumber[0] - '0';
            }
            if (lowDigit > 9) lowDigit -= 7;
            if (lowDigit > 15) lowDigit -= 32;
            if (lowDigit > 15) throw new InvalidCastException("The number to convert is not hexadecimal");
            if (highDigit > 9) highDigit -= 7;
            if (highDigit > 15) highDigit -= 32;
            if (highDigit > 15) throw new InvalidCastException("The number to convert is not hexadecimal");
            return (byte)(lowDigit + (highDigit << 4));
        }
    }
}
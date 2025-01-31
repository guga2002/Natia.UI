using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Natia.Persistance.Model;
using Natia.Persistance.Interface;

namespace Natia.Persistance.Repositories
{
    public class UdpComunicationRepository : IUdpComunicationRepository
    {
        public UdpComunicationRepository()
        {

        }
        public async Task<string> Receive()
        {
            await Task.Delay(1);
            int port = 183;

            UdpClient server = new UdpClient(port);

            Console.WriteLine("UDP Server is listening on port " + port);

            try
            {
                while (true)
                {
                    IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
                    byte[] receivedBytes = server.Receive(ref clientEndPoint);

                    string receivedMessage = Encoding.UTF8.GetString(receivedBytes);
                    Console.WriteLine(receivedMessage);
                    return receivedMessage;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                server.Close();
            }

            return "";
        }

        public async Task<List<ExcellDataMode3l>> StartTcpServer()
        {
            await Task.Delay(1);
            int port = 183;
            TcpListener server = null;
            List<ExcellDataMode3l> person = new List<ExcellDataMode3l>();

            try
            {
                IPAddress localAddress = IPAddress.Any;
                server = new TcpListener(localAddress, port);
                server.Start();

                Console.WriteLine($"Server started on port {port}.");

                TcpClient client = null;
                try
                {
                    client = server.AcceptTcpClient();
                    Console.WriteLine("Client connected.");

                    using (NetworkStream stream = client.GetStream())
                    {
                        stream.ReadTimeout = 5000; // 5 seconds timeout
                        stream.WriteTimeout = 5000;

                        byte[] buffer = new byte[20096];
                        int bytesRead = stream.Read(buffer, 0, buffer.Length);

                        if (bytesRead > 0)
                        {
                            string receivedMessage = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                            Console.WriteLine($"Received message: {receivedMessage}");


                            person = System.Text.Json.JsonSerializer.Deserialize<List<ExcellDataMode3l>>(receivedMessage) ?? new List<ExcellDataMode3l>();


                            string response = "Message received!";
                            byte[] responseBytes = Encoding.UTF8.GetBytes(response);
                            stream.Write(responseBytes, 0, responseBytes.Length);


                            if (person != null && person.Count > 0)
                                return person;
                        }
                    }
                }
                catch (IOException ex)
                {
                    Console.WriteLine($"Timeout or network error: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Unexpected error: {ex.Message}");
                }
                finally
                {
                    client?.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Server error: {ex.Message}");
            }
            finally
            {
                server?.Stop();
            }

            return person ?? new List<ExcellDataMode3l>();
        }
    }
}
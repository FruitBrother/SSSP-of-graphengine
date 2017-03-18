using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity;
using TSLProject1;
using System.Threading;


namespace GraphEngineApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            TrinityConfig.AddServer(new Trinity.Network.ServerInfo(
                "127.0.0.1", 5304, Global.MyAssemblyPath, Trinity.Diagnostics.LogLevel.Error));
            if (args.Length >= 1 && args[0].StartsWith("-s"))
            {
                MyServer server = new MyServer();
                server.Start();
            }

            //sssp.exe -c startcell
            if (args.Length >= 2 && args[0].StartsWith("-c"))
            {
                TrinityConfig.CurrentRunningMode = RunningMode.Client;
                for(int i = 0; i<Global.ServerCount; i++)
                {
                    using (var msg = new StartSSSPMessageWriter(long.Parse(args[1].Trim())))
                    {
                        Global.CloudStorage.StartSSSPToSSSPServer(i, msg);
                    }
                }
            }

            //sssp.exe -q cellID
            if (args.Length >= 2 && args[0].StartsWith("-q"))
            {
                TrinityConfig.CurrentRunningMode = RunningMode.Client;
                var cell = Global.CloudStorage.LoadSSSPCell(int.Parse(args[1]));
                Console.WriteLine("Current vertex is {0}, the distance to the source vertex is {1}.",
                    cell.CellID, cell.distance);
                while (cell.distance > 0)
                {
                    cell = Global.CloudStorage.LoadSSSPCell(cell.parent);
                    Console.WriteLine("Current vertex is {0}, the distance to the source vertex is {1}.",
                        cell.CellID, cell.distance);
                }
            }

            //sssp.exe -g nodecount
            if (args.Length >= 2 && args[0].StartsWith("-g"))
            {
                TrinityConfig.CurrentRunningMode = RunningMode.Client;

                Random rand = new Random();
                int nodeCount = int.Parse(args[1].Trim());
                for (int i = 0; i < nodeCount; i++)
                {
                    HashSet<long> neighbors = new HashSet<long>();
                    for (int j = 0; j < 10; j++)
                    {
                        long neighbor = rand.Next(0, nodeCount);
                        if (neighbor != i)
                        {
                            neighbors.Add(neighbor);
                        }
                    }
                    Global.CloudStorage.SaveSSSPCell(i, distance: int.MaxValue, parent: -1, neighbors: neighbors.ToList());
                }
            }
        }
            
    }
}
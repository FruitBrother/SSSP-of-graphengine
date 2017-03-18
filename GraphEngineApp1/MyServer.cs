using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity;
using Trinity.Core.Lib;
using TSLProject1;
using Trinity.Network.Messaging;
namespace GraphEngineApp1
{
    internal class MyServer : TSLProject1.SSSPServerBase
    {
        public override void DistanceUpdatingHandler(DistanceUpdatingMessageReader request)
        {
            List<DistanceUpdatingMessage> DistanceUpdatingMessageList =
                new List<DistanceUpdatingMessage>();
            request.recipients.ForEach( (cellId) =>
            {
                using (var cell = Global.LocalStorage.UseSSSPCell(cellId))
                {
                    if (cell.distance > request.distance + 1)
                    {
                        cell.distance = request.distance + 1;
                        cell.parent = request.senderId;
                        Console.Write(cell.distance + " ");
                        MessageSorter sorter = new MessageSorter(cell.neighbors);

                       for (int i = 0; i< Global.ServerCount; i++)
                        {
                            DistanceUpdatingMessageWriter msg =
                                new DistanceUpdatingMessageWriter(cell.CellID.Value,
                                cell.distance, sorter.GetCellRecipientList(i));
                            Global.CloudStorage.DistanceUpdatingToSSSPServer(i, msg);
                        }
                    }
                }
            });
        }

        public override void StartSSSPHandler(StartSSSPMessageReader request)
        {
            if (Global.CloudStorage.IsLocalCell(request.root))
            {
                using (var rootCell = Global.LocalStorage.UseSSSPCell(request.root))
                {
                    rootCell.distance = 0;
                    rootCell.parent = -1;
                    MessageSorter sorter = new MessageSorter(rootCell.neighbors);

                    for (int i = 0; i < Global.ServerCount; i++)
                    {
                        DistanceUpdatingMessageWriter msg = new
                            DistanceUpdatingMessageWriter(rootCell.CellID.Value, 0,
                            sorter.GetCellRecipientList(i));
                        Global.CloudStorage.DistanceUpdatingToSSSPServer(i, msg);
                    }
                }

            }
            return;
        }

    }
}
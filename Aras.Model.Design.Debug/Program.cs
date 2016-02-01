﻿/*  
  Aras.Model provides a .NET cient library for Aras Innovator

  Copyright (C) 2015 Processwall Limited.

  This program is free software: you can redistribute it and/or modify
  it under the terms of the GNU Affero General Public License as published
  by the Free Software Foundation, either version 3 of the License, or
  (at your option) any later version.

  This program is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
  GNU Affero General Public License for more details.

  You should have received a copy of the GNU Affero General Public License
  along with this program.  If not, see http://opensource.org/licenses/AGPL-3.0.
 
  Company: Processwall Limited
  Address: The Winnowing House, Mill Lane, Askham Richard, York, YO23 3NW, United Kingdom
  Tel:     +44 113 815 3440
  Email:   support@processwall.com
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aras.Model;

namespace Aras.Model.Design.Debug
{
    class Program
    {
        static void OutputOrder(Order Order)
        {
            Console.WriteLine(Order.ItemNumber);

            Part configuredpart = Order.ConfiguredPart;

            foreach (OrderContext ordercontext in Order.Store("v_Order Context"))
            {
                if (ordercontext.Action != Item.Actions.Deleted)
                {
                    Console.WriteLine(ordercontext.Property("value").Value + "\t" + ordercontext.ValueList.Value);
                }
            }

            Console.WriteLine();

            foreach(PartBOM partbom in configuredpart.Store("Part BOM"))
            {
                if (partbom.Action != Item.Actions.Deleted)
                {
                    Console.WriteLine(partbom.Related.Property("name").Value + "\t" + partbom.Quantity.ToString());
                }
            }

            Console.WriteLine();
        }

        static String ItemNumber(int cnt)
        {
            return "DX-" + cnt.ToString().PadLeft(10, '0');
        }

        static void CreateBOM(Part Part, int cnt, int depth, Transaction Transaction)
        {
            if (depth < 1)
            {
                for(int i=0; i<10; i++)
                {
                    Part part = (Part)Part.Session.Store("Part").Create(Transaction);
                    part.ItemNumber = ItemNumber(cnt);
                    cnt++;

                    Part.Store("Part BOM").Create(part, Transaction);
                }
            }
        }

        static void Main(string[] args)
        {
            Server server = new Server("http://localhost/11SP1");
            server.LoadAssembly("Aras.Model.Design");
            Database database = server.Database("VariantsDemo11SP1");
            Session session = database.Login("admin", Server.PasswordHash("innovator"));

            using (Transaction transaction = session.BeginTransaction())
            {
                Part toplevel = (Part)session.Store("Part").Create(transaction);
                toplevel.ItemNumber = ItemNumber(1);
                
                Part bompart = (Part)session.Store("Part").Create(transaction);
                bompart.ItemNumber = ItemNumber(2);
                toplevel.Store("Part BOM").Create(bompart, transaction);

                Part bompart2 = (Part)session.Store("Part").Create(transaction);
                bompart2.ItemNumber = ItemNumber(3);
                bompart.Store("Part BOM").Create(bompart2, transaction);

                transaction.Commit();
            }
        }
    }
}

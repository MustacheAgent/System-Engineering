using System;
using System.Collections.Generic;
using System.Net;
using SnmpSharpNet;

namespace SNMP
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("1.3.6.1.2.1.1.6");
            Console.WriteLine("Введите адрес узла:");
            string key = Console.ReadLine();

            if (key[key.Length - 1] == '0')
            {
                OctetString community = new OctetString("rpublic");
                AgentParameters param = new AgentParameters(community);
                param.Version = SnmpVersion.Ver2;

                IpAddress agent = new IpAddress("192.168.9.1");
                UdpTarget target = new UdpTarget((IPAddress)agent, 161, 2000, 1);

                Pdu pdu = new Pdu(PduType.Get);
                pdu.VbList.Add(key);

                SnmpV2Packet result = (SnmpV2Packet)target.Request(pdu, param);

                if (result != null)
                {
                    Console.WriteLine("{0}: type - {1}, value - {2}",
                                result.Pdu.VbList[0].Oid.ToString(),
                                SnmpConstants.GetTypeName(result.Pdu.VbList[0].Value.Type),
                                result.Pdu.VbList[0].Value.ToString());
                }
                else
                {
                    Console.WriteLine("No response received from SNMP agent.");
                }

                target.Close();
            }
            else
            {
                SimpleSnmp snmp = new SimpleSnmp("192.168.9.1", 161, "rpublic", 2000, 1);
                Dictionary<Oid, AsnType> result = snmp.GetBulk(new string[] { key });

                if (result != null)
                {
                    foreach (var kvp in result)
                    {
                        Console.WriteLine("{0}: {1} {2}", kvp.Key.ToString(),
                                              SnmpConstants.GetTypeName(kvp.Value.Type),
                                              kvp.Value.ToString());
                    }
                }
                else
                {
                    Console.WriteLine("No response received from SNMP agent.");
                }
            }
            Console.ReadKey();
        }
    }
}

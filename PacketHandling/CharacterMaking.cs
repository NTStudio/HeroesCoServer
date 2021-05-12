using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.PacketHandling
{
    class CharacterMaking
    {
        public static void Handle(GameClient GC, byte[] Data)
        {            
            string CharName = "";
            for (int i = 0; i < 16; i++)
                if (Data[20 + i] != 0)
                    CharName += Convert.ToChar(Data[20 + i]);

            ushort Body = BitConverter.ToUInt16(Data, 52);
            byte Job = Data[54];
            if (GC.ValidName(CharName))
            {
                GC.SendPacket(Packets.PopUpMessage(GC.MessageID, Database.CreateCharacter(GC.AuthInfo.Account, CharName, Body, Job)));
            }
            else
                GC.SendPacket(Packets.PopUpMessage(GC.MessageID, "Invalid Character Name!"));
        }
    }
}

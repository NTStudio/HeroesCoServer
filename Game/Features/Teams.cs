using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Features
{
    public class Team
    {
        public ArrayList Members = new ArrayList(5);
        public bool ItemsOn = false;
        public bool Forbid = false;
        public DateTime LastCoords = DateTime.Now;

        public Team(Game.Character C)
        {
            Members.Add(C);
            C.TeamLeader = true;
            C.MyClient.SendPacket(Packets.TeamPacket(C.EntityID, 0));
            C.StatEff.Add(Game.StatusEffectEn.TeamLeader);
        }
        public void LeaderCoords()
        {
            Game.Character _Leader = Leader;
            if (_Leader != null)
            {
                byte[] P = Packets.GeneralData(0, _Leader.EntityID, _Leader.Loc.X, _Leader.Loc.Y, 101);
                foreach (Game.Character Member in Members)
                    if (!Member.isAvatar)
                    if (Member != null && Member.MyClient != null && Member != _Leader && Member.Loc.Map == _Leader.Loc.Map)
                        Member.MyClient.SendPacket(P);
            }
            LastCoords = DateTime.Now;
        }
        public void Message(Game.Character C, byte[] Data)
        {
            if (Members.Contains(C))
            {
                foreach (Game.Character P in Members)
                    if (!P.isAvatar)
                    if (P != C)
                    {
                        P.MyClient.SendPacket2(Data);
                       
                    }
            }
        }
        public void Message(byte[] Data)
        {
            foreach (Game.Character P in Members)
                P.MyClient.SendPacket2(Data);
        }
        public Game.Character Leader
        {
            get
            {
                foreach (Game.Character C in Members)
                    if (!C.isAvatar)
                    if (C != null)
                        if (C.MyClient != null)
                            if (C.TeamLeader)
                                return C;
                return null;
            }
        }
        public void Dismiss(Game.Character C)
        {
            if (C == Leader)
            {
                C.StatEff.Remove(Game.StatusEffectEn.TeamLeader);
                foreach (Game.Character P in Members)
                {
                    if (!P.isAvatar)
                    if (P.MyClient != null)
                    {
                        P.MyClient.SendPacket(Packets.TeamPacket(C.EntityID, 6));
                        P.MyTeam = null;
                    }
                }
                C.TeamLeader = false;
            }
        }
        public bool Joins(Game.Character C)
        {
            if (Members.Count < Members.Capacity && !Members.Contains(C))
            {
                foreach (Game.Character P in Members)
                {
                    if (!P.isAvatar)
                    if (P.MyClient != null)
                    {
                        P.MyClient.SendPacket(Packets.PlayerJoinsTeam(C));
                        C.MyClient.SendPacket(Packets.PlayerJoinsTeam(P));
                    }
                }
                Members.Add(C);
                C.MyClient.SendPacket(Packets.PlayerJoinsTeam(C));
                C.MyTeam = this;
                return true;
            }
            return false;
        }
        public void Leaves(Game.Character C)
        {
            foreach (Game.Character P in Members)
                if (!P.isAvatar)
                if (P.MyClient != null)
                    P.MyClient.SendPacket(Packets.TeamPacket(C.EntityID, 2));
            C.MyClient.SendPacket(Packets.TeamPacket(C.EntityID, 6));
            Members.Remove(C);
            C.MyTeam = null;
        }        
    }
}

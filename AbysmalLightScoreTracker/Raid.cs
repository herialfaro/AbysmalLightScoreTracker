﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace AbysmalLightScoreTracker
{
    class RaidManager:PVEManager
    {
        public Activity[] Activity_list;

        public RaidManager(RESTClient client, DateTime reset,
        DateTime limit, List<Member> Members, string ID, byte mode) :
            base(client,reset,limit,Members,ID,mode)
        {
        }

        public void ParseRaid(string OtherID, string OtherCharacterID, string OtherType)
        {
            List<Activity> tempList = ParseActivitiesList(OtherID, OtherCharacterID, OtherType);
            Activity_list = new Activity[tempList.Count];
            int iCounter = 0;
            foreach (Activity element in tempList)
            {
                Activity_list[iCounter] = element;
                iCounter++;
            }
            GetPostGameCarnageReport();
        }

        public void GetPostGameCarnageReport()
        {
            for (int i = 0; i < Activity_list.Length; i++)
            {
                rClient.URL = "https://www.bungie.net/Platform/Destiny2/Stats/PostGameCarnageReport/" + Activity_list[i].InstanceID + "/";
                //rClient.URL = "https://www.bungie.net/Platform/Destiny2/Stats/PostGameCarnageReport/2987820189/";
                string strPGCR = rClient.makeRequest();
                StringReader PGCRreader = new StringReader(strPGCR);
                bool PGCRSTOP = false; char[] PGCRParse = new char[5];
                string PGCoutPut = string.Empty;
                jCounter = 0;

                while (true)
                {
                    while (!PGCRSTOP)
                    {
                        PGCRParse[0] = (char)PGCRreader.Read();
                        if (PGCRParse[0] == 'i')
                        {
                            PGCRParse[1] = (char)PGCRreader.Read();
                            if (PGCRParse[1] == 'p')
                            {
                                PGCRParse[2] = (char)PGCRreader.Read();
                                if (PGCRParse[2] == 'I')
                                {
                                    PGCRParse[3] = (char)PGCRreader.Read();
                                    if (PGCRParse[3] == 'd')
                                    {
                                        PGCRParse[4] = (char)PGCRreader.Read();
                                        if (PGCRParse[4] == '"')
                                        {
                                            PGCRSTOP = true;
                                        }
                                    }
                                }
                            }
                        }
                        else if (PGCRParse[0] == 'r')
                        {
                            PGCRParse[1] = (char)PGCRreader.Read();
                            if (PGCRParse[1] == 'C')
                            {
                                PGCRParse[2] = (char)PGCRreader.Read();
                                if (PGCRParse[2] == 'o')
                                {
                                    PGCRParse[3] = (char)PGCRreader.Read();
                                    if (PGCRParse[3] == 'd')
                                    {
                                        PGCRParse[4] = (char)PGCRreader.Read();
                                        if (PGCRParse[4] == 'e')
                                        {
                                            PGCRSTOP = true;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    PGCRSTOP = false;

                    if (PGCRParse[0] == 'r' && PGCRParse[1] == 'C' && PGCRParse[2] == 'o' &&
                    PGCRParse[3] == 'd' && PGCRParse[4] == 'e')
                    {
                        break;
                    }

                    while (PGCRParse[0] != '"')
                    {
                        PGCRParse[0] = (char)PGCRreader.Read();
                    }
                    PGCRParse[0] = 'a';

                    while (PGCRParse[0] != '"')
                    {
                        PGCRParse[0] = (char)PGCRreader.Read();
                        if (PGCRParse[0] == '"')
                        {
                            break;
                        }
                        PGCoutPut += PGCRParse[0];
                    }
                    CompareMembers(PGCoutPut, Activity_list[i].Playercount);
                    PGCoutPut = string.Empty;
                }
                if(jCounter >= 2)
                {
                    Activity_list[i].Compwclanmembers = true;
                }
            }
        }

        public void CompareMembers(string ID, int Playercount)
        {
            if (Playercount > 1)
            {
                foreach (Member element in Member_list)
                {
                    if (ID == element.ID)
                    {
                        if (ID != Actual_member_ID)
                        {
                            jCounter++;
                            break;
                        }
                    }
                }
            }
        }
    }
}

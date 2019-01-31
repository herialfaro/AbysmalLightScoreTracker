using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace AbysmalLightScoreTracker
{
    class GambitManager:PVEManager
    {
        public Activity[] Activity_list;

        public GambitManager(RESTClient client, DateTime reset,
        DateTime limit, List<Member> Members, string ID, byte mode) :
            base(client,reset,limit,Members,ID,mode)
        {
        }

        public void ParseGambit(string OtherID, string OtherCharacterID, string OtherType)
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
                //rClient.URL = "https://www.bungie.net/Platform/Destiny2/Stats/PostGameCarnageReport/3198056999/";
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
                    CheckIfSameMember(PGCoutPut, PGCRreader, i);
                    if(Activity_list[i].Compwclanmembers == false)
                    {
                        Activity_list[i].Compwclanmembers = CheckIfAnotherMember(PGCoutPut, Activity_list[i].Playercount);
                    }
                    PGCoutPut = string.Empty;
                }
            }
        }

        public void CheckIfSameMember(string ID, StringReader strReader, int index)
        {
            foreach (Member element in Member_list)
            {
                if (ID == element.ID)
                {
                    if (ID == Actual_member_ID)
                    {
                        ParseGambitStats(strReader, index);
                    }
                }
            }
        }

        public bool CheckIfAnotherMember(string ID, int Playercount)
        {
            if (Playercount > 1)
            {
                foreach (Member element in Member_list)
                {
                    if (ID == element.ID)
                    {
                        if (ID != Actual_member_ID)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public void ParseGambitStats(StringReader strReader, int index)
        {
            bool GMBSTOP = false; char[] GMBRParse = new char[5];
            string GMBoutPut = string.Empty;

            //JUMP TO INVASON KEYWORD
            while (!GMBSTOP)
            {
                GMBRParse[0] = (char)strReader.Read();
                if (GMBRParse[0] == 'i')
                {
                    GMBRParse[1] = (char)strReader.Read();
                    if (GMBRParse[1] == 'n')
                    {
                        GMBRParse[2] = (char)strReader.Read();
                        if (GMBRParse[2] == 'v')
                        {
                            GMBRParse[3] = (char)strReader.Read();
                            if (GMBRParse[3] == 'a')
                            {
                                GMBRParse[4] = (char)strReader.Read();
                                if (GMBRParse[4] == 's')
                                {
                                    GMBSTOP = true;
                                }
                            }
                        }
                    }
                }
            }GMBSTOP = false;

            //JUMP TO KILLS KEYWORD
            while (!GMBSTOP)
            {
                GMBRParse[0] = (char)strReader.Read();
                if (GMBRParse[0] == 'K')
                {
                    GMBRParse[1] = (char)strReader.Read();
                    if (GMBRParse[1] == 'i')
                    {
                        GMBRParse[2] = (char)strReader.Read();
                        if (GMBRParse[2] == 'l')
                        {
                            GMBRParse[3] = (char)strReader.Read();
                            if (GMBRParse[3] == 'l')
                            {
                                GMBRParse[4] = (char)strReader.Read();
                                if (GMBRParse[4] == 's')
                                {
                                    GMBSTOP = true;
                                }
                            }
                        }
                    }
                }
            }GMBSTOP = false; 
            
            //JUMP TO VALUE KEYWORD
            while (!GMBSTOP)
            {
                GMBRParse[0] = (char)strReader.Read();
                if (GMBRParse[0] == 'l')
                {
                    GMBRParse[1] = (char)strReader.Read();
                    if (GMBRParse[1] == 'u')
                    {
                        GMBRParse[2] = (char)strReader.Read();
                        if (GMBRParse[2] == 'e')
                        {
                            GMBRParse[3] = (char)strReader.Read();
                            if (GMBRParse[3] == '"')
                            {
                                GMBRParse[4] = (char)strReader.Read();
                                if (GMBRParse[4] == ':')
                                {
                                    GMBSTOP = true;
                                }
                            }
                        }
                    }
                }
            }GMBSTOP = false;

            //INVASION KILLS
            while (GMBRParse[0] != ',')
            {
                GMBRParse[0] = (char)strReader.Read();
                if (GMBRParse[0] == ',')
                {
                    break;
                }
                GMBoutPut += GMBRParse[0];
            }
            float guardiankills;
            float.TryParse(GMBoutPut, out guardiankills); GMBoutPut = string.Empty;
            Activity_list[index].Gambit_guardiankills = (ushort)guardiankills;
            GMBRParse[0] = 'a';
            while (GMBRParse[0] != ',')
            {
                GMBRParse[0] = (char)strReader.Read();
            }

            //JUMP TO KILLS KEYWORD
            while (!GMBSTOP)
            {
                GMBRParse[0] = (char)strReader.Read();
                if (GMBRParse[0] == 'K')
                {
                    GMBRParse[1] = (char)strReader.Read();
                    if (GMBRParse[1] == 'i')
                    {
                        GMBRParse[2] = (char)strReader.Read();
                        if (GMBRParse[2] == 'l')
                        {
                            GMBRParse[3] = (char)strReader.Read();
                            if (GMBRParse[3] == 'l')
                            {
                                GMBRParse[4] = (char)strReader.Read();
                                if (GMBRParse[4] == 's')
                                {
                                    GMBSTOP = true;
                                }
                            }
                        }
                    }
                }
            }
            GMBSTOP = false;

            //JUMP TO VALUE KEYWORD
            while (!GMBSTOP)
            {
                GMBRParse[0] = (char)strReader.Read();
                if (GMBRParse[0] == 'l')
                {
                    GMBRParse[1] = (char)strReader.Read();
                    if (GMBRParse[1] == 'u')
                    {
                        GMBRParse[2] = (char)strReader.Read();
                        if (GMBRParse[2] == 'e')
                        {
                            GMBRParse[3] = (char)strReader.Read();
                            if (GMBRParse[3] == '"')
                            {
                                GMBRParse[4] = (char)strReader.Read();
                                if (GMBRParse[4] == ':')
                                {
                                    GMBSTOP = true;
                                }
                            }
                        }
                    }
                }
            }
            GMBSTOP = false;

            //INVADER KILLS
            while (GMBRParse[0] != ',')
            {
                GMBRParse[0] = (char)strReader.Read();
                if (GMBRParse[0] == ',')
                {
                    break;
                }
                GMBoutPut += GMBRParse[0];
            }
            float invaderkills;
            float.TryParse(GMBoutPut, out invaderkills); GMBoutPut = string.Empty;
            Activity_list[index].Gambit_guardiankills += (ushort)invaderkills;
            GMBRParse[0] = 'a';
            while (GMBRParse[0] != ',')
            {
                GMBRParse[0] = (char)strReader.Read();
            }

            //JUMP TO KILLS KEYWORD
            while (!GMBSTOP)
            {
                GMBRParse[0] = (char)strReader.Read();
                if (GMBRParse[0] == 'K')
                {
                    GMBRParse[1] = (char)strReader.Read();
                    if (GMBRParse[1] == 'i')
                    {
                        GMBRParse[2] = (char)strReader.Read();
                        if (GMBRParse[2] == 'l')
                        {
                            GMBRParse[3] = (char)strReader.Read();
                            if (GMBRParse[3] == 'l')
                            {
                                GMBRParse[4] = (char)strReader.Read();
                                if (GMBRParse[4] == 's')
                                {
                                    GMBSTOP = true;
                                }
                            }
                        }
                    }
                }
            }
            GMBSTOP = false;

            //JUMP TO VALUE KEYWORD
            while (!GMBSTOP)
            {
                GMBRParse[0] = (char)strReader.Read();
                if (GMBRParse[0] == 'l')
                {
                    GMBRParse[1] = (char)strReader.Read();
                    if (GMBRParse[1] == 'u')
                    {
                        GMBRParse[2] = (char)strReader.Read();
                        if (GMBRParse[2] == 'e')
                        {
                            GMBRParse[3] = (char)strReader.Read();
                            if (GMBRParse[3] == '"')
                            {
                                GMBRParse[4] = (char)strReader.Read();
                                if (GMBRParse[4] == ':')
                                {
                                    GMBSTOP = true;
                                }
                            }
                        }
                    }
                }
            }
            GMBSTOP = false;

            //PRIMEVAL KILLS
            while (GMBRParse[0] != ',')
            {
                GMBRParse[0] = (char)strReader.Read();
                if (GMBRParse[0] == ',')
                {
                    break;
                }
                GMBoutPut += GMBRParse[0];
            }
            float primevalkills;
            float.TryParse(GMBoutPut, out primevalkills); GMBoutPut = string.Empty;
            Activity_list[index].Gambit_primevalkills = (ushort)primevalkills;
            GMBRParse[0] = 'a';
            while (GMBRParse[0] != ',')
            {
                GMBRParse[0] = (char)strReader.Read();
            }

            //JUMP TO MOBKILLS KEYWORD
            while (!GMBSTOP)
            {
                GMBRParse[0] = (char)strReader.Read();
                if (GMBRParse[0] == 'm')
                {
                    GMBRParse[1] = (char)strReader.Read();
                    if (GMBRParse[1] == 'o')
                    {
                        GMBRParse[2] = (char)strReader.Read();
                        if (GMBRParse[2] == 'b')
                        {
                            GMBRParse[3] = (char)strReader.Read();
                            if (GMBRParse[3] == 'K')
                            {
                                GMBRParse[4] = (char)strReader.Read();
                                if (GMBRParse[4] == 'i')
                                {
                                    GMBSTOP = true;
                                }
                            }
                        }
                    }
                }
            }
            GMBSTOP = false;

            //JUMP TO VALUE KEYWORD
            while (!GMBSTOP)
            {
                GMBRParse[0] = (char)strReader.Read();
                if (GMBRParse[0] == 'l')
                {
                    GMBRParse[1] = (char)strReader.Read();
                    if (GMBRParse[1] == 'u')
                    {
                        GMBRParse[2] = (char)strReader.Read();
                        if (GMBRParse[2] == 'e')
                        {
                            GMBRParse[3] = (char)strReader.Read();
                            if (GMBRParse[3] == '"')
                            {
                                GMBRParse[4] = (char)strReader.Read();
                                if (GMBRParse[4] == ':')
                                {
                                    GMBSTOP = true;
                                }
                            }
                        }
                    }
                }
            }
            GMBSTOP = false;

            //MOB KILLS
            while (GMBRParse[0] != ',')
            {
                GMBRParse[0] = (char)strReader.Read();
                if (GMBRParse[0] == ',')
                {
                    break;
                }
                GMBoutPut += GMBRParse[0];
            }
            float mobkills;
            float.TryParse(GMBoutPut, out mobkills); GMBoutPut = string.Empty;
            Activity_list[index].Gambit_mobkills = (ushort)mobkills;
            GMBRParse[0] = 'a';
            while (GMBRParse[0] != ',')
            {
                GMBRParse[0] = (char)strReader.Read();
            }

            //JUMP TO MOTESDEPOSITED KEYWORD
            while (!GMBSTOP)
            {
                GMBRParse[0] = (char)strReader.Read();
                if (GMBRParse[0] == 's')
                {
                    GMBRParse[1] = (char)strReader.Read();
                    if (GMBRParse[1] == 'i')
                    {
                        GMBRParse[2] = (char)strReader.Read();
                        if (GMBRParse[2] == 't')
                        {
                            GMBRParse[3] = (char)strReader.Read();
                            if (GMBRParse[3] == 'e')
                            {
                                GMBRParse[4] = (char)strReader.Read();
                                if (GMBRParse[4] == 'd')
                                {
                                    GMBSTOP = true;
                                }
                            }
                        }
                    }
                }
            }
            GMBSTOP = false;

            //JUMP TO VALUE KEYWORD
            while (!GMBSTOP)
            {
                GMBRParse[0] = (char)strReader.Read();
                if (GMBRParse[0] == 'l')
                {
                    GMBRParse[1] = (char)strReader.Read();
                    if (GMBRParse[1] == 'u')
                    {
                        GMBRParse[2] = (char)strReader.Read();
                        if (GMBRParse[2] == 'e')
                        {
                            GMBRParse[3] = (char)strReader.Read();
                            if (GMBRParse[3] == '"')
                            {
                                GMBRParse[4] = (char)strReader.Read();
                                if (GMBRParse[4] == ':')
                                {
                                    GMBSTOP = true;
                                }
                            }
                        }
                    }
                }
            }
            GMBSTOP = false;

            //MOTES DEPOSITED
            while (GMBRParse[0] != ',')
            {
                GMBRParse[0] = (char)strReader.Read();
                if (GMBRParse[0] == ',')
                {
                    break;
                }
                GMBoutPut += GMBRParse[0];
            }
            float motes;
            float.TryParse(GMBoutPut, out motes); GMBoutPut = string.Empty;
            Activity_list[index].Gambit_motesdeposited = (ushort)motes;
            GMBRParse[0] = 'a';
            while (GMBRParse[0] != ',')
            {
                GMBRParse[0] = (char)strReader.Read();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace AbysmalLightScoreTracker
{
    class Member
    {
        public string Name;
        public string ID;
        public string Type;
        public string CrossSaveOverride;
        List<string> characterIDs;

        Activity[][] StoryMissions = new Activity[3][];
        Activity[][] ForgeMissions = new Activity[3][];
        Activity[][] ReckoningMissions = new Activity[3][];
        Activity[][] StrikeMissions = new Activity[3][];
        Activity[][] MenagerieMissions = new Activity[3][];
        Activity[][] NightfallMissions = new Activity[3][];
        Activity[][] CrucibleMatches = new Activity[3][];
        Activity[][] GambitMatches = new Activity[3][];
        Activity[][] GambitPrimeMatches = new Activity[3][];
        Activity[][] Raids = new Activity[3][];

        public Activity[][] GetStoryMissions { get => StoryMissions; }
        public Activity[][] GetForgeMissions { get => ForgeMissions; }
        public Activity[][] GetReckoningMissions { get => ReckoningMissions; }
        public Activity[][] GetStrikeMissions { get => StrikeMissions; }
        public Activity[][] GetMenagerieMissions { get => MenagerieMissions; }
        public Activity[][] GetNightfallMissions { get => NightfallMissions; }
        public Activity[][] GetCrucibleMatches { get => CrucibleMatches; }
        public Activity[][] GetGambitMatches { get => GambitMatches; }
        public Activity[][] GetGambitPrimeMatches { get => GambitPrimeMatches; }
        public Activity[][] GetRaids { get => Raids; }

        public List<string> GetcharacterIDs()
        {
            return characterIDs;
        }

        public void ParseCharacterIDs(RESTClient client)
        {
            characterIDs = new List<string>();
            //MAKE REST CLIENT REQUEST
            client.URL = "https://www.bungie.net/Platform/Destiny2/" + Type + "/Profile/" + ID + "/?components=100";
            string strLocal = client.makeRequest();

            //START PARSING
            char charParse = ' ';
            StringReader reader = new StringReader(strLocal);

            while (charParse != '[')
            {
                charParse = (char)reader.Read();
            }
            reader.Read();

            while (charParse != ']')
            {
                int count = 19;
                char[] tempCharBuffer = new char[count];
                reader.Read(tempCharBuffer, 0, count);
                string temporal = string.Empty;
                for (int i = 0; i < count; i++)
                {
                    temporal += tempCharBuffer[i].ToString();
                }
                characterIDs.Add(temporal);

                if (charParse == ']')
                {
                    break;
                }
                charParse = (char)reader.Read();
                if (charParse == ']')
                {
                    break;
                }
                charParse = (char)reader.Read();
                if (charParse == ']')
                {
                    break;
                }
                charParse = (char)reader.Read();
            }
        }

    }

    class ClanMemberParser
    {
        public List<Member> ClanMembersList;
        StringReader localreader;
        public string outPut;

        private char[] charParse = new char[5];
        private bool STOP = false;

        public ClanMemberParser()
        {
            ClanMembersList = new List<Member>();
        }

        public void ParseClanMembers(RESTClient client, string ID)
        {
            client.URL = "https://www.bungie.net/Platform/GroupV2/" + ID + "/Members/";
            outPut = client.makeRequest();

            localreader = new StringReader(outPut);

            while (charParse[0] != '[')
            {
                charParse[0] = (char)localreader.Read();
            }

            while(charParse[0] != ']')
            {
                Member tempMember = new Member();
                outPut = string.Empty;

                while (!STOP)
                {
                    charParse[0] = (char)localreader.Read();
                    if (charParse[0] == ']')
                    {
                        break;
                    }
                    if (charParse[0] == 'r')
                    {
                        charParse[1] = (char)localreader.Read();
                        if (charParse[1] == 'r')
                        {
                            charParse[2] = (char)localreader.Read();
                            if (charParse[2] == 'i')
                            {
                                charParse[3] = (char)localreader.Read();
                                if (charParse[3] == 'd')
                                {
                                    charParse[4] = (char)localreader.Read();
                                    if (charParse[4] == 'e')
                                    {
                                        STOP = true;
                                    }
                                }
                            }
                        }
                    }
                }
                STOP = false;
                if (charParse[0] == ']')
                {
                    break;
                }

                localreader.Read();
                localreader.Read();

                charParse[0] = (char)localreader.Read();
                tempMember.CrossSaveOverride = charParse[0].ToString();
                localreader.Read();

                while (charParse[0] != ']')
                {
                    charParse[0] = (char)localreader.Read();
                }
                ////////////////////////////////////////////////

                while (!STOP)
                {
                    charParse[0] = (char)localreader.Read();
                    if(charParse[0] == ']')
                    {
                        break;
                    }
                    if (charParse[0] == 'p')
                    {
                        charParse[1] = (char)localreader.Read();
                        if (charParse[1] == 'T')
                        {
                            charParse[2] = (char)localreader.Read();
                            if (charParse[2] == 'y')
                            {
                                charParse[3] = (char)localreader.Read();
                                if (charParse[3] == 'p')
                                {
                                    charParse[4] = (char)localreader.Read();
                                    if (charParse[4] == 'e')
                                    {
                                        STOP = true;
                                    }
                                }
                            }
                        }
                    }
                }
                STOP = false;
                if (charParse[0] == ']')
                {
                    break;
                }

                localreader.Read();
                localreader.Read();
                //localreader.Read();

                charParse[0] = (char)localreader.Read();
                tempMember.Type = charParse[0].ToString();
                //int val = (int)Char.GetNumericValue(charParse);

                while (charParse[0] != ':')
                {
                    charParse[0] = (char)localreader.Read();
                }

                localreader.Read();

                int count = 19;
                char[] tempCharBuffer = new char[count];
                localreader.Read(tempCharBuffer, 0, count);
                string temporal = string.Empty;
                for (int i = 0; i < count; i++)
                {
                    temporal += tempCharBuffer[i].ToString();
                }
                tempMember.ID = temporal;

                charParse[0] = 'a';
                while (charParse[0] != ':')
                {
                    charParse[0] = (char)localreader.Read();
                }
                localreader.Read();

                while (charParse[0] != '"')
                {
                    charParse[0] = (char)localreader.Read();
                    if (charParse[0] == '"')
                    {
                        break;
                    }
                    outPut += charParse[0];
                }
                tempMember.Name = outPut;

                while (!STOP)
                {
                    charParse[0] = (char)localreader.Read();
                    if (charParse[0] == 'n')
                    {
                        charParse[1] = (char)localreader.Read();
                        if (charParse[1] == 'D')
                        {
                            charParse[2] = (char)localreader.Read();
                            if (charParse[2] == 'a')
                            {
                                charParse[3] = (char)localreader.Read();
                                if (charParse[3] == 't')
                                {
                                    charParse[4] = (char)localreader.Read();
                                    if (charParse[4] == 'e')
                                    {
                                        STOP = true;
                                    }
                                }
                            }
                        }
                    }
                }
                STOP = false;

                tempMember.ParseCharacterIDs(client);
                ClanMembersList.Add(tempMember);
            }
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace AbysmalLightScoreTracker
{
    class PVPManager : Manager
    {
        public RESTClient rClient;
        public List<Member> Member_list;
        public string Actual_member_ID;
        public int jCounter;

        Activity Match;
        DateTime Reset_time;
        DateTime Date_limit;
        byte Activity_mode;

        public PVPManager(RESTClient client, DateTime reset,
        DateTime limit, List<Member> Members, string ID, byte mode)
        {
            rClient = client;
            Reset_time = reset;
            Date_limit = limit;
            Member_list = Members;
            Actual_member_ID = ID;
            Activity_mode = mode;
        }

        public List<Activity> ParseActivitiesList(string MemberID, string CharacterID, string Type)
        {
            List<Activity> tempList = new List<Activity>();
            DateTime ActualActiviyDT;

            StringReader localreader;
            uint page = 0;
            bool STOP = false;
            char[] charParse = new char[5];
            string strLocal;

            bool outofDate = false;

            //PAGE START
            while (outofDate == false)
            {
                rClient.URL = "https://www.bungie.net/Platform/Destiny2/" + Type + "/Account/" + MemberID + "/Character/" + CharacterID + "/Stats/Activities/?mode=" + Activity_mode + "&count=30&page=" + page;
                strLocal = rClient.makeRequest();
                localreader = new StringReader(strLocal);
                charParse = new char[5];

                while (charParse[0] != 'r' && charParse[1] != 'C' && charParse[2] != 'o' &&
                        charParse[3] != 'd' && charParse[4] != 'e')
                {
                    ///////////////////////////////////////////////////////////////////////////////////
                    //READ ACTIVITY START
                    ///////////////////////////////////////////////////////////////////////////////////

                    while (!STOP)
                    {
                        charParse[0] = (char)localreader.Read();
                        if (charParse[0] == 'e')
                        {
                            charParse[1] = (char)localreader.Read();
                            if (charParse[1] == 'r')
                            {
                                charParse[2] = (char)localreader.Read();
                                if (charParse[2] == 'i')
                                {
                                    charParse[3] = (char)localreader.Read();
                                    if (charParse[3] == 'o')
                                    {
                                        charParse[4] = (char)localreader.Read();
                                        if (charParse[4] == 'd')
                                        {
                                            STOP = true;
                                        }
                                    }
                                }
                            }
                        }
                        else if (charParse[0] == 'r')
                        {
                            charParse[1] = (char)localreader.Read();
                            if (charParse[1] == 'C')
                            {
                                charParse[2] = (char)localreader.Read();
                                if (charParse[2] == 'o')
                                {
                                    charParse[3] = (char)localreader.Read();
                                    if (charParse[3] == 'd')
                                    {
                                        charParse[4] = (char)localreader.Read();
                                        if (charParse[4] == 'e')
                                        {
                                            STOP = true;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    STOP = false;
                    if (charParse[0] == 'r' && charParse[1] == 'C' && charParse[2] == 'o' &&
                        charParse[3] == 'd' && charParse[4] == 'e')
                    {
                        break;
                    }

                    localreader.Read(); localreader.Read(); localreader.Read();

                    string outPut = string.Empty; char[] tempCharBuffer; int count;
                    int year, month, day, hour, minute, second;

                    count = 4; tempCharBuffer = new char[count];
                    localreader.Read(tempCharBuffer, 0, count);
                    for (int i = 0; i < count; i++)
                    {
                        outPut += tempCharBuffer[i].ToString();
                    }
                    int.TryParse(outPut, out year); outPut = string.Empty;
                    localreader.Read();

                    count = 2; tempCharBuffer = new char[count];
                    localreader.Read(tempCharBuffer, 0, count);
                    for (int i = 0; i < count; i++)
                    {
                        outPut += tempCharBuffer[i].ToString();
                    }
                    int.TryParse(outPut, out month); outPut = string.Empty;
                    localreader.Read();

                    count = 2; tempCharBuffer = new char[count];
                    localreader.Read(tempCharBuffer, 0, count);
                    for (int i = 0; i < count; i++)
                    {
                        outPut += tempCharBuffer[i].ToString();
                    }
                    int.TryParse(outPut, out day); outPut = string.Empty;
                    localreader.Read();

                    count = 2; tempCharBuffer = new char[count];
                    localreader.Read(tempCharBuffer, 0, count);
                    for (int i = 0; i < count; i++)
                    {
                        outPut += tempCharBuffer[i].ToString();
                    }
                    int.TryParse(outPut, out hour); outPut = string.Empty;
                    localreader.Read();

                    count = 2; tempCharBuffer = new char[count];
                    localreader.Read(tempCharBuffer, 0, count);
                    for (int i = 0; i < count; i++)
                    {
                        outPut += tempCharBuffer[i].ToString();
                    }
                    int.TryParse(outPut, out minute); outPut = string.Empty;
                    localreader.Read();

                    count = 2; tempCharBuffer = new char[count];
                    localreader.Read(tempCharBuffer, 0, count);
                    for (int i = 0; i < count; i++)
                    {
                        outPut += tempCharBuffer[i].ToString();
                    }
                    int.TryParse(outPut, out second); outPut = string.Empty;

                    ActualActiviyDT = DateTime.Parse(day + "/" + month + "/" + year);
                    ActualActiviyDT = ActualActiviyDT.AddHours(hour);
                    ActualActiviyDT = ActualActiviyDT.AddMinutes(minute);
                    ActualActiviyDT = ActualActiviyDT.AddSeconds(second);

                    if (ActualActiviyDT < Reset_time/* || ActualActiviyDT >= Date_limit*/)
                    {
                        outofDate = true;
                        break;
                    }
                    else if (ActualActiviyDT >= Date_limit)
                    {
                    }
                    else
                    {

                        Match = new Activity();
                        Match.Period = ActualActiviyDT.ToLocalTime();

                        while (!STOP)
                        {
                            charParse[0] = (char)localreader.Read();
                            if (charParse[0] == 'y')
                            {
                                charParse[1] = (char)localreader.Read();
                                if (charParse[1] == 'H')
                                {
                                    charParse[2] = (char)localreader.Read();
                                    if (charParse[2] == 'a')
                                    {
                                        charParse[3] = (char)localreader.Read();
                                        if (charParse[3] == 's')
                                        {
                                            charParse[4] = (char)localreader.Read();
                                            if (charParse[4] == 'h')
                                            {
                                                STOP = true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        STOP = false;
                        localreader.Read(); localreader.Read();

                        while (charParse[0] != ',')
                        {
                            charParse[0] = (char)localreader.Read();
                            if (charParse[0] == ',')
                            {
                                break;
                            }
                            outPut += charParse[0];
                        }
                        Match.directorActivityHash = outPut; outPut = string.Empty;

                        while (charParse[0] != ':')
                        {
                            charParse[0] = (char)localreader.Read();
                        }
                        localreader.Read(); charParse[0] = 'a';

                        while (charParse[0] != '"')
                        {
                            charParse[0] = (char)localreader.Read();
                            if (charParse[0] == '"')
                            {
                                break;
                            }
                            outPut += charParse[0];
                        }
                        Match.InstanceID = outPut; outPut = string.Empty;

                        ///////////////////////////////////////////////////////////////////////////////////
                        //CHANGES BETWEEN ACTIVITIES
                        ///////////////////////////////////////////////////////////////////////////////////

                        //ASSISTS
                        while (!STOP)
                        {
                            charParse[0] = (char)localreader.Read();
                            if (charParse[0] == 'a')
                            {
                                charParse[1] = (char)localreader.Read();
                                if (charParse[1] == 'l')
                                {
                                    charParse[2] = (char)localreader.Read();
                                    if (charParse[2] == 'u')
                                    {
                                        charParse[3] = (char)localreader.Read();
                                        if (charParse[3] == 'e')
                                        {
                                            charParse[4] = (char)localreader.Read();
                                            if (charParse[4] == '"')
                                            {
                                                STOP = true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        STOP = false; localreader.Read();
                        while (charParse[0] != ',')
                        {
                            charParse[0] = (char)localreader.Read();
                            if (charParse[0] == ',')
                            {
                                break;
                            }
                            outPut += charParse[0];
                        }
                        float assists;
                        float.TryParse(outPut, out assists); outPut = string.Empty;
                        Match.Assists = (ushort)assists;
                        charParse[0] = 'a';
                        while (charParse[0] != ',')
                        {
                            charParse[0] = (char)localreader.Read();
                        }

                        //SCORE
                        while (!STOP)
                        {
                            charParse[0] = (char)localreader.Read();
                            if (charParse[0] == 'a')
                            {
                                charParse[1] = (char)localreader.Read();
                                if (charParse[1] == 'l')
                                {
                                    charParse[2] = (char)localreader.Read();
                                    if (charParse[2] == 'u')
                                    {
                                        charParse[3] = (char)localreader.Read();
                                        if (charParse[3] == 'e')
                                        {
                                            charParse[4] = (char)localreader.Read();
                                            if (charParse[4] == '"')
                                            {
                                                STOP = true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        STOP = false; localreader.Read();
                        while (charParse[0] != ',')
                        {
                            charParse[0] = (char)localreader.Read();
                            if (charParse[0] == ',')
                            {
                                break;
                            }
                            outPut += charParse[0];
                        }
                        float score;
                        float.TryParse(outPut, out score); outPut = string.Empty;
                        Match.Score = (ushort)score;
                        charParse[0] = 'a';
                        while (charParse[0] != ',')
                        {
                            charParse[0] = (char)localreader.Read();
                        }

                        //KILLS
                        while (!STOP)
                        {
                            charParse[0] = (char)localreader.Read();
                            if (charParse[0] == 'a')
                            {
                                charParse[1] = (char)localreader.Read();
                                if (charParse[1] == 'l')
                                {
                                    charParse[2] = (char)localreader.Read();
                                    if (charParse[2] == 'u')
                                    {
                                        charParse[3] = (char)localreader.Read();
                                        if (charParse[3] == 'e')
                                        {
                                            charParse[4] = (char)localreader.Read();
                                            if (charParse[4] == '"')
                                            {
                                                STOP = true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        STOP = false; localreader.Read();
                        while (charParse[0] != ',')
                        {
                            charParse[0] = (char)localreader.Read();
                            if (charParse[0] == ',')
                            {
                                break;
                            }
                            outPut += charParse[0];
                        }
                        float kills;
                        float.TryParse(outPut, out kills); outPut = string.Empty;
                        Match.Kills = (ushort)kills;
                        charParse[0] = 'a';
                        while (charParse[0] != ',')
                        {
                            charParse[0] = (char)localreader.Read();
                        }

                        //JUMPS TO DEATHS
                        while (!STOP)
                        {
                            charParse[0] = (char)localreader.Read();
                            if (charParse[0] == 'e')
                            {
                                charParse[1] = (char)localreader.Read();
                                if (charParse[1] == 'a')
                                {
                                    charParse[2] = (char)localreader.Read();
                                    if (charParse[2] == 't')
                                    {
                                        charParse[3] = (char)localreader.Read();
                                        if (charParse[3] == 'h')
                                        {
                                            charParse[4] = (char)localreader.Read();
                                            if (charParse[4] == 's')
                                            {
                                                STOP = true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        STOP = false;
                        //////////////////////////////////////////////////////////////////

                        //DEATHS
                        while (!STOP)
                        {
                            charParse[0] = (char)localreader.Read();
                            if (charParse[0] == 'a')
                            {
                                charParse[1] = (char)localreader.Read();
                                if (charParse[1] == 'l')
                                {
                                    charParse[2] = (char)localreader.Read();
                                    if (charParse[2] == 'u')
                                    {
                                        charParse[3] = (char)localreader.Read();
                                        if (charParse[3] == 'e')
                                        {
                                            charParse[4] = (char)localreader.Read();
                                            if (charParse[4] == '"')
                                            {
                                                STOP = true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        STOP = false; localreader.Read();
                        while (charParse[0] != ',')
                        {
                            charParse[0] = (char)localreader.Read();
                            if (charParse[0] == ',')
                            {
                                break;
                            }
                            outPut += charParse[0];
                        }
                        float deaths;
                        float.TryParse(outPut, out deaths); outPut = string.Empty;
                        Match.Deaths = (ushort)deaths;
                        charParse[0] = 'a';
                        while (charParse[0] != ',')
                        {
                            charParse[0] = (char)localreader.Read();
                        }

                        //JUMPS TO COMPLETE
                        while (!STOP)
                        {
                            charParse[0] = (char)localreader.Read();
                            if (charParse[0] == 'l')
                            {
                                charParse[1] = (char)localreader.Read();
                                if (charParse[1] == 'e')
                                {
                                    charParse[2] = (char)localreader.Read();
                                    if (charParse[2] == 't')
                                    {
                                        charParse[3] = (char)localreader.Read();
                                        if (charParse[3] == 'e')
                                        {
                                            charParse[4] = (char)localreader.Read();
                                            if (charParse[4] == 'd')
                                            {
                                                STOP = true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        STOP = false;
                        //////////////////////////////////////////////////////////////////

                        //COMPLETE
                        while (!STOP)
                        {
                            charParse[0] = (char)localreader.Read();
                            if (charParse[0] == 'a')
                            {
                                charParse[1] = (char)localreader.Read();
                                if (charParse[1] == 'l')
                                {
                                    charParse[2] = (char)localreader.Read();
                                    if (charParse[2] == 'u')
                                    {
                                        charParse[3] = (char)localreader.Read();
                                        if (charParse[3] == 'e')
                                        {
                                            charParse[4] = (char)localreader.Read();
                                            if (charParse[4] == '"')
                                            {
                                                STOP = true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        STOP = false; localreader.Read();
                        while (charParse[0] != ',')
                        {
                            charParse[0] = (char)localreader.Read();
                            if (charParse[0] == ',')
                            {
                                break;
                            }
                            outPut += charParse[0];
                        }
                        float mission_complete;
                        float.TryParse(outPut, out mission_complete); outPut = string.Empty;
                        if (mission_complete == 1) { Match.Completed = true; }
                        charParse[0] = 'a';
                        while (charParse[0] != ',')
                        {
                            charParse[0] = (char)localreader.Read();
                        }

                        //JUMPS TO KD
                        while (!STOP)
                        {
                            charParse[0] = (char)localreader.Read();
                            if (charParse[0] == 'R')
                            {
                                charParse[1] = (char)localreader.Read();
                                if (charParse[1] == 'a')
                                {
                                    charParse[2] = (char)localreader.Read();
                                    if (charParse[2] == 't')
                                    {
                                        charParse[3] = (char)localreader.Read();
                                        if (charParse[3] == 'i')
                                        {
                                            charParse[4] = (char)localreader.Read();
                                            if (charParse[4] == 'o')
                                            {
                                                STOP = true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        STOP = false;
                        //////////////////////////////////////////////////////////////////

                        //KD
                        while (!STOP)
                        {
                            charParse[0] = (char)localreader.Read();
                            if (charParse[0] == 'a')
                            {
                                charParse[1] = (char)localreader.Read();
                                if (charParse[1] == 'l')
                                {
                                    charParse[2] = (char)localreader.Read();
                                    if (charParse[2] == 'u')
                                    {
                                        charParse[3] = (char)localreader.Read();
                                        if (charParse[3] == 'e')
                                        {
                                            charParse[4] = (char)localreader.Read();
                                            if (charParse[4] == '"')
                                            {
                                                STOP = true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        STOP = false; localreader.Read();
                        while (charParse[0] != ',')
                        {
                            charParse[0] = (char)localreader.Read();
                            if (charParse[0] == ',')
                            {
                                break;
                            }
                            outPut += charParse[0];
                        }
                        float KD;
                        float.TryParse(outPut, out KD); outPut = string.Empty;
                        Match.KD = (float)Math.Round(KD, 2);
                        charParse[0] = 'a';
                        while (charParse[0] != ',')
                        {
                            charParse[0] = (char)localreader.Read();
                        }

                        //KDA
                        while (!STOP)
                        {
                            charParse[0] = (char)localreader.Read();
                            if (charParse[0] == 'a')
                            {
                                charParse[1] = (char)localreader.Read();
                                if (charParse[1] == 'l')
                                {
                                    charParse[2] = (char)localreader.Read();
                                    if (charParse[2] == 'u')
                                    {
                                        charParse[3] = (char)localreader.Read();
                                        if (charParse[3] == 'e')
                                        {
                                            charParse[4] = (char)localreader.Read();
                                            if (charParse[4] == '"')
                                            {
                                                STOP = true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        STOP = false; localreader.Read();
                        while (charParse[0] != ',')
                        {
                            charParse[0] = (char)localreader.Read();
                            if (charParse[0] == ',')
                            {
                                break;
                            }
                            outPut += charParse[0];
                        }
                        float KDA;
                        float.TryParse(outPut, out KDA); outPut = string.Empty;
                        Match.KDA = (float)Math.Round(KDA, 2);
                        charParse[0] = 'a';
                        while (charParse[0] != ',')
                        {
                            charParse[0] = (char)localreader.Read();
                        }

                        //DURATION
                        while (!STOP)
                        {
                            charParse[0] = (char)localreader.Read();
                            if (charParse[0] == 'V')
                            {
                                charParse[1] = (char)localreader.Read();
                                if (charParse[1] == 'a')
                                {
                                    charParse[2] = (char)localreader.Read();
                                    if (charParse[2] == 'l')
                                    {
                                        charParse[3] = (char)localreader.Read();
                                        if (charParse[3] == 'u')
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
                        STOP = false; localreader.Read(); localreader.Read();
                        while (charParse[0] != '}')
                        {
                            charParse[0] = (char)localreader.Read();
                            if (charParse[0] == '}')
                            {
                                break;
                            }
                            outPut += charParse[0];
                        }
                        Match.Duration = outPut; outPut = string.Empty;
                        charParse[0] = 'a';
                        while (charParse[0] != ',')
                        {
                            charParse[0] = (char)localreader.Read();
                        }

                        //STANDING
                        while (!STOP)
                        {
                            charParse[0] = (char)localreader.Read();
                            if (charParse[0] == 'V')
                            {
                                charParse[1] = (char)localreader.Read();
                                if (charParse[1] == 'a')
                                {
                                    charParse[2] = (char)localreader.Read();
                                    if (charParse[2] == 'l')
                                    {
                                        charParse[3] = (char)localreader.Read();
                                        if (charParse[3] == 'u')
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
                        STOP = false; localreader.Read(); localreader.Read(); localreader.Read();
                        while (charParse[0] != '"')
                        {
                            charParse[0] = (char)localreader.Read();
                            if (charParse[0] == '"')
                            {
                                break;
                            }
                            outPut += charParse[0];
                        }
                        Match.Standing = outPut; outPut = string.Empty;
                        charParse[0] = 'a';
                        while (charParse[0] != ',')
                        {
                            charParse[0] = (char)localreader.Read();
                        }

                        //TEAM
                        while (!STOP)
                        {
                            charParse[0] = (char)localreader.Read();
                            if (charParse[0] == 'a')
                            {
                                charParse[1] = (char)localreader.Read();
                                if (charParse[1] == 'l')
                                {
                                    charParse[2] = (char)localreader.Read();
                                    if (charParse[2] == 'u')
                                    {
                                        charParse[3] = (char)localreader.Read();
                                        if (charParse[3] == 'e')
                                        {
                                            charParse[4] = (char)localreader.Read();
                                            if (charParse[4] == '"')
                                            {
                                                STOP = true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        STOP = false; localreader.Read();
                        while (charParse[0] != ',')
                        {
                            charParse[0] = (char)localreader.Read();
                            if (charParse[0] == ',')
                            {
                                break;
                            }
                            outPut += charParse[0];
                        }
                        float team;
                        float.TryParse(outPut, out team); outPut = string.Empty;
                        if (team == 17) { outPut = "Bravo"; }
                        else if (team == 18) { outPut = "Alpha"; }
                        else { outPut = " "; }
                        Match.Team = outPut; outPut = string.Empty;
                        charParse[0] = 'a';
                        while (charParse[0] != ',')
                        {
                            charParse[0] = (char)localreader.Read();
                        }

                        //JUMPS TO PLAYERCOUNT
                        while (!STOP)
                        {
                            charParse[0] = (char)localreader.Read();
                            if (charParse[0] == 'C')
                            {
                                charParse[1] = (char)localreader.Read();
                                if (charParse[1] == 'o')
                                {
                                    charParse[2] = (char)localreader.Read();
                                    if (charParse[2] == 'u')
                                    {
                                        charParse[3] = (char)localreader.Read();
                                        if (charParse[3] == 'n')
                                        {
                                            charParse[4] = (char)localreader.Read();
                                            if (charParse[4] == 't')
                                            {
                                                STOP = true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        STOP = false;
                        //////////////////////////////////////////////////////////////////

                        //PLAYERCOUNT
                        while (!STOP)
                        {
                            charParse[0] = (char)localreader.Read();
                            if (charParse[0] == 'a')
                            {
                                charParse[1] = (char)localreader.Read();
                                if (charParse[1] == 'l')
                                {
                                    charParse[2] = (char)localreader.Read();
                                    if (charParse[2] == 'u')
                                    {
                                        charParse[3] = (char)localreader.Read();
                                        if (charParse[3] == 'e')
                                        {
                                            charParse[4] = (char)localreader.Read();
                                            if (charParse[4] == '"')
                                            {
                                                STOP = true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        STOP = false; localreader.Read();
                        while (charParse[0] != ',')
                        {
                            charParse[0] = (char)localreader.Read();
                            if (charParse[0] == ',')
                            {
                                break;
                            }
                            outPut += charParse[0];
                        }
                        float playercount;
                        float.TryParse(outPut, out playercount); outPut = string.Empty;
                        Match.Playercount = (byte)playercount;
                        charParse[0] = 'a';
                        while (charParse[0] != ',')
                        {
                            charParse[0] = (char)localreader.Read();
                        }

                        //TEAMSCORE
                        while (!STOP)
                        {
                            charParse[0] = (char)localreader.Read();
                            if (charParse[0] == 'a')
                            {
                                charParse[1] = (char)localreader.Read();
                                if (charParse[1] == 'l')
                                {
                                    charParse[2] = (char)localreader.Read();
                                    if (charParse[2] == 'u')
                                    {
                                        charParse[3] = (char)localreader.Read();
                                        if (charParse[3] == 'e')
                                        {
                                            charParse[4] = (char)localreader.Read();
                                            if (charParse[4] == '"')
                                            {
                                                STOP = true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        STOP = false; localreader.Read();
                        while (charParse[0] != ',')
                        {
                            charParse[0] = (char)localreader.Read();
                            if (charParse[0] == ',')
                            {
                                break;
                            }
                            outPut += charParse[0];
                        }
                        float teamscore;
                        float.TryParse(outPut, out teamscore); outPut = string.Empty;
                        Match.Teamscore = (ushort)teamscore;
                        charParse[0] = 'a';
                        while (charParse[0] != ',')
                        {
                            charParse[0] = (char)localreader.Read();
                        }

                        ///////////////////////////////////////////////////////////////////////////////////
                        //READ ACTIVITY END
                        ///////////////////////////////////////////////////////////////////////////////////

                        GetActivityDefinition();
                        tempList.Add(Match);

                    }
                }
                //PAGE END
                if (outofDate == false)
                {
                    page++;
                }
                if (page >= 3)
                {
                    break;
                }
            }
            return tempList;
        }

        //public void GetPostGameCarnageReport()
        //{
        //}

        public void GetActivityDefinition()
        {
            rClient.URL = "https://www.bungie.net/Platform/Destiny2/Manifest/DestinyActivityDefinition/" + Match.directorActivityHash + "/";
            string strDef = rClient.makeRequest();
            StringReader Defreader = new StringReader(strDef);
            bool DefSTOP = false; char[] DefParse = new char[5];
            string DefoutPut = string.Empty;

            while (!DefSTOP)
            {
                DefParse[0] = (char)Defreader.Read();
                if (DefParse[0] == 'r')
                {
                    DefParse[1] = (char)Defreader.Read();
                    if (DefParse[1] == 't')
                    {
                        DefParse[2] = (char)Defreader.Read();
                        if (DefParse[2] == 'i')
                        {
                            DefParse[3] = (char)Defreader.Read();
                            if (DefParse[3] == 'e')
                            {
                                DefParse[4] = (char)Defreader.Read();
                                if (DefParse[4] == 's')
                                {
                                    DefSTOP = true;
                                }
                            }
                        }
                    }
                }
            }
            DefSTOP = false;

            while (!DefSTOP)
            {
                DefParse[0] = (char)Defreader.Read();
                if (DefParse[0] == 'n')
                {
                    DefParse[1] = (char)Defreader.Read();
                    if (DefParse[1] == 'a')
                    {
                        DefParse[2] = (char)Defreader.Read();
                        if (DefParse[2] == 'm')
                        {
                            DefParse[3] = (char)Defreader.Read();
                            if (DefParse[3] == 'e')
                            {
                                DefParse[4] = (char)Defreader.Read();
                                if (DefParse[4] == '"')
                                {
                                    DefSTOP = true;
                                }
                            }
                        }
                    }
                }
                else if (DefParse[0] == 65535)
                {
                    break;
                }
            }
            DefSTOP = false; Defreader.Read(); Defreader.Read();

            if (DefParse[0] != 65535)
            {
                while (DefParse[0] != '"')
                {
                    DefParse[0] = (char)Defreader.Read();
                    if (DefParse[0] == '"')
                    {
                        break;
                    }
                    DefoutPut += DefParse[0];
                }
                Match.ActivityDefinition = DefoutPut;
            }
        }
    }
}
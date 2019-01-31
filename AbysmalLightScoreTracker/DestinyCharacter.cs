using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace AbysmalLightScoreTracker
{
    class DestinyCharacter
    {
        public string ID;
        public string LastPlayed;
        public ushort baselevel;
        public ushort light;
        public byte race;
        public byte gender;
        public byte c_class;

        StringReader localreader;
        public string outPut;

        private char[] charParse = new char[5];
        private bool STOP = false;

        public DestinyCharacter(RESTClient client, string characterID, string playerID, string playerType)
        {
            baselevel = ushort.MinValue;
            light = baselevel;
            race = byte.MinValue;
            gender = race;
            c_class = race;
            ID = characterID;

            client.URL = "https://www.bungie.net/Platform/Destiny2/" + playerType + "/Profile/" + playerID + "/Character/" + ID + "/?components=200";
            outPut = client.makeRequest();
            ParseCharacter();

            string Race = string.Empty;
            if (race == 0) { Race = "Humano"; } else if(race == 1) { Race = "Insomne"; } else if(race == 2) { Race = "Exo"; } else { Race = "Desconocido"; }
            string Class = string.Empty;
            if (c_class == 0) { Class = "Titan"; } else if (c_class == 1) { Class = "Cazador"; } else if (c_class == 2) { Class = "Hechicero"; } else { Class = "Desconocido"; }
            string Gender = string.Empty;
            if (gender == 0) { Gender = "Hombre"; } else if (gender == 1) { Gender = "Mujer"; } else { Gender = "Desconocido"; }

            outPut = "Personaje: " + Environment.NewLine +
                Environment.NewLine + Class + " " + Race + Environment.NewLine +
                Gender + Environment.NewLine + Environment.NewLine +
                "Nivel: " + baselevel + Environment.NewLine + 
                "Potencia: " + light + Environment.NewLine +
                Environment.NewLine + "Ultima vez jugado: " + LastPlayed;
        }

        private void ParseCharacter()
        {
            localreader = new StringReader(outPut);

            while (!STOP)
            {
                charParse[0] = (char)localreader.Read();
                if (charParse[0] == 'l')
                {
                    charParse[1] = (char)localreader.Read();
                    if (charParse[1] == 'a')
                    {
                        charParse[2] = (char)localreader.Read();
                        if (charParse[2] == 'y')
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

            localreader.Read();
            localreader.Read();

            int count = 22;
            char[] tempCharBuffer = new char[count];
            localreader.Read(tempCharBuffer, 0, count);
            for (int i = 0; i < count; i++)
            {
                LastPlayed += tempCharBuffer[i].ToString();
            }

            while (!STOP)
            {
                charParse[0] = (char)localreader.Read();
                if (charParse[0] == 'l')
                {
                    charParse[1] = (char)localreader.Read();
                    if (charParse[1] == 'i')
                    {
                        charParse[2] = (char)localreader.Read();
                        if (charParse[2] == 'g')
                        {
                            charParse[3] = (char)localreader.Read();
                            if (charParse[3] == 'h')
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

            localreader.Read();
            localreader.Read();

            outPut = string.Empty;
            while (charParse[0] != ',')
            {
                charParse[0] = (char)localreader.Read();
                if(charParse[0] == ',')
                {
                    break;
                }
                outPut += charParse[0];
            }
            
            ushort.TryParse(outPut, out light);

            while (!STOP)
            {
                charParse[0] = (char)localreader.Read();
                if (charParse[0] == 'e')
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

            localreader.Read();
            localreader.Read();

            race = (byte)Char.GetNumericValue((char)localreader.Read());

            while (charParse[0] != ':')
            {
                charParse[0] = (char)localreader.Read();
            }charParse[0] = 'a';

            c_class = (byte)Char.GetNumericValue((char)localreader.Read());

            while (charParse[0] != ':')
            {
                charParse[0] = (char)localreader.Read();
            }

            gender = (byte)Char.GetNumericValue((char)localreader.Read());

            while (!STOP)
            {
                charParse[0] = (char)localreader.Read();
                if (charParse[0] == 'e')
                {
                    charParse[1] = (char)localreader.Read();
                    if (charParse[1] == 'v')
                    {
                        charParse[2] = (char)localreader.Read();
                        if (charParse[2] == 'e')
                        {
                            charParse[3] = (char)localreader.Read();
                            if (charParse[3] == 'l')
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
            STOP = false;
            localreader.Read();

            outPut = string.Empty;
            while (charParse[0] != ',')
            {
                charParse[0] = (char)localreader.Read();
                if (charParse[0] == ',')
                {
                    break;
                }
                outPut += charParse[0];
            }

            ushort.TryParse(outPut, out baselevel);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;

using AbysmalLightScoreTracker;

namespace SheetsQuickstart
{
    class PointTracker
    {
        // If modifying these scopes, delete your previously saved credentials
        // at ~/.credentials/sheets.googleapis.com-dotnet-quickstart.json
        static string[] Scopes = { SheetsService.Scope.SpreadsheetsReadonly };
        static string ApplicationName = "Google Sheets API .NET Quickstart";

        IList<IList<Object>> values;
        uint StoryPoints = 0, StrikePoints = 0,
             NightfallPoints = 0, RaidPoints = 0, CruciblePoints = 0, GambitPoints = 0;
        uint TotalPoints = 0;

        Member actual_member;

        public IList<IList<Object>> GetValues()
        {
            return values;
        }

        public uint GetStoryPoints() { return StoryPoints; }
        public uint GetStrikePoints() { return StrikePoints; }
        public uint GetNightfallPoints() { return NightfallPoints; }
        public uint GetRaidPoints() { return RaidPoints; }
        public uint GetCruciblePoints() { return CruciblePoints; }
        public uint GetGambitPoints() { return GambitPoints; }
        public uint GetTotalPoints() { return TotalPoints; }

        public PointTracker(string sheetID, Member member)
        {
            UserCredential credential;

            using (var stream =
                new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }

            // Create Google Sheets API service.
            var service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            // Define request parameters.
            String spreadsheetId = sheetID;
            String range = "A2:B";
            SpreadsheetsResource.ValuesResource.GetRequest request =
                    service.Spreadsheets.Values.Get(spreadsheetId, range);

            // Prints the names and majors of students in a sample spreadsheet:
            // https://docs.google.com/spreadsheets/d/1BxiMVs0XRA5nFMdKvBdBZjgmUUqptlbs74OgvE2upms/edit
            ValueRange response = request.Execute();
            values = response.Values;
            if (values != null && values.Count > 0)
            {
                Console.WriteLine("Name, Major");
                foreach (var row in values)
                {
                    // Print columns A and E, which correspond to indices 0 and 4.
                    Console.WriteLine("{0}, {1}", row[0], row[1]);
                }
            }
            else
            {
                Console.WriteLine("No data found.");
            }
            Console.Read();

            actual_member = member;
        }

        public ushort GetNumofActivities()
        {
            ushort StoryMissions = 0, StrikeMissions = 0, NightfallMissions = 0,
                   CrucibleMatches = 0, GambitMatches = 0, Raids = 0;

            Activity[][] Stories = actual_member.GetStoryMissions;
            for (int i = 0; i < Stories.Count(); i++)
            {
                if (Stories[i] != null)
                    StoryMissions += (ushort)Stories[i].Count();
            }

            Stories = actual_member.GetForgeMissions;
            for (int i = 0; i < Stories.Count(); i++)
            {
                if (Stories[i] != null)
                    StoryMissions += (ushort)Stories[i].Count();
            }

            Stories = actual_member.GetReckoningMissions;
            for (int i = 0; i < Stories.Count(); i++)
            {
                if (Stories[i] != null)
                    StoryMissions += (ushort)Stories[i].Count();
            }

            Activity[][] Strikes = actual_member.GetStrikeMissions;
            for (int i = 0; i < Strikes.Count(); i++)
            {
                if (Strikes[i] != null)
                    StrikeMissions += (ushort)Strikes[i].Count();
            }

            Activity[][] Nightfalls = actual_member.GetNightfallMissions;
            for (int i = 0; i < Nightfalls.Count(); i++)
            {
                if (Nightfalls[i] != null)
                    NightfallMissions += (ushort)Nightfalls[i].Count();
            }

            Activity[][] Crucible = actual_member.GetCrucibleMatches;
            for (int i = 0; i < Crucible.Count(); i++)
            {
                if (Crucible[i] != null)
                    CrucibleMatches += (ushort)Crucible[i].Count();
            }

            Activity[][] Gambit = actual_member.GetGambitMatches;
            for (int i = 0; i < Gambit.Count(); i++)
            {
                if (Gambit[i] != null)
                    GambitMatches += (ushort)Gambit[i].Count();
            }

            Gambit = actual_member.GetGambitPrimeMatches;
            for (int i = 0; i < Gambit.Count(); i++)
            {
                if (Gambit[i] != null)
                    GambitMatches += (ushort)Gambit[i].Count();
            }

            Activity[][] Raid = actual_member.GetRaids;
            for (int i = 0; i < Raid.Count(); i++)
            {
                if (Raid[i] != null)
                    Raids += (ushort)Raid[i].Count();
            }

            return (ushort)(StoryMissions + StrikeMissions + NightfallMissions + CrucibleMatches + GambitMatches + Raids);
        }

        public ushort GetActivityLevel()
        {
            ushort ActivityLevel = 0;
            foreach (IList<object> element in values)
            {
                if (element[0].ToString() == "Actividades(NOPUNTOS)")
                {
                    ushort MaxActivities = 0;
                    ushort.TryParse(element[1].ToString(), out MaxActivities);
                    ActivityLevel = (ushort)(10 * GetNumofActivities() / MaxActivities);
                    if (ActivityLevel >= 10)
                    {
                        ActivityLevel = 10;
                    }
                }
            }
            return ActivityLevel;
        }

        public byte GetStoplightColor()
        {
            ushort Level = GetActivityLevel();
            byte Color; //red
            if (Level >= 6)
            {
                Color = 1; //green
            }
            else if (Level >= 3 && Level < 6)
            {
                Color = 3; //yellow
            }
            else
            {
                Color = 2;
            }
            return Color;
        }

        public void CalculateTotalPoints()
        {
            StoryPoints = CalculateStoryPoints();
            StrikePoints = CalculateStrikePoints();
            NightfallPoints = CalculateNightfallPoints();
            CruciblePoints = CalculateCruciblePoints();
            GambitPoints = CalculateGambitPoints();
            RaidPoints = CalculateRaidPoints();

            TotalPoints = StoryPoints + StrikePoints +
                          NightfallPoints + CruciblePoints +
                          GambitPoints + RaidPoints;
        }

        uint CalculateStoryPoints()
        {
            Activity[][] Stories = actual_member.GetStoryMissions;
            uint PointsperMission = 0;
            uint ReckoningTierI = 0;
            uint ReckoningTierII = 0;
            uint ReckoningTierIII = 0;
            uint ShatteredThronePoints = 0;
            uint Points = 0;

            foreach (IList<object> element in values)
            {
                if (element[0].ToString() == "Mision diaria")
                {
                    uint.TryParse(element[1].ToString(), out PointsperMission);
                }
                else if (element[0].ToString() == "Shattered Throne")
                {
                    uint.TryParse(element[1].ToString(), out ShatteredThronePoints);
                }
                else if (element[0].ToString() == "Reckoning tier 1")
                {
                    uint.TryParse(element[1].ToString(), out ReckoningTierI);
                }
                else if (element[0].ToString() == "Reckoning tier 2")
                {
                    uint.TryParse(element[1].ToString(), out ReckoningTierII);
                }
                else if (element[0].ToString() == "Reckoning tier 3")
                {
                    uint.TryParse(element[1].ToString(), out ReckoningTierIII);
                }
            }

            for (int i = 0; i < Stories.Count(); ++i)
            {
                if (Stories[i] != null)
                    for (int j = 0; j < Stories[i].Count(); ++j)
                    {
                        if (Stories[i][j].Completed && Stories[i][j].Compwclanmembers)
                        {
                            if (Stories[i][j].ActivityDefinition == "The Shattered Throne")
                            {
                                Points += ShatteredThronePoints;
                            }
                            else
                            {
                                Points += PointsperMission;
                            }
                        }
                    }
            }

            Stories = actual_member.GetForgeMissions;

            for (int i = 0; i < Stories.Count(); ++i)
            {
                if (Stories[i] != null)
                    for (int j = 0; j < Stories[i].Count(); ++j)
                    {
                        if (Stories[i][j].Completed && Stories[i][j].Compwclanmembers)
                        {
                            Points += PointsperMission;
                        }
                    }
            }

            Stories = actual_member.GetReckoningMissions;

            for (int i = 0; i < Stories.Count(); ++i)
            {
                if (Stories[i] != null)
                    for (int j = 0; j < Stories[i].Count(); ++j)
                    {
                        if (Stories[i][j].Compwclanmembers)
                        {
                            if (Stories[i][j].ActivityDefinition == "The Reckoning: Tier I")
                            {
                                Points += ReckoningTierI;
                            }
                            else if (Stories[i][j].ActivityDefinition == "The Reckoning: Tier II")
                            {
                                Points += ReckoningTierII;
                            }
                            else if (Stories[i][j].ActivityDefinition == "The Reckoning: Tier III")
                            {
                                Points += ReckoningTierIII;
                            }
                        }
                    }
            }
            return Points;
        }

        uint CalculateStrikePoints()
        {
            Activity[][] Strikes = actual_member.GetStrikeMissions;
            uint PointsperMission = 0;
            uint Points = 0;

            foreach (IList<object> element in values)
            {
                if (element[0].ToString() == "Asalto")
                {
                    uint.TryParse(element[1].ToString(), out PointsperMission);
                }
            }

            for (int i = 0; i < Strikes.Count(); ++i)
            {
                if (Strikes[i] != null)
                    for (int j = 0; j < Strikes[i].Count(); ++j)
                    {
                        if (Strikes[i][j].Completed && Strikes[i][j].Compwclanmembers)
                        {
                            Points += PointsperMission;
                        }
                    }
            }
            return Points;
        }

        uint CalculateNightfallPoints()
        {
            Activity[][] Nightfalls = actual_member.GetNightfallMissions;
            uint PointsperMission = 0;
            uint Points = 0;

            foreach (IList<object> element in values)
            {
                if (element[0].ToString() == "Ocaso")
                {
                    uint.TryParse(element[1].ToString(), out PointsperMission);
                }
            }

            for (int i = 0; i < Nightfalls.Count(); ++i)
            {
                if (Nightfalls[i] != null)
                    for (int j = 0; j < Nightfalls[i].Count(); ++j)
                    {
                        if (Nightfalls[i][j].Completed && Nightfalls[i][j].Compwclanmembers)
                        {
                            Points += PointsperMission;
                        }
                    }
            }
            return Points;
        }

        uint CalculateRaidPoints()
        {
            Activity[][] Raids = actual_member.GetRaids;

            uint LeviathanRaidsNormal = 0;
            uint LeviathanRaidsPrestige = 0;
            uint LastWish = 0;
            uint ScourgeofthePast = 0;

            uint Points = 0;

            foreach (IList<object> element in values)
            {
                if (element[0].ToString() == "Leviatan raids Normal")
                {
                    uint.TryParse(element[1].ToString(), out LeviathanRaidsNormal);
                }
                else if (element[0].ToString() == "Leviatan raids Prestigio")
                {
                    uint.TryParse(element[1].ToString(), out LeviathanRaidsPrestige);
                }
                else if (element[0].ToString() == "Ultimo Deseo")
                {
                    uint.TryParse(element[1].ToString(), out LastWish);
                }
                else if (element[0].ToString() == "Scourge of the Past")
                {
                    uint.TryParse(element[1].ToString(), out ScourgeofthePast);
                }
            }

            for (int i = 0; i < Raids.Count(); ++i)
            {
                if (Raids[i] != null)
                    for (int j = 0; j < Raids[i].Count(); ++j)
                    {
                        if (Raids[i][j].Completed && Raids[i][j].Compwclanmembers)
                        {
                            if (Raids[i][j].ActivityDefinition == "Leviathan: Normal" ||
                               Raids[i][j].ActivityDefinition == "Leviathan, Eater of Worlds: Normal" ||
                               Raids[i][j].ActivityDefinition == "Leviathan, Spire of Stars: Normal")
                            {
                                Points += LeviathanRaidsNormal;
                            }
                            else if (Raids[i][j].ActivityDefinition == "Leviathan: Prestige" ||
                               Raids[i][j].ActivityDefinition == "Leviathan, Eater of Worlds: Prestige" ||
                               Raids[i][j].ActivityDefinition == "Leviathan, Spire of Stars: Prestige")
                            {
                                Points += LeviathanRaidsPrestige;
                            }
                            else if (Raids[i][j].ActivityDefinition == "Last Wish: Level 55")
                            {
                                Points += LastWish;
                            }
                            else if (Raids[i][j].ActivityDefinition == "Scourge of the Past")
                            {
                                Points += ScourgeofthePast;
                            }
                        }
                    }
            }
            return Points;
        }

        uint CalculateCruciblePoints()
        {
            Activity[][] Crucible = actual_member.GetCrucibleMatches;

            uint CompetitiveLose = 0;
            uint CompetitiveWin = 0;
            uint OtherLose = 0;
            uint OtherWin = 0;
            uint IronWin = 0;
            uint IronLose = 0;

            uint MayhemWin = 0;
            uint MayhemLose = 0;
            uint CrimsonWin = 0;
            uint CrimsonLose = 0;
            uint BreakthroughWin = 0;
            uint BreakthroughLose = 0;
            uint ShowdownWin = 0;
            uint ShowdownLose = 0;
            uint PrivateWin = 0;
            uint PrivateLose = 0;

            uint FortyPlusNormal = 0;
            uint FortyPlusSpecial = 0;
            uint ThirtyPlusNormal = 0;
            uint ThirtyPlusSpecial = 0;
            uint TwopointfiveKDNormal = 0;
            uint TwopointfiveKDSpecial = 0;
            uint FiveKDNormal = 0;
            uint FiveKDSpecial = 0;
            uint TenKDNormal = 0;
            uint TenKDSpecial = 0;

            uint Points = 0;

            foreach (IList<object> element in values)
            {
                if (element[0].ToString() == "Derrota quickplay")
                {
                    uint.TryParse(element[1].ToString(), out OtherLose);
                }
                else if (element[0].ToString() == "Victoria quickplay")
                {
                    uint.TryParse(element[1].ToString(), out OtherWin);
                }
                else if (element[0].ToString() == "Victoria estandarte")
                {
                    uint.TryParse(element[1].ToString(), out IronWin);
                }
                else if (element[0].ToString() == "Derrota estandarte")
                {
                    uint.TryParse(element[1].ToString(), out IronLose);
                }
                else if (element[0].ToString() == "Victoria privada")
                {
                    uint.TryParse(element[1].ToString(), out PrivateWin);
                }
                else if (element[0].ToString() == "Derrota privada")
                {
                    uint.TryParse(element[1].ToString(), out PrivateLose);
                }
                else if (element[0].ToString() == "Derrota competitivo")
                {
                    uint.TryParse(element[1].ToString(), out CompetitiveLose);
                }
                else if (element[0].ToString() == "Victoria competitivo")
                {
                    uint.TryParse(element[1].ToString(), out CompetitiveWin);
                }
                else if (element[0].ToString() == "Victoria Caos")
                {
                    uint.TryParse(element[1].ToString(), out MayhemWin);
                }
                else if (element[0].ToString() == "Derrota Caos")
                {
                    uint.TryParse(element[1].ToString(), out MayhemLose);
                }
                else if (element[0].ToString() == "Victoria Carmesi")
                {
                    uint.TryParse(element[1].ToString(), out CrimsonWin);
                }
                else if (element[0].ToString() == "Derrota Carmesi")
                {
                    uint.TryParse(element[1].ToString(), out CrimsonLose);
                }
                else if (element[0].ToString() == "Victoria Breakthrough")
                {
                    uint.TryParse(element[1].ToString(), out BreakthroughWin);
                }
                else if (element[0].ToString() == "Derrota Breakthrough")
                {
                    uint.TryParse(element[1].ToString(), out BreakthroughLose);
                }
                else if (element[0].ToString() == "Victoria Showdown")
                {
                    uint.TryParse(element[1].ToString(), out ShowdownWin);
                }
                else if (element[0].ToString() == "Derrota Showdown")
                {
                    uint.TryParse(element[1].ToString(), out ShowdownLose);
                }
                else if (element[0].ToString() == "30+ derrotados")
                {
                    uint.TryParse(element[1].ToString(), out ThirtyPlusNormal);
                }
                else if (element[0].ToString() == "40+ derrotados")
                {
                    uint.TryParse(element[1].ToString(), out FortyPlusNormal);
                }
                else if (element[0].ToString() == "30+ derrotados especial")
                {
                    uint.TryParse(element[1].ToString(), out ThirtyPlusSpecial);
                }
                else if (element[0].ToString() == "40+ derrotados especial")
                {
                    uint.TryParse(element[1].ToString(), out FortyPlusSpecial);
                }
                else if (element[0].ToString() == "2.5 kd")
                {
                    uint.TryParse(element[1].ToString(), out TwopointfiveKDNormal);
                }
                else if (element[0].ToString() == "5 kd")
                {
                    uint.TryParse(element[1].ToString(), out FiveKDNormal);
                }
                else if (element[0].ToString() == "10 kd")
                {
                    uint.TryParse(element[1].ToString(), out TenKDNormal);
                }
                else if (element[0].ToString() == "2.5 kd especial")
                {
                    uint.TryParse(element[1].ToString(), out TwopointfiveKDSpecial);
                }
                else if (element[0].ToString() == "5 kd especial")
                {
                    uint.TryParse(element[1].ToString(), out FiveKDSpecial);
                }
                else if (element[0].ToString() == "10 kd especial")
                {
                    uint.TryParse(element[1].ToString(), out TenKDSpecial);
                }
            }

            for (int i = 0; i < Crucible.Count(); ++i)
            {
                if (Crucible[i] != null)
                    for (int j = 0; j < Crucible[i].Count(); ++j)
                    {
                        if (Crucible[i][j].Completed && Crucible[i][j].Compwclanmembers)
                        {
                            if (Crucible[i][j].ActivityDefinition == "Competitive")
                            {
                                if (Crucible[i][j].Standing == "Victory")
                                {
                                    Points += CompetitiveWin;
                                }
                                else if (Crucible[i][j].Standing == "Defeat")
                                {
                                    Points += CompetitiveLose;
                                }
                            }
                            else if (Crucible[i][j].ActivityDefinition == "Iron Banner")
                            {
                                if (Crucible[i][j].Standing == "Victory")
                                {
                                    Points += IronWin;
                                }
                                else if (Crucible[i][j].Standing == "Defeat")
                                {
                                    Points += IronLose;
                                }
                            }
                            else if (Crucible[i][j].ActivityDefinition == "Crimson Days")
                            {
                                if (Crucible[i][j].Standing == "Victory")
                                {
                                    Points += CrimsonWin;
                                }
                                else if (Crucible[i][j].Standing == "Defeat")
                                {
                                    Points += CrimsonLose;
                                }
                            }
                            else if (Crucible[i][j].ActivityDefinition == "Breakthrough")
                            {
                                if (Crucible[i][j].Standing == "Victory")
                                {
                                    Points += BreakthroughWin;
                                }
                                else if (Crucible[i][j].Standing == "Defeat")
                                {
                                    Points += BreakthroughLose;
                                }
                            }
                            else if (Crucible[i][j].ActivityDefinition == "Showdown")
                            {
                                if (Crucible[i][j].Standing == "Victory")
                                {
                                    Points += ShowdownWin;
                                }
                                else if (Crucible[i][j].Standing == "Defeat")
                                {
                                    Points += ShowdownLose;
                                }
                            }
                            else if (Crucible[i][j].ActivityDefinition == "Mayhem")
                            {
                                if (Crucible[i][j].Standing == "Victory")
                                {
                                    Points += MayhemWin;
                                }
                                else if (Crucible[i][j].Standing == "Defeat")
                                {
                                    Points += MayhemLose;
                                }
                            }
                            else if (Crucible[i][j].ActivityDefinition == "Private Match")
                            {
                                if (Crucible[i][j].Standing == "Victory")
                                {
                                    Points += PrivateWin;
                                }
                                else if (Crucible[i][j].Standing == "Defeat")
                                {
                                    Points += PrivateLose;
                                }
                            }
                            else
                            {
                                if (Crucible[i][j].Standing == "Victory")
                                {
                                    Points += OtherWin;
                                }
                                else if (Crucible[i][j].Standing == "Defeat")
                                {
                                    Points += OtherLose;
                                }
                            }

                            if (Crucible[i][j].Kills >= 30 && Crucible[i][j].Kills < 40)
                            {
                                if (Crucible[i][j].ActivityDefinition == "Crimson Days" ||
                                    Crucible[i][j].ActivityDefinition == "Iron Banner")
                                { Points += ThirtyPlusSpecial; }
                                else
                                {
                                    Points += ThirtyPlusNormal;
                                }
                            }
                            else if (Crucible[i][j].Kills >= 40)
                            {
                                if (Crucible[i][j].ActivityDefinition == "Crimson Days" ||
                                    Crucible[i][j].ActivityDefinition == "Iron Banner")
                                { Points += FortyPlusSpecial; }
                                else
                                {
                                    Points += FortyPlusNormal;
                                }
                            }

                            if (Crucible[i][j].KD >= 2.5f && Crucible[i][j].KD < 5.0f)
                            {
                                if (Crucible[i][j].ActivityDefinition == "Crimson Days" ||
                                    Crucible[i][j].ActivityDefinition == "Iron Banner")
                                { Points += TwopointfiveKDSpecial; }
                                else
                                {
                                    Points += TwopointfiveKDNormal;
                                }
                            }
                            else if (Crucible[i][j].KD >= 5.0f && Crucible[i][j].KD < 10.0f)
                            {
                                if (Crucible[i][j].ActivityDefinition == "Crimson Days" ||
                                    Crucible[i][j].ActivityDefinition == "Iron Banner")
                                { Points += FiveKDSpecial; }
                                else
                                {
                                    Points += FiveKDNormal;
                                }
                            }
                            else if (Crucible[i][j].KD >= 10.0f)
                            {
                                if (Crucible[i][j].ActivityDefinition == "Crimson Days" ||
                                    Crucible[i][j].ActivityDefinition == "Iron Banner")
                                { Points += TenKDSpecial; }
                                else
                                {
                                    Points += TenKDNormal;
                                }
                            }
                        }
                    }
            }
            return Points;
        }

        uint CalculateGambitPoints()
        {
            Activity[][] Gambit = actual_member.GetGambitMatches;

            uint RoundWon = 0;
            uint RoundPrimeWon = 0;
            uint FiveGuardians = 0;
            uint EightGuardians = 0;
            uint FiftyMotes = 0;
            uint SeventyMotes = 0;

            uint Points = 0;

            foreach (IList<object> element in values)
            {
                if (element[0].ToString() == "Ronda gambito")
                {
                    uint.TryParse(element[1].ToString(), out RoundWon);
                }
                else if (element[0].ToString() == "Ronda gambito supremo")
                {
                    uint.TryParse(element[1].ToString(), out RoundPrimeWon);
                }
                else if (element[0].ToString() == "5 guardianes gambito")
                {
                    uint.TryParse(element[1].ToString(), out FiveGuardians);
                }
                else if (element[0].ToString() == "8 guardianes gambito")
                {
                    uint.TryParse(element[1].ToString(), out EightGuardians);
                }
                else if (element[0].ToString() == "Depositar 50+ motas")
                {
                    uint.TryParse(element[1].ToString(), out FiftyMotes);
                }
                else if (element[0].ToString() == "Depositar 70+ motas")
                {
                    uint.TryParse(element[1].ToString(), out SeventyMotes);
                }
            }

            for (int i = 0; i < Gambit.Count(); ++i)
            {
                if (Gambit[i] != null)
                    for (int j = 0; j < Gambit[i].Count(); ++j)
                    {
                        if (Gambit[i][j].Completed && Gambit[i][j].Compwclanmembers)
                        {
                            Points += RoundWon * Gambit[i][j].Teamscore;

                            if (Gambit[i][j].Gambit_guardiankills >= 5 && Gambit[i][j].Gambit_guardiankills < 8)
                            {
                                Points += FiveGuardians;
                            }
                            else if (Gambit[i][j].Gambit_guardiankills >= 8)
                            {
                                Points += EightGuardians;
                            }

                            if (Gambit[i][j].Gambit_motesdeposited >= 50 && Gambit[i][j].Gambit_motesdeposited < 70)
                            {
                                Points += FiftyMotes;
                            }
                            else if (Gambit[i][j].Gambit_motesdeposited >= 70)
                            {
                                Points += SeventyMotes;
                            }
                        }
                    }
            }

            Gambit = actual_member.GetGambitPrimeMatches;

            for (int i = 0; i < Gambit.Count(); ++i)
            {
                if (Gambit[i] != null)
                    for (int j = 0; j < Gambit[i].Count(); ++j)
                    {
                        if (Gambit[i][j].Completed && Gambit[i][j].Compwclanmembers)
                        {
                            Points += RoundPrimeWon * Gambit[i][j].Teamscore;

                            if (Gambit[i][j].Gambit_guardiankills >= 5 && Gambit[i][j].Gambit_guardiankills < 8)
                            {
                                Points += FiveGuardians;
                            }
                            else if (Gambit[i][j].Gambit_guardiankills >= 8)
                            {
                                Points += EightGuardians;
                            }

                            if (Gambit[i][j].Gambit_motesdeposited >= 50 && Gambit[i][j].Gambit_motesdeposited < 70)
                            {
                                Points += FiftyMotes;
                            }
                            else if (Gambit[i][j].Gambit_motesdeposited >= 70)
                            {
                                Points += SeventyMotes;
                            }
                        }
                    }
            }

            return Points;
        }
    }
}
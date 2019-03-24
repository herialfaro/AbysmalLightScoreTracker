using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using SheetsQuickstart;
using System.Runtime.InteropServices;

namespace AbysmalLightScoreTracker
{
    public partial class ALScoreTracker : Form
    {
        RESTClient rClient;

        DestinyCharacter new_character;
        OtherPVE new_missionmanager;
        OtherPVE new_forgemanager;
        OtherPVE new_reckoningmanager;
        OtherPVE new_strikemanager;
        OtherPVE new_nightfallmanager;
        RaidManager new_raidmanager;
        CrucibleManager new_cruciblemanager;
        GambitManager new_gambitmanager;
        GambitManager new_gambitprimemanager;
        ClanMemberParser new_clanmembermanager;

        PointTracker myPointTracker;
        IList<IList<Object>> points;

        Member actual_member;

        public DateTime Reset_time;
        public DateTime Date_limit;
        public DateTime tmp_time;
        public DateTime tmp_limit;

        public DayOfWeek Date_selected;

        public string strGlobal;
        public string clanID;
        public string STATE;

        public bool LastWeek;

        List<DestinyCharacter> characterList;

        public ALScoreTracker()
        {
            InitializeComponent();
        }

        private void ALScoreTracker_Load(object sender, EventArgs e)
        {
            rClient = new RESTClient();
            rClient.header = "X-API-KEY:01c645986566474199d1fb7a466c5699";

            new_clanmembermanager = new ClanMemberParser();

            characterList = new List<DestinyCharacter>();
            strGlobal = string.Empty;
            LastWeek = true;

            Reset();
        }

        private void ConvertDateTime()
        {
            Reset_time = DateTime.Now.Date;
            Reset_time = Reset_time.AddDays(-1);

            while (Reset_time.DayOfWeek != Date_selected)
            //while (Reset_time.DayOfWeek != DayOfWeek.Wednesday)
            { Reset_time = Reset_time.AddDays(-1); }
            Reset_time = Reset_time.AddHours(18);

            Date_limit = Reset_time.AddDays(7);

            //Debug_box.Text = Last_tuesday.ToLongDateString();

            tmp_time = Reset_time;
            tmp_limit = Date_limit;
        }

        private void SelectActualMember()
        {
            foreach (Member element in new_clanmembermanager.ClanMembersList)
            {
                if (element.Name == select_User.Text)
                {
                    actual_member = element;
                    break;
                }
            }
        }

        private void SelectPeriod()
        {
            if (periodSelection.Text == "Lunes")
            {
                Date_selected = DayOfWeek.Monday;
            }
            else if (periodSelection.Text == "Martes")
            {
                Date_selected = DayOfWeek.Tuesday;
            }
            else if (periodSelection.Text == "Miercoles")
            {
                Date_selected = DayOfWeek.Wednesday;
            }
            else if (periodSelection.Text == "Jueves")
            {
                Date_selected = DayOfWeek.Thursday;
            }
            else if (periodSelection.Text == "Viernes")
            {
                Date_selected = DayOfWeek.Friday;
            }
            else if (periodSelection.Text == "Sabado")
            {
                Date_selected = DayOfWeek.Saturday;
            }
            else
            {
                Date_selected = DayOfWeek.Sunday;
            }
        }

        private void load_id_Click(object sender, EventArgs e)
        {
            SelectPeriod();
            ConvertDateTime();

            if (checkBox1.Checked)
            {
                Reset_time = tmp_time.AddDays(-7);
                Date_limit = tmp_limit.AddDays(-7);
            }
            else
            {
                Reset_time = tmp_time;
                Date_limit = tmp_limit;
            }

            strGlobal = "";

            STATE = modeSelection.Text;
            SelectActualMember();

            //STATE MACHINE

            if (actual_member != null)
            {
                if (actual_member.Type == "1")
                {
                    pictureBox1.Visible = false;
                    pictureBox2.Visible = true;
                }
                else if (actual_member.Type == "4")
                {
                    pictureBox2.Visible = false;
                    pictureBox1.Visible = true;
                }

                //STATES

                if (STATE == "General" || STATE == "")
                {
                    General_box.Visible = true;
                    Mission_Box.Visible = false;
                    Strike_box.Visible = false;
                    Raid_box.Visible = false;
                    Crucible_Box.Visible = false;
                    Gambit_Box.Visible = false;
                    Points_Box.Visible = false;

                    string[] characterDesctiptions = new string[3];
                    characterDesctiptions[0] = string.Empty;
                    characterDesctiptions[1] = string.Empty;
                    characterDesctiptions[2] = string.Empty;
                    List<string> IDs = actual_member.GetcharacterIDs();
                    for (int i = 0; i < IDs.Count; i++)
                    {
                        new_character = new DestinyCharacter(rClient, IDs[i], actual_member.ID, actual_member.Type);
                        characterList.Add(new_character);
                        characterDesctiptions[i] = new_character.outPut;
                    }
                    charText1.Text = characterDesctiptions[0];
                    charText2.Text = characterDesctiptions[1];
                    charText3.Text = characterDesctiptions[2];
                }
                else if (STATE == "Misiones de historia")
                {
                    Mission_Box.Visible = true;
                    General_box.Visible = false;
                    Strike_box.Visible = false;
                    Raid_box.Visible = false;
                    Crucible_Box.Visible = false;
                    Gambit_Box.Visible = false;
                    Points_Box.Visible = false;

                    MissionView1.Items.Clear();
                    MissionView2.Items.Clear();
                    MissionView3.Items.Clear();

                    new_missionmanager = new OtherPVE(rClient, Reset_time, Date_limit, new_clanmembermanager.ClanMembersList, actual_member.ID, 2);

                    int i = 0;
                    List<string> IDs = actual_member.GetcharacterIDs();
                    Activity[][] StoryMissions = actual_member.GetStoryMissions;
                    foreach (string element in IDs)
                    {
                        new_missionmanager.ParseOther(actual_member.ID, element, actual_member.Type);
                        StoryMissions[i] = new_missionmanager.Activity_list;
                        i++;
                    }

                    ListViewItem itm = new ListViewItem();
                    foreach (Activity element in StoryMissions[0])
                    {
                        string[] arr = new string[8];
                        //add items to ListView
                        arr[0] = element.ActivityDefinition;
                        arr[1] = element.Completed.ToString();
                        arr[2] = element.Kills.ToString();
                        arr[3] = element.Assists.ToString();
                        arr[4] = element.Deaths.ToString();
                        arr[5] = element.Duration;
                        arr[6] = element.Compwclanmembers.ToString();
                        arr[7] = element.Period.ToString();
                        itm = new ListViewItem(arr);
                        MissionView1.Items.Add(itm);
                    }

                    if (StoryMissions[1] != null)
                    {
                        itm = new ListViewItem();
                        foreach (Activity element in StoryMissions[1])
                        {
                            string[] arr = new string[8];
                            //add items to ListView
                            arr[0] = element.ActivityDefinition;
                            arr[1] = element.Completed.ToString();
                            arr[2] = element.Kills.ToString();
                            arr[3] = element.Assists.ToString();
                            arr[4] = element.Deaths.ToString();
                            arr[5] = element.Duration;
                            arr[6] = element.Compwclanmembers.ToString();
                            arr[7] = element.Period.ToString();
                            itm = new ListViewItem(arr);
                            MissionView2.Items.Add(itm);
                        }
                    }

                    if (StoryMissions[2] != null)
                    {
                        itm = new ListViewItem();
                        foreach (Activity element in StoryMissions[2])
                        {
                            string[] arr = new string[8];
                            //add items to ListView
                            arr[0] = element.ActivityDefinition;
                            arr[1] = element.Completed.ToString();
                            arr[2] = element.Kills.ToString();
                            arr[3] = element.Assists.ToString();
                            arr[4] = element.Deaths.ToString();
                            arr[5] = element.Duration;
                            arr[6] = element.Compwclanmembers.ToString();
                            arr[7] = element.Period.ToString();
                            itm = new ListViewItem(arr);
                            MissionView3.Items.Add(itm);
                        }
                    }

                    new_forgemanager = new OtherPVE(rClient, Reset_time, Date_limit, new_clanmembermanager.ClanMembersList, actual_member.ID, 66);

                    i = 0;
                    Activity[][] ForgeMissions = actual_member.GetStoryMissions;
                    foreach (string element in IDs)
                    {
                        new_forgemanager.ParseOther(actual_member.ID, element, actual_member.Type);
                        ForgeMissions[i] = new_forgemanager.Activity_list;
                        i++;
                    }


                    foreach (Activity element in ForgeMissions[0])
                    {
                        string[] arr = new string[8];
                        //add items to ListView
                        arr[0] = element.ActivityDefinition;
                        arr[1] = element.Completed.ToString();
                        arr[2] = element.Kills.ToString();
                        arr[3] = element.Assists.ToString();
                        arr[4] = element.Deaths.ToString();
                        arr[5] = element.Duration;
                        arr[6] = element.Compwclanmembers.ToString();
                        arr[7] = element.Period.ToString();
                        itm = new ListViewItem(arr);
                        MissionView1.Items.Add(itm);
                    }

                    if (ForgeMissions[1] != null)
                    {
                        itm = new ListViewItem();
                        foreach (Activity element in ForgeMissions[1])
                        {
                            string[] arr = new string[8];
                            //add items to ListView
                            arr[0] = element.ActivityDefinition;
                            arr[1] = element.Completed.ToString();
                            arr[2] = element.Kills.ToString();
                            arr[3] = element.Assists.ToString();
                            arr[4] = element.Deaths.ToString();
                            arr[5] = element.Duration;
                            arr[6] = element.Compwclanmembers.ToString();
                            arr[7] = element.Period.ToString();
                            itm = new ListViewItem(arr);
                            MissionView2.Items.Add(itm);
                        }
                    }

                    if (ForgeMissions[2] != null)
                    {
                        itm = new ListViewItem();
                        foreach (Activity element in ForgeMissions[2])
                        {
                            string[] arr = new string[8];
                            //add items to ListView
                            arr[0] = element.ActivityDefinition;
                            arr[1] = element.Completed.ToString();
                            arr[2] = element.Kills.ToString();
                            arr[3] = element.Assists.ToString();
                            arr[4] = element.Deaths.ToString();
                            arr[5] = element.Duration;
                            arr[6] = element.Compwclanmembers.ToString();
                            arr[7] = element.Period.ToString();
                            itm = new ListViewItem(arr);
                            MissionView3.Items.Add(itm);
                        }
                    }

                    new_reckoningmanager = new OtherPVE(rClient, Reset_time, Date_limit, new_clanmembermanager.ClanMembersList, actual_member.ID, 76);

                    i = 0;
                    Activity[][] ReckoningMissions = actual_member.GetReckoningMissions;
                    foreach (string element in IDs)
                    {
                        new_reckoningmanager.ParseOther(actual_member.ID, element, actual_member.Type);
                        ReckoningMissions[i] = new_reckoningmanager.Activity_list;
                        i++;
                    }


                    foreach (Activity element in ReckoningMissions[0])
                    {
                        string[] arr = new string[8];
                        //add items to ListView
                        arr[0] = element.ActivityDefinition;
                        arr[1] = element.Completed.ToString();
                        arr[2] = element.Kills.ToString();
                        arr[3] = element.Assists.ToString();
                        arr[4] = element.Deaths.ToString();
                        arr[5] = element.Duration;
                        arr[6] = element.Compwclanmembers.ToString();
                        arr[7] = element.Period.ToString();
                        itm = new ListViewItem(arr);
                        MissionView1.Items.Add(itm);
                    }

                    if (ReckoningMissions[1] != null)
                    {
                        itm = new ListViewItem();
                        foreach (Activity element in ReckoningMissions[1])
                        {
                            string[] arr = new string[8];
                            //add items to ListView
                            arr[0] = element.ActivityDefinition;
                            arr[1] = element.Completed.ToString();
                            arr[2] = element.Kills.ToString();
                            arr[3] = element.Assists.ToString();
                            arr[4] = element.Deaths.ToString();
                            arr[5] = element.Duration;
                            arr[6] = element.Compwclanmembers.ToString();
                            arr[7] = element.Period.ToString();
                            itm = new ListViewItem(arr);
                            MissionView2.Items.Add(itm);
                        }
                    }

                    if (ReckoningMissions[2] != null)
                    {
                        itm = new ListViewItem();
                        foreach (Activity element in ReckoningMissions[2])
                        {
                            string[] arr = new string[8];
                            //add items to ListView
                            arr[0] = element.ActivityDefinition;
                            arr[1] = element.Completed.ToString();
                            arr[2] = element.Kills.ToString();
                            arr[3] = element.Assists.ToString();
                            arr[4] = element.Deaths.ToString();
                            arr[5] = element.Duration;
                            arr[6] = element.Compwclanmembers.ToString();
                            arr[7] = element.Period.ToString();
                            itm = new ListViewItem(arr);
                            MissionView3.Items.Add(itm);
                        }
                    }
                    strGlobal = Reset_time.ToShortDateString() + " - " + Date_limit.ToShortDateString();
                }
                else if (STATE == "Asaltos")
                {
                    Strike_box.Visible = true;
                    General_box.Visible = false;
                    Mission_Box.Visible = false;
                    Raid_box.Visible = false;
                    Crucible_Box.Visible = false;
                    Gambit_Box.Visible = false;
                    Points_Box.Visible = false;

                    StrikeView1.Items.Clear();
                    StrikeView2.Items.Clear();
                    StrikeView3.Items.Clear();

                    new_strikemanager = new OtherPVE(rClient, Reset_time, Date_limit, new_clanmembermanager.ClanMembersList, actual_member.ID, 3);

                    int i = 0;
                    List<string> IDs = actual_member.GetcharacterIDs();
                    Activity[][] StrikeMissions = actual_member.GetStrikeMissions;
                    foreach (string element in IDs)
                    {
                        new_strikemanager.ParseOther(actual_member.ID, element, actual_member.Type);
                        StrikeMissions[i] = new_strikemanager.Activity_list;
                        i++;
                    }

                    ListViewItem itm = new ListViewItem();
                    foreach (Activity element in StrikeMissions[0])
                    {
                        string[] arr = new string[8];
                        //add items to ListView
                        arr[0] = element.ActivityDefinition;
                        arr[1] = element.Completed.ToString();
                        arr[2] = element.Kills.ToString();
                        arr[3] = element.Assists.ToString();
                        arr[4] = element.Deaths.ToString();
                        arr[5] = element.Duration;
                        arr[6] = element.Compwclanmembers.ToString();
                        arr[7] = element.Period.ToString();
                        itm = new ListViewItem(arr);
                        StrikeView1.Items.Add(itm);
                    }

                    if (StrikeMissions[1] != null)
                    {
                        itm = new ListViewItem();
                        foreach (Activity element in StrikeMissions[1])
                        {
                            string[] arr = new string[8];
                            //add items to ListView
                            arr[0] = element.ActivityDefinition;
                            arr[1] = element.Completed.ToString();
                            arr[2] = element.Kills.ToString();
                            arr[3] = element.Assists.ToString();
                            arr[4] = element.Deaths.ToString();
                            arr[5] = element.Duration;
                            arr[6] = element.Compwclanmembers.ToString();
                            arr[7] = element.Period.ToString();
                            itm = new ListViewItem(arr);
                            StrikeView2.Items.Add(itm);
                        }
                    }

                    if (StrikeMissions[2] != null)
                    {
                        itm = new ListViewItem();
                        foreach (Activity element in StrikeMissions[2])
                        {
                            string[] arr = new string[8];
                            //add items to ListView
                            arr[0] = element.ActivityDefinition;
                            arr[1] = element.Completed.ToString();
                            arr[2] = element.Kills.ToString();
                            arr[3] = element.Assists.ToString();
                            arr[4] = element.Deaths.ToString();
                            arr[5] = element.Duration;
                            arr[6] = element.Compwclanmembers.ToString();
                            arr[7] = element.Period.ToString();
                            itm = new ListViewItem(arr);
                            StrikeView3.Items.Add(itm);
                        }
                    }
                    strGlobal = Reset_time.ToShortDateString() + " - " + Date_limit.ToShortDateString();
                }
                else if (STATE == "Ocasos")
                {
                    Strike_box.Visible = true;
                    General_box.Visible = false;
                    Mission_Box.Visible = false;
                    Raid_box.Visible = false;
                    Crucible_Box.Visible = false;
                    Gambit_Box.Visible = false;
                    Points_Box.Visible = false;

                    StrikeView1.Items.Clear();
                    StrikeView2.Items.Clear();
                    StrikeView3.Items.Clear();

                    new_nightfallmanager = new OtherPVE(rClient, Reset_time, Date_limit, new_clanmembermanager.ClanMembersList, actual_member.ID, 46);

                    int i = 0;
                    List<string> IDs = actual_member.GetcharacterIDs();
                    Activity[][] NightfallMissions = actual_member.GetNightfallMissions;
                    foreach (string element in IDs)
                    {
                        new_nightfallmanager.ParseOther(actual_member.ID, element, actual_member.Type);
                        NightfallMissions[i] = new_nightfallmanager.Activity_list;
                        i++;
                    }

                    ListViewItem itm = new ListViewItem();
                    foreach (Activity element in NightfallMissions[0])
                    {
                        string[] arr = new string[8];
                        //add items to ListView
                        arr[0] = element.ActivityDefinition;
                        arr[1] = element.Completed.ToString();
                        arr[2] = element.Kills.ToString();
                        arr[3] = element.Assists.ToString();
                        arr[4] = element.Deaths.ToString();
                        arr[5] = element.Duration;
                        arr[6] = element.Compwclanmembers.ToString();
                        arr[7] = element.Period.ToString();
                        itm = new ListViewItem(arr);
                        StrikeView1.Items.Add(itm);
                    }

                    if (NightfallMissions[1] != null)
                    {
                        itm = new ListViewItem();
                        foreach (Activity element in NightfallMissions[1])
                        {
                            string[] arr = new string[8];
                            //add items to ListView
                            arr[0] = element.ActivityDefinition;
                            arr[1] = element.Completed.ToString();
                            arr[2] = element.Kills.ToString();
                            arr[3] = element.Assists.ToString();
                            arr[4] = element.Deaths.ToString();
                            arr[5] = element.Duration;
                            arr[6] = element.Compwclanmembers.ToString();
                            arr[7] = element.Period.ToString();
                            itm = new ListViewItem(arr);
                            StrikeView2.Items.Add(itm);
                        }
                    }

                    if (NightfallMissions[2] != null)
                    {
                        itm = new ListViewItem();
                        foreach (Activity element in NightfallMissions[2])
                        {
                            string[] arr = new string[8];
                            //add items to ListView
                            arr[0] = element.ActivityDefinition;
                            arr[1] = element.Completed.ToString();
                            arr[2] = element.Kills.ToString();
                            arr[3] = element.Assists.ToString();
                            arr[4] = element.Deaths.ToString();
                            arr[5] = element.Duration;
                            arr[6] = element.Compwclanmembers.ToString();
                            arr[7] = element.Period.ToString();
                            itm = new ListViewItem(arr);
                            StrikeView3.Items.Add(itm);
                        }
                    }
                    strGlobal = Reset_time.ToShortDateString() + " - " + Date_limit.ToShortDateString();
                }
                else if (STATE == "Incursiones")
                {
                    Raid_box.Visible = true;
                    General_box.Visible = false;
                    Mission_Box.Visible = false;
                    Strike_box.Visible = false;
                    Crucible_Box.Visible = false;
                    Gambit_Box.Visible = false;
                    Points_Box.Visible = false;

                    RaidView1.Items.Clear();
                    RaidView2.Items.Clear();
                    RaidView3.Items.Clear();

                    new_raidmanager = new RaidManager(rClient, Reset_time, Date_limit, new_clanmembermanager.ClanMembersList, actual_member.ID, 4);

                    int i = 0;
                    List<string> IDs = actual_member.GetcharacterIDs();
                    Activity[][] Raids = actual_member.GetRaids;
                    foreach (string element in IDs)
                    {
                        new_raidmanager.ParseRaid(actual_member.ID, element, actual_member.Type);
                        Raids[i] = new_raidmanager.Activity_list;
                        i++;
                    }

                    ListViewItem itm = new ListViewItem();
                    foreach (Activity element in Raids[0])
                    {
                        string[] arr = new string[8];
                        //add items to ListView
                        arr[0] = element.ActivityDefinition;
                        arr[1] = element.Completed.ToString();
                        arr[2] = element.Kills.ToString();
                        arr[3] = element.Assists.ToString();
                        arr[4] = element.Deaths.ToString();
                        arr[5] = element.Duration;
                        arr[6] = element.Compwclanmembers.ToString();
                        arr[7] = element.Period.ToString();
                        itm = new ListViewItem(arr);
                        RaidView1.Items.Add(itm);
                    }

                    if (Raids[1] != null)
                    {
                        itm = new ListViewItem();
                        foreach (Activity element in Raids[1])
                        {
                            string[] arr = new string[8];
                            //add items to ListView
                            arr[0] = element.ActivityDefinition;
                            arr[1] = element.Completed.ToString();
                            arr[2] = element.Kills.ToString();
                            arr[3] = element.Assists.ToString();
                            arr[4] = element.Deaths.ToString();
                            arr[5] = element.Duration;
                            arr[6] = element.Compwclanmembers.ToString();
                            arr[7] = element.Period.ToString();
                            itm = new ListViewItem(arr);
                            RaidView2.Items.Add(itm);
                        }
                    }

                    if (Raids[2] != null)
                    {
                        itm = new ListViewItem();
                        foreach (Activity element in Raids[2])
                        {
                            string[] arr = new string[8];
                            //add items to ListView
                            arr[0] = element.ActivityDefinition;
                            arr[1] = element.Completed.ToString();
                            arr[2] = element.Kills.ToString();
                            arr[3] = element.Assists.ToString();
                            arr[4] = element.Deaths.ToString();
                            arr[5] = element.Duration;
                            arr[6] = element.Compwclanmembers.ToString();
                            arr[7] = element.Period.ToString();
                            itm = new ListViewItem(arr);
                            RaidView3.Items.Add(itm);
                        }
                    }
                    strGlobal = Reset_time.ToShortDateString() + " - " + Date_limit.ToShortDateString();
                }
                else if (STATE == "Crisol")
                {
                    Crucible_Box.Visible = true;
                    General_box.Visible = false;
                    Mission_Box.Visible = false;
                    Strike_box.Visible = false;
                    Raid_box.Visible = false;
                    Gambit_Box.Visible = false;
                    Points_Box.Visible = false;

                    PVPView1.Items.Clear();
                    PVPView2.Items.Clear();
                    PVPView3.Items.Clear();

                    new_cruciblemanager = new CrucibleManager(rClient, Reset_time, Date_limit, new_clanmembermanager.ClanMembersList, actual_member.ID, 5);

                    int i = 0;
                    List<string> IDs = actual_member.GetcharacterIDs();
                    Activity[][] CrucibleMatches = actual_member.GetCrucibleMatches;
                    foreach (string element in IDs)
                    {
                        new_cruciblemanager.ParseCrucible(actual_member.ID, element, actual_member.Type);
                        CrucibleMatches[i] = new_cruciblemanager.Activity_list;
                        i++;
                    }

                    ListViewItem itm = new ListViewItem();
                    foreach (Activity element in CrucibleMatches[0])
                    {
                        string[] arr = new string[14];
                        //add items to ListView
                        arr[0] = element.ActivityDefinition;
                        arr[1] = element.Team;
                        arr[2] = element.Standing;
                        arr[3] = element.Score.ToString();
                        arr[4] = element.Teamscore.ToString();
                        arr[5] = element.Completed.ToString();
                        arr[6] = element.KD.ToString();
                        arr[7] = element.KDA.ToString();
                        arr[8] = element.Kills.ToString();
                        arr[9] = element.Assists.ToString();
                        arr[10] = element.Deaths.ToString();
                        arr[11] = element.Duration;
                        arr[12] = element.Compwclanmembers.ToString();
                        arr[13] = element.Period.ToString();
                        itm = new ListViewItem(arr);
                        PVPView1.Items.Add(itm);
                    }

                    if (CrucibleMatches[1] != null)
                    {
                        itm = new ListViewItem();
                        foreach (Activity element in CrucibleMatches[1])
                        {
                            string[] arr = new string[14];
                            //add items to ListView
                            arr[0] = element.ActivityDefinition;
                            arr[1] = element.Team;
                            arr[2] = element.Standing;
                            arr[3] = element.Score.ToString();
                            arr[4] = element.Teamscore.ToString();
                            arr[5] = element.Completed.ToString();
                            arr[6] = element.KD.ToString();
                            arr[7] = element.KDA.ToString();
                            arr[8] = element.Kills.ToString();
                            arr[9] = element.Assists.ToString();
                            arr[10] = element.Deaths.ToString();
                            arr[11] = element.Duration;
                            arr[12] = element.Compwclanmembers.ToString();
                            arr[13] = element.Period.ToString();
                            itm = new ListViewItem(arr);
                            PVPView2.Items.Add(itm);
                        }
                    }

                    if (CrucibleMatches[2] != null)
                    {
                        itm = new ListViewItem();
                        foreach (Activity element in CrucibleMatches[2])
                        {
                            string[] arr = new string[14];
                            //add items to ListView
                            arr[0] = element.ActivityDefinition;
                            arr[1] = element.Team;
                            arr[2] = element.Standing;
                            arr[3] = element.Score.ToString();
                            arr[4] = element.Teamscore.ToString();
                            arr[5] = element.Completed.ToString();
                            arr[6] = element.KD.ToString();
                            arr[7] = element.KDA.ToString();
                            arr[8] = element.Kills.ToString();
                            arr[9] = element.Assists.ToString();
                            arr[10] = element.Deaths.ToString();
                            arr[11] = element.Duration;
                            arr[12] = element.Compwclanmembers.ToString();
                            arr[13] = element.Period.ToString();
                            itm = new ListViewItem(arr);
                            PVPView3.Items.Add(itm);
                        }
                    }
                    strGlobal = Reset_time.ToShortDateString() + " - " + Date_limit.ToShortDateString();
                }
                else if (STATE == "Gambito")
                {
                    Gambit_Box.Visible = true;
                    General_box.Visible = false;
                    Mission_Box.Visible = false;
                    Strike_box.Visible = false;
                    Raid_box.Visible = false;
                    Crucible_Box.Visible = false;
                    Points_Box.Visible = false;

                    GambitView1.Items.Clear();
                    GambitView2.Items.Clear();
                    GambitView3.Items.Clear();

                    new_gambitmanager = new GambitManager(rClient, Reset_time, Date_limit, new_clanmembermanager.ClanMembersList, actual_member.ID, 63);

                    int i = 0;
                    List<string> IDs = actual_member.GetcharacterIDs();
                    Activity[][] GambitMatches = actual_member.GetGambitMatches;
                    foreach (string element in IDs)
                    {
                        new_gambitmanager.ParseGambit(actual_member.ID, element, actual_member.Type);
                        GambitMatches[i] = new_gambitmanager.Activity_list;
                        i++;
                    }

                    ListViewItem itm = new ListViewItem();
                    foreach (Activity element in GambitMatches[0])
                    {
                        string[] arr = new string[12];
                        //add items to ListView
                        arr[0] = element.ActivityDefinition;
                        arr[1] = element.Score.ToString();
                        arr[2] = element.Teamscore.ToString();
                        arr[3] = element.Completed.ToString();
                        arr[4] = element.Gambit_guardiankills.ToString();
                        arr[5] = element.Gambit_mobkills.ToString();
                        arr[6] = element.Gambit_primevalkills.ToString();
                        arr[7] = element.Gambit_motesdeposited.ToString();
                        arr[8] = element.Deaths.ToString();
                        arr[9] = element.Duration;
                        arr[10] = element.Compwclanmembers.ToString();
                        arr[11] = element.Period.ToString();
                        itm = new ListViewItem(arr);
                        GambitView1.Items.Add(itm);
                    }

                    if (GambitMatches[1] != null)
                    {
                        itm = new ListViewItem();
                        foreach (Activity element in GambitMatches[1])
                        {
                            string[] arr = new string[12];
                            //add items to ListView
                            arr[0] = element.ActivityDefinition;
                            arr[1] = element.Score.ToString();
                            arr[2] = element.Teamscore.ToString();
                            arr[3] = element.Completed.ToString();
                            arr[4] = element.Gambit_guardiankills.ToString();
                            arr[5] = element.Gambit_mobkills.ToString();
                            arr[6] = element.Gambit_primevalkills.ToString();
                            arr[7] = element.Gambit_motesdeposited.ToString();
                            arr[8] = element.Deaths.ToString();
                            arr[9] = element.Duration;
                            arr[10] = element.Compwclanmembers.ToString();
                            arr[11] = element.Period.ToString();
                            itm = new ListViewItem(arr);
                            GambitView2.Items.Add(itm);
                        }
                    }

                    if (GambitMatches[2] != null)
                    {
                        itm = new ListViewItem();
                        foreach (Activity element in GambitMatches[2])
                        {
                            string[] arr = new string[12];
                            //add items to ListView
                            arr[0] = element.ActivityDefinition;
                            arr[1] = element.Score.ToString();
                            arr[2] = element.Teamscore.ToString();
                            arr[3] = element.Completed.ToString();
                            arr[4] = element.Gambit_guardiankills.ToString();
                            arr[5] = element.Gambit_mobkills.ToString();
                            arr[6] = element.Gambit_primevalkills.ToString();
                            arr[7] = element.Gambit_motesdeposited.ToString();
                            arr[8] = element.Deaths.ToString();
                            arr[9] = element.Duration;
                            arr[10] = element.Compwclanmembers.ToString();
                            arr[11] = element.Period.ToString();
                            itm = new ListViewItem(arr);
                            GambitView3.Items.Add(itm);
                        }
                    }

                    new_gambitprimemanager = new GambitManager(rClient, Reset_time, Date_limit, new_clanmembermanager.ClanMembersList, actual_member.ID, 75);

                    i = 0;
                    Activity[][] GambitPrimeMatches = actual_member.GetGambitMatches;
                    foreach (string element in IDs)
                    {
                        new_gambitprimemanager.ParseGambit(actual_member.ID, element, actual_member.Type);
                        GambitPrimeMatches[i] = new_gambitprimemanager.Activity_list;
                        i++;
                    }


                    foreach (Activity element in GambitPrimeMatches[0])
                    {
                        string[] arr = new string[12];
                        //add items to ListView
                        arr[0] = element.ActivityDefinition;
                        arr[1] = element.Score.ToString();
                        arr[2] = element.Teamscore.ToString();
                        arr[3] = element.Completed.ToString();
                        arr[4] = element.Gambit_guardiankills.ToString();
                        arr[5] = element.Gambit_mobkills.ToString();
                        arr[6] = element.Gambit_primevalkills.ToString();
                        arr[7] = element.Gambit_motesdeposited.ToString();
                        arr[8] = element.Deaths.ToString();
                        arr[9] = element.Duration;
                        arr[10] = element.Compwclanmembers.ToString();
                        arr[11] = element.Period.ToString();
                        itm = new ListViewItem(arr);
                        GambitView1.Items.Add(itm);
                    }

                    if (GambitPrimeMatches[1] != null)
                    {
                        itm = new ListViewItem();
                        foreach (Activity element in GambitPrimeMatches[1])
                        {
                            string[] arr = new string[12];
                            //add items to ListView
                            arr[0] = element.ActivityDefinition;
                            arr[1] = element.Score.ToString();
                            arr[2] = element.Teamscore.ToString();
                            arr[3] = element.Completed.ToString();
                            arr[4] = element.Gambit_guardiankills.ToString();
                            arr[5] = element.Gambit_mobkills.ToString();
                            arr[6] = element.Gambit_primevalkills.ToString();
                            arr[7] = element.Gambit_motesdeposited.ToString();
                            arr[8] = element.Deaths.ToString();
                            arr[9] = element.Duration;
                            arr[10] = element.Compwclanmembers.ToString();
                            arr[11] = element.Period.ToString();
                            itm = new ListViewItem(arr);
                            GambitView2.Items.Add(itm);
                        }
                    }

                    if (GambitPrimeMatches[2] != null)
                    {
                        itm = new ListViewItem();
                        foreach (Activity element in GambitPrimeMatches[2])
                        {
                            string[] arr = new string[12];
                            //add items to ListView
                            arr[0] = element.ActivityDefinition;
                            arr[1] = element.Score.ToString();
                            arr[2] = element.Teamscore.ToString();
                            arr[3] = element.Completed.ToString();
                            arr[4] = element.Gambit_guardiankills.ToString();
                            arr[5] = element.Gambit_mobkills.ToString();
                            arr[6] = element.Gambit_primevalkills.ToString();
                            arr[7] = element.Gambit_motesdeposited.ToString();
                            arr[8] = element.Deaths.ToString();
                            arr[9] = element.Duration;
                            arr[10] = element.Compwclanmembers.ToString();
                            arr[11] = element.Period.ToString();
                            itm = new ListViewItem(arr);
                            GambitView3.Items.Add(itm);
                        }
                    }

                    strGlobal = Reset_time.ToShortDateString() + " - " + Date_limit.ToShortDateString();
                }
                else if (STATE == "Puntaje semanal")
                {
                    List<string> IDs = actual_member.GetcharacterIDs();

                    Activity[][] StoryMissions = actual_member.GetStoryMissions;
                    Activity[][] ForgeMissions = actual_member.GetForgeMissions;
                    Activity[][] ReckoningMissions = actual_member.GetReckoningMissions;
                    Activity[][] StrikeMissions = actual_member.GetStrikeMissions;
                    Activity[][] NightfallMissions = actual_member.GetNightfallMissions;
                    Activity[][] Raids = actual_member.GetRaids;
                    Activity[][] CrucibleMatches = actual_member.GetCrucibleMatches;
                    Activity[][] GambitMatches = actual_member.GetGambitMatches;
                    Activity[][] GambitPrimeMatches = actual_member.GetGambitPrimeMatches;

                    Points_Box.Visible = true;
                    General_box.Visible = false;
                    Mission_Box.Visible = false;
                    Strike_box.Visible = false;
                    Raid_box.Visible = false;
                    Crucible_Box.Visible = false;
                    Gambit_Box.Visible = false;

                    new_missionmanager = new OtherPVE(rClient, Reset_time, Date_limit, new_clanmembermanager.ClanMembersList, actual_member.ID, 2);
                    int i = 0;
                    foreach (string element in IDs)
                    {
                        new_missionmanager.ParseOther(actual_member.ID, element, actual_member.Type);
                        StoryMissions[i] = new_missionmanager.Activity_list;
                        i++;
                    }

                    new_forgemanager = new OtherPVE(rClient, Reset_time, Date_limit, new_clanmembermanager.ClanMembersList, actual_member.ID, 66);
                    i = 0;
                    foreach (string element in IDs)
                    {
                        new_forgemanager.ParseOther(actual_member.ID, element, actual_member.Type);
                        ForgeMissions[i] = new_forgemanager.Activity_list;
                        i++;
                    }

                    new_reckoningmanager = new OtherPVE(rClient, Reset_time, Date_limit, new_clanmembermanager.ClanMembersList, actual_member.ID, 66);
                    i = 0;
                    foreach (string element in IDs)
                    {
                        new_reckoningmanager.ParseOther(actual_member.ID, element, actual_member.Type);
                        ReckoningMissions[i] = new_reckoningmanager.Activity_list;
                        i++;
                    }

                    new_strikemanager = new OtherPVE(rClient, Reset_time, Date_limit, new_clanmembermanager.ClanMembersList, actual_member.ID, 3);
                    i = 0;
                    foreach (string element in IDs)
                    {
                        new_strikemanager.ParseOther(actual_member.ID, element, actual_member.Type);
                        StrikeMissions[i] = new_strikemanager.Activity_list;
                        i++;
                    }

                    new_nightfallmanager = new OtherPVE(rClient, Reset_time, Date_limit, new_clanmembermanager.ClanMembersList, actual_member.ID, 46);
                    i = 0;
                    foreach (string element in IDs)
                    {
                        new_nightfallmanager.ParseOther(actual_member.ID, element, actual_member.Type);
                        NightfallMissions[i] = new_nightfallmanager.Activity_list;
                        i++;
                    }

                    new_raidmanager = new RaidManager(rClient, Reset_time, Date_limit, new_clanmembermanager.ClanMembersList, actual_member.ID, 4);
                    i = 0;
                    foreach (string element in IDs)
                    {
                        new_raidmanager.ParseRaid(actual_member.ID, element, actual_member.Type);
                        Raids[i] = new_raidmanager.Activity_list;
                        i++;
                    }

                    new_cruciblemanager = new CrucibleManager(rClient, Reset_time, Date_limit, new_clanmembermanager.ClanMembersList, actual_member.ID, 5);
                    i = 0;
                    foreach (string element in IDs)
                    {
                        new_cruciblemanager.ParseCrucible(actual_member.ID, element, actual_member.Type);
                        CrucibleMatches[i] = new_cruciblemanager.Activity_list;
                        i++;
                    }

                    new_gambitmanager = new GambitManager(rClient, Reset_time, Date_limit, new_clanmembermanager.ClanMembersList, actual_member.ID, 63);
                    i = 0;
                    foreach (string element in IDs)
                    {
                        new_gambitmanager.ParseGambit(actual_member.ID, element, actual_member.Type);
                        GambitMatches[i] = new_gambitmanager.Activity_list;
                        i++;
                    }

                    new_gambitprimemanager = new GambitManager(rClient, Reset_time, Date_limit, new_clanmembermanager.ClanMembersList, actual_member.ID, 75);
                    i = 0;
                    foreach (string element in IDs)
                    {
                        new_gambitprimemanager.ParseGambit(actual_member.ID, element, actual_member.Type);
                        GambitPrimeMatches[i] = new_gambitprimemanager.Activity_list;
                        i++;
                    }

                    myPointTracker = new PointTracker("1rQB4k2qpspgZpDz6dgxgpRl8kt9cGZd3kW3sRSxAdvc", actual_member);
                    myPointTracker.CalculateTotalPoints();

                    ActivitiesBox.Text = myPointTracker.GetNumofActivities().ToString();
                    trackBar1.Value = myPointTracker.GetActivityLevel();
                    ModifyProgressBarColor.SetState(progressBar1, myPointTracker.GetStoplightColor());

                    StoryBox.Text = myPointTracker.GetStoryPoints().ToString();
                    StrikeBox.Text = myPointTracker.GetStrikePoints().ToString();
                    NightfallBox.Text = myPointTracker.GetNightfallPoints().ToString();
                    CrucibleBox.Text = myPointTracker.GetCruciblePoints().ToString();
                    GambitBox.Text = myPointTracker.GetGambitPoints().ToString();
                    RaidBox.Text = myPointTracker.GetRaidPoints().ToString();
                    TotalBox.Text = myPointTracker.GetTotalPoints().ToString();

                    points = myPointTracker.GetValues();
                    PointsView1.Items.Clear();
                    ListViewItem itm = new ListViewItem();
                    foreach (IList<object> element in points)
                    {
                        string[] arr = new string[2];
                        //add items to ListView
                        arr[0] = element[0].ToString();
                        arr[1] = element[1].ToString();
                        itm = new ListViewItem(arr);
                        PointsView1.Items.Add(itm);
                    }
                    strGlobal = Reset_time.ToShortDateString() + " - " + Date_limit.ToShortDateString();
                }
                else
                {
                    strGlobal += " " + "Modo invalido";
                }
                Debug_box.Text = strGlobal;
                strGlobal = string.Empty;
            }
            else
            {
                Debug_box.Text = "Usuario invalido";
            }
        }

        private void Select_clan_button_Click_1(object sender, EventArgs e)
        {
            clanID = Clan_options.Text;
            if (clanID == "Abysmal Light Yin (XBOX)")
            {
                clanID = "3398247";
                Initial_Box.Visible = false;
                pictureBox2.Visible = true;
                new_clanmembermanager.ParseClanMembers(rClient, clanID);
                foreach (Member element in new_clanmembermanager.ClanMembersList)
                {
                    select_User.Items.Add(element.Name);
                }
                Debug_box.Text = "Listo";
                //for (int i = 0; i < new_clanmembermanager.ClanMembersList.Count; ++i)
                //{
                //    int j = 0;
                //    foreach (string element in new_clanmembermanager.ClanMembersList[i].characterIDs)
                //    {
                //        new_clanmembermanager.ClanMembersList[i].StoryMissions[j] = new_missionmanager.ParseActivitiesList(new_clanmembermanager.ClanMembersList[i].ID, element, new_clanmembermanager.ClanMembersList[i].Type);
                //        j++;
                //    }
                //}
            }
            else if (clanID == "Abysmal Light Yang (PC)")
            {
                clanID = "3504040";
                Initial_Box.Visible = false;
                pictureBox1.Visible = true;
                new_clanmembermanager.ParseClanMembers(rClient, clanID);
                foreach (Member element in new_clanmembermanager.ClanMembersList)
                {
                    select_User.Items.Add(element.Name);
                }
                Debug_box.Text = "Listo";
            }
            else
            {
                invalid_clan.Visible = true;
            }
        }

        private void switch_console_Click(object sender, EventArgs e)
        {
            Reset();
        }

        private void Reset()
        {
            Debug_box.Text = "Cargando miembros...";
            Initial_Box.Visible = true;
            pictureBox1.Visible = false;
            pictureBox2.Visible = false;
            invalid_clan.Visible = false;
            General_box.Visible = false;
            Mission_Box.Visible = false;
            Strike_box.Visible = false;
            Raid_box.Visible = false;
            Crucible_Box.Visible = false;
            Gambit_Box.Visible = false;
            Points_Box.Visible = false;

            select_User.Items.Clear();
            select_User.Text = "";
            modeSelection.Text = "";
            new_clanmembermanager.ClanMembersList.Clear();
        }

        private void label8_Click(object sender, EventArgs e)
        {
        }
    }

    public static class ModifyProgressBarColor
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr w, IntPtr l);
        public static void SetState(this ProgressBar pBar, int state)
        {
            SendMessage(pBar.Handle, 1040, (IntPtr)state, IntPtr.Zero);
        }
    }
}
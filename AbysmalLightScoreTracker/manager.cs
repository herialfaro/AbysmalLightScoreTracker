using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace AbysmalLightScoreTracker
{
    public struct Activity
    {
        public DateTime Period;
        public string InstanceID;
        public string directorActivityHash;
        public string ActivityDefinition;

        public string Duration;
        public string Standing; //Crucible
        public string Team; //Crucible
        public bool Completed;
        public ushort Deaths;
        public ushort Kills;
        public ushort Assists;
        public ushort Score;
        public ushort Teamscore; //Crucible -- GAMBIT POINTS
        public float KD;
        public float KDA;
        public byte Playercount;

        public ushort Gambit_motesdeposited;
        public ushort Gambit_mobkills;
        public ushort Gambit_guardiankills;
        public ushort Gambit_primevalkills;

        public bool Compwclanmembers;
    }

    interface Manager
    {
        List<Activity> ParseActivitiesList(string a, string b, string c);
        void GetActivityDefinition();
    }
}
using Rocket.Unturned;
using Rocket.Unturned.Events;
using Rocket.Unturned.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Dune.Unturned.Rocket.BackInBlack
{
    public class BackInBlackRocketPlugin : RocketPlugin<BackInBlackRocketPluginConfiguration>
    {
        private IDictionary<string, DeathNote> deathNotes;

        public static BackInBlackRocketPlugin Instance { get; private set; }

        protected override void Load()
        {
            base.Load();

            Instance = this;

            deathNotes = new Dictionary<string, DeathNote>();

            RegisterEvents();
        }

        protected override void Unload()
        {
            base.Unload();

            UnregsiterEvents();
        }

        #region Events
        private void RegisterEvents()
        {
            RocketServerEvents.OnServerShutdown += RocketServerEvents_OnServerShutdown;
            RocketPlayerEvents.OnPlayerDeath += RocketPlayerEvents_OnPlayerDeath;
        }

        private void UnregsiterEvents()
        {
            RocketServerEvents.OnServerShutdown -= RocketServerEvents_OnServerShutdown;
            RocketPlayerEvents.OnPlayerDeath -= RocketPlayerEvents_OnPlayerDeath;
        }

        #region Handlers
        void RocketServerEvents_OnServerShutdown()
        {
            UnregsiterEvents();
        }

        void RocketPlayerEvents_OnPlayerDeath(global::Rocket.Unturned.Player.RocketPlayer player, SDG.Unturned.EDeathCause cause, SDG.Unturned.ELimb limb, Steamworks.CSteamID murderer)
        {
            if (!Configuration.Enabled)
                return;

            deathNotes[player.SteamName] = new DeathNote() { Position = player.Position, TimeOfDeath = DateTime.Now };
        }
        #endregion
        #endregion

        internal void Enable()
        {
            Configuration.Enabled = true;

            BackInBlackRocketPlugin.Instance.Say("is Enabled!");
        }

        internal void Disable()
        {
            Configuration.Enabled = false;

            BackInBlackRocketPlugin.Instance.Say("Disabled!", Color.red);

            // Clearing all deap positions;
            deathNotes.Clear();
        }

        internal void EnableTimeLimit()
        {
            Configuration.TimeLimitEnabled = true;

            BackInBlackRocketPlugin.Instance.Say("Time limit ON, we have no time to cry for the dead!");
        }

        internal void DisableTimeLimit()
        {
            Configuration.TimeLimitEnabled = false;

            BackInBlackRocketPlugin.Instance.Say("No time limitations anymore!");
        }

        internal void SetTimeLimit(int value)
        {
            Configuration.TimeLimit = value;

            BackInBlackRocketPlugin.Instance.Say(string.Format("Time limit set to {0} seconds. Hurry guys!", value), Color.red);
        }

        internal void TrySendBack(global::Rocket.Unturned.Player.RocketPlayer player)
        {
            if (!Configuration.Enabled)
                return;

            if (deathNotes.ContainsKey(player.SteamName))
            {
                DeathNote deathNote = deathNotes[player.SteamName];

                if (DateTime.Now <= deathNote.TimeOfDeath.AddSeconds(Configuration.TimeLimit))
                {
                    // Teleporting the player back.
                    player.Teleport(deathNote.Position, player.Rotation);

                    // Saying hello to the dead :)
                    Say(player, "Back In Black!");
                }
                else
                    Say(player, "too slow, have a nice walk. XD");

                // Removing the reference, we dont want a smart player using our plugin as fast travel.
                deathNotes.Remove(player.SteamName);
            }
            else
            {
                // Cant find the poor guy =/
                Say(player, "You have to die first smarty pants XD");
            }
        }

        internal void Say(string message)
        {
            Say(message, Color.green);
        }

        internal void Say(string message, Color color)
        {
            RocketChat.Say(string.Format("[BackInBlack] {0}", message), color);
        }

        internal void Say(global::Rocket.Unturned.Player.RocketPlayer player, string message)
        {
            Say(player, message, Color.green);
        }

        internal void Say(global::Rocket.Unturned.Player.RocketPlayer player, string message, Color color)
        {
            RocketChat.Say(player, string.Format("[BackInBlack] {0}", message), color);
        }
    }
}
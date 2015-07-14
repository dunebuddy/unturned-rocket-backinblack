using Rocket.Unturned;
using Rocket.Unturned.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Dune.Unturned.Rocket.BackInBlack
{
    public class BackInBlackRocketCommand : IRocketCommand
    {
        public List<string> Aliases
        {
            get { return new List<string> { "bib", "bibz" }; }
        }

        public string Help
        {
            get { return "See /BackInBlack help"; }
        }

        public string Name
        {
            get { return "BackInBlack"; }
        }

        public bool RunFromConsole
        {
            get { return true; }
        }

        public string Syntax
        {
            get { return "/BackInBlack <params>"; }
        }

        public void Execute(global::Rocket.Unturned.Player.RocketPlayer caller, string[] command)
        {
            if (command.Length == 0)
            {
                if (caller != null)
                    BackInBlackRocketPlugin.Instance.TrySendBack(caller);
            }
            else
            {
                string verb = command[0].ToLower();

                switch (verb)
                {
                    case "enable":
                    case "on":
                        ForAdiminsOnly(caller, (z) => { BackInBlackRocketPlugin.Instance.Enable(); });
                        break;
                    case "disable":
                    case "off":
                        ForAdiminsOnly(caller, (z) => { BackInBlackRocketPlugin.Instance.Disable(); });
                        break;
                    case "status":
                        BackInBlackRocketPlugin.Instance.Say(caller, string.Format("is {0}", BackInBlackRocketPlugin.Instance.Configuration.Enabled ? "Enabled" : "Disabled"));
                        break;
                    case "timelimit":
                        if (command.Length == 2)
                        {
                            string argument = command[1].ToLower();

                            switch (argument)
                            {
                                case "enable":
                                case "on":
                                    ForAdiminsOnly(caller, (z) => { BackInBlackRocketPlugin.Instance.EnableTimeLimit(); });
                                    break;
                                case "disable":
                                case "off":
                                    ForAdiminsOnly(caller, (z) => { BackInBlackRocketPlugin.Instance.DisableTimeLimit(); });
                                    break;
                                case "status":
                                    BackInBlackRocketPlugin.Instance.Say(caller, string.Format("Time Limit is {0}.", BackInBlackRocketPlugin.Instance.Configuration.TimeLimitEnabled ? "Enabled" : "Disabled"));

                                    if (BackInBlackRocketPlugin.Instance.Configuration.TimeLimitEnabled)
                                        BackInBlackRocketPlugin.Instance.Say(caller, string.Format("You will have {0} seconds after each death. Better hurry :)", BackInBlackRocketPlugin.Instance.Configuration.TimeLimit));

                                    break;
                                default:
                                    BadCommand(caller);
                                    break;
                            }
                        }
                        else if (command.Length == 3)
                        {
                            string argument = command[1].ToLower();
                            int value = 0;

                            if (int.TryParse(command[2].ToLower(), out value))
                            {
                                switch (argument)
                                {
                                    case "set":
                                        ForAdiminsOnly(caller, (z) => { BackInBlackRocketPlugin.Instance.SetTimeLimit(value); });
                                        break;
                                    default:
                                        BadCommand(caller);
                                        break;
                                }
                            }
                            else
                                BadCommand(caller);
                        }
                        else
                            BadCommand(caller);
                        break;
                    case "help":
                        SendHelp(caller);
                        break;
                    default:
                        BadCommand(caller);
                        break;
                }
            }
        }

        private void BadCommand(global::Rocket.Unturned.Player.RocketPlayer caller)
        {
            BackInBlackRocketPlugin.Instance.Say(caller, "Bad command.", Color.red);
        }

        private void SendHelp(global::Rocket.Unturned.Player.RocketPlayer caller)
        {
            string[] everyOneHelpLines = 
            {
                "/BackInBlack - Teleports a player to the last death place.",
                "/BackInBlack status - Let you know if BackInBlack is enabled or not.",
                "/BackInBlack timelimit status - Tells you if there are any limitations for teleportation after death.",
                "/BackInBlack help - Seriously?"
            };

            string[] adminsHelpLines = { 
                "[Admin talk]",
                "/BackInBlack [enable|on] - Enables the teleportation feature.",
                "/BackInBlack [disable|off] - Disables the thing.",
                "/BackInBlack timelimit [enable|on] - Enables the time limitations.",
                "/BackInBlack timelimit [disable|off] - Make all zombies becoming to fly ¬¬.",
                "/BackInBlack timelimit set [seconds] - Set the time limit in seconds."
            };

            List<string> helpLines = new List<string>(everyOneHelpLines);

            if ((caller == null) || (caller.IsAdmin))
                helpLines.AddRange(adminsHelpLines);

            foreach (string l in helpLines)
                BackInBlackRocketPlugin.Instance.Say(caller, l, Color.red);
        }

        private void ForAdiminsOnly(global::Rocket.Unturned.Player.RocketPlayer caller, Action<int> command)
        {
            if ((caller == null) || (caller.IsAdmin))
                command(default(int));
            else
                BackInBlackRocketPlugin.Instance.Say(caller, "For Admins only, sorry =/");
        }
    }
}

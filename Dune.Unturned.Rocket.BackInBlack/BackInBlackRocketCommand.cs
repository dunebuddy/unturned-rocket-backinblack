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
                    BackInBlack.Instance.TrySendBack(caller);
            }
            else
            {
                string verb = command[0].ToLower();

                switch (verb)
                {
                    case "enable":
                    case "on":
                        ForAdiminsOnly(caller, () => { BackInBlack.Instance.Enable(); });
                        break;
                    case "disable":
                    case "off":
                        ForAdiminsOnly(caller, () => { BackInBlack.Instance.Disable(); });
                        break;
                    case "status":
                        BackInBlack.Instance.Say(caller, string.Format("is {0}", BackInBlack.Instance.Configuration.Enabled ? "Enabled" : "Disabled"));
                        break;
                    case "timelimit":
                        if (command.Length == 2)
                        {
                            string argument = command[1].ToLower();

                            switch (argument)
                            {
                                case "enable":
                                case "on":
                                    ForAdiminsOnly(caller, () => { BackInBlack.Instance.EnableTimeLimit(); });
                                    break;
                                case "disable":
                                case "off":
                                    ForAdiminsOnly(caller, () => { BackInBlack.Instance.DisableTimeLimit(); });
                                    break;
                                case "status":
                                    BackInBlack.Instance.Say(caller, string.Format("Time Limit is {0}.", BackInBlack.Instance.Configuration.TimeLimitEnabled ? "Enabled" : "Disabled"));
                                                                        
                                    if (BackInBlack.Instance.Configuration.TimeLimitEnabled)
                                        BackInBlack.Instance.Say(caller, string.Format("You will have {0} seconds after each death. Better hurry :)", BackInBlack.Instance.Configuration.TimeLimit.TotalSeconds));

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
                                        ForAdiminsOnly(caller, () => { BackInBlack.Instance.SetTimeLimit(value); });
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
            const string message = "Bad command.";

            if (caller != null)
                BackInBlack.Instance.Say(caller, message, Color.red);
            else
                RocketConsole.print(message);
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

            if (caller.IsAdmin)
                helpLines.AddRange(adminsHelpLines);

            foreach (string l in helpLines)
            {
                if (caller != null)
                    BackInBlack.Instance.Say(caller, l, Color.red);
                else
                    RocketConsole.print(l);
            }
        }

        private void ForAdiminsOnly(global::Rocket.Unturned.Player.RocketPlayer caller, Action command)
        {
            if (caller.IsAdmin)
                command.Invoke();
            else
                BackInBlack.Instance.Say(caller, "For Admins only, sorry =/");
        }

        private T ForAdiminsOnly<T>(global::Rocket.Unturned.Player.RocketPlayer caller, Func<T> command)
        {
            T result = default(T);

            if (caller.IsAdmin)
                result = command.Invoke();
            else
                BackInBlack.Instance.Say(caller, "For Admins only, sorry =/");

            return result;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLIRC
{
    class IRCCommand
    {
        private string m_channel;
        private string[] m_users;

        public string Channel { get { return m_channel; } set { m_channel = value; } }
        public string[] UserList { get { return m_users; } set { m_users = value; } }

        private Dictionary<string, Func<string[], string[], IRCMessage>> cmds = new Dictionary<string, Func<string[], string[], IRCMessage>>();

        public IRCCommand(string channel)
        {
            this.Channel = channel;

            cmds["+add"] = Add;
            cmds["+subtract"] = Subtract;
            cmds["+multiply"] = Multiply;
            cmds["+divide"] = Divide;
            cmds["+say"] = Say;
            cmds["+time"] = Time;
            cmds["+random"] = Rand;
            cmds["+randuser"] = RandUser;
        }

        public IRCMessage Interpret(string cmd, string[] args, string[] userl)
        {
            if (cmds.ContainsKey(cmd))
            {
                return (IRCMessage)cmds[cmd](args, userl);
            }
            else
            {
                return new IRCMessage(IRCType.DONOTHING, "Do nothing!");
            }
        }

        public IRCMessage Add(string[] args, string[] userl)
        {
            var a = 0.0;
            var b = 0.0;
            double.TryParse(args[0], out a);
            double.TryParse(args[1], out b);
            if (a > (double.MaxValue - b) || a < (double.MinValue - b)) 
            {
                return new IRCMessage(IRCType.PRIVMSG, this.Channel, "Unable to calculate.");
            }
            else
            {
                return new IRCMessage(IRCType.PRIVMSG, this.Channel, String.Format("{0} + {1} = {2}", a, b, a+b));
            }
        }

        public IRCMessage Subtract(string[] args, string[] userl)
        {
            var a = 0.0;
            var b = 0.0;
            double.TryParse(args[0], out a);
            double.TryParse(args[1], out b);
            if (a < (double.MinValue + b) || a > (double.MaxValue + b))
            {
                return new IRCMessage(IRCType.PRIVMSG, this.Channel, "Unable to calculate.");
            }
            else
            {
                return new IRCMessage(IRCType.PRIVMSG, this.Channel, String.Format("{0} - {1} = {2}", a, b, a - b));
            }
        }

        public IRCMessage Multiply(string[] args, string[] userl)
        {
            var a = 0.0;
            var b = 0.0;
            double.TryParse(args[0], out a);
            double.TryParse(args[1], out b);
            if (a > (double.MaxValue / b))
            {
                return new IRCMessage(IRCType.PRIVMSG, this.Channel, "Unable to calculate.");
            }
            else
            {
                return new IRCMessage(IRCType.PRIVMSG, this.Channel, String.Format("{0} * {1} = {2}", a, b, a * b));
            }
        }

        public IRCMessage Divide(string[] args, string[] userl)
        {
            var a = 0.0;
            var b = 0.0;
            double.TryParse(args[0], out a);
            double.TryParse(args[1], out b);
            if (a > (double.MaxValue * b))
            {
                return new IRCMessage(IRCType.PRIVMSG, this.Channel, "Unable to calculate.");
            }
            else
            {
                return new IRCMessage(IRCType.PRIVMSG, this.Channel, String.Format("{0} / {1} = {2}", a, b, a / b));
            }
        }

        public IRCMessage Say(string[] args, string[] userl)
        {
            return new IRCMessage(IRCType.PRIVMSG, this.Channel, String.Join(" ", args));
        }

        public IRCMessage Time(string[] args, string[] userl)
        {
            var format = "The current time is {0} {1}.";
            var time = DateTime.Now;
            var zone = "";
            if (args.Length >= 1) zone = args[0].ToLower(); else zone = "est";
            switch (zone)
            {
                case "gmt":
                    time = DateTime.UtcNow;
                    if (args.Length >= 2)
                    {
                        var add = 0.0;
                        double.TryParse(args[1], out add);
                        time = time.AddHours(add);
                        if (add >= 0) zone += "+" + add.ToString(); else zone += add.ToString();
                    }
                    break;
                case "aest":
                    time = DateTime.UtcNow.AddHours(10);
                    break;
                case "aedt":
                    time = DateTime.UtcNow.AddHours(11);
                    break;
                case "mdt":
                    time = DateTime.UtcNow.AddHours(-6);
                    break;
                case "u":
                case "akdt":
                case "pst":
                    time = DateTime.UtcNow.AddHours(-8);
                    break;
                case "mst":
                case "pdt":
                    time = DateTime.UtcNow.AddHours(-7);
                    break;
                case "est":
                default:
                    time = DateTime.Now;
                    break;
            }
            return new IRCMessage(IRCType.PRIVMSG, this.Channel, String.Format(format, time.ToString("hh:mm:ss tt"), zone.ToUpper()));
        }

        public IRCMessage Rand(string[] args, string[] userl)
        {
            var r = new Random();
            int a;
            int b;
            int.TryParse(args[0], out a);
            int.TryParse(args[1], out b);
            return new IRCMessage(IRCType.PRIVMSG, this.Channel, String.Format("You random number is {0}.", r.Next(a, b)));
        }

        public IRCMessage RandUser(string[] args, string[] userl)
        {
            var r = new Random();
            var i = r.Next(0, userl.Length);
            return new IRCMessage(IRCType.PRIVMSG, this.Channel, String.Format("I have randomly selected {0}.", userl[i]));
        }
    }
}

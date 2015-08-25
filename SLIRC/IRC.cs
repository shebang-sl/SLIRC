using System;
using System.Linq;
using System.Net.Sockets;
using System.Collections.Generic;
using System.IO;

namespace SLIRC
{
    class IRC
    {
        private int m_port;
        private string m_serv;
        private string m_nick;
        private string m_chan;
        private string m_user;
        private List<string> m_users;

        public int Port { get { return m_port; } set { m_port = value; } }
        public string Server { get { return m_serv; } set { m_serv = value; } }
        public string Nickname { get { return m_nick; } set { m_nick = value; } }
        public string Channel { get { return m_chan; } set { m_chan = value; } }
        public string User { get { return m_user; } set { m_user = value; } }
        public List<string> UserList { get { return m_users; } set { m_users = value; } }

        public IRC(string server, int port, string nick, string user, string channel, string botm = "Default bot message.")
        {
            this.Server = server;
            this.Port = port;
            this.Nickname = nick;
            this.Channel = channel.ToUpper();
            this.User = String.Format("USER {0} 8 * : {1}", user, botm);
            this.UserList = new List<string>();
        }

        public void Connect()
        {
            var ircc = new IRCCommand(this.Channel);
            try
            {
                var v_irc = new TcpClient(Server, Port);
                var v_str = v_irc.GetStream();
                var v_rdr = new StreamReader(v_str);
                var v_wtr = new StreamWriter(v_str);

                try
                {
                    Send(v_wtr, new IRCMessage(User));
                    Send(v_wtr, new IRCMessage(IRCType.NICK, Nickname));
                    Send(v_wtr, new IRCMessage(IRCType.JOIN, Channel));

                    while (true)
                    {
                        var input = String.Empty;
                        while ((input = v_rdr.ReadLine()) != String.Empty)
                        {
                            Console.WriteLine(input);
                            if (input.Contains("JOIN :" + Channel))
                            {
                                Send(v_wtr, new IRCMessage(IRCType.NAMES, Channel));
                            }
                            else if (input.Contains(String.Format("{0} = {1} :", Nickname, Channel)))
                            {
                                var users = input.Split(':')[2].Split(' ');
                                UserList.Clear();
                                foreach (var user in users) {
                                    if (user != Nickname) UserList.Add(user);
                                }
                            }
                            else
                            {
                                Process(v_wtr, ircc, input, UserList.ToArray());
                            }
                        }
                        v_rdr.Close();
                        v_wtr.Close();
                        v_irc.Close();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(String.Concat("Unable to join: ", ex.Message));
                    return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(String.Concat("Unable to connect: ", ex.Message));
                return;
            }
        }

        public void Process(StreamWriter wtr, IRCCommand ircc, string input, string[] userl)
        {
            var split = String.Join(" ", IRCType.PRIVMSG, Channel, ":");

            if (input.Contains(split))
            {
                var cmd_s = input.Split(new string[] { split }, StringSplitOptions.None)[1];
                var args_ = cmd_s.Split(' ');
                var args = args_.Skip(1).Take(args_.Length - 1).ToArray();
                var cmd = args_[0];

                var res = ircc.Interpret(cmd, args, userl);

                if (res.Type != IRCType.DONOTHING)
                {
                    Send(wtr, res);
                }
            }
        }

        public void Send(StreamWriter wtr, string msg)
        {
            wtr.WriteLine(msg);
            wtr.Flush();
        }

        public void Send(StreamWriter wtr, IRCMessage msg)
        {
            wtr.WriteLine(msg.ToString());
            wtr.Flush();
        }

    }
}
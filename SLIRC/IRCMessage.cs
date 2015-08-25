using System;

namespace SLIRC
{
    class IRCMessage
    {
        private string m_type;
        private string m_chan;
        private string m_data;

        public string Type { get { return m_type; } set { m_type = value; } }
        public string Channel { get { return m_chan; } set { m_chan = value; } }
        public string Data { get { return m_data; } set { m_data = value; } }

        public IRCMessage(string type, string channel, string data)
        {
            this.Type = type;
            this.Channel = channel;
            this.Data = data;
        }

        public IRCMessage(string fullmessage) : this(String.Empty, String.Empty, fullmessage) { }
        public IRCMessage(string type, string data) : this(type, String.Empty, data) { }

        public override string ToString()
        {
            if (this.Type == String.Empty && this.Channel == String.Empty)
            {
                return this.Data;
            }
            else if (this.Channel == String.Empty)
            {
                return String.Concat(this.Type, " ", this.Data);
            }
            else
            {
                return String.Format("{0} {1} :{2}", this.Type, this.Channel, this.Data);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace baseFunc
{
    public class string_pipe
    {
        private object l = new object();

        private List<string> messages = new List<string>();

        public void write(string msg)
        {
            lock(l)
            {
                messages.Add(msg);
            }
        }

        public string pull_message()
        {
            string ret = "";
            lock (l)
            {
                if (messages.Count != 0)
                {
                    ret = messages[0];
                    messages.RemoveAt(0);
                }
            }
            return ret;
        }
    }
}

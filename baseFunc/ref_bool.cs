using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace baseFunc
{
    public class ref_bool
    {
        public delegate void dell(bool new_val);
        private dell del;

        private bool b = false;

        public void set_on_chhange(dell del_) { del = del_; }

        public bool Value
        {
            get { return b; }
            set
            {
                if (b != value)
                {
                    b = value;
                    if (del != null) del(value);
                }
            }
        }

        public ref_bool(bool value) { this.Value = value; }
    }
}

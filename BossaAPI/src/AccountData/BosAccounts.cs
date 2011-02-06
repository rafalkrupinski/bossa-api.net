using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pjank.BossaAPI
{
    public class BosAccounts
    {
        private List<BosAccount> list = new List<BosAccount>();

        public int Count
        {
            get { return list.Count; }
        }

        public BosAccount this[int index]
        {
            get { return list[index]; }
        }

        public BosAccount this[string number]
        {
            get { return list.Where(account => account.Number.Contains(number)).Single(); }
        }
    }
}

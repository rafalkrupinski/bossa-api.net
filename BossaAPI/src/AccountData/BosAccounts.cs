using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pjank.BossaAPI
{
    public class BosAccounts
    {
        private List<BosAccount> accounts = new List<BosAccount>();

        public int Count
        {
            get { return accounts.Count; }
        }
        public BosAccount this[int index]
        {
            get { return accounts[index]; }
        }
        public BosAccount this[string name]
        {
            get { return accounts.Where(a => a.Name == name).Single(); }
        }
    }
}

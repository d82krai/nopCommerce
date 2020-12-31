using System;
using System.Collections.Generic;

namespace Nop.Plugin.Soft2Print.Model
{
    public class ProjectInfo
    {
        public class _Module
        {
            public int id { get; set; }
            public string name { get; set; }
        }
        public class _Theme
        {
            public Guid id { get; set; }
            public string name { get; set; }
        }
        public class _ShareLock
        {
            public bool byMe { get; set; }
            public string by { get; set; }
            public DateTime expectedUnlockAt { get; set; }
        }
        public class _Share
        {
            public Guid id { get; set; }
            public _ShareMode mode { get; set; }
        }
        public class _ShareMode
        {
            public int id { get; set; }
            public string name { get; set; }
        }

        public int id { get; set; }
        public string name { get; set; }
        public DateTime created { get; set; }
        public DateTime lastChanged { get; set; }
        public string productCode { get; set; }
        public _Module module { get; set; }

        public int? inheritedBy { get; set; }

        public bool isLocked { get; set; }
        public _Theme theme { get; set; }
        public string previewUrl { get; set; }
        public _ShareLock shareLock { get; set; }
        public IEnumerable<_Share> shares { get; set; }
    }
}

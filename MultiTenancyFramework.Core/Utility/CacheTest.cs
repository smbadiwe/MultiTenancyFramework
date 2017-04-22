using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace MultiTenancyFramework.Core.Utility {
    
    /// <summary>
    /// Cache change monitor that allows an app to fire a change notification
    /// to all associated cache items.
    /// </summary>
    public class SignaledChangeMonitor : ChangeMonitor {
        // Shared across all SignaledChangeMonitors in the AppDomain
        private static event Action<string> Signaled;

        private string _name;
        private string _uniqueId = Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture);

        public override string UniqueId {
            get { return _uniqueId; }
        }

        public SignaledChangeMonitor(string name = null) {
            _name = name;
            // Register instance with the shared event
            Signaled += OnSignalRaised;
            base.InitializationComplete();
        }

        public static void Signal(string name = null) {
            // Raise shared event to notify all subscribers
            Signaled(name);
        }

        protected override void Dispose(bool disposing) {
            Signaled -= OnSignalRaised;
        }

        private void OnSignalRaised(string name) {
            if (string.IsNullOrWhiteSpace(name) || string.Compare(name, _name, true) == 0) {
                Debug.WriteLine(
                    _uniqueId + " notifying cache of change.", "SignaledChangeMonitor");
                // Cache objects are obligated to remove entry upon change notification.
                base.OnChanged(null);
            }
        }
    }

    public static class CacheTester {
        public static void TestCache() {
            MemoryCache cache = MemoryCache.Default;

            // Add data to cache
            for (int idx = 0; idx < 50; idx++) {
                cache.Add("Key" + idx.ToString(), "Value" + idx.ToString(), GetPolicy(idx));
            }

            // Flush cached items associated with "NamedData" change monitors
            SignaledChangeMonitor.Signal("NamedData");

            // Flush all cached items
            SignaledChangeMonitor.Signal();
        }

        private static CacheItemPolicy GetPolicy(int idx) {
            string name = (idx % 2 == 0) ? null : "NamedData";

            CacheItemPolicy cip = new CacheItemPolicy();
            cip.AbsoluteExpiration = System.DateTimeOffset.UtcNow.AddHours(1);
            cip.ChangeMonitors.Add(new SignaledChangeMonitor(name));
            return cip;
        }
    }
}

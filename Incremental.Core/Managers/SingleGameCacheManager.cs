using Incremental.Data.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incremental.Core.Managers
{
    public class SingleGameCacheManager
    {
        private Point? _cachedPoint;
        private DateTime _lastCacheUpdate = DateTime.MinValue;
        private readonly TimeSpan _cacheLifetime = TimeSpan.FromSeconds(30);

        public bool TryGetCachedPoint(out Point? point)
        {
            point = null;

            if (_cachedPoint != null &&
                DateTime.UtcNow - _lastCacheUpdate < _cacheLifetime)
            {
                point = _cachedPoint;
                return true;
            }

            return false;
        }

        public void SetPoint(Point point)
        {
            _cachedPoint = point;
            _lastCacheUpdate = DateTime.UtcNow;
        }

        public void Invalidate()
        {
            _cachedPoint = null;
            _lastCacheUpdate = DateTime.MinValue;
        }
    }
}

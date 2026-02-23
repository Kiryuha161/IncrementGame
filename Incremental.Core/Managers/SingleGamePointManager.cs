using Incremental.Core.DTOs.Common;
using Incremental.Core.Managers.Interfaces;
using Incremental.Core.ModelFactories.Interfaces;
using Incremental.Data;
using Incremental.Data.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Incremental.Core.Managers
{
    public class SingleGamePointManager : IPointManager
    {
        private readonly ProjectContext _context;
        private readonly IPointFactory _pointFactory;
        private readonly SingleGameCacheManager _cache;
        public SingleGamePointManager(ProjectContext context, IPointFactory pointFactory, SingleGameCacheManager cache)
        {
            _context = context;
            _pointFactory = pointFactory;
            _cache = cache;
        }

        public async Task<GameStateDto> ClickAsync()
        {
            var point = await GetPointAsync();

            if (point == null)
            {
                point = new Point
                {
                    Amount = 0,
                    ClickPower = 1
                };
                _context.Points.Add(point);
            }

            point.Amount += point.ClickPower;
            await _context.SaveChangesAsync();

            // Обновляем кэш
            _cache.SetPoint(point);

            return _pointFactory.PrepareGameStateDto(point);
        }

        public async Task<GameStateDto> GetStateAsync()
        {
            var point = await GetPointAsync();

            if (point == null)
            {
                return await ClickAsync();
            }

            return _pointFactory.PrepareGameStateDto(point);
        }

        public async Task SaveStateAsync(GameStateDto state)
        {
            var point = await GetPointAsync(forceRefresh: true);

            if (point == null)
            {
                point = new Point();
                _context.Points.Add(point);
            }

            point.Amount = state.Value;
            point.ClickPower = state.ClickPower;

            await _context.SaveChangesAsync();

            // Обновляем кэш
            _cache.SetPoint(point);
        }

        private async Task<Point?> GetPointAsync(bool forceRefresh = false)
        {
            // Пробуем получить из кэша
            if (!forceRefresh && _cache.TryGetCachedPoint(out var cachedPoint))
            {
                return cachedPoint;
            }

            // Идем в БД
            var point = await _context.Points.FirstOrDefaultAsync();

            if (point != null)
            {
                _cache.SetPoint(point); // Сохраняем в кэш
            }

            return point;
        }

    }
}

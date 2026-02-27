using Incremental.Core.DTOs.Common;
using Incremental.Core.Managers.Interfaces;
using Incremental.Core.ModelFactories.Interfaces;
using Incremental.Core.Services.Interfaces;
using Incremental.Data;
using Incremental.Data.Domain;
using Incremental.Data.Enums;
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
        private readonly IGameCalculationService _calculations;

        public SingleGamePointManager(
            ProjectContext context,
            IPointFactory pointFactory,
            SingleGameCacheManager cache,
            IGameCalculationService calculations)
        {
            _context = context;
            _pointFactory = pointFactory;
            _cache = cache;
            _calculations = calculations;
        }

        // Вспомогательный метод для получения полных данных
        private async Task<Point?> GetPointWithUpgradesAsync(bool forceRefresh = false)
        {
            // Пробуем получить из кэша (без улучшений)
            if (!forceRefresh && _cache.TryGetCachedPoint(out var cachedPoint))
            {
                return cachedPoint;
            }

            var point = await _context.Points
                .Include(p => p.PlayerUpgrades)
                    .ThenInclude(pu => pu.Upgrade)
                .FirstOrDefaultAsync();

            if (point != null)
            {
                _cache.SetPoint(point);
            }

            return point;
        }

        public async Task<GameStateDto> ClickAsync()
        {
            var point = await GetPointWithUpgradesAsync();

            if (point == null)
            {
                point = new Point
                {
                    Amount = 0
                };
                _context.Points.Add(point);
                await _context.SaveChangesAsync();
            }

            var clickPower = _calculations.CalculateTotalClickPower(point);
            point.Amount += clickPower;

            await _context.SaveChangesAsync();

            // Обновляем кэш
            _cache.SetPoint(point);

            return _pointFactory.PrepareGameStateDto(point);
        }

        public async Task<GameStateDto> GetStateAsync()
        {
            var point = await GetPointWithUpgradesAsync();

            if (point == null)
            {
                return _pointFactory.PrepareDefaultGameStateDto();
            }

            return _pointFactory.PrepareGameStateDto(point);
        }

        public async Task SaveStateAsync(GameStateDto state)
        {
            var point = await GetPointWithUpgradesAsync(forceRefresh: true);

            if (point == null)
            {
                point = new Point();
                _context.Points.Add(point);
            }

            point.Amount = state.Value;

            await _context.SaveChangesAsync();

            // Обновляем кэш
            _cache.SetPoint(point);
        }

        public async Task<GameStateDto> ProcessPassiveIncomeAsync()
        {
            var point = await GetPointWithUpgradesAsync();
            if (point == null)
                return _pointFactory.PrepareDefaultGameStateDto();

            var interval = _calculations.CalculateTotalPassiveInterval(point);
            if (interval <= 0)
                return _pointFactory.PrepareGameStateDto(point);

            var now = DateTime.UtcNow;
            var lastPassiveTick = point.LastPassiveTick ?? now.AddMilliseconds(-interval);

            var timeSinceLastTick = (now - lastPassiveTick).TotalMilliseconds;

            if (timeSinceLastTick >= interval)
            {
                int ticks = (int)(timeSinceLastTick / interval);
                if (ticks > 0)
                {
                    var passiveIncome = _calculations.CalculateTotalPassiveIncome(point);
                    point.Amount += passiveIncome * ticks;
                    point.LastPassiveTick = lastPassiveTick.AddMilliseconds(ticks * interval);

                    await _context.SaveChangesAsync();
                    _cache.SetPoint(point);
                }
            }

            return _pointFactory.PrepareGameStateDto(point);
        }
    }
}
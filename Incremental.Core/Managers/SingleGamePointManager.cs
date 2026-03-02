using Incremental.Core.Managers.Interfaces;
using Incremental.Data;
using Incremental.Data.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Incremental.Core.Managers
{
    public class SingleGamePointManager : IPointManager
    {
        private readonly ProjectContext _context;

        public SingleGamePointManager(ProjectContext context)
        {
            _context = context;
        }

        private async Task<Point> GetOrCreatePointAsync()
        {
            var point = await _context.Points
                .FirstOrDefaultAsync();

            if (point == null)
            {
                point = new Point
                {
                    Amount = 0,
                    LastPassiveTick = null
                };
                _context.Points.Add(point);
                await _context.SaveChangesAsync();
            }

            return point;
        }

        public async Task<long> GetCurrentAmountAsync()
        {
            var point = await GetOrCreatePointAsync();
            return point.Amount;
        }

        public async Task<long> ClickAsync(long clickPower)
        {
            var point = await GetOrCreatePointAsync();

            point.Amount += clickPower;

            await _context.SaveChangesAsync();
            return point.Amount;
        }

        public async Task SaveAmountAsync(long amount)
        {
            var point = await GetOrCreatePointAsync();
            point.Amount = amount;
            await _context.SaveChangesAsync();
        }

        public async Task<long> ProcessPassiveIncomeAsync(long passiveIncome, int passiveInterval)
        {
            var point = await GetOrCreatePointAsync();

            if (passiveIncome <= 0 || passiveInterval <= 0)
                return point.Amount;

            var now = DateTime.UtcNow;
            var lastPassiveTick = point.LastPassiveTick ?? now.AddMilliseconds(-passiveInterval);

            var timeSinceLastTick = (now - lastPassiveTick).TotalMilliseconds;

            if (timeSinceLastTick >= passiveInterval)
            {
                int ticks = (int)(timeSinceLastTick / passiveInterval);
                if (ticks > 0)
                {
                    point.Amount += passiveIncome * ticks;
                    point.LastPassiveTick = lastPassiveTick.AddMilliseconds(ticks * passiveInterval);
                    await _context.SaveChangesAsync();
                }
            }

            return point.Amount;
        }
    }
}
using Incremental.Core.DTOs.Common;
using Incremental.Core.Managers.Interfaces;
using Incremental.Data;
using Incremental.Data.Domain;
using Incremental.Data.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Incremental.Core.Managers
{
    public class UpgradeManager : IUpgradeManager
    {
        private readonly ProjectContext _context;

        public UpgradeManager(ProjectContext context)
        {
            _context = context;
        }

        public async Task<UpgradeEffectsDto> GetCurrentEffectsAsync(int pointsId)
        {
            var playerUpgrades = await _context.PlayerUpgrades
                .Include(pu => pu.Upgrade)
                .Where(pu => pu.PointsId == pointsId)
                .ToListAsync();

            var clickPower = playerUpgrades
                .Where(pu => pu.Upgrade.UpgradeType == UpgradeTypes.ClickPower)
                .Sum(pu => pu.CurrentValue);

            var passiveIncome = playerUpgrades
                .Where(pu => pu.Upgrade.UpgradeType == UpgradeTypes.PassiveIncome)
                .Sum(pu => pu.CurrentValue);

            var intervalBonus = playerUpgrades
                .Where(pu => pu.Upgrade.UpgradeType == UpgradeTypes.PassiveInterval)
                .Sum(pu => pu.CurrentValue);

            var passiveInterval = Math.Max(1000, 5000 - (int)intervalBonus);

            return new UpgradeEffectsDto
            {
                ClickPower = clickPower > 0 ? clickPower : 1,
                PassiveIncome = passiveIncome,
                PassiveInterval = passiveInterval
            };
        }

        public async Task<List<UpgradeDto>> GetAvailableUpgradesAsync(int pointsId)
        {
            var allUpgrades = await _context.Upgrades.ToListAsync();
            var playerUpgrades = await _context.PlayerUpgrades
                .Where(pu => pu.PointsId == pointsId)
                .ToDictionaryAsync(pu => pu.UpgradeId, pu => pu);

            var result = allUpgrades.Select(u =>
            {
                playerUpgrades.TryGetValue(u.Id, out var playerUpgrade);
                var currentLevel = playerUpgrade?.Level ?? 0;

                return new UpgradeDto
                {
                    Id = u.Id,
                    Name = u.Name,
                    Description = u.Description,
                    UpgradeType = u.UpgradeType.ToString(),
                    Icon = GetIconForType(u.UpgradeType),
                    CurrentLevel = currentLevel,
                    MaxLevel = int.MaxValue,
                    CurrentValue = playerUpgrade?.CurrentValue ?? 0,
                    NextValue = u.BaseValue * (currentLevel + 1),
                    CurrentPrice = playerUpgrade?.NextPrice ?? u.BasePrice
                };
            }).ToList();

            return result;
        }

        public async Task<List<PlayerUpgradeDto>> GetPlayerUpgradesAsync(int pointsId)
        {
            var playerUpgrades = await _context.PlayerUpgrades
                .Include(pu => pu.Upgrade)
                .Where(pu => pu.PointsId == pointsId)
                .ToListAsync();

            var result = playerUpgrades.Select(pu => new PlayerUpgradeDto
            {
                UpgradeId = pu.UpgradeId,
                Name = pu.Upgrade.Name,
                Level = pu.Level,
                CurrentValue = pu.CurrentValue,
                NextPrice = pu.NextPrice,
                NextValue = pu.Upgrade.BaseValue * (pu.Level + 1)
            }).ToList();

            return result;
        }

        public async Task<PlayerUpgradeDto> BuyUpgradeAsync(int pointsId, int upgradeId)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            var upgrade = await _context.Upgrades.FindAsync(upgradeId);
            if (upgrade == null)
                throw new Exception("Улучшение не найдено");

            var playerUpgrade = await _context.PlayerUpgrades
                .FirstOrDefaultAsync(pu => pu.PointsId == pointsId && pu.UpgradeId == upgradeId);

            int currentLevel = playerUpgrade?.Level ?? 0;
            long price = playerUpgrade?.NextPrice ?? upgrade.BasePrice;

            var point = await _context.Points.FindAsync(pointsId);
            if (point == null)
                throw new Exception("Игрок не найден");

            if (point.Amount < price)
                throw new Exception("Недостаточно очков");

            point.Amount -= price;

            if (playerUpgrade == null)
            {
                playerUpgrade = new PlayerUpgrade
                {
                    PointsId = pointsId,
                    UpgradeId = upgradeId,
                    Level = 1
                };
                _context.PlayerUpgrades.Add(playerUpgrade);
            }
            else
            {
                playerUpgrade.Level++;
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            await _context.Entry(playerUpgrade)
                .Reference(pu => pu.Upgrade)
                .LoadAsync();

            return new PlayerUpgradeDto
            {
                UpgradeId = playerUpgrade.UpgradeId,
                Name = playerUpgrade.Upgrade.Name,
                Level = playerUpgrade.Level,
                CurrentValue = playerUpgrade.CurrentValue,
                NextPrice = playerUpgrade.NextPrice,
                NextValue = playerUpgrade.Upgrade.BaseValue * (playerUpgrade.Level + 1)
            };
        }

        public async Task<bool> CanAffordUpgradeAsync(int pointsId, int upgradeId)
        {
            var point = await _context.Points.FindAsync(pointsId);
            if (point == null)
                return false;

            var playerUpgrade = await _context.PlayerUpgrades
                .FirstOrDefaultAsync(pu => pu.PointsId == pointsId && pu.UpgradeId == upgradeId);

            long price = playerUpgrade?.NextPrice ??
                (await _context.Upgrades.FindAsync(upgradeId))?.BasePrice ?? 0;

            return point.Amount >= price;
        }

        public async Task<long> GetNextUpgradePriceAsync(int pointsId, int upgradeId)
        {
            var playerUpgrade = await _context.PlayerUpgrades
                .FirstOrDefaultAsync(pu => pu.PointsId == pointsId && pu.UpgradeId == upgradeId);

            if (playerUpgrade != null)
                return playerUpgrade.NextPrice;

            var upgrade = await _context.Upgrades.FindAsync(upgradeId);
            if (upgrade == null)
                throw new Exception("Улучшение не найдено");

            return upgrade.BasePrice;
        }

        private string GetIconForType(UpgradeTypes type)
        {
            return type switch
            {
                UpgradeTypes.ClickPower => "⚡",
                UpgradeTypes.PassiveIncome => "💰",
                UpgradeTypes.PassiveInterval => "⏱️",
                _ => "❓"
            };
        }
    }
}
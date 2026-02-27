using Incremental.Core.DTOs.Common;
using Incremental.Core.Managers.Interfaces;
using Incremental.Core.Services.Interfaces;
using Incremental.Data;
using Incremental.Data.Domain;
using Incremental.Data.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incremental.Core.Managers
{
    public class UpgradeManager : IUpgradeManager
    {
        private readonly ProjectContext _context;
        private readonly IGameCalculationService _calculations;

        public UpgradeManager(
            ProjectContext context,
            IGameCalculationService calculations)
        {
            _context = context;
            _calculations = calculations;
        }

        public async Task<ApiResult<List<UpgradeDto>>> GetAvailableUpgradesAsync(int pointsId)
        {
            try
            {
                var point = await _context.Points
                    .Include(p => p.PlayerUpgrades)
                    .FirstOrDefaultAsync(p => p.Id == pointsId);

                if (point == null)
                    return ApiResult<List<UpgradeDto>>.Fail("Игрок не найден");

                var allUpgrades = await _context.Upgrades.ToListAsync();

                var playerUpgrades = point.PlayerUpgrades?
                    .ToDictionary(pu => pu.UpgradeId, pu => pu.Level)
                    ?? new Dictionary<int, int>();

                var result = allUpgrades.Select(u => new UpgradeDto
                {
                    Id = u.Id,
                    Name = u.Name,
                    Description = u.Description,
                    UpgradeType = u.UpgradeType.ToString(),
                    Icon = GetIconForType(u.UpgradeType),
                    CurrentLevel = playerUpgrades.ContainsKey(u.Id) ? playerUpgrades[u.Id] : 0,
                    MaxLevel = int.MaxValue, 
                    CurrentValue = playerUpgrades.ContainsKey(u.Id)
                        ? u.BaseValue * playerUpgrades[u.Id]
                        : 0,
                    NextValue = playerUpgrades.ContainsKey(u.Id)
                        ? u.BaseValue * (playerUpgrades[u.Id] + 1)
                        : u.BaseValue,
                    CurrentPrice = playerUpgrades.ContainsKey(u.Id)
                        ? (long)(u.BasePrice * Math.Pow((double)u.PriceMultiplier, playerUpgrades[u.Id]))
                        : u.BasePrice
                }).ToList();

                return ApiResult<List<UpgradeDto>>.Ok(result);
            }
            catch (Exception ex)
            {
                return ApiResult<List<UpgradeDto>>.Fail($"Ошибка: {ex.Message}");
            }
        }

        public async Task<ApiResult<List<PlayerUpgradeDto>>> GetPlayerUpgradesAsync(int pointsId)
        {
            try
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

                return ApiResult<List<PlayerUpgradeDto>>.Ok(result);
            }
            catch (Exception ex)
            {
                return ApiResult<List<PlayerUpgradeDto>>.Fail($"Ошибка: {ex.Message}");
            }
        }

        public async Task<ApiResult<PlayerUpgradeDto>> BuyUpgradeAsync(int pointsId, int upgradeId)
        {
            try
            {
                // Начинаем транзакцию
                await using var transaction = await _context.Database.BeginTransactionAsync();

                var point = await _context.Points.FindAsync(pointsId);
                if (point == null)
                    return ApiResult<PlayerUpgradeDto>.Fail("Игрок не найден");

                var upgrade = await _context.Upgrades.FindAsync(upgradeId);
                if (upgrade == null)
                    return ApiResult<PlayerUpgradeDto>.Fail("Улучшение не найдено");

                var playerUpgrade = await _context.PlayerUpgrades
                    .FirstOrDefaultAsync(pu => pu.PointsId == pointsId && pu.UpgradeId == upgradeId);

                int currentLevel = playerUpgrade?.Level ?? 0;
                long price = playerUpgrade?.NextPrice ?? upgrade.BasePrice;

                // Проверяем, хватает ли очков
                if (point.Amount < price)
                    return ApiResult<PlayerUpgradeDto>.Fail("Недостаточно очков");

                // Списываем очки
                point.Amount -= price;

                // Обновляем или создаем запись об улучшении
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

                // Загружаем Upgrade для вычислений
                await _context.Entry(playerUpgrade)
                    .Reference(pu => pu.Upgrade)
                    .LoadAsync();

                var result = new PlayerUpgradeDto
                {
                    UpgradeId = playerUpgrade.UpgradeId,
                    Name = playerUpgrade.Upgrade.Name,
                    Level = playerUpgrade.Level,
                    CurrentValue = playerUpgrade.CurrentValue,
                    NextPrice = playerUpgrade.NextPrice,
                    NextValue = playerUpgrade.Upgrade.BaseValue * (playerUpgrade.Level + 1)
                };

                return ApiResult<PlayerUpgradeDto>.Ok(result, "Улучшение куплено");
            }
            catch (Exception ex)
            {
                return ApiResult<PlayerUpgradeDto>.Fail($"Ошибка при покупке: {ex.Message}");
            }
        }

        public async Task<ApiResult<bool>> CanAffordUpgradeAsync(int pointsId, int upgradeId)
        {
            try
            {
                var point = await _context.Points.FindAsync(pointsId);
                if (point == null)
                    return ApiResult<bool>.Ok(false, "Игрок не найден");

                var playerUpgrade = await _context.PlayerUpgrades
                    .FirstOrDefaultAsync(pu => pu.PointsId == pointsId && pu.UpgradeId == upgradeId);

                long price = playerUpgrade?.NextPrice ??
                    (await _context.Upgrades.FindAsync(upgradeId))?.BasePrice ?? 0;

                return ApiResult<bool>.Ok(point.Amount >= price);
            }
            catch (Exception ex)
            {
                return ApiResult<bool>.Fail($"Ошибка: {ex.Message}");
            }
        }

        public async Task<ApiResult<long>> GetNextUpgradePriceAsync(int pointsId, int upgradeId)
        {
            try
            {
                var playerUpgrade = await _context.PlayerUpgrades
                    .FirstOrDefaultAsync(pu => pu.PointsId == pointsId && pu.UpgradeId == upgradeId);

                if (playerUpgrade != null)
                    return ApiResult<long>.Ok(playerUpgrade.NextPrice);

                var upgrade = await _context.Upgrades.FindAsync(upgradeId);
                return upgrade != null
                    ? ApiResult<long>.Ok(upgrade.BasePrice)
                    : ApiResult<long>.Fail("Улучшение не найдено");
            }
            catch (Exception ex)
            {
                return ApiResult<long>.Fail($"Ошибка: {ex.Message}");
            }
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
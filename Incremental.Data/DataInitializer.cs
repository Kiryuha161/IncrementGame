using Incremental.Data.Domain;
using Incremental.Data.Enums;
using Incremental.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incremental.Data
{
    public class DataInitializer : IDataInitializer
    {
        private readonly ProjectContext _context;
        private readonly ILogger<DataInitializer> _logger;

        public DataInitializer(ProjectContext context, ILogger<DataInitializer> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task InitializeAsync()
        {
            try
            {
                await EnsureUpgradesAsync();
                _logger.LogInformation("✅ Данные инициализированы");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Ошибка инициализации данных");
            }
        }

        private async Task EnsureUpgradesAsync()
        {
            _logger.LogInformation("Добавление базовых улучшений...");
            await EnsureUpgradeAsync("Сила клика", new Upgrade
            {
                Name = "Сила клика",
                Description = "Увеличивает количество очков за клик",
                BaseValue = 1,
                ValueMultiplier = 1.5m,
                BasePrice = 50,
                PriceMultiplier = 1.5m,
                UpgradeType = UpgradeTypes.ClickPower
            });

            await EnsureUpgradeAsync("Пассивный доход", new Upgrade
            {
                Name = "Пассивный доход",
                Description = "Добавляет пассивный доход каждые 5 секунд",
                BaseValue = 1,
                ValueMultiplier = 1.5m,
                BasePrice = 500,
                PriceMultiplier = 1.5m,
                UpgradeType = UpgradeTypes.PassiveIncome
            });

            await EnsureUpgradeAsync("Скорость", new Upgrade
            {
                Name = "Скорость",
                Description = "Уменьшает интервал пассивного дохода на 100мс",
                BaseValue = 100,
                ValueMultiplier = 1.0m,
                BasePrice = 1000,
                PriceMultiplier = 1.5m,
                UpgradeType = UpgradeTypes.PassiveInterval
            });

            await EnsureUpgradeAsync("Оптовик", new Upgrade
            {
                Name = "Оптовик",
                Description = "Уменьшает стоимость всех улучшений на 5% за уровень",
                BaseValue = 5,
                ValueMultiplier = 1.0m,
                BasePrice = 2000,
                PriceMultiplier = 2.0m,
                UpgradeType = UpgradeTypes.DiscountAll
            });

            await EnsureUpgradeAsync("Мощь", new Upgrade
            {
                Name = "Мощь",
                Description = "Увеличивает эффект уровня каждого умения, кроме Оптовика и Скорости, на 10% за уровень",
                BaseValue = 10, // 10% за уровень
                ValueMultiplier = 1.0m,
                BasePrice = 5000,
                PriceMultiplier = 2.0m,
                UpgradeType = UpgradeTypes.PowerBoost
            });

            // await _context.Upgrades.AddRangeAsync(upgrades);
            await _context.SaveChangesAsync();
        }

        private async Task EnsureUpgradeAsync(string name, Upgrade upgrade)
        {
            var exists = await _context.Upgrades.AnyAsync(u => u.Name == name);
            if (!exists)
            {
                await _context.Upgrades.AddAsync(upgrade);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"✅ Добавлено улучшение: {name}");
            }
        }
    }
}

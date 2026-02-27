using FluentAssertions;
using Incremental.Core.Services;
using Incremental.Data.Domain;
using Incremental.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incremental.Tests.Services.GameCalculationServiceTests
{
    public class CalculateTotalClickPowerTests
    {
        private readonly GameCalculationService _service;

        public CalculateTotalClickPowerTests()
        {
            _service = new GameCalculationService();
        }

        [Fact]
        public void WhenNoUpgrades_ReturnsBaseValue()
        {
            // Arrange (готовим данные)
            var point = new Point
            {
                Id = 1,
                Amount = 100,
                PlayerUpgrades = new List<PlayerUpgrade>() // Пустой список
            };

            // Act (выполняем)
            var result = _service.CalculateTotalClickPower(point);

            // Assert (проверяем)
            result.Should().Be(1); // Ожидаем базовое значение 1
        }

        [Fact]
        public void WithClickUpgrades_ReturnsSum()
        {
            // Arrange
            var upgrade = new Upgrade
            {
                Id = 1,
                Name = "Click Power",
                BaseValue = 1,
                UpgradeType = UpgradeTypes.ClickPower
            };

            var playerUpgrade = new PlayerUpgrade
            {
                Id = 1,
                Level = 5,
                Upgrade = upgrade,
                PointsId = 1
            };

            var point = new Point
            {
                Id = 1,
                Amount = 100,
                PlayerUpgrades = new List<PlayerUpgrade> { playerUpgrade }
            };

            // Act
            var result = _service.CalculateTotalClickPower(point);

            // Assert
            // CurrentValue = BaseValue * Level = 1 * 5 = 5
            result.Should().Be(5);
        }

        [Fact]
        public void WithMultipleUpgrades_ReturnsSumOfAll()
        {
            // Arrange
            var upgrade1 = new Upgrade
            {
                Id = 1,
                Name = "Click Power 1",
                BaseValue = 2,
                UpgradeType = UpgradeTypes.ClickPower
            };

            var upgrade2 = new Upgrade
            {
                Id = 2,
                Name = "Click Power 2",
                BaseValue = 3,
                UpgradeType = UpgradeTypes.ClickPower
            };

            var playerUpgrade1 = new PlayerUpgrade
            {
                Id = 1,
                Level = 3, // 2 * 3 = 6
                Upgrade = upgrade1,
                PointsId = 1
            };

            var playerUpgrade2 = new PlayerUpgrade
            {
                Id = 2,
                Level = 2, // 3 * 2 = 6
                Upgrade = upgrade2,
                PointsId = 1
            };

            var point = new Point
            {
                Id = 1,
                Amount = 100,
                PlayerUpgrades = new List<PlayerUpgrade> { playerUpgrade1, playerUpgrade2 }
            };

            // Act
            var result = _service.CalculateTotalClickPower(point);

            // Assert: 6 + 6 = 12
            result.Should().Be(12);
        }

        [Fact]
        public void WhenNullPoint_ReturnsBaseValue()
        {
            // Arrange
            Point point = null;

            // Act
            var result = _service.CalculateTotalClickPower(point);

            // Assert
            result.Should().Be(1);
        }

        [Fact]
        public void WithOtherUpgradeTypes_IgnoresThem()
        {
            // Arrange
            var clickUpgrade = new Upgrade
            {
                Id = 1,
                Name = "Click Power",
                BaseValue = 1,
                UpgradeType = UpgradeTypes.ClickPower
            };

            var passiveUpgrade = new Upgrade
            {
                Id = 2,
                Name = "Passive Income",
                BaseValue = 1,
                UpgradeType = UpgradeTypes.PassiveIncome // Другой тип!
            };

            var playerClickUpgrade = new PlayerUpgrade
            {
                Id = 1,
                Level = 5,
                Upgrade = clickUpgrade,
                PointsId = 1
            };

            var playerPassiveUpgrade = new PlayerUpgrade
            {
                Id = 2,
                Level = 10,
                Upgrade = passiveUpgrade,
                PointsId = 1
            };

            var point = new Point
            {
                Id = 1,
                Amount = 100,
                PlayerUpgrades = new List<PlayerUpgrade> { playerClickUpgrade, playerPassiveUpgrade }
            };

            // Act
            var result = _service.CalculateTotalClickPower(point);

            // Assert: только клик-апгрейд = 5
            result.Should().Be(5);
        }
    }
}

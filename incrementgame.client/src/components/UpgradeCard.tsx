import styles from '../App.module.css';
import type { Upgrade } from '../api/upgrade';

interface UpgradeCardProps {
    upgrade: Upgrade;
    userPoints: number;
    loading: boolean;
    powerMultiplier?: number;
    onBuy: (id: number) => void;
    additionalStats?: React.ReactNode;
}

export function UpgradeCard({
    upgrade,
    userPoints,
    loading,
    powerMultiplier = 1,
    onBuy,
    additionalStats
}: UpgradeCardProps) {
    const variant =
        upgrade.upgradeType === 'ClickPower' ? 'click' :
            upgrade.upgradeType === 'PassiveIncome' ? 'passive' :
                upgrade.upgradeType === 'PassiveInterval' ? 'speed' :
                    upgrade.upgradeType === 'DiscountAll' ? 'discount' :
                        'power';

    const variantClass =
        variant === 'click' ? styles.clickUpgrade :
            variant === 'passive' ? styles.passiveUpgrade :
                variant === 'speed' ? styles.speedUpgrade :
                    variant === 'discount' ? styles.discountUpgrade :
                        styles.powerUpgrade;

    const isDisabled = loading || userPoints < upgrade.currentPrice;
    const neededPoints = upgrade.currentPrice - userPoints;

    // Форматируем значения для разных типов улучшений
    const formatValue = (value: number) => {
        if (upgrade.upgradeType === 'PassiveInterval') {
            return `${(value / 1000).toFixed(1)}с`;
        }
        if (upgrade.upgradeType === 'DiscountAll') {
            return `${value}% скидки`;
        }
        if (upgrade.upgradeType === 'PowerBoost') {
            return `+${value}% силы`;
        }
        return `+${value}`;
    };

    // Показываем усиленное значение для ClickPower и PassiveIncome
    const getDisplayValue = () => {
        if (powerMultiplier > 1 &&
            (upgrade.upgradeType === 'ClickPower' || upgrade.upgradeType === 'PassiveIncome')) {
            const boostedValue = Math.round(upgrade.currentValue * powerMultiplier);
            return (
                <span>
                    {formatValue(boostedValue)}
                    <span className={styles.baseValue}>
                        ({formatValue(upgrade.currentValue)})
                    </span>
                </span>
            );
        }
        return formatValue(upgrade.currentValue);
    };

    const getNextDisplayValue = () => {
        if (powerMultiplier > 1 &&
            (upgrade.upgradeType === 'ClickPower' || upgrade.upgradeType === 'PassiveIncome')) {
            const boostedNextValue = Math.round(upgrade.nextValue * powerMultiplier);
            return (
                <span>
                    {formatValue(boostedNextValue)}
                    <span className={styles.baseValue}>
                        ({formatValue(upgrade.nextValue)})
                    </span>
                </span>
            );
        }
        return formatValue(upgrade.nextValue);
    };

    return (
        <div className={`${styles.upgradeCard} ${variantClass}`}>
            <div className={styles.upgradeIcon}>{upgrade.icon}</div>
            <h3 className={styles.upgradeTitle}>{upgrade.name}</h3>
            {upgrade.description && (
                <div className={styles.upgradeDescription}>
                    {upgrade.description}
                </div>
            )}
            <div className={styles.upgradeStats}>
                <div>Уровень: {upgrade.currentLevel}</div>
                <div>Текущее: {getDisplayValue()}</div>
                <div>Следующее: {getNextDisplayValue()}</div>
                {powerMultiplier > 1 && upgrade.upgradeType === 'PowerBoost' && (
                    <div className={styles.powerMultiplier}>
                        Текущий множитель: x{powerMultiplier.toFixed(2)}
                    </div>
                )}
                {additionalStats}
            </div>
            <div className={styles.priceContainer}>
                {upgrade.originalPrice && upgrade.originalPrice !== upgrade.currentPrice ? (
                    <>
                        <span className={styles.oldPrice}>
                            {upgrade.originalPrice.toLocaleString()}
                        </span>
                        <span className={styles.newPrice}>
                            {upgrade.currentPrice.toLocaleString()} очков
                        </span>
                        <span className={styles.discountBadge}>
                            -{Math.round((1 - upgrade.currentPrice / upgrade.originalPrice) * 100)}%
                        </span>
                    </>
                ) : (
                    <div className={styles.upgradePrice}>
                        {upgrade.currentPrice.toLocaleString()} очков
                    </div>
                )}
            </div>
            <div className={styles.buttonWrapper}
                title={isDisabled && neededPoints > 0 ? `Нужно еще ${neededPoints} очков` : 'Купить улучшение'}>
                <button
                    onClick={() => onBuy(upgrade.id)}
                    className={styles.buyButton}
                    disabled={isDisabled}
                >
                    Купить
                </button>
            </div>
        </div>
    );
}
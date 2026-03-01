import styles from '../App.module.css';
import type { Upgrade } from '../api/upgrade';

interface UpgradeCardProps {
    upgrade: Upgrade;
    userPoints: number;
    loading: boolean;
    onBuy: (id: number) => void;
    additionalStats?: React.ReactNode;
}

export function UpgradeCard({
    upgrade,
    userPoints,
    loading,
    onBuy,
    additionalStats
}: UpgradeCardProps) {
    const variant =
        upgrade.upgradeType === 'ClickPower' ? 'click' :
            upgrade.upgradeType === 'PassiveIncome' ? 'passive' : 'speed';

    const variantClass =
        variant === 'click' ? styles.clickUpgrade :
            variant === 'passive' ? styles.passiveUpgrade :
                styles.speedUpgrade;

    const isDisabled = loading || userPoints < upgrade.currentPrice;
    const neededPoints = upgrade.currentPrice - userPoints;

    // Форматируем значения для разных типов улучшений
    const formatValue = (value: number) => {
        if (upgrade.upgradeType === 'PassiveInterval') {
            return `${(value / 1000).toFixed(1)}с`;
        }
        return `+${value}`;
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
                <div>Текущее: {formatValue(upgrade.currentValue)}</div>
                <div>Следующее: {formatValue(upgrade.nextValue)}</div>
                {additionalStats}
            </div>
            <div className={styles.upgradePrice}>
                {upgrade.currentPrice.toLocaleString()} очков
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
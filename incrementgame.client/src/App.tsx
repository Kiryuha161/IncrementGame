import './App.css';
import { useGame } from './hooks/useGame';
import { startSignalR, stopSignalR } from './api/signalR';
import { useEffect, useState } from 'react';
import styles from './App.module.css';

function App() {
    const {
        state, setState, upgrades, loading, error, syncStatus,
        setSyncStatus, click, buyUpgrade
    } = useGame();
    const [showUpgrades, setShowUpgrades] = useState(false);

    useEffect(() => {
        if (!state) return;
        startSignalR(
            (updatedState) => {
                setState(updatedState);
                setSyncStatus('synced');
            },
            (count) => {
                console.log('Активных клиентов:', count);
            }
        );

        return () => {
            stopSignalR();
        };
    }, []);

    if (loading && !state) return <div>Загрузка...</div>;
    if (error) return <div>Ошибка: {error}</div>;
    if (!state) return <div>Нет данных</div>;

    const formattedPassiveInterval = (state.passiveInterval / 1000).toFixed(1);
    const syncStatusClass = syncStatus === 'syncing' ? styles.syncing :
        syncStatus === 'synced' ? styles.synced : styles.error;

    // Группируем улучшения по типу
    const clickUpgrade = upgrades.find(u => u.upgradeType === 'ClickPower');
    const passiveUpgrade = upgrades.find(u => u.upgradeType === 'PassiveIncome');
    const speedUpgrade = upgrades.find(u => u.upgradeType === 'PassiveInterval');

    return (
        <div className={styles.container}>
            {/* Основная информация */}
            <div className={styles.mainBlock}>
                <h1 className={styles.pointsCounter}>
                    {state.value.toLocaleString()} очков
                </h1>

                <div className={styles.statsGrid}>
                    <div className={styles.statCard}>
                        <div className={styles.statLabel}>Сила клика</div>
                        <div className={styles.statValue}>⚡ {state.clickPower}</div>
                    </div>

                    <div className={styles.statCard}>
                        <div className={styles.statLabel}>Пассивный доход</div>
                        <div className={styles.statValue}>💰 {state.passiveIncome || 0}</div>
                    </div>

                    <div className={styles.statCard}>
                        <div className={styles.statLabel}>Интервал</div>
                        <div className={styles.statValue}>
                            ⏱️ {state.passiveInterval ? formattedPassiveInterval : 0}с
                        </div>
                    </div>
                </div>

                {/* Статус синхронизации */}
                <div className={`${styles.syncStatus} ${syncStatusClass}`}>
                    {syncStatus === 'syncing' && '⏳ Сохранение...'}
                    {syncStatus === 'synced' && '💾 Все сохранено'}
                    {syncStatus === 'error' && '⚠️ Ошибка сохранения'}
                </div>
            </div>

            {/* Кнопка клика */}
            <button
                onClick={click}
                disabled={loading}
                className={styles.clickButton}
            >
                КЛИК!
            </button>

            {/* Кнопка показа/скрытия улучшений */}
            <button
                onClick={() => setShowUpgrades(!showUpgrades)}
                className={styles.toggleButton}
            >
                {showUpgrades ? '▼ Скрыть улучшения' : '▶ Показать улучшения'}
            </button>

            {/* Улучшения */}
            {showUpgrades && (
                <div className={styles.upgradesGrid}>
                    {/* Улучшение клика */}
                    {clickUpgrade && (
                        <div className={`${styles.upgradeCard} ${styles.clickUpgrade}`}>
                            <div className={styles.upgradeIcon}>⚡</div>
                            <h3 className={styles.upgradeTitle}>{clickUpgrade.name}</h3>
                            <div className={styles.upgradeDescription}>
                                {clickUpgrade.description}
                            </div>
                            <div className={styles.upgradeStats}>
                                <div>Уровень: {clickUpgrade.currentLevel}</div>
                                <div>Текущее: +{clickUpgrade.currentValue}</div>
                                <div>Следующее: +{clickUpgrade.nextValue}</div>
                            </div>
                            <div className={styles.upgradePrice}>
                                {clickUpgrade.currentPrice.toLocaleString()} очков
                            </div>
                            <button
                                onClick={() => buyUpgrade(clickUpgrade.id)}
                                className={styles.buyButton}
                                disabled={state.value < clickUpgrade.currentPrice}
                            >
                                Купить
                            </button>
                        </div>
                    )}

                    {/* Улучшение пассивного дохода */}
                    {passiveUpgrade && (
                        <div className={`${styles.upgradeCard} ${styles.passiveUpgrade}`}>
                            <div className={styles.upgradeIcon}>💰</div>
                            <h3 className={styles.upgradeTitle}>{passiveUpgrade.name}</h3>
                            <div className={styles.upgradeDescription}>
                                {passiveUpgrade.description}
                            </div>
                            <div className={styles.upgradeStats}>
                                <div>Уровень: {passiveUpgrade.currentLevel}</div>
                                <div>Доход: +{passiveUpgrade.currentValue}/тик</div>
                                <div>След.: +{passiveUpgrade.nextValue}/тик</div>
                            </div>
                            <div className={styles.upgradePrice}>
                                {passiveUpgrade.currentPrice.toLocaleString()} очков
                            </div>
                            <button
                                onClick={() => buyUpgrade(passiveUpgrade.id)}
                                className={styles.buyButton}
                                disabled={state.value < passiveUpgrade.currentPrice}
                            >
                                Купить
                            </button>
                        </div>
                    )}

                    {/* Улучшение скорости */}
                    {speedUpgrade && (
                        <div className={`${styles.upgradeCard} ${styles.speedUpgrade}`}>
                            <div className={styles.upgradeIcon}>⏱️</div>
                            <h3 className={styles.upgradeTitle}>{speedUpgrade.name}</h3>
                            <div className={styles.upgradeDescription}>
                                {speedUpgrade.description}
                            </div>
                            <div className={styles.upgradeStats}>
                                <div>Уровень: {speedUpgrade.currentLevel}</div>
                                <div>Интервал: {formattedPassiveInterval}с</div>
                                <div>След.: {((state.passiveInterval - 100) / 1000).toFixed(1)}с</div>
                            </div>
                            <div className={styles.upgradePrice}>
                                {speedUpgrade.currentPrice.toLocaleString()} очков
                            </div>
                            <button
                                onClick={() => buyUpgrade(speedUpgrade.id)}
                                className={styles.buyButton}
                                disabled={state.value < speedUpgrade.currentPrice}
                            >
                                Купить
                            </button>
                        </div>
                    )}
                </div>
            )}

            {/* Подвал */}
            <div className={styles.footer}>
                <div>Активных клиентов: 2</div>
                {state.passiveIncome > 0
                    ? `✨ Пассивный доход активен: +${state.passiveIncome} каждые ${formattedPassiveInterval}с`
                    : '💤 Пассивный доход отключен (купите улучшение)'}
            </div>
        </div>
    );
}

export default App;
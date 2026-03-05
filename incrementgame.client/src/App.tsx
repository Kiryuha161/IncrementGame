import './App.css';
import { useGame } from './hooks/useGame';
import { useEffect, useState } from 'react';
import styles from './App.module.css';
import { ErrorToast } from './components/ErrorToast';
import { StatsCard } from './components/StatsCard';
import { UpgradeCard } from './components/UpgradeCard';
import { LoadingScreen } from './components/LoadingScreen';
import { ErrorScreen } from './components/ErrorScreen';
import { SyncStatus } from './components/SyncStatus';

function App() {
    const {
        state,
        upgrades,
        activeClients,
        loading,
        error,
        syncStatus,
        click,
        buyUpgrade,
        refresh
    } = useGame();

    const [showUpgrades, setShowUpgrades] = useState(false);
    const [showErrorToast, setShowErrorToast] = useState(false);

    useEffect(() => {
        if (error) {
            setShowErrorToast(true);
            const timer = setTimeout(() => setShowErrorToast(false), 5000);
            return () => clearTimeout(timer);
        }
    }, [error]);

    if (loading && !state) {
        return <LoadingScreen />;
    }

    if (error && !state) {
        return <ErrorScreen error={error} onRetry={refresh} />;
    }

    if (!state) return <div>Нет данных</div>;

    const formattedPassiveInterval = (state.passiveInterval / 1000).toFixed(1);

    // Вычисляем базовые значения (без учета PowerBoost)
    const baseClickPower = state.powerMultiplier > 1
        ? Math.round(state.clickPower / state.powerMultiplier)
        : state.clickPower;
    const basePassiveIncome = state.powerMultiplier > 1
        ? Math.round(state.passiveIncome / state.powerMultiplier)
        : state.passiveIncome;

    return (
        <div className={styles.container}>
            <ErrorToast
                error={error}
                show={showErrorToast}
                onClose={() => setShowErrorToast(false)}
            />

            <div className={styles.mainBlock}>
                <h1 className={styles.pointsCounter}>
                    {state.value.toLocaleString()} очков
                </h1>

                <div className={styles.statsGrid}>
                    <StatsCard
                        label="Сила клика"
                        value={
                            <span>
                                {state.clickPower}
                                {state.powerMultiplier > 1 && (
                                    <span className={styles.baseValue}>
                                        ({baseClickPower} +{state.clickPower - baseClickPower})
                                    </span>
                                )}
                            </span>
                        }
                        icon="⚡"
                    />

                    <StatsCard
                        label="Пассивный доход"
                        value={
                            <span>
                                {state.passiveIncome || 0}
                                {state.powerMultiplier > 1 && state.passiveIncome > 0 && (
                                    <span className={styles.baseValue}>
                                        ({basePassiveIncome} +{state.passiveIncome - basePassiveIncome})
                                    </span>
                                )}
                            </span>
                        }
                        icon="💰"
                    />

                    <StatsCard
                        label="Интервал"
                        value={state.passiveInterval ? `${formattedPassiveInterval}с` : '0с'}
                        icon="⏱️"
                    />

                    {state.powerMultiplier > 1 && (
                        <StatsCard
                            label="Усилитель"
                            value={`x${state.powerMultiplier.toFixed(2)}`}
                            icon="✨"
                        />
                    )}
                </div>

                <SyncStatus status={syncStatus} />
            </div>

            <button
                onClick={click}
                disabled={loading}
                className={styles.clickButton}
            >
                КЛИК!
            </button>

            <button
                onClick={() => setShowUpgrades(!showUpgrades)}
                className={styles.toggleButton}
            >
                {showUpgrades ? '▼ Скрыть улучшения' : '▶ Показать улучшения'}
            </button>

            {showUpgrades && (
                <div className={styles.upgradesGrid}>
                    {upgrades.map(upgrade => (
                        <UpgradeCard
                            key={upgrade.id}
                            upgrade={upgrade}
                            userPoints={state.value}
                            loading={loading}
                            powerMultiplier={state.powerMultiplier}
                            onBuy={(id) => {
                                const found = upgrades.find(u => u.id === id);
                                if (found) {
                                    buyUpgrade(id, found.currentPrice);
                                }
                            }}
                            additionalStats={
                                upgrade.upgradeType === 'PassiveInterval' ? (
                                    <div className={styles.upgradeStat}>
                                        ⏱️ Текущий интервал: {formattedPassiveInterval}с
                                    </div>
                                ) : undefined
                            }
                        />
                    ))}
                </div>
            )}

            <div className={styles.footer}>
                <div>👥 Активных клиентов: {activeClients}</div>
                {state.passiveIncome > 0
                    ? `✨ Пассивный доход активен: +${state.passiveIncome} каждые ${formattedPassiveInterval}с`
                    : '💤 Пассивный доход отключен (купите улучшение)'}
            </div>
        </div>
    );
}

export default App;
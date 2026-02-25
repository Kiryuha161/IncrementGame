import './App.css';
import { useGame } from './hooks/useGame';
import { startSignalR, stopSignalR } from './api/signalR';
import { useEffect, useState } from 'react';
import styles from './App.module.css'; 

function App() {
    const { state, setState, loading, error, syncStatus, setSyncStatus, click } = useGame();
    const [showUpgrades, setShowUpgrades] = useState(false);

    useEffect(() => {
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

    const buyClickUpgrade = () => alert('Улучшение силы клика (будет позже)');
    const buyPassiveUpgrade = () => alert('Улучшение пассивного дохода (будет позже)');
    const buySpeedUpgrade = () => alert('Улучшение скорости пассивного дохода (будет позже)');

    const formattedPassiveInterval = (state.passiveInterval / 1000).toFixed(1);

    // Определяем класс статуса синхронизации
    const syncStatusClass =
        syncStatus === 'syncing' ? styles.syncing :
            syncStatus === 'synced' ? styles.synced : styles.error;

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
                    <div className={`${styles.upgradeCard} ${styles.clickUpgrade}`}>
                        <div className={styles.upgradeIcon}>⚡</div>
                        <h3 className={styles.upgradeTitle}>Сила клика +1</h3>
                        <div className={styles.upgradeDescription}>
                            Увеличивает количество очков за клик
                        </div>
                        <div className={styles.upgradePrice}>100 очков</div>
                        <button
                            onClick={buyClickUpgrade}
                            className={styles.buyButton}
                        >
                            Купить
                        </button>
                    </div>

                    {/* Улучшение пассивного дохода */}
                    <div className={`${styles.upgradeCard} ${styles.passiveUpgrade}`}>
                        <div className={styles.upgradeIcon}>💰</div>
                        <h3 className={styles.upgradeTitle}>Пассивный доход +1</h3>
                        <div className={styles.upgradeDescription}>
                            Увеличивает доход за интервал
                        </div>
                        <div className={styles.upgradePrice}>200 очков</div>
                        <button
                            onClick={buyPassiveUpgrade}
                            className={styles.buyButton}
                        >
                            Купить
                        </button>
                    </div>

                    {/* Улучшение скорости */}
                    <div className={`${styles.upgradeCard} ${styles.speedUpgrade}`}>
                        <div className={styles.upgradeIcon}>⏱️</div>
                        <h3 className={styles.upgradeTitle}>Скорость -0.1с</h3>
                        <div className={styles.upgradeDescription}>
                            Уменьшает интервал пассивного дохода
                        </div>
                        <div className={styles.upgradePrice}>300 очков</div>
                        <button
                            onClick={buySpeedUpgrade}
                            className={styles.buyButton}
                        >
                            Купить
                        </button>
                    </div>
                </div>
            )}

            {/* Подвал */}
            <div className={styles.footer}>
                <div>Активных клиентов: {state.passiveInterval > 0 ? '2' : '1'}</div>
                {state.passiveIncome > 0
                    ? `✨ Пассивный доход активен: +${state.passiveIncome} каждые ${formattedPassiveInterval}с`
                    : '💤 Пассивный доход отключен (купите улучшение)'}
            </div>
        </div>
    );
}

export default App;
import { useState, useEffect, useCallback } from 'react';
import { getGameState, saveGameState, processPassiveIncome, type GameState } from '../api/points';
import { getAvailableUpgrades, buyUpgrade, type Upgrade, type PlayerUpgrade, getPlayerUpgrades } from '../api/upgrade';

export function useGame() {
    const [state, setState] = useState<GameState | null>(null);
    const [upgrades, setUpgrades] = useState<Upgrade[]>([]);
    const [playerUpgrades, setPlayerUpgrades] = useState<PlayerUpgrade[]>([]);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const [syncStatus, setSyncStatus] = useState<'synced' | 'syncing' | 'error'>('synced');
    const [pendingClicks, setPendingClicks] = useState<number>(0);

    // Пассивный доход
    useEffect(() => {
        if (!state || state.passiveInterval <= 0 || state.passiveIncome <= 0) return;

        // НЕ запускаем пассивный доход, если есть несохраненные клики
        if (pendingClicks > 0) {
            console.log('⏳ Ожидание сохранения... Пассивный доход пропущен');
            return;
        }

        console.log("✅ Запуск пассивного дохода:", state.passiveInterval, "мс");

        const timer = setInterval(async () => {
            try {
                const data = await processPassiveIncome();
                setState(prev => {
                    if (prev && prev.value === data.value) return prev;
                    return data;
                });
            } catch (err) {
                console.error('Passive income error:', err);
            }
        }, state.passiveInterval);

        return () => {
            console.log("🛑 Остановка пассивного дохода");
            clearInterval(timer);
        };
    }, [state?.passiveInterval, state?.passiveIncome, pendingClicks]);

    // Загрузка всех данных
    const loadAllData = useCallback(async () => {
        setLoading(true);
        try {
            const [gameData, availableUpgrades, playerUpgradesData] = await Promise.all([
                getGameState(),
                getAvailableUpgrades(),
                getPlayerUpgrades()
            ]);
            setState(gameData);
            setUpgrades(availableUpgrades);
            setPlayerUpgrades(playerUpgradesData);
            setError(null);
        } catch (err) {
            setError(err instanceof Error ? err.message : 'Ошибка при загрузке');
        } finally {
            setLoading(false);
        }
    }, []);

    // Покупка улучшения
    const handleBuyUpgrade = useCallback(async (upgradeId: number) => {
        if (!state) return;

        const upgrade = upgrades.find(u => u.id === upgradeId);
        if (!upgrade) return;

        setLoading(true);
        try {
            const result = await buyUpgrade(upgradeId);

            // Обновляем список доступных улучшений
            setUpgrades(prev => prev.map(u =>
                u.id === upgradeId
                    ? {
                        ...u,
                        currentLevel: u.currentLevel + 1,
                        currentValue: result.currentValue,
                        currentPrice: result.nextPrice
                    }
                    : u
            ));

            // Обновляем состояние игры 
            const newState = await getGameState();
            setState(newState);
            setError(null);

            // Обновляем прогресс игрока
            setPlayerUpgrades(prev => {
                const exists = prev.find(p => p.upgradeId === upgradeId);
                if (exists) {
                    return prev.map(p =>
                        p.upgradeId === upgradeId
                            ? { ...p, level: p.level + 1, currentValue: result.currentValue, nextPrice: result.nextPrice }
                            : p
                    );
                } else {
                    return [...prev, {
                        upgradeId: result.upgradeId,
                        name: result.name,
                        level: result.level,
                        currentValue: result.currentValue,
                        nextPrice: result.nextPrice,
                        nextValue: result.nextValue
                    }];
                }
            });

        } catch (err: any) {
            // Только серверная ошибка
            setError(err.message || 'Ошибка при покупке');
        } finally {
            setLoading(false);
        }
    }, [state, upgrades]);

    const syncWithServer = useCallback(async () => {
        if (pendingClicks === 0 || !state) return;

        setSyncStatus('syncing');
        try {
            await saveGameState({
                value: state.value,
                clickPower: state.clickPower,
                passiveIncome: state.passiveIncome,
                passiveInterval: state.passiveInterval
            });

            setPendingClicks(0);
            setSyncStatus('synced');
            setError(null);
        } catch (err) {
            setSyncStatus('error');
            setError('Ошибка при сохранении');
        }
    }, [state, pendingClicks]);

    // Автосохранение
    useEffect(() => {
        const timer = setTimeout(() => {
            if (pendingClicks > 0) {
                syncWithServer();
            }
        }, 500);

        return () => clearTimeout(timer);
    }, [pendingClicks, syncWithServer]);

    const click = useCallback(async () => {
        if (!state) return;

        setState(prev => prev ? {
            ...prev,
            value: prev.value + prev.clickPower
        } : null);

        setPendingClicks(prev => prev + 1);
        setSyncStatus('syncing');
    }, [state]);

    // Загружаем все данные при монтировании
    useEffect(() => {
        loadAllData();
    }, []);

    return {
        state,
        setState,
        upgrades,
        playerUpgrades,
        loading,
        error,
        syncStatus,
        setSyncStatus,
        click,
        buyUpgrade: handleBuyUpgrade,
        refresh: loadAllData  
    };
}
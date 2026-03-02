import { useState, useEffect, useCallback } from 'react';
import { getGameState, clickGame, processPassiveIncome } from '../api/points';
import { getUpgradeEffects, getAvailableUpgrades, getPlayerUpgrades, buyUpgrade, type UpgradeEffects, type Upgrade, type PlayerUpgrade } from '../api/upgrade';
import { startSignalR, stopSignalR } from '../api/signalR';

export interface GameState {
    value: number;
    clickPower: number;
    passiveIncome: number;
    passiveInterval: number;
}

export function useGame() {
    const [amount, setAmount] = useState<number>(0);
    const [effects, setEffects] = useState<UpgradeEffects>({
        clickPower: 1,
        passiveIncome: 0,
        passiveInterval: 5000
    });
    const [upgrades, setUpgrades] = useState<Upgrade[]>([]);
    const [playerUpgrades, setPlayerUpgrades] = useState<PlayerUpgrade[]>([]);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const [syncStatus, setSyncStatus] = useState<'synced' | 'syncing' | 'error'>('synced');
    const [activeClients, setActiveClients] = useState<number>(0);

    // SignalR подписка
    useEffect(() => {
        startSignalR(
            (newAmount: number) => {
                console.log('📦 SignalR получил новое количество:', newAmount);
                setAmount(newAmount);
                setSyncStatus('synced');
            },
            (count: number) => {
                console.log('👥 Активных клиентов:', count);
                setActiveClients(count);
            }
        );

        return () => {
            stopSignalR();
        };
    }, []);

    // Пассивный доход
    useEffect(() => {
        if (effects.passiveInterval <= 0 || effects.passiveIncome <= 0) return;

        console.log("✅ Запуск пассивного дохода:", effects.passiveInterval, "мс");

        const timer = setInterval(async () => {
            try {
                const newAmount = await processPassiveIncome();
                setAmount(newAmount);
            } catch (err) {
                console.error('Passive income error:', err);
            }
        }, effects.passiveInterval);

        return () => {
            console.log("🛑 Остановка пассивного дохода");
            clearInterval(timer);
        };
    }, [effects.passiveInterval, effects.passiveIncome]);

    // Загрузка всех данных
    const loadAllData = useCallback(async () => {
        setLoading(true);
        try {
            const [initialAmount, initialEffects, availableUpgrades, playerUpgradesData] = await Promise.all([
                getGameState(),
                getUpgradeEffects(),
                getAvailableUpgrades(),
                getPlayerUpgrades()
            ]);

            console.log('📥 Загружены данные:', { initialAmount, initialEffects, availableUpgrades, playerUpgradesData });

            setAmount(initialAmount);
            setEffects(initialEffects);
            setUpgrades(availableUpgrades);
            setPlayerUpgrades(playerUpgradesData);
            setError(null);
        } catch (err) {
            console.error('❌ Ошибка загрузки:', err);
            setError(err instanceof Error ? err.message : 'Ошибка при загрузке');
        } finally {
            setLoading(false);
        }
    }, []);

    useEffect(() => {
        loadAllData();
    }, []);

    // Клик
    const click = useCallback(async () => {
        setSyncStatus('syncing');
        try {
            const newAmount = await clickGame(effects.clickPower);
            console.log('🖱️ Клик:', newAmount);
            setAmount(newAmount);
            setSyncStatus('synced');
        } catch (err) {
            console.error('❌ Ошибка клика:', err);
            setSyncStatus('error');
            setError(err instanceof Error ? err.message : 'Ошибка при клике');
        }
    }, [effects.clickPower]);

    // Покупка улучшения
    const handleBuyUpgrade = useCallback(async (upgradeId: number, price: number) => {
        if (amount < price) {
            setError('Недостаточно очков');
            return;
        }

        setAmount(prev => prev - price);

        try {
            const result = await buyUpgrade(upgradeId);
            console.log('✅ Улучшение куплено:', result);

            const [newEffects, newAmount, availableUpgrades, playerUpgradesData] = await Promise.all([
                getUpgradeEffects(),
                getGameState(),
                getAvailableUpgrades(),
                getPlayerUpgrades()
            ]);

            setEffects(newEffects);
            setAmount(newAmount);
            setUpgrades(availableUpgrades);
            setPlayerUpgrades(playerUpgradesData);

            setError(null);
        } catch (err: any) {
            setAmount(prev => prev + price);
            console.error('❌ Ошибка покупки:', err);
            setError(err.message || 'Ошибка при покупке');
        }
    }, [amount]);

    // Полное состояние для обратной совместимости
    const fullState: GameState = {
        value: amount,
        ...effects
    };

    return {
        state: fullState,
        setState: (newState: GameState) => {
            setAmount(newState.value);
            setEffects({
                clickPower: newState.clickPower,
                passiveIncome: newState.passiveIncome,
                passiveInterval: newState.passiveInterval
            });
        },
        upgrades,
        playerUpgrades,
        activeClients,
        loading,
        error,
        syncStatus,
        setSyncStatus,
        click,
        buyUpgrade: handleBuyUpgrade,
        refresh: loadAllData
    };
}
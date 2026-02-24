import { useState, useEffect, useCallback } from 'react';
import { getGameState, saveGameState } from '../api/points';
import type { GameState } from '../api/points';

export function useGame() {
    const [state, setState] = useState<GameState | null>(null);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const [syncStatus, setSyncStatus] = useState<'synced' | 'syncing' | 'error'>('synced');

    // Обычные state для локальных значений
    const [localValue, setLocalValue] = useState<number>(0);
    const [localPower, setLocalPower] = useState<number>(1);
    const [pendingClicks, setPendingClicks] = useState<number>(0);

    const loadState = useCallback(async () => {
        setLoading(true);
        try {
            const data = await getGameState();
            setState(data);
            setLocalValue(data.value);
            setLocalPower(data.clickPower);
            setError(null);
        } catch (err) {
            setError(err instanceof Error ? err.message : 'Ошибка при загрузке');
        } finally {
            setLoading(false);
        }
    }, []);

    const syncWithServer = useCallback(async () => {
        if (pendingClicks === 0) return;

        setSyncStatus('syncing');
        try {
            await saveGameState({
                value: localValue,
                clickPower: localPower
            });

            setPendingClicks(0);
            setSyncStatus('synced');
            setError(null);
        } catch (err) {
            setSyncStatus('error');
            setError('Ошибка при сохранении');
        }
    }, [localValue, localPower, pendingClicks]);

    // Автосохранение каждые 3 секунды
    useEffect(() => {
        const timer = setTimeout(() => {
            if (pendingClicks > 0) {
                syncWithServer();
            }
        }, 3000);

        return () => clearTimeout(timer);
    }, [localValue, pendingClicks, syncWithServer]);

    const click = useCallback(async () => {
        if (!state) return;

        // Мгновенно обновляем локальное значение
        const newValue = localValue + localPower;
        setLocalValue(newValue);
        setPendingClicks(prev => prev + 1);

        // Обновляем UI
        setState(prev => prev ? {
            ...prev,
            value: newValue
        } : null);

        setSyncStatus('syncing');
    }, [state, localValue, localPower]);

    useEffect(() => {
        loadState();
    }, []);

    return {
        state,
        loading,
        error,
        syncStatus,
        click,
        refresh: loadState
    };
}
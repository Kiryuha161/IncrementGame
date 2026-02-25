import { useState, useEffect, useCallback } from 'react';
import { getGameState, saveGameState } from '../api/points';
import type { GameState } from '../api/points';

export function useGame() {
    const [state, setState] = useState<GameState | null>(null);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const [syncStatus, setSyncStatus] = useState<'synced' | 'syncing' | 'error'>('synced');
    const [pendingClicks, setPendingClicks] = useState<number>(0);

    const loadState = useCallback(async () => {
        setLoading(true);
        try {
            const data = await getGameState();
            setState(data);
            setError(null);
        } catch (err) {
            setError(err instanceof Error ? err.message : 'Ошибка при загрузке');
        } finally {
            setLoading(false);
        }
    }, []);

    const syncWithServer = useCallback(async () => {
        if (pendingClicks === 0 || !state) return;

        setSyncStatus('syncing');
        try {
            await saveGameState({
                value: state.value,  
                clickPower: state.clickPower
            });

            setPendingClicks(0);
            setSyncStatus('synced');
            setError(null);
        } catch (err) {
            setSyncStatus('error');
            setError('Ошибка при сохранении');
        }
    }, [state, pendingClicks]); 

    // Автосохранение каждые 3 секунды
    useEffect(() => {
        const timer = setTimeout(() => {
            if (pendingClicks > 0) {
                syncWithServer();
            }
        }, 3000);

        return () => clearTimeout(timer);
    }, [pendingClicks, syncWithServer]);

    const click = useCallback(async () => {
        if (!state) return;

        // Мгновенно обновляем UI
        setState(prev => prev ? {
            ...prev,
            value: prev.value + prev.clickPower  
        } : null);

        setPendingClicks(prev => prev + 1);
        setSyncStatus('syncing');
    }, [state]); // 👈 зависимость от state

    useEffect(() => {
        loadState();
    }, []);

    return {
        state,
        setState,        // для SignalR
        loading,
        error,
        syncStatus,
        setSyncStatus,   // для SignalR
        click,
        refresh: loadState
    };
}
import { useState, useEffect, useCallback } from 'react';
import { getGameState, clickGame } from '../api/points';
import type { GameState } from '../api/points';

export function useGame() {
    const [state, setState] = useState<GameState | null>(null);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);

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

    const click = useCallback(async () => {
        setLoading(true);
        try {
            const data = await clickGame();
            setState(data);
            setError(null);
        } catch (err) {
            setError(err instanceof Error ? err.message : 'Ошибка при клике');
        } finally {
            setLoading(false);
        }
    }, []);

    useEffect(() => { loadState(); }, []);

    return { state, loading, error, click, refresh: loadState };
}
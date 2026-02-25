import './App.css'
import { useGame } from './hooks/useGame';
import { startSignalR, stopSignalR } from './api/signalR';
import { useEffect } from 'react';

function App() {
    const { state, setState, loading, error, syncStatus, setSyncStatus, click } = useGame();

    useEffect(() => {
        // Подключаем SignalR при монтировании
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

    return (
        <div>
            <h1>Очки: {state.value}</h1>
            <h2>Сила клика: {state.clickPower}</h2>

            <div style={{ marginBottom: '10px', fontSize: '14px' }}>
                {syncStatus === 'syncing' && '⏳ Сохранение...'}
                {syncStatus === 'synced' && '💾 Все сохранено'}
                {syncStatus === 'error' && '⚠️ Ошибка сохранения'}
            </div>

            <button
                onClick={click}
                disabled={loading}
                style={{
                    padding: '15px 30px',
                    fontSize: '18px',
                    cursor: 'pointer'
                }}
            >
                Клик!
            </button>
        </div>
    );
}

export default App;
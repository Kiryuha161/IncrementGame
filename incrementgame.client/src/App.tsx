import './App.css'
import { useGame } from './hooks/useGame';

function App() {
    const { state, loading, error, syncStatus, click } = useGame();

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
import './App.css'
import { useGame } from './hooks/useGame';

function App() {
    const { state, loading, error, click } = useGame();

    if (loading) return <div>Загрузка...</div>;
    if (error) return <div>Ошибка: {error}</div>;
    if (!state) return <div>Нет данных</div>;

    return (
        <div>
            <h1>Очки: {state.value}</h1>
            <h2>Сила клика: {state.clickPower}</h2>
            <button onClick={click} disabled={loading}>Клик!</button>
        </div>
    );
}


export default App

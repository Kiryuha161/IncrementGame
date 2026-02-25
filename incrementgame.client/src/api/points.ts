import { api } from './client';

export interface GameState {
    value: number;
    clickPower: number;
    passiveIncome: number;      
    passiveInterval: number;
}

// Простые функции для каждого эндпоинта
export async function getGameState() {
    const response = await api.get<GameState>('/points');
    if (!response.success) throw new Error(response.message);
    return response.data!;
}

export async function clickGame() {
    const response = await api.post<GameState>('/points/click');
    if (!response.success) throw new Error(response.message);
    return response.data!;
}

export async function saveGameState(state: GameState) {
    const response = await api.post('/points/state', state);
    if (!response.success) throw new Error(response.message);
}

export async function processPassiveIncome() {
    const response = await api.post<GameState>('/points/passive');
    if (!response.success) throw new Error(response.message);
    return response.data!;
}
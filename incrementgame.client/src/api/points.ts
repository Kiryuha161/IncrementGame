import { api } from './client';

export async function getGameState() {
    const response = await api.get<number>('/points');
    if (!response.success) throw new Error(response.message);
    return response.data!;
}

export async function clickGame(clickPower: number) {
    const response = await api.post<number>('/points/click', { clickPower });
    if (!response.success) throw new Error(response.message);
    return response.data!;
}

export async function saveGameState(amount: number) {
    const response = await api.post('/points/state', amount);
    if (!response.success) throw new Error(response.message);
}

export async function processPassiveIncome() {
    const response = await api.post<number>('/points/passive');
    if (!response.success) throw new Error(response.message);
    return response.data!;
}
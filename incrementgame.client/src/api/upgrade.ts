import { api } from './client';

export interface Upgrade {
    id: number;
    name: string;
    description?: string;
    currentValue: number;
    nextValue: number;
    currentPrice: number;
    currentLevel: number;
    maxLevel: number;
    upgradeType: string;
    icon: string;
}

export interface PlayerUpgrade {
    upgradeId: number;
    name: string;
    level: number;
    currentValue: number;
    nextPrice: number;
    nextValue: number;
}

// Получить все доступные улучшения
export async function getAvailableUpgrades() {
    const response = await api.get<Upgrade[]>('/upgrade/available');
    if (!response.success) throw new Error(response.message);
    return response.data!;
}

// Получить прогресс улучшений игрока
export async function getPlayerUpgrades() {
    const response = await api.get<PlayerUpgrade[]>('/upgrade/player');
    if (!response.success) throw new Error(response.message);
    return response.data!;
}

// Купить улучшение
export async function buyUpgrade(upgradeId: number) {
    const response = await api.post<PlayerUpgrade>(`/upgrade/buy/${upgradeId}`);
    if (!response.success) throw new Error(response.message);
    return response.data!;
}

// Проверить, можно ли купить
export async function canAffordUpgrade(upgradeId: number) {
    const response = await api.get<boolean>(`/upgrade/can-buy/${upgradeId}`);
    if (!response.success) throw new Error(response.message);
    return response.data!;
}

// Получить цену следующего уровня
export async function getNextPrice(upgradeId: number) {
    const response = await api.get<number>(`/upgrade/price/${upgradeId}`);
    if (!response.success) throw new Error(response.message);
    return response.data!;
}
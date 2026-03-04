import { api } from './client';

export interface UpgradeEffects {
    clickPower: number;
    passiveIncome: number;
    passiveInterval: number;
    discountPercent?: number;
    powerMultiplier: number;
    powerLevel: number;
}

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
    originalPrice?: number;
}

export interface PlayerUpgrade {
    upgradeId: number;
    name: string;
    level: number;
    currentValue: number;
    nextPrice: number;
    nextValue: number;
}

export async function getUpgradeEffects() {
    const response = await api.get<UpgradeEffects>('/upgrade/effects');
    if (!response.success) throw new Error(response.message);
    return response.data!;
}

export async function getAvailableUpgrades() {
    const response = await api.get<Upgrade[]>('/upgrade/available');
    if (!response.success) throw new Error(response.message);
    return response.data!;
}

export async function getPlayerUpgrades() {
    const response = await api.get<PlayerUpgrade[]>('/upgrade/player');
    if (!response.success) throw new Error(response.message);
    return response.data!;
}

export async function buyUpgrade(upgradeId: number) {
    const response = await api.post<PlayerUpgrade>(`/upgrade/buy/${upgradeId}`);
    if (!response.success) throw new Error(response.message);
    return response.data!;
}

export async function canAffordUpgrade(upgradeId: number) {
    const response = await api.get<boolean>(`/upgrade/can-buy/${upgradeId}`);
    if (!response.success) throw new Error(response.message);
    return response.data!;
}

export async function getNextPrice(upgradeId: number) {
    const response = await api.get<number>(`/upgrade/price/${upgradeId}`);
    if (!response.success) throw new Error(response.message);
    return response.data!;
}
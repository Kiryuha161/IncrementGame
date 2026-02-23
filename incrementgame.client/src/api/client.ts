const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'https://localhost:7261/api';

interface ApiResponse<T = any> {
    success: boolean;
    message?: string;
    data?: T;
    errors?: string[];
}

// Простые функции для запросов
async function get<T>(endpoint: string): Promise<ApiResponse<T>> {
    try {
        const response = await fetch(`${API_BASE_URL}${endpoint}`, {
            headers: { 'Content-Type': 'application/json' }
        });
        return await response.json();
    } catch (error) {
        return {
            success: false,
            message: error instanceof Error ? error.message : 'Network error'
        };
    }
}

async function post<T>(endpoint: string, body?: any): Promise<ApiResponse<T>> {
    try {
        const response = await fetch(`${API_BASE_URL}${endpoint}`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(body)
        });
        return await response.json();
    } catch (error) {
        return {
            success: false,
            message: error instanceof Error ? error.message : 'Network error'
        };
    }
}

export const api = { get, post };
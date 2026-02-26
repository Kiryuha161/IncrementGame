import * as signalR from '@microsoft/signalr';
import type { GameState } from './points';

let connection: signalR.HubConnection | null = null;

export const startSignalR = (
    onStateUpdate: (state: GameState) => void,
    onClientCountUpdate: (count: number) => void
) => {
    if (connection) {
        console.log('SignalR уже подключен');
        return Promise.resolve();
    }

    connection = new signalR.HubConnectionBuilder()
        .withUrl('https://localhost:7261/gameHub')
        .withAutomaticReconnect([0, 2000, 5000, 10000, 30000]) // 👈 Явные интервалы
        .configureLogging(signalR.LogLevel.Information)
        .build();

    connection.on('ReceiveGameStateUpdate', (state: GameState) => {
        console.log('🔥 SignalR принят:', state);
        onStateUpdate(state);
    });

    connection.on('UpdateClientCount', (count: number) => {
        console.log('👥 Активные кл:', count);
        onClientCountUpdate(count);
    });

    connection.onreconnecting((error) => {
        console.log('🔄 Переподключение...', error);
    });

    connection.onreconnected((connectionId) => {
        console.log('✅ Переподключено:', connectionId);
    });

    return connection.start()
        .then(() => console.log('✅ SignalR connected'))
        .catch(err => {
            console.error('❌ SignalR error:', err);
            connection = null;
        });
};

export const stopSignalR = () => {
    if (connection) {
        connection.stop();
        connection = null;
    }
};
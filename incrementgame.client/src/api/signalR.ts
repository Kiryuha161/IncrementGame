import * as signalR from '@microsoft/signalr';
import type { GameState } from './points';

let connection: signalR.HubConnection | null = null;

export const startSignalR = (
    onStateUpdate: (state: GameState) => void,
    onClientCountUpdate: (count: number) => void
) => {
    connection = new signalR.HubConnectionBuilder()
        .withUrl('https://localhost:7261/gameHub')
        .withAutomaticReconnect()
        .build();

    connection.on('ReceiveGameStateUpdate', (state: GameState) => {
        console.log('🔥 SignalR принят:', state);
        onStateUpdate(state);
    });

    connection.on('UpdateClientCount', (count: number) => {
        console.log('👥 Активные кл:', count);
        onClientCountUpdate(count);
    });

    return connection.start()
        .then(() => console.log('✅ SignalR connected'))
        .catch(err => console.error('❌ SignalR error:', err));
};

export const stopSignalR = () => {
    if (connection) {
        connection.stop();
    }
};
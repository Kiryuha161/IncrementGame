import * as signalR from '@microsoft/signalr';

let connection: signalR.HubConnection | null = null;

export const startSignalR = (
    onAmountUpdate: (amount: number) => void,
    onClientCountUpdate: (count: number) => void
) => {
    if (connection) {
        console.log('SignalR уже подключен');
        return Promise.resolve();
    }

    connection = new signalR.HubConnectionBuilder()
        .withUrl('https://localhost:7261/gameHub')
        .withAutomaticReconnect([0, 2000, 5000, 10000, 30000])
        .configureLogging(signalR.LogLevel.Information)
        .build();

    connection.on('ReceiveAmountUpdate', (amount: number) => {
        console.log('💰 SignalR принял сумму:', amount, 'время:', new Date().toISOString());
        onAmountUpdate(amount);
    });

    connection.on('UpdateClientCount', (count: number) => {
        console.log('👥 Активные клиенты:', count);
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